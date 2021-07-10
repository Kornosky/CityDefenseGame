using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct HealthBundle
{
    public int curHealth;
    public int minHealth;
    public int maxHealth;

    public HealthBundle(int curHealth, int minHealth, int maxHealth)
    {
        this.curHealth = curHealth;
        this.minHealth = minHealth;
        this.maxHealth = maxHealth;
    }
    public HealthBundle(float curHealth, float minHealth, float maxHealth)
    {
        this.curHealth = (int) curHealth;
        this.minHealth = (int) minHealth;
        this.maxHealth = (int) maxHealth;
    }
}

public abstract class Unit : MonoBehaviour, IDamageable
{
    [SerializeField] protected UnitScriptableObject info;
    public bool isEnemy;
    [SerializeField] protected Healthbar hpBar;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator stateMachine;
    protected Rigidbody2D rb;
    protected Transform goalTarget;
    protected ActionCollider actionCollider;
    [SerializeField] protected bool isActing = false;
    [Header("Needs Refactoring")]
    public GameObject projectile;
    public Transform projSpawnLoc;
    protected bool isGrounded = false;
    protected bool isDead = false;
    private bool arrivedAtDestination = false;
    protected Transform homeBase;
    protected UnitAnimator unitAnimator;
    protected bool isFacingRight;
    public float toVel = .1f;
    public float maxVel = 1.0f;
    public float maxForce = 20.0f;
    public float gain = 5f;
    public bool isInvicible;

