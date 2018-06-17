using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class CommunicationServer : NetworkBehaviour {

    private static CommunicationServer _this = null;



    protected virtual void Awake(){
        _this = this;
    }

}
