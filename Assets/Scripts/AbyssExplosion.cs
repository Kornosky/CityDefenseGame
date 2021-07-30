using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbyssExplosion : MonoBehaviour
{
    float tick = .5f;
    public UnityEvent<IDamageable> damaged;
    List<UnitEnter> caught = new List<UnitEnter>();
    struct UnitEnter
    {
        public IDamageable unit;
        public bool isCollded;

        public UnitEnter(IDamageable unit, bool isCollded)
        {
            this.unit = unit;
            this.isCollded = isCollded;
        }
    }
    IEnumerator OnTriggerEnter2D(Collider2D collider)
    {
        var iDamageable = collider.gameObject.GetComponent<IDamageable>();
        if (iDamageable != null)
        {            
            caught.Add(new UnitEnter(iDamageable, true));
            yield return new WaitForSeconds(tick); //wait 2 seconds
            if (caught.Exists(x => x.unit == iDamageable)) //check if collided is still true
            {
                //Action after 2 seconds trigger.
                damaged.Invoke(iDamageable);
            }
        }
    }

    //Turns bool collided to false if there is no trigger anymore.
    private void OnTriggerExit2D(Collider2D collision)
    {
        var iDamageable = collision.gameObject.GetComponent<IDamageable>();
        caught.Remove(caught.Find(x => x.unit == iDamageable));
    }
}
