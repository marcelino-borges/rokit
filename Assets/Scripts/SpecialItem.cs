using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialItem : MonoBehaviour {
    private Gem ownerGem;
    private Match3Manager match3Manager;
    private GameGrid grid;

    // Start is called before the first frame update
    void Start() {
        ownerGem = gameObject.GetComponent<Gem>();
        match3Manager = ownerGem.match3Manager;
        grid = match3Manager.grid;

        ownerGem.isSpecialItem = true;

        //Debug.Log("SpecialItem.Start(): match3Manager is null = " + (match3Manager == null));
        //Debug.Log("ownerGem.row = " + ownerGem.row);
    }

    // Update is called once per frame
    void Update() {
        //If the item has not been collected, checks
        if (!match3Manager.hasCollectedSpecialItem) {
            //If the special item has reached the end of the grid
            if (ownerGem.row == 0) {
                Collect();
            //Or if the item has reached row 1 and has a blank space at row 0
            } else if (ownerGem.row == 1 && ownerGem.grid.blankSpaces[ownerGem.column, 0]) {
                Collect();
            }
        }
    }

    private void Collect() {
        //Collect
        match3Manager.hasCollectedSpecialItem = true;

        //Add the respective score of the item
        match3Manager.AddScore(ownerGem.match3Manager.specialItemBaseValue);

        //Showing on the UI that the special item exists
        match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().specialItemCheckUI.SetActive(true);

        //Animating the special item when mission is complete
        match3Manager.uIElements.playerHud.GetComponent<Animator>().Play("special_item_earned");

        //If we have a list of missions initialized
        //And if we have a special item mission already added to the list
        if (match3Manager.missionsInLevel != null && match3Manager.missionsInLevel.Contains(new Mission(MissionType.SpecialItem))) {
            //Find this mission and set it's status to true
            match3Manager.missionsInLevel.Find(x => x.missionType == MissionType.SpecialItem).isCompleted = true;
            //Debug.Log("Special item status in the list: " + match3Manager.missionsInLevel.Find(x => x.missionType == MissionType.SpecialItem).isCompleted);
        }

        //Destroy the item using the correct method
        Invoke("DestroySpecialItem", 1.5f);
    }   

    private void DestroySpecialItem() {
        //Destroy it, following an ordinary gem destruction process
        ownerGem.grid.PickGem(ownerGem.row, ownerGem.column);
        ownerGem.grid.DestroySingleGem(ownerGem.column, ownerGem.row);
        StartCoroutine(grid.DecreaseRowCoRoutine());
        SoundManagerScript.PlaySound("collectedItem");

        //--------NAO TA DESTRUINDO O ITEM ESPECIAL
    }
}
