using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitInfo", menuName = "ScriptableObjects/Unit", order = 1)]

public class UnitScriptableObject : ScriptableObject
{
    public new string name;
    [TextArea] public string description;
    public Sprite sprite;
    public GameObject prefab;
    [Header("Structure")]
    public bool isStructure;
    public float buildTime;
    public int workersRequired;
    [Header("General")]
    public float cooldownPeriod;
    public int cost;
    public int health;
    public int damage;
    public Vector2 knockback;
    public Vector2 recoil;
    public float moveSpeed;
    public bool canMoveWhileActing;
    public bool hasDeathAnimation;
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
    public float u_buildTime;
    public int u_workersRequired;
    public float u_cooldownPeriod;
    public int u_cost;
    public int u_health;
    public int u_damage;
    public Vector2 u_knockback;
    public Vector2 u_recoil;
    public float u_moveSpeed;
    public int u_actionDmgInterruptThreshold;
    public float u_actionCD;
    public bool u_hasSpecial;
    public int u_specialCost;
    public bool u_isRanged;
    public float u_range;
    public float u_projectileSpeed;
    public float u_accuracy;

    [Button("Reset Upgrades")]
    void ResetUpgrades()
    {

    }

    public bool isUnlocked;
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

    public void Unlock()
    { 
        isUnlocked = true;
    }
    public void Save()
    {
        GameManager.Instance.data.unitScriptableObjects[name] = JsonUtility.ToJson(this);
    }
    public void Load()
    {
        if (GameManager.Instance.data.unitScriptableObjects.ContainsKey(name))
            JsonUtility.FromJsonOverwrite(GameManager.Instance.data.unitScriptableObjects[name], this);

        //Make sure everything is set up correct after reading dataf
        if (isUnlocked)
        {
            Unlock();
        }
    }

}