    protected virtual void Start()
    {
        actionCollider.actionEvent.AddListener(TryAction);
        unitAnimator?.actionEvent.AddListener(Action);
       // unitAnimator?.hitFinishedEvent.AddListener(FinishedHit);
    }
    public HealthBundle GetHealth()
    {
        return hpBar.GetHealth();
    }
    protected virtual void Flipped(bool isFacingLeft)
    {
        spriteRenderer.flipX = isFacingLeft;
    }
    protected virtual void FixedUpdate()
    {
        if (!info.canMoveWhileActing && isActing)
            return;
        if (isGrounded && goalTarget != null)
        {
            Vector2 dist = goalTarget.position - transform.position;
            bool isFlipped = dist.x < 0 ;
            if (isFacingRight != isFlipped && rb.velocity.magnitude != Mathf.Epsilon)
            {
                isFacingRight = isFlipped;
                Flipped(isFacingRight);
            }            

            dist.y = 0; // ignore height differences
                        // calc a target vel proportional to distance (clamped to maxVel)
            Vector2 tgtVel = Vector2.ClampMagnitude(20 * dist, info.moveSpeed);
            // Vector2 the velocity error
            Vector2 error = tgtVel - rb.velocity;
            // calc a force proportional to the error (clamped to maxForce)
            Vector2 force = Vector2.ClampMagnitude(gain * error, maxForce);
            rb.AddForce(force);

            if (rb.velocity.magnitude == Mathf.Epsilon)
            {
                stateMachine?.SetBool("Moving", false);
            }
            else
            {
                stateMachine?.SetBool("Moving", true);
            }
        }
       
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {      
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    protected virtual void Awake()
    {
        actionCollider ??= GetComponentInChildren<ActionCollider>(true);
        rb ??= GetComponent<Rigidbody2D>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
        hpBar ??= GetComponentInChildren<Healthbar>(true);
        stateMachine ??= GetComponentInChildren<Animator>(true);
        unitAnimator ??= GetComponentInChildren<UnitAnimator>(true);

        PlayerRecording.Instance.AddUnitsToDictionary(PlayerRecording.Instance.ActiveUnits, info, 1);
        PlayerManager.Instance.activeUnits.Add(this);
    }
    protected void ChangeTarget(Transform location)
    {
        goalTarget = location;
    }
    public void DropCoin()
    {
        int earned = (int)(info.cost * PlayerManager.Instance.percentageEarnedFromKill);
        //  GameManager.Instance.Money += earned;
        for (int i = 0; i < earned; i++)
            Instantiate(Resources.Load("Coin") as GameObject, transform.position, Quaternion.identity);
    }
    public void DestroyInteraction()
    { 
        Destroy(rb);
        Destroy(GetComponent<Collider2D>());
        Destroy(actionCollider.gameObject);
    }
    public void Invincibility(bool isInvincible)
    {
        this.isInvicible = isInvincible;
    }
    public virtual void Init(bool isEnemy, UnitScriptableObject info = null)
    {
        if (info != null)
            this.info = info;

        if (this.info == null)
            Debug.LogError(this + " is missing it's info");

        this.isEnemy = isEnemy;
        if (isEnemy)
        {
            isFacingRight = !isEnemy; //hmmm
            homeBase = PlayerManager.Instance.enemyBase;
        }
        else
        {

            isFacingRight = isEnemy; //hmmm
            homeBase = PlayerManager.Instance.playerBase;
        }
        Flipped(isFacingRight);
        ChangeLayer(isEnemy);
        hpBar.Init(this.info);
    }

    protected void ChangeLayer(bool isEnemy, string layer = "")
    {
        if(layer != "")
        {
            gameObject.layer = LayerMask.NameToLayer(layer);

            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
        else if (isEnemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }
    public UnitScriptableObject GetInfo()
    {
        return info;
    }
    public bool IsOpponent(GameObject toCheck)
    {
        if(isEnemy)
        {
            if(toCheck.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (toCheck.layer == LayerMask.NameToLayer("Enemy"))
            {
                return true;
            }
            return false;
        }
    }
    public virtual void Death()
    {
        isDead = true;

        if (isEnemy)
        {
            int earned = (int)(info.cost * PlayerManager.Instance.percentageEarnedFromKill);
          //  GameManager.Instance.Money += earned;
            for(int i = 0; i < earned; i++)
                Instantiate(Resources.Load("Coin") as GameObject, transform.position, Quaternion.identity);
        }

        if (info.hasDeathAnimation)
        {
            DestroyInteraction();
            //Play animation 
            stateMachine.SetTrigger("Death");
        }
        else
            Destroy(gameObject);
    }
    public virtual void GetHit(int damage, Vector3 sourcePos, Vector2 knockback)
    {
        if (isInvicible)
            return;

        rb.AddForce(new Vector2((gameObject.transform.position - sourcePos).normalized.x * knockback.x, knockback.y), ForceMode2D.Impulse);
        TakeDamage(damage);
    }
    public virtual bool TakeDamage(int dmg)
    {
        if (isInvicible)
            return false;

        hpBar.UpdateValue(-dmg);
        if (hpBar.GetValue() <= 0)
        {
            //is now dead
            
            if(stateMachine)
                stateMachine.SetTrigger("Death");
            else
                Death();
            return true;
        }

        //Calculate if plays hit animation
        if(dmg >= info.actionDmgInterruptThreshold || Random.Range(0, info.actionDmgInterruptThreshold) < dmg && dmg > 0)
        {
            //reset so they can attack
            isActing = false;

            stateMachine?.SetTrigger("Hit");
        }//less likely based off damage difference
     
        return false;

    }

    public virtual void FinishedHit()
    {
        isActing = false;
        TryAction();
    }
    protected virtual void OnDestroy()
    {
        PlayerManager.Instance?.activeUnits?.Remove(this);
        PlayerRecording.Instance?.AddUnitsToDictionary(PlayerRecording.Instance.ActiveUnits, info, -1);
    }
    //Each unit has a unique action
    protected abstract void Action();
    public abstract void TryAction();
    protected virtual IEnumerator Wait()
    {
        //Stuff before waiting
        yield return new WaitForSeconds(info.actionCD);
        isActing = false;
    }

    public AnimationClip GetAnimationClip(string clipName, Animator anim)
    {
        //Because it isn't always set in prefab
        stateMachine ??= GetComponentInChildren<Animator>();
        
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip;
        }
        Debug.LogError(clipName + " was not found for " + gameObject);
        return null;
    }

}
