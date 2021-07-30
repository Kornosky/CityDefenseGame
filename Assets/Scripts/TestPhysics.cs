using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysics : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    private float time;
    [SerializeField] float spawnTime;
    [SerializeField] float spawnForce;

    // Start is called before the first frame update
    void Start()
    {

       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if( time > spawnTime)
        {
            time = 0;

            ToTest();
        }
    }

    void ToTest()
    {
        var acid = Instantiate(toSpawn, transform.position, Quaternion.identity);
        Vector2 randomCircle = new Vector2(Random.insideUnitCircle.x, Mathf.Abs(Random.insideUnitCircle.y + 1.1f));

        //This code will get rewritten a lot
        acid.GetComponent<Rigidbody2D>().AddForce(randomCircle * spawnForce, ForceMode2D.Impulse);
    }
}
