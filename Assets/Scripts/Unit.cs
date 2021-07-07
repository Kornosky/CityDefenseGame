using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour, IDamageable
{
    [SerializeField] protected UnitScriptableObject info;
    public bool isEnemy;
    [SerializeField] protected Healthbar hpBar;
    protected SpriteRenderer spriteRenderer;
    protected Animator anim;
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

    protected virtual void Start()
    {
        actionCollider.actionEvent.AddListener(TryAction);
        unitAnimator?.actionEvent.AddListener(Action);
        unitAnimator?.hitFinishedEvent.AddListener(FinishedHit);
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
                anim.SetBool("Moving", false);
            }
            else
            {
                anim.SetBool("Moving", true);
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
        actionCollider ??= GetComponentInChildren<ActionCollider>();

        anim ??= GetComponentInChildren<Animator>();
        unitAnimator ??= GetComponentInChildren<UnitAnimator>();
        rb ??= GetComponent<Rigidbody2D>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
        hpBar ??= GetComponentInChildren<Healthbar>(true);

        PlayerRecording.Instance.AddUnitsToDictionary(PlayerRecording.Instance.ActiveUnits, info, 1);
        GameManager.Instance.activeUnits.Add(this);
    }
    protected void ChangeTarget(Transform location)
    {
        goalTarget = location;
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
            homeBase = GameManager.Instance.enemyBase;
        }
        else
        {

            isFacingRight = isEnemy; //hmmm
            homeBase = GameManager.Instance.playerBase;
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
            GameManager.Instance.Money += (int) (info.cost * GameManager.Instance.percentageEarnedFromKill);
        }

        if (info.hasDeathAnimation)
        {
            //remove colliders
            Destroy(rb);
            Destroy(GetComponent<Collider2D>());
            Destroy(actionCollider.gameObject);
            //Play animation 
            anim.SetTrigger("Death");
        }
        else
            Destroy(gameObject);
    }
    //Destroy method called from animation event

    public virtual bool TakeDamage(int dmg)
    {

        hpBar.UpdateValue(-dmg);
        if (hpBar.GetValue() <= 0)
        {
            //is now dead
            Death();
            return true;
        }

        //Calculate if plays hit animation
        if(dmg >= info.actionDmgInterruptThreshold || Random.Range(0, info.actionDmgInterruptThreshold) < dmg && dmg > 0)
        {
            //reset so they can attack
            isActing = false;

            anim?.SetTrigger("Hit");
        }//less likely based off damage difference
     
        return false;

    }

    protected virtual void FinishedHit()
    {
        isActing = false;
        TryAction();
    }
    protected virtual void OnDestroy()
    {
        GameManager.Instance?.activeUnits?.Remove(this);
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

}