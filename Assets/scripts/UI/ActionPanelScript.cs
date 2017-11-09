using UnityEngine;
using VGDC_RPG;
using UnityEngine.UI;
using System;

public class ActionPanelScript : MonoBehaviour
{
    public bool isUnitMine = false;
    public bool inInventory = false;
    private RectTransform rt;
    private RectTransform invRect;

    private Button moveButton, attackButton, inventoryButton, potionButton;
   
    // Use this for initialization
    void Start()
    {
        rt = GetComponent<RectTransform>();
        moveButton = transform.FindChild("MoveButton").GetComponent<Button>();
        attackButton = transform.FindChild("AttackButton").GetComponent<Button>();
        inventoryButton = transform.FindChild("InventoryButton").GetComponent<Button>();
        invRect = inventoryButton.transform.FindChild("Inventory").GetComponent<RectTransform>(); ///FindChild("Inventory").GetComponent<RectTransform>();
        potionButton = invRect.FindChild("PotionButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        isUnitMine = GameLogic.IsMyTurn && !InputManager.InEditMode;

        if (isUnitMine && rt.anchoredPosition.y < 0)
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y + 50);
        if (!isUnitMine && rt.anchoredPosition.y > -1000)
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y - 50);

        if (isUnitMine)
        {
            var u = GameLogic.Units[GameLogic.CurrentPlayer][GameLogic.CurrentUnitID];

            if (u != null && !u.HasMoved)
            {
                moveButton.interactable = true;
                moveButton.enabled = true;
            }
            else
            {
                moveButton.interactable = false;
                moveButton.enabled = false;
            }
               

            if (u != null && !u.HasAttacked)
            {
                attackButton.interactable = true;
                attackButton.enabled = true;
            }
            else
            {
                attackButton.interactable = false;
                attackButton.enabled = false;
            }

            if (u != null)
            {
                inventoryButton.interactable = true;
            }
            else
            {
                inventoryButton.interactable = false;
            }

            if (u != null)
            {
                potionButton.interactable = true;
            }    
            else
            {
                potionButton.interactable = false;
            }
                
        }
    }

    public void EndTurnPressed()
    {
        GameLogic.EndTurn();
    }

    public void MovePressed()
    {
        GameLogic.ReqSetState(GameLogic.ActionState.Move);
    }

    public void AttackPressed()
    {
        GameLogic.ReqSetState(GameLogic.ActionState.Attack);
    }

    public void InventoryPressed()
    {
        Debug.Log("In InventoryPressed");
        /// Spawn inventory
        if(invRect == null)
        {
            Debug.Log("Null component");
        }

        /// Check if inventory is inactive
        if (!inInventory)
        {
            /// Show inventory
            invRect.gameObject.SetActive(true);
            inInventory = true;
        }
        else
        {
            /// don't show inventory
            invRect.gameObject.SetActive(false);
            inInventory = false;
        }
    }

    public void PotionPressed()
    {

        GameLogic.ReqSetState(GameLogic.ActionState.Potion);
    }


    public void PrevPressed()
    {
        /*if (GameLogic.CurrentUnitID == 0)
            GameLogic.ReqSetUnit((byte)(GameLogic.Units[GameLogic.MyPlayerID].Count - 1));
        else
            GameLogic.ReqSetUnit((byte)(GameLogic.CurrentUnitID - 1));*/
        //var cuid = GameLogic.CurrentUnitID;
        //var i = 200;
        //do
        //{
        //    cuid--;
        //    if (cuid < 0)
        //        cuid = (byte)(GameLogic.Units[GameLogic.MyPlayerID].Count - 1);
        //    i--;
        //}
        //while (!GameLogic.Units[GameLogic.MyPlayerID][cuid].Stats.Alive && i >= 0);
        //GameLogic.ReqSetUnit(cuid);
    }

    public void NextPressed()
    {
        //GameLogic.ReqSetUnit((byte)((GameLogic.CurrentUnitID + 1) % GameLogic.Units[GameLogic.MyPlayerID].Count));
        //var cuid = GameLogic.CurrentUnitID;
        //var i = 200;
        //do
        //{
        //    cuid++;
        //    if (cuid >= GameLogic.Units[GameLogic.CurrentPlayer].Count)
        //        cuid = 0;
        //    i--;
        //}
        //while (!GameLogic.Units[GameLogic.CurrentPlayer][cuid].Stats.Alive && i >= 0);
        //GameLogic.ReqSetUnit(cuid);
    }
}
