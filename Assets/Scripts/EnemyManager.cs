using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject Sword;
    public GameObject Archer;
    public Transform location;
    public bool shouldSpawn;
    [SerializeField] private Dictionary<UnitScriptableObject, int> unitSpawnedSinceLastTick = new Dictionary<UnitScriptableObject, int>();
    private float chanceModifier;
    private Unit nexus;
    [SerializeField] int spawnCoin;
    [SerializeField] List<UnitScriptableObject> availableUnits = new List<UnitScriptableObject>();
    //Information gathering
    public UnityEvent<UnitScriptableObject> spawnedUnit;
    [SerializeField] float actionMeter;
    [SerializeField] int difficulty;
    [SerializeField] int maxDifficulty = 10;
    private void Start()
    {
        if (!shouldSpawn)
            return;
        chanceModifier = 1;
        nexus = PlayerManager.Instance.enemyBase.GetComponent<Unit>();
        InvokeRepeating("EnemyTick", 0, 1f);
    }
    void IncreaseActionChance(float amt)
    {
        actionMeter += amt;
    }

    void ExpendAction(float amt)
    {
        actionMeter -= amt;
    }

    bool TryActionChance(float actionAmt, Action action)
    {
        if(actionMeter >= actionAmt)
        {
            ExpendAction(actionAmt);
            action.Invoke();
            return true;
        }
        return false;
    }
    protected override void Awake()
    {
        base.Awake();
        spawnedUnit.AddListener(ProcessUnit);
    }
    public void Init(LevelScriptableObject info)
    {
        availableUnits = new List<UnitScriptableObject>(info.units);
        difficulty = info.difficulty;
    }
    private void EnemyTick()
    {
        if (GameManager.Instance.DebugActive("Spawn For Enemy"))
        {
            // Bounce if actived
            return;
        }

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
        TryActionChance(.5f, SpendCoin);

        foreach (var unit in PlayerRecording.Instance.UnitSpawnCount)
        {
            float chance = 0;
            chance += unit.Key.Cost;
            chance += unit.Value;
            //Spawned Since last tick
            chance *= unitSpawnedSinceLastTick[unit.Key];
            //Currently active of this type
            //chance *= PlayerRecording.Instance.ActiveUnits[unit.Key];
            ////Total active right now
            //chance *= PlayerRecording.Instance.GetTotalInDictionary(PlayerRecording.Instance.ActiveUnits);

            Debug.Log("Chance for " + unit.Key + ": " + chance);


            //Look at the number of units on field
            if (chance > UnityEngine.Random.Range(0, chance) && UnityEngine.Random.Range(0,5) == 0)
            {
                SpawnUnit(unit.Key);
                spawnedSomething = true;
            }
        }

        if(!spawnedSomething)
        {
            //SpawnSword();
            //SpawnArcher();
        }

        var healthBundle = nexus.GetHealth();
        if (healthBundle.curHealth < healthBundle.maxHealth / 2)
        {
            //SpawnSword();
            //SpawnArcher();
            IncreaseActionChance(.1f);
        }

        IncreaseActionChance(.05f);
        //Set for next tick
        unitSpawnedSinceLastTick = new Dictionary<UnitScriptableObject, int>(PlayerRecording.Instance.UnitSpawnCount);
    }
    void SpendCoin()
    {
        int i = 4;
        UnitScriptableObject unit = availableUnits[UnityEngine.Random.Range(0, availableUnits.Count)];
        SpawnUnit(unit);
        ////Randomly try to buy a unit, but quit after 4 attempts
        //while(spawnCoin != 0 && i >= 0)
        //{
        //    UnitScriptableObject unit = availableUnits[UnityEngine.Random.Range(0, availableUnits.Count)];
        //    if (TryPurchase(unit)) 
        //        SpawnUnit(unit);
        //    i--;
        //}
    }
    public bool TryPurchase(UnitScriptableObject unit)
    {
        if (spawnCoin - unit.Cost >= 0)
        {
            spawnCoin -= unit.Cost;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ProcessUnit(UnitScriptableObject unit)
    {
        IncreaseActionChance(.2f);
        spawnCoin += Mathf.Clamp(unit.Cost * (difficulty) / maxDifficulty, 0, 10000);
    }

    public void SpawnUnit(UnitScriptableObject unit)
    {       
        ExpendAction(.4f);
        Unit temp = null;
        if (unit is StructureScriptableObject)
        {
           // temp = Instantiate(unit.prefab, location.position, Quaternion.identity).GetComponent<Unit>();
        }
        else
        {
            temp = Instantiate(unit.prefab, location.position, Quaternion.identity).GetComponent<Unit>();
        }
        temp.GetComponent<Unit>().Init(true, unit);
    }

    [Button("Spawn Sword")]
    public void SpawnSwordTrue()
    {

        var temp = Instantiate(Sword, location.position, Quaternion.identity);
        temp.GetComponent<Unit>().Init(true);
    }

}
