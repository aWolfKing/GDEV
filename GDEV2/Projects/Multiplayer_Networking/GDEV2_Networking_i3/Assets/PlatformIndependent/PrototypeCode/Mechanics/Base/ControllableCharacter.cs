using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ControllableCharacter : NetworkBehaviour {

    [SerializeField][SyncVar] private int controllingPlayer = -1;
    public int PlayerID{
        get{
            return this.controllingPlayer;
        }
    }

    private static ControllableCharacter _this = null;


    private void Start() {
        _this = this;
        //this.controllingPlayer = CommunicationClient.ClientID;
    }

    private void Update() {
        //this.controllingPlayer = CommunicationClient.ClientID;
    }

    public void SetController(int id){
        if(this.isServer){ 
            this.controllingPlayer = id;
        }
    }


    internal static void UpdateController(){
        _this.controllingPlayer = CommunicationClient.ClientID;
    }

}

