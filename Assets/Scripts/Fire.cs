using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Fire : MonoBehaviour
{
    [SerializeField] private List<UnitEnterTime> unitsInTrigger = new List<UnitEnterTime>();
    float lifeTime;
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

    public void BeginBurn()
    {
        Invoke("Die", 2.2f);
    }
    void Die()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        lifeTime += Time.deltaTime;
        
        for(int i = 0; i < unitsInTrigger.Count; i++)
        {
            unitsInTrigger[i].enteredTime -= Time.deltaTime;
            if (unitsInTrigger[i].enteredTime <= Mathf.Epsilon)
            {
                unitsInTrigger[i].unit.TakeDamage(1, DamageType.FIRE);
                unitsInTrigger[i].enteredTime = 1f;
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Flammable flamObj = collision.gameObject.GetComponent<Flammable>();
        if (collision.GetComponent<IDamageable>() != null)
        {
            unitsInTrigger.Add(new UnitEnterTime(collision.GetComponent<Unit>(), 0));
        }
        else if (flamObj && !flamObj.onFire)
        {
            flamObj.SetOnFire();
            //set on fire
            var fire = Resources.Load<GameObject>("Fire");
            Bounds bounds = flamObj.GetComponentInChildren<Renderer>().bounds;
            int flameCount = (int) (bounds.size.x * bounds.size.y);
            for (int i = 0; i < flameCount; i++)
            {
                Instantiate(fire, GeneralUtility.RandomPointInBounds(bounds), Quaternion.identity);
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() != null)
        {
            unitsInTrigger.Remove(unitsInTrigger.Find(x => x.unit == collision.GetComponent<Unit>()));
        }
    }
}
