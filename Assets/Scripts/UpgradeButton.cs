using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GridLayoutGroup gemLayoutGroup;
    [SerializeField] TMP_Text fieldName;
    [SerializeField] TMP_Text cost;
    GameObject gemPrefab;
    UnitScriptableObject info;
    FieldInfo field;
    private object upgradeValue;
    private bool isNumber;

    UnitScriptableObject.UpgradeType upgradeType;
    void Awake()
    {
        gemPrefab = Resources.Load<GameObject>("UpgradeGem");
        button.onClick.AddListener(OnPressed);
    }
    public void Init(UnitScriptableObject info, KeyValuePair<UnitScriptableObject.UpgradeType, UnitScriptableObject.UpgradeStruct> field)
    {
        this.info = info;
        upgradeType = field.Key;
        if (field.Value.overrideName == "")
            fieldName.text = field.Key.ToString().Replace('_', ' '); //convention~!
        else
            fieldName.text = field.Value.overrideName;

        FieldInfo[] properties = typeof(UnitScriptableObject.UpgradeStruct).GetFields();
        FieldInfo upgradeProperty = null;
        upgradeValue = null;
        isNumber = false;

        //find and cache relevant field
        foreach (FieldInfo property in properties)
        {
            switch (property.FieldType.FullName)
            {
                case "System.Int16": 
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    Debug.Log(GetFieldValue(field.Value, property.Name).GetType());
                    float value = Convert.ToSingle(GetFieldValue(field.Value, property.Name));
                    if (value != 0)
                    {
                        isNumber = true;
                        this.field = property;
                        upgradeValue = value;
                    }
                    break;
                case "System.Boolean":
                    bool v = GetFieldValue<bool>(field.Value, property.Name); //cant check for bool (only two states)
                    upgradeValue = v;
                    break;
                case "System.Char":
                case null:
                    Debug.LogError(info + " has the field " + property.Name + " with a unsupported type");
                    break;
            }

            //Found valid value
            if (upgradeValue != null)
            {
                upgradeProperty = property;
                break;
            }
        }

        //Create rank upgrade gems
        for (int i = 0; i < field.Value.rankMax; i++)
        {
            var temp = Instantiate(gemPrefab, gemLayoutGroup.transform);
            if (i < field.Value.rank)
                temp.GetComponent<UpgradeGem>().ActivateGem(true);
            else
                temp.GetComponent<UpgradeGem>().ActivateGem(false);
        }

        //if it is a number then put a number otherwise bool
        if (isNumber)
        {           
            if (field.Value.rank == field.Value.rankMax)
                button.interactable = false;
        }
        else
        {
            if ((bool)upgradeValue)
                button.interactable = false;
        }

        //Cost
        if(field.Value.cost.Length > field.Value.rank) //if there are incrementing costs
            cost.text = field.Value.cost[field.Value.rank].ToString();
        else
            cost.text = field.Value.cost[0].ToString();

        //Go ahead and update visuals
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        //Create rank upgrade gems
        for (int i = 0; i < info.upgrades[upgradeType].rankMax; i++)
        {
            var temp = gemLayoutGroup.transform.GetChild(i);
            if (i < info.upgrades[upgradeType].rank)
                temp.GetComponent<UpgradeGem>().ActivateGem(true);
            else
                temp.GetComponent<UpgradeGem>().ActivateGem(false);
        }

        //update cost if still available
        if (cost != null)
        {
             if (info.upgrades[upgradeType].cost.Length > info.upgrades[upgradeType].rank) //if there are incrementing costs
                cost.text = info.upgrades[upgradeType].cost[info.upgrades[upgradeType].rank].ToString();
             else
                cost.text = info.upgrades[upgradeType].cost[0].ToString();
        }

        //update if max rank
        if (info.upgrades[upgradeType].rankMax == info.upgrades[upgradeType].rank)
        {
            Destroy(button.gameObject);
            Destroy(cost.gameObject);
        }
    }
    private void OnPressed()
    {
        //buy upgrade if possible
        if(info.upgrades[upgradeType].rankMax != info.upgrades[upgradeType].rank && GameManager.Instance.TryPurchase(Int32.Parse(cost.text), GameManager.CurrencyType.GOLD))
        {
            //Level up rank if not max rank
            info.upgrades[upgradeType].rank += 1;
            UpdateVisuals();
        }
        else
        {
            Tween.AnchoredPosition(button.GetComponent<RectTransform>(),(Vector2) button.transform.localPosition + UnityEngine.Random.insideUnitCircle * 6f, .2f, 0, Tween.EaseWobble, Tween.LoopType.None);
        }
    }

    public static T GetFieldValue<T>(object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        var field = obj.GetType().GetField(fieldName, BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance);

        if (field == null)
            throw new ArgumentException("fieldName", "No such field was found.");

        if (!typeof(T).IsAssignableFrom(field.FieldType))
            throw new InvalidOperationException("Field type and requested type are not compatible.");

        return (T)field.GetValue(obj);
    }   
    public static object GetFieldValue(object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        var field = obj.GetType().GetField(fieldName, BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance);

        if (field == null)
            throw new ArgumentException("fieldName", "No such field was found.");


        return field.GetValue(obj);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
