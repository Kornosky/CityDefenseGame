using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Sword;
    public GameObject Archer;
    public Transform location;
    public bool shouldSpawn;
    [SerializeField] private Dictionary<UnitScriptableObject, int> unitSpawnedSinceLastTick = new Dictionary<UnitScriptableObject, int>();

    private void Start()
    {
        if (!shouldSpawn)
            return;
        InvokeRepeating("EnemyTick", 0, 1f);
    }

    private void EnemyTick()
    {
        UnitScriptableObject unitToSpawn;
        bool spawnedSomething = false;
        int value = 0;
        foreach (var unit in PlayerRecording.Instance.UnitSpawnCount)
        {
            if(unitSpawnedSinceLastTick.TryGetValue(unit.Key, out value))
                unitSpawnedSinceLastTick[unit.Key] = unit.Value - unitSpawnedSinceLastTick[unit.Key];
            else
            {
                unitSpawnedSinceLastTick.Add(unit.Key, unit.Value);
            }
            Debug.Log(unit.Key + " count since last tick is: " + unitSpawnedSinceLastTick[unit.Key]);
        }

        foreach (var unit in PlayerRecording.Instance.UnitSpawnCount)
        {
            float chance = 0;
            chance += unit.Key.cost;
            chance += unit.Value;
            //Spawned Since last tick
            chance *= unitSpawnedSinceLastTick[unit.Key];
            //Currently active of this type
            //chance *= PlayerRecording.Instance.ActiveUnits[unit.Key];
            ////Total active right now
            //chance *= PlayerRecording.Instance.GetTotalInDictionary(PlayerRecording.Instance.ActiveUnits);

            Debug.Log("Chance for " + unit.Key + ": " + chance);


            //Look at the number of units on field
            if (chance > Random.Range(0, chance) && Random.Range(0,5) == 0)
            {
                SpawnUnit(unit.Key);
                spawnedSomething = true;
            }
        }

        if(!spawnedSomething)
        {
            SpawnSword();
            SpawnArcher();
        }
        //Set for next tick
        unitSpawnedSinceLastTick = new Dictionary<UnitScriptableObject, int>(PlayerRecording.Instance.UnitSpawnCount);
    }
    public void SpawnUnit(UnitScriptableObject unit)
    {
        Unit temp = null;
        if (unit.isStructure)
        {
           // temp = Instantiate(unit.prefab, location.position, Quaternion.identity).GetComponent<Unit>();
        }
        else
        {
            temp = Instantiate(unit.prefab, location.position, Quaternion.identity).GetComponent<Unit>();
        }
        temp.GetComponent<Unit>().Init(true);
    }
    public void SpawnSword()
    {
        if (!(Random.Range(0, 15) == 0))
            return;
        var temp = Instantiate(Sword, location.position, Quaternion.identity);
        temp.GetComponent<Unit>().Init(true);
    }
    [Button("Spawn Sword")]
    public void SpawnSwordTrue()
    {

        var temp = Instantiate(Sword, location.position, Quaternion.identity);
        temp.GetComponent<Unit>().Init(true);
    }
    public void SpawnArcher()
    {
        if (!(Random.Range(0, 20) == 0))
            return;
        var temp = Instantiate(Archer, location.position, Quaternion.identity);
        temp.GetComponent<Unit>().Init(true);
    }
}
