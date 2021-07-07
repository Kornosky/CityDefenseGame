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
        proj.GetComponent<Arrow>().Init(unit.GetInfo(), unit.isEnemy);
    }

}
