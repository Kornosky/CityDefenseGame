using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Acid : MonoBehaviour
{
    [SerializeField] private List<UnitEnterTime> unitsInTrigger = new List<UnitEnterTime>();
    float lifeTime;
    [SerializeField] bool canInteract = false;
    bool hitGround = false;
    bool hasSplit = false;
    [SerializeField]  bool canSpread = true;
    Rigidbody2D rigidbody2D1;
    class UnitEnterTime
    {
        public Unit unit;
        public float enteredTime;

        public UnitEnterTime(Unit unit, float enteredTime)
        {
            this.unit = unit;
            this.enteredTime = enteredTime;
        }
    }
    private void Awake()
    {
        UnitUtility.ChangeLayer(gameObject, false, "OnlyUnitCollision");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

    }
    public void Init(bool interactsImmediately, bool canSpread)
    {
        this.canInteract = canSpread;
        canInteract = false;
        if (!interactsImmediately)
            this.Invoke(() => canInteract = true, 1f);
        else
            canInteract = true;
    }
    public void BeginDecay()
    {
        this.Invoke(() => Destroy(gameObject), lifeTime);
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
        for (int i = 0; i < unitsInTrigger.Count; i++)
        {
            unitsInTrigger[i].enteredTime -= Time.deltaTime;
            if (unitsInTrigger[i].enteredTime <= Mathf.Epsilon)
            {
                unitsInTrigger[i].unit.TakeDamage(1, DamageType.FIRE);
                unitsInTrigger[i].enteredTime = 1f;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) //dont hit player tower
            return;

        if (collision.gameObject.GetComponent<IDamageable>() != null && unitsInTrigger.Count < 1)
        {
            //poision one then destroy
            unitsInTrigger.Add(new UnitEnterTime(collision.gameObject.GetComponent<Unit>(), 0));
            Destroy(GetComponent<Collider2D>());

        }
        else if (collision.gameObject.CompareTag("Ground") && !hasSplit && canSpread)
        {
            hitGround = true;
            hasSplit = true;
            //stop acid from moving

            //glob based off of velocity
            int maxGlobCount = 6;
            int globCount =  Mathf.FloorToInt(Mathf.Lerp(0, maxGlobCount, GetComponent<Rigidbody2D>().velocity.magnitude / 13));            
            
            //if hit ground then "glob" out
            for (int i = 0; i < globCount; i++)
            {
                canSpread = false;
                var acid = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<Acid>();
                acid.Init(false, false);
                //set it to half size
                acid.gameObject.transform.localScale = Vector2.one * .5f;
                Vector2 randomCircle = new Vector2(Random.insideUnitCircle.x, Mathf.Abs(Random.insideUnitCircle.y + 1.1f));

                //This code will get rewritten a lot
                acid.GetComponent<Rigidbody2D>().AddForce(randomCircle * 3.5f, ForceMode2D.Impulse);
            }

            FreezeRigidbody();

            BeginDecay();
        }
        else if(collision.gameObject.CompareTag("Ground"))
        {
            //Freeze in place
            FreezeRigidbody();

            BeginDecay();
        }
    }

    private void FreezeRigidbody()
    {
        rigidbody2D1.isKinematic = true;
        rigidbody2D1.velocity = Vector2.zero;
        rigidbody2D1.angularVelocity = 0;
        GetComponent<Collider2D>().isTrigger = true;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canInteract)
            return;
        if (collision.GetComponent<IDamageable>() != null)
        {
            //Only allow one unit to take damage from this instance of acid
            if(hitGround)
            {
                unitsInTrigger.Add(new UnitEnterTime(collision.GetComponent<Unit>(), 0));
            }
            else
            {
                if (unitsInTrigger.Count < 1)
                    unitsInTrigger.Add(new UnitEnterTime(collision.GetComponent<Unit>(), 0));
            }
            
        }
        
    }


    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (!canInteract)
            return;

        if (collision.GetComponent<IDamageable>() != null)
        {
            unitsInTrigger.Remove(unitsInTrigger.Find(x => x.unit == collision.GetComponent<Unit>()));
        }
    }

    private void FixedUpdate()
    {
        
    }
}

