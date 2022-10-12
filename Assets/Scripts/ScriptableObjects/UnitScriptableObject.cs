using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitInfo", menuName = "ScriptableObjects/Unit", order = 1)]

public class UnitScriptableObject : ScriptableObject
{
    public new string name;
    [TextArea] public string description;
    public Sprite sprite;
    public GameObject prefab;

    [Header("General")]
    [SerializeField] private float cooldownPeriod;
    [SerializeField] private int cost;
    [SerializeField] private int health;
    [SerializeField] private int damage;
    [SerializeField] private Vector2 knockback;
    [SerializeField] public Vector2 recoil;
    [SerializeField] private float moveSpeed;
    [SerializeField] public bool canMoveWhileActing;
    [SerializeField] public bool hasDeathAnimation;
    [SerializeField] public float actionDuration;
    //Dmg needed to interrupt action
    public int actionDmgInterruptThreshold;
    public bool hitsAllInRange;
    public float actionCD;
    public bool hasSpecial;
    public int specialCost;
    public bool isRanged;
    public float range;
    public float projectileSpeed;
    public float accuracy;
    [Header("Meta")]
    public bool isAvailable;
    [Header("Upgrades")]
    [SerializeField]
    public UpgradeDict upgrades;
    [System.Serializable]
    public enum UpgradeType
    {
        BUILD_TIME,
        SPAWN_COOLDOWN,
        COST,
        HEALTH,
        KNOCKBACK,
        DAMAGE,
        RECOIL,
        MOVESPEED,
        INTERRUPT_THRESHOLD,
        ACTION_COOLDOWN,
        SPECIAL_ABILITY,
        PROJECTILE_UPGRADE,
        ISRANGED,
        RANGE,
        PROJECTILE_SPEED,
        ACCURACY,
        ACTION_DURATION
    }
    [System.Serializable] public class UpgradeDict : SerializableDictionaryBase<UpgradeType, UpgradeStruct> { }
    [System.Serializable] public class UpgradeStruct
    {
        public string overrideName;
        public float f;
        public int i;
        public bool b;
        public Vector2 vec2;
        public GameObject projectilePrefab;
        //Cost per level, optionally a scaling value
        public int[] cost;
        //Upgrade level
        public int rank;
        public int rankMax;
    }
    [Button("Reset Upgrades")]
    public void ResetData()
    {
        foreach (var up in upgrades)
            up.Value.rank = 0;
    }

    public bool isUnlocked;

    public int Health
    { 
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.HEALTH);
            int change = uStruct != null ? uStruct.i * uStruct.rank + 1 : 0; 
            return health + change; 
        } 
        set => health = value; 
    }
    public int Damage
    {
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.DAMAGE);
            int change = uStruct != null ? uStruct.i * uStruct.rank + 1: 0;
            return damage + change;
        }
        set => damage = value;
    }
    public Vector2 Knockback
    {
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.KNOCKBACK);
            Vector2 change = uStruct != null ? uStruct.vec2 : Vector2.zero;
            return knockback + change;
        }
        set => knockback = value;
    }
    public int Cost
    {
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.COST);
            int change = uStruct != null ? uStruct.i * uStruct.rank + 1 : 0;
            return cost + change;
        }
        set => cost = value;
    }
    public float CooldownPeriod
    {
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.SPAWN_COOLDOWN);
            float change = uStruct != null ? uStruct.f * uStruct.rank + 1 : 0;
            return cooldownPeriod + change;
        }
        set => cooldownPeriod = value;
    }
    public float MoveSpeed
    {
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.MOVESPEED);
            float change = uStruct != null ? uStruct.f * uStruct.rank + 1 : 0;
            return moveSpeed + change;
        }
        set => moveSpeed = value;
    }
    public float ActionDuration
    {
        get
        {
            UpgradeStruct uStruct = GetUpgrade(UpgradeType.ACTION_DURATION);
            float change = uStruct != null ? uStruct.f * uStruct.rank + 1 : 0;
            return actionDuration + change;
        }
        set => actionDuration = value;
    }

    void OnEnable()
    {
        isUnlocked = false;

    }

    //Inits this scriptable object
    public void Initialize()
    {
        GameManager.Instance.SaveData += Save;
        GameManager.Instance.LoadData += Load;
    }

    public UpgradeStruct GetUpgrade(UpgradeType type)
    {
        UpgradeStruct uStruct;
        upgrades.TryGetValue(type, out uStruct);
        return uStruct;
    }
    /// <summary>
    /// Depricated
    /// </summary>
    /// <returns></returns>
    [ContextMenu("Upgrade Properties")]
    public List<FieldInfo> GetUpgradeableProperties()
    {
        FieldInfo[] properties = typeof(UnitScriptableObject).GetFields();
        List<FieldInfo> propList = new List<FieldInfo>();
        foreach (FieldInfo property in properties)
        {
            if (property.Name.StartsWith("u_"))
                propList.Add(property);
        }
        return propList;
    }
    public void Unlock()
    {
        isUnlocked = true;
    }
    public void Save()
    {
        GameManager.Instance.Data.unitScriptableObjects[name] = JsonUtility.ToJson(this);
    }
    public void Load()
    {
        if (GameManager.Instance.Data.unitScriptableObjects.ContainsKey(name))
            JsonUtility.FromJsonOverwrite(GameManager.Instance.Data.unitScriptableObjects[name], this);

        //Make sure everything is set up correct after reading dataf
        if (isUnlocked)
        {
            Unlock();
        }
    }

}
