using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    private Unit unit;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projSpawnLoc;
    private void Start()
    {
        unit ??= GetComponentInParent<Unit>();
        unit.GetComponentInChildren<ActionCollider>().GetComponent<CircleCollider2D>().radius = unit.GetInfo().range;
    }

    //Launch it with physics
    public void LaunchProjectile(Vector2 target)
    {
        //stop
        //drawback
        //fire
        var proj = Instantiate(projectilePrefab, projSpawnLoc.position, Quaternion.identity);
        Vector2 direction = (Vector2)transform.position + (target - (Vector2)transform.position) / 2;
        Debug.DrawLine(transform.position, direction, Color.red, .2f);
        direction = direction - (Vector2)transform.position;
        direction = direction.normalized;

        //This code will get rewritten a lot
        proj.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x * Random.Range(1f, 2f), direction.y + Random.Range(1f, 2f)) * unit.GetInfo().projectileSpeed, ForceMode2D.Impulse);
        proj.GetComponent<Projectile>().Init(unit.GetInfo(), unit.isEnemy);
    }

    //Send it forward without physics
    public void SendProjectile(Vector2 target)
    {
        var proj = Instantiate(projectilePrefab, projSpawnLoc.position, Quaternion.identity);
        Vector2 direction = (Vector2)transform.position + (target - (Vector2)transform.position) / 2;
        Debug.DrawLine(transform.position, direction, Color.red, .2f);
        direction = direction - (Vector2)transform.position;
        direction = direction.normalized;

        //This code will get rewritten a lot
        object[] parms = new object[2] { proj.GetComponent<Rigidbody2D>(), direction };
        StartCoroutine("Sending", parms);
        proj.GetComponent<Projectile>().Init(unit.GetInfo(), unit.isEnemy);
    }
    IEnumerator Sending(Rigidbody2D proj, Vector2 direction)
    {
        float time = Random.Range(.1f, 1f);
        float curTime = 0;
        while (curTime < time)
        {
            curTime += Time.deltaTime;
            proj.MovePosition((Vector2) transform.position + direction * Time.deltaTime * 1f);
            yield return new WaitForFixedUpdate();
        }
    }
    //Send it forward without physics
    public void LobProjectile(Vector2 target)
    {
        var proj = Instantiate(projectilePrefab, projSpawnLoc.position, Quaternion.identity);
        Vector2 direction = (Vector2)transform.position + (target - (Vector2)transform.position) / 2;
        Debug.DrawLine(transform.position, direction, Color.red, .2f);
        direction = direction - (Vector2)transform.position;
        direction = direction.normalized;

        //This code will get rewritten a lot
        proj.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x * Random.Range(.2f, .5f), Random.Range(3f, 4f)) * unit.GetInfo().projectileSpeed, ForceMode2D.Impulse);
        proj.GetComponent<Projectile>().Init(unit.GetInfo(), unit.isEnemy);
    }

}
