using System;
using System.Collections.Generic;
using UnityEngine;


class EndTurnButton : MonoBehaviour {

    [SerializeField] private GameObject buttonObj = null;

    private void Update() {
        if(NetworkedGameManager.IsThisPlayersTurn && !this.buttonObj.activeInHierarchy){
            this.buttonObj.SetActive(true);
        }
        else if(!NetworkedGameManager.IsThisPlayersTurn && this.buttonObj.activeInHierarchy){
            this.buttonObj.SetActive(false);
        }
    }


    public void EndTurn(){

        CommunicationClient.LocalInstance.CallOnServer(NetworkedGameManager.SwitchTurns);

    }

}

