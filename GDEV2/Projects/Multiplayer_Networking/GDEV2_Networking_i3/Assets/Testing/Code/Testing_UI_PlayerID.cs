using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Testing_UI_PlayerID : MonoBehaviour {

    [SerializeField] private Text text = null;

    private void FixedUpdate() {
        int id = CommunicationClient.ClientID;
        if(id > 0){ 
            this.text.text = "Client ID: " + id;
        }
        else{
            this.text.text = "Error: No client ID found";
        }
    }

}
