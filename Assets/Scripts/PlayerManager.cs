using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class StructureRequested : UnityEvent<Structure> { }
[System.Serializable] public class StructureCancelled : UnityEvent<Structure> { }
public class PlayerManager : Singleton<PlayerManager>
{
    private int money;
    private int mana;
    [SerializeField] private int moneyPassiveIncrease = 1;
    [SerializeField] private int manaPassiveIncrease = 1;
    public Transform location;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI manaText;
    public List<UnitScriptableObject> availableUnits = new List<UnitScriptableObject>();
    public List<AbilityScriptableObject> availableAbilities = new List<AbilityScriptableObject>();
    public Camera mainCam;
    [Range(0,2)] public float percentageEarnedFromKill;
    private GameObject currentPlacingGO;
    private bool isPlacing;
    private bool isPlacingCancelled;
    private bool isPlaced;
    public List<Unit> activeUnits = new List<Unit>();
    [SerializeField] private List<Structure> structureBuildQueue = new List<Structure>();
    public StructureRequested structureRequestedAction;
    public StructureCancelled structureCancelledAction;
    public Transform enemyBase;
    public Transform playerBase;
    public AbilityScriptableObject currentAbility;
    [Header("Temp")]
    public GameObject winScreen;
    public GameObject loseScreen;
    private bool hasLoaded;

    public int Money { get => money; set { money = value; if(moneyText) moneyText.text = money.ToString(); PlayerRecording.Instance.MoneyEarnedTotal += 1; } }
    //temp solution
    public int Mana { get => mana; set { mana = value; if (manaText) manaText.text = mana.ToString(); } }

    private void Start()
    {

    }
    public void Init(LevelScriptableObject info)
    {
        InvokeRepeating("MoneyTick", 0, 3);
        InvokeRepeating("ManaTick", 0, 2);
    }

    public void AddStructureToQueue(Structure structure)
    {
        structureBuildQueue.Add(structure);
        structureRequestedAction.Invoke(structure);
    }
    public void RemoveStructureFromQueue(Structure structure)
    {
        structureBuildQueue.Remove(structure);
    }
    public Structure CheckForStructures()
    {
        if(structureBuildQueue.Count > 0)
            return structureBuildQueue[0];
        return null;
    }
    public void SpawnUnit(UnitScriptableObject unit)
    {
        PlayerRecording.Instance.AddUnitsToDictionary(PlayerRecording.Instance.UnitSpawnCount, unit, 1);

         var temp = Instantiate(unit.prefab, location.position, Quaternion.identity);
        temp.GetComponent<Unit>().Init(false, unit);
    }
    public bool CheckPurchase(UnitScriptableObject unit)
    {
        if (Money - unit.cost >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckCast(AbilityScriptableObject unit)
    {
        if (Mana - unit.manaCost >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void BuyUnit(UnitScriptableObject unit)
    {
        PlayerRecording.Instance.MoneyEarnedTotal -= unit.cost;
        Money -= unit.cost;
        if(!unit.isStructure)
            SpawnUnit(unit);
    }
    private void Update()
    {
        isPlaced = false;
        //I dont like this here
        if (Input.GetMouseButtonDown(0))
        {
            if(isPlacing)
                isPlaced = true;
            if(currentAbility != null)
            {
                currentAbility.executeEffect.Invoke();
            }
        }            
        else if (Input.GetMouseButtonDown(1))
            CancelPlacing();
    }
    public void TryPlacingUnit(UnitScriptableObject unit, Action donePlacingAction)
    {
        if (isPlacing)
            return;
        GameObject uGO = Instantiate(unit.prefab, Vector3.zero, Quaternion.identity);
        uGO.GetComponent<Rigidbody2D>().isKinematic = true;
        uGO.GetComponent<Unit>().Init(false, unit);
        StartCoroutine(PlacingUnit(uGO, donePlacingAction));
    }

    void CancelPlacing()
    {
        if (!isPlacing)
            return;
        isPlacing = false;
        isPlacingCancelled = true;       
    }
    IEnumerator PlacingUnit(GameObject unit, Action donePlacingAction)
    {
        isPlacingCancelled = false;
        Vector3 point = new Vector3();
        currentPlacingGO = unit;
        isPlaced = false;
        isPlacing = true;

        unit.GetComponent<IPlaceable>().Placing(true);
        while (isPlacing)
        {           
            Vector2 mousePos = Input.mousePosition;
            point = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
            unit.transform.position = new Vector3(point.x, point.y, 0);

            yield return new WaitForEndOfFrame();

            if (unit.GetComponent<IPlaceable>().IsValidPosition() && isPlaced)
            {
                isPlacing = false;
            }
        }

        if(isPlacingCancelled) //is placed
        {
            Destroy(currentPlacingGO);
        }
        else
        {
            unit.GetComponent<IPlaceable>().Placing(false);
            BuyUnit(unit.GetComponent<Unit>().GetInfo());
            donePlacingAction.Invoke();
        }

    }
    public void GameOver(bool isWin)
    {
        if (isWin)
            winScreen.SetActive(true);
        else
            loseScreen.SetActive(true);
    }
    public void MoneyTick()
    {
        Money += moneyPassiveIncrease;
    }

    public void ManaTick()
    {
        Mana += manaPassiveIncrease;
    }

}
