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
    protected UnitScriptableObject info;
    public bool isEnemy;
    [SerializeField] protected Healthbar hpBar;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator stateMachine;
    public Transform goalTarget;
    protected ActionCollider actionCollider;
    [SerializeField] public bool isActing = false;
    [Header("Needs Refactoring")]
    public bool isGrounded = false;
    protected bool isDead = false;
    private bool arrivedAtDestination = false;
    public Transform homeBase;
    protected UnitAnimator unitAnimator;
    protected bool isFacingRight;
    protected bool canMove = true;

    public bool isInvicible;

    GameObject coinPrefab;
    GameObject manaPrefab;
    GameObject vfxPrefab;

    public UnitScriptableObject Info { get => info;}

    protected virtual void Awake()
    {
        actionCollider ??= GetComponentInChildren<ActionCollider>(true);
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
        hpBar ??= GetComponentInChildren<Healthbar>(true);
        stateMachine ??= GetComponentInChildren<Animator>(true);
        unitAnimator ??= GetComponentInChildren<UnitAnimator>(true);

        //Resources
        coinPrefab = Resources.Load("Coin") as GameObject;
        manaPrefab = Resources.Load("Mana") as GameObject;
        vfxPrefab = Resources.Load("VFX") as GameObject;
        PlayerRecording.Instance.AddUnitsToDictionary(PlayerRecording.Instance.ActiveUnits, info, 1);
        PlayerManager.Instance.activeUnits.Add(this);
    }
    protected virtual void Start()
    {
        actionCollider.actionEvent.AddListener(TryAction);
        unitAnimator?.actionEvent.AddListener(Action);
       // unitAnimator?.hitFinishedEvent.AddListener(FinishedHit);
    }


    //TODO get closest point somewhere on the actual unit... if possible
    public Vector3 GetClosestPoint(Vector3 location)
    {
        return spriteRenderer.bounds.ClosestPoint(location);
    }

    public HealthBundle GetHealth()
    {
        return hpBar.GetHealth();
    }
    public void ChangeTarget(Transform location)
    {
        goalTarget = location;
    }
    // Scan for goalTarget if none exist
    public virtual void ScanForGoal()
    {
        goalTarget = homeBase;
    }

    public void DropPickup()
    {
        if(Random.Range(0f, 1f) > .5f)
        {
            int earned = (int)(info.Cost * PlayerManager.Instance.percentageEarnedFromKill);
            //  GameManager.Instance.Money += earned;
            for (int i = 0; i < earned; i++)
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(manaPrefab, transform.position, Quaternion.identity);
        }
    }
    public void RemoveInteraction()
    {
        ChangeLayer(true, "NoUnitCollision");
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

    public enum States { MOVING, RECOIL, HIT, ATTACK, DEATH }
    public void ChangeState(States state, bool isBool = false, bool boolState = false)
    {
        if(isBool)
            stateMachine?.SetBool(char.ToUpper(state.ToString()[0]) + (state.ToString().Substring(1)).ToLower(), boolState);
        else
            stateMachine?.SetTrigger(char.ToUpper(state.ToString()[0]) + (state.ToString().Substring(1)).ToLower());

    }
    public void ChangeState(string state, bool isBool = false, bool boolState = false)
    {
        if(isBool)
            stateMachine?.SetBool(state, boolState);
        else
            stateMachine?.SetTrigger(state);
    }
    public UnitScriptableObject GetInfo()
    {
        return info;
    }

    //Flip components of entity that need to be in the direction they're facing
    public void FlipComponents(bool isFacingLeft)
    {
        // Manual correction, this isn't great
        if(isFacingLeft)
        {
            if (actionCollider.gameObject.transform.localPosition.x > 0)
                actionCollider.gameObject.transform.localPosition *= -Vector2.one;
        }
        else
        {
            if (actionCollider.gameObject.transform.localPosition.x < 0)
                actionCollider.gameObject.transform.localPosition *= -Vector2.one;
        }
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
            int earned = (int)(info.Cost * PlayerManager.Instance.percentageEarnedFromKill);
          //  GameManager.Instance.Money += earned;
            for(int i = 0; i < earned; i++)
                Instantiate(Resources.Load("Coin") as GameObject, transform.position, Quaternion.identity);
        }

        if (info.hasDeathAnimation)
        {
            RemoveInteraction();
            //Play animation 
            ChangeState(States.DEATH);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// When unit gets hit by damaging object
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="sourcePos"></param>
    /// <param name="knockback"></param>
    /// <param name="location"></param>
    /// <param name="vfx"></param>
    /// <returns>Returns true if the hit completes/lands </returns>
    public virtual bool GetHit(int damage, Vector3 sourcePos, Vector2 knockback, Vector3 location, AnimationClip vfx)
    {
        if (isInvicible)
            return false;

        TakeDamage(damage, location, vfx);

        return true;
    }
    
    public virtual bool GetHit(int damage, Vector3 sourcePos, Vector2 knockback)
    {
        if (isInvicible)
            return false;

        TakeDamage(damage);

        return true;
    }

    //Wrapper VFX damage
    public virtual bool TakeDamage(int dmg, Vector3 location, AnimationClip vfx)
    {
        if (TakeDamage(dmg)) return true; //died

        //vfx
        var temp = Instantiate(vfxPrefab, location, Quaternion.identity).GetComponent<VFX>();
        temp.Init(vfx);

        return false;
    }
    public virtual bool TakeDamage(int dmg, DamageType type)
    {
        return TakeDamage(dmg);
    }
    public virtual bool TakeDamage(int dmg)
    {
        if (isInvicible)
            return false;

        hpBar.UpdateValue(-dmg);
        if (hpBar.GetValue() <= 0)
        {
            //is now dead

            if (stateMachine)
                ChangeState(States.DEATH);
            else
                Death();
            return true;
        }

        //Calculate if plays hit animation
        if(dmg >= info.actionDmgInterruptThreshold || Random.Range(0, info.actionDmgInterruptThreshold) < dmg && dmg > 0)
        {
            //reset so they can attack
            isActing = true;
            ChangeState(States.HIT);
        }//less likely based off damage difference
     
        return false;

    }

    public virtual void FinishedHit()
    {
        isActing = false;
        TryAction();
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
    protected virtual void OnDestroy()
    {
        PlayerManager.Instance?.activeUnits?.Remove(this);
        PlayerRecording.Instance?.AddUnitsToDictionary(PlayerRecording.Instance.ActiveUnits, info, -1);
    }

    void IDamageable.GetHit(int damage, Vector3 sourcePos, Vector2 knockback)
    {
        GetHit(damage, sourcePos, knockback);
    }
}
