using System;
using System.Collections.Generic;

public static class CommunicationExtensions {

    public static void CallOnServer(this CommunicationDelegates<UnityEngine.GameObject>.Method method){
        CommunicationClient.LocalInstance.CallOnServer(method);
    }

}

