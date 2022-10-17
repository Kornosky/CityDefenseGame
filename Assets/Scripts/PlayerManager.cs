using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class StructureRequested : UnityEvent<UnitStructure> { }
[System.Serializable] public class StructureCancelled : UnityEvent<UnitStructure> { }
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] int SELECTION_RADIUS = 200;
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
    [SerializeField] private List<UnitStructure> structureBuildQueue = new List<UnitStructure>();
    public StructureRequested structureRequestedAction;
    public StructureCancelled structureCancelledAction;
    public Transform enemyBase;
    public Transform playerBase;
    public AbilityScriptableObject currentAbility;
    public Canvas playerCanvas;
    [Header("Temp")]
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] CommandWheel commandWheel;
    private bool hasLoaded;
    public bool levelWin;
    private float timeSinceTouchBegin;
    private bool isLongHold;
    private bool isHeld;
    private Vector3 startingTouchLocation;

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

    public void AddStructureToQueue(UnitStructure structure)
    {
        structureBuildQueue.Add(structure);
        structureRequestedAction.Invoke(structure);
    }
    public void RemoveStructureFromQueue(UnitStructure structure)
    {
        structureBuildQueue.Remove(structure);
    }
    public UnitStructure CheckForStructures()
    {
        if(structureBuildQueue.Count != 0)
            //Search through queue and find a structure that hasn't hit their limit
            foreach(UnitStructure structure in structureBuildQueue)
            {
                if(!structure.IsAtWorkerLimit())
                    return structure;
            }

        return null;
    }
    public void SpawnUnit(UnitScriptableObject unit)
    {
        if (GameManager.Instance.DebugActive("Spawn For Enemy"))
        {
            // Spawn for enemy instead
            EnemyManager.Instance.SpawnUnit(unit);
            return;
        }
        PlayerRecording.Instance.AddUnitsToDictionary(PlayerRecording.Instance.UnitSpawnCount, unit, 1);

         var temp = Instantiate(unit.prefab, location.position, Quaternion.identity);
        temp.GetComponent<Unit>().Init(false, unit);
    }
    public bool CheckPurchase(UnitScriptableObject unit)
    {
        if (GameManager.Instance.debugDict["UnlimitedMoney"])
            return true;
        if (Money - unit.Cost >= 0)
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
        if (GameManager.Instance.debugDict["UnlimitedMana"])
            return true;
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
        PlayerRecording.Instance.MoneyEarnedTotal -= unit.Cost;
        EnemyManager.Instance.spawnedUnit.Invoke(unit);
        Money -= unit.Cost;
        if(!(unit is StructureScriptableObject))
            SpawnUnit(unit);
    }
    private void Update()
    {
        isPlaced = false;
        //I dont like this here
#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            bool twoFingersDown = false;
            Touch touch = Input.GetTouch(0);
            if (Input.touchCount == 2)
                twoFingersDown = true;

            Vector3 touchLoc = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            if (twoFingersDown && isPlacing)
            {
                isPlaced = true;
                return;
            }

            // Handle finger movements based on touch phase.
            switch (touch.phase)
            {
                // Record initial touch position.
                case TouchPhase.Began:
                    startingTouchLocation = touch.position;
                    isHeld = true;
                    break;

                // Determine direction by omaring the current touch position with the initial one.
                case TouchPhase.Moved:
                    // If finger moves too far, then it's not being held in place anymore
                    if(Mathf.Abs(Vector3.Distance(startingTouchLocation, touch.position)) > 100)
                    {
                        isHeld = false;
                    }
                    break;
                // A finger is touching the screen but hasn't moved.
                case TouchPhase.Stationary:                 
                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:
                    timeSinceTouchBegin = 0;
                    isLongHold = false;
                    
                    CameraController.Instance.LockCameraControl(false);
                    commandWheel.Despawn();
                    break;
            }

            // Determine if long press and don't go past if-so
            timeSinceTouchBegin += Time.deltaTime;
            if (timeSinceTouchBegin > .5f && !isLongHold && isHeld)
            {
                CameraController.Instance.LockCameraControl(true);
                commandWheel.Spawn(Camera.main.WorldToScreenPoint(touchLoc));
                isLongHold = true;
            }
            else if (isLongHold)
                commandWheel.UpdateVisuals(Camera.main.WorldToScreenPoint(touchLoc));
                return;

            /////////// Tapping actions
            //Handle what is being touched here (negate collider overlap with logic)
            Collider2D[] hits = Physics2D.OverlapCircleAll(touchLoc, .3f);
            foreach (Collider2D hit in hits)
            {
                Pickup pick = hit.GetComponent<Pickup>();
                if (pick != null)
                {
                    pick.OnPressed();
                    break;
                }
              
            }

            if (currentAbility != null)
            {
                currentAbility.executeEffect.Invoke();
            }
        }
#else

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
#endif
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
        Action onFinished;
       // onFinished = CameraController.Instance.FocusOnTarget(unit.transform);

        while (isPlacing)
        {
            if (Input.touchCount > 0)
            {
                CameraController.Instance.LockCameraControl(true);
                Touch touch = Input.GetTouch(0);
                point = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                unit.transform.position = new Vector3(point.x, point.y, 0);
            }
            else
            {
                CameraController.Instance.LockCameraControl(false);
            }
            yield return new WaitForEndOfFrame();

            if (unit.GetComponent<IPlaceable>().IsValidPosition() && isPlaced)
            {
                isPlacing = false;
            }            
        }
        CameraController.Instance.LockCameraControl(false);
        if (isPlacingCancelled) //is placed
        {
            Destroy(currentPlacingGO);
        }
        else
        {
            if(!unit.GetComponent<IPlaceable>().IsValidPosition()) //needs refactoring
            {
                Destroy(currentPlacingGO);
            }
            unit.GetComponent<IPlaceable>().Placing(false);
            BuyUnit(unit.GetComponent<Unit>().GetInfo());
            donePlacingAction.Invoke();
        }
     //   onFinished.Invoke();

    }
    public void Charge()
    {
        // get allies in area and give retreat        
        Vector3 touchLoc = Camera.main.ScreenToWorldPoint(commandWheel.transform.position);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(touchLoc, SELECTION_RADIUS);
        foreach (Collider2D collider in colliders)
        {
            Unit selectedUnit = collider.GetComponent<Unit>();
            if (selectedUnit != null && !selectedUnit.isEnemy)
                selectedUnit.ChangeTarget(enemyBase.transform);
        }
    }
    public void Retreat()
    {
        // get allies in area and give retreat        
        Vector3 touchLoc = Camera.main.ScreenToWorldPoint(commandWheel.transform.position);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(touchLoc, SELECTION_RADIUS);
        foreach (Collider2D collider in colliders)
        {
            Unit selectedUnit = collider.GetComponent<Unit>();
            if (selectedUnit != null && !selectedUnit.isEnemy)
                selectedUnit.ChangeTarget(playerBase.transform);
        }
    }
    public void GameOver(bool isWin)
    {
        levelWin = isWin;
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
