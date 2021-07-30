using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class GenericLootDropTableGameObject : GenericLootDropTable<GenericLootDropItemGameObject, GameObject> { }
[System.Serializable]
public class GenericLootDropItemGameObject : GenericLootDropItem<GameObject> { }

public class LootboxCanvas : MenuElement
{
    [SerializeField] Animator lootBoxAnim;
    [SerializeField] Button boxButton;
    [SerializeField] Button resetButton;
    // Loot drop table that contains items that can spawn
    public GenericLootDropTableGameObject lootDropTable;

    // How many items treasure will spawn
    public int numItemsToDrop;
    private void Reset()
    {
        Awake();
    }
    private void Awake()
    {
        lootBoxAnim ??= GetComponentInChildren<Animator>();
        boxButton ??= GetComponentInChildren<Button>();
        resetButton.gameObject.SetActive(false);
    }
    public void Init()
    {

    }
    public void OpenBox()
    {
        //If can't pay for box then play animation
        if (GameManager.Instance.TryPurchase(5, GameManager.CurrencyType.RARE))
        {
            resetButton.gameObject.SetActive(true);
            boxButton.interactable = false;
            lootBoxAnim.Play("Open");
            //get random box
            DropLootNearChest(5);
        }
        else
        {
            lootBoxAnim.Play("UnableToPurchase");
        }
    }

    public void ResetBox()
    {
        resetButton.gameObject.SetActive(false);
        boxButton.interactable = true;

        lootBoxAnim.Play("Reset");

    }

    // Runs when we start our game
    public void Start()
    {

        // Spawn objects in a straight line
        DropLootNearChest(numItemsToDrop);

    }

    void OnValidate()
    {

        // Validate table and notify the programmer / designer if something went wrong.
        lootDropTable.ValidateTable();

    }

    /// <summary>
    /// Spawning objects in horizontal line
    /// </summary>
    /// <param name="numItemsToDrop"></param>
    void DropLootNearChest(int numItemsToDrop)
    {
        for (int i = 0; i < numItemsToDrop; i++)
        {
            GenericLootDropItemGameObject selectedItem = lootDropTable.PickLootDropItem();
            GameObject selectedItemGameObject = Instantiate(selectedItem.item);
            selectedItemGameObject.transform.position = new Vector2(i / 2f, 0f);
        }
    }
}
