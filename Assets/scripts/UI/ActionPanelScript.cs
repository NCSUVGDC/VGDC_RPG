using UnityEngine;
using VGDC_RPG;
using UnityEngine.UI;

public class ActionPanelScript : MonoBehaviour
{
    public bool isUnitMine = false;
    private RectTransform rt;

    private Button moveButton, attackButton, inventoryButton;

    // Use this for initialization
    void Start()
    {
        rt = GetComponent<RectTransform>();
        moveButton = transform.FindChild("MoveButton").GetComponent<Button>();
        attackButton = transform.FindChild("AttackButton").GetComponent<Button>();
        //inventoryButton = transform.FindChild("InventoryButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        isUnitMine = GameLogic.IsMyTurn && !InputManager.InEditMode;

        if (isUnitMine && rt.anchoredPosition.y < 0)
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y + 1);
        if (!isUnitMine && rt.anchoredPosition.y > -30)
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y - 1);
        //if (isUnitMine && rt.anchoredPosition.y > )

        if (isUnitMine)
        {
            var u = GameLogic.Units[GameLogic.CurrentPlayer][GameLogic.CurrentUnitID];

            if (u != null && !u.HasMoved)
                moveButton.interactable = true;
            else
                moveButton.interactable = false;

            if (u != null && !u.HasAttacked)
                attackButton.interactable = true;
            else
                attackButton.interactable = false;
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
