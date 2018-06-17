using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;

public class CommunicationClient : NetworkBehaviour {

    private static CommunicationClient _this = null;
    internal static CommunicationClient LocalInstance{
        get{
            return _this;
        }
    }

    public static int ClientID{
        get{
            if(_this == null){ 
                return -1; 
            }
            else{ 
                return _this.id;
            }
        }
    }


    [SerializeField][SyncVar] private int id = -1;
    public int ID{
        get{
            return this.id;
        }
    }

    private NetworkIdentity networkIdentity = null;
    public NetworkIdentity NetworkIdentity{
        get{
            if(this.networkIdentity == null){
                this.networkIdentity = this.GetComponent<NetworkIdentity>();
            }
            return this.networkIdentity;
        }
    }



    protected virtual void Start(){
        if(this.isLocalPlayer){
            _this = this;
        }
        if(this.isServer){
            CommunicationServer.Clients.RegisterClient(this);
        }
    }

    protected virtual void OnDestroy(){
        if(this.isServer){
            CommunicationServer.Clients.DeRegisterClient(this);
        }
    }



    internal void SetID(int id){
        if(this.isServer){
            this.id = id;
        }
    }



    
    #region Server calling
    

    public void CallOnServer(CommunicationDelegates<GameObject>.Method method){
        GameObject obj = null;
        if(method.Target != null && method.Target is Component && ((Component)method.Target).gameObject.GetComponent<NetworkIdentity>() != null){
            obj = (method.Target as Component).gameObject;
        }
        else if(method.Target != null && method.Target is GameObject){
            obj = method.Target as GameObject;
        }
        this.Cmd_CallOnServer0(obj, method.Method.DeclaringType.FullName, method.Method.Name, LocalInstance.gameObject);
    }
    /*
    public void CallOnServer<T>(CommunicationDelegates<T>.Method method, T arg) {
    }
    public void CallOnServer<T0, T1>(CommunicationDelegates<T0, T1>.Method method, T0 arg0, T1 arg1) {
    }
    public void CallOnServer<T0, T1, T2>(CommunicationDelegates<T0, T1, T2>.Method method, T0 arg0, T1 arg1, T2 arg2) {
    }
    public void CallOnServer<T0, T1, T2, T3>(CommunicationDelegates<T0, T1, T2, T3>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3) {
    }
    public void CallOnServer<T0, T1, T2, T3, T4>(CommunicationDelegates<T0, T1, T2, T3, T4>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
    }
    public void CallOnServer<T0, T1, T2, T3, T4, T5>(CommunicationDelegates<T0, T1, T2, T3, T4, T5>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
    }
    public void CallOnServer<T0, T1, T2, T3, T4, T5, T6>(CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
    }
    public void CallOnServer<T0, T1, T2, T3, T4, T5, T6, T7>(CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6, T7>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) {
    }
    public void CallOnServer<T0, T1, T2, T3, T4, T5, T6, T7, T8>(CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6, T7, T8>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) {
    }
    public void CallOnServer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Method method, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) {
    }
    */

    private void ServerSide_CallOnServer(GameObject obj, string type, string method, GameObject caller){
        System.Type t = System.Type.GetType(type);
        Component component = null;
        if(obj != null){ 
            component = obj.GetComponent(t);
        }
        var methodInfo = t.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo != null){
            methodInfo.Invoke(component, new object[] { caller });
        }
    }

    [Command]
    protected void Cmd_CallOnServer0(GameObject obj, string type, string method, GameObject caller){
        ServerSide_CallOnServer(obj, type, method, caller);
    }



    public void CallOnAllClients(CommunicationDelegates.Method method){
        GameObject obj = null;
        if(method.Target != null && method.Target is Component && ((Component)method.Target).gameObject.GetComponent<NetworkIdentity>() != null){
            obj = (method.Target as Component).gameObject;
        }
        this.Rpc_CallOnAllClients(obj, method.Method.DeclaringType.FullName, method.Method.Name);
    }

    private void ClientSide_CallOnAllClients(GameObject obj, string type, string method, params object[] args){
        System.Type t = System.Type.GetType(type);
        Component component = null;
        if(obj != null){ 
            component = obj.GetComponent(t);
        }
        var methodInfo = t.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo != null){
            methodInfo.Invoke(component, args);
        }
    }

    [ClientRpc]
    protected void Rpc_CallOnAllClients(GameObject obj, string type, string method){
        ClientSide_CallOnAllClients(obj, type, method);
    }



    public void CallOnTargetClient(CommunicationDelegates.Method method) {
        GameObject obj = null;
        if(method.Target != null && method.Target is Component && ((Component)method.Target).gameObject.GetComponent<NetworkIdentity>() != null) {
            obj = (method.Target as Component).gameObject;
        }
        this.Target_CallOnTargetClient(this.connectionToClient, obj, method.Method.DeclaringType.FullName, method.Method.Name);
    }

    private void ClientSide_CallOnTargetClient(GameObject obj, string type, string method, params object[] args) {
        System.Type t = System.Type.GetType(type);
        Component component = null;
        if(obj != null) {
            component = obj.GetComponent(t);
        }
        var methodInfo = t.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo != null) {
            methodInfo.Invoke(component, args);
        }
    }

    [TargetRpc]
    protected void Target_CallOnTargetClient(NetworkConnection networkConnection, GameObject obj, string type, string method) {
        ClientSide_CallOnTargetClient(obj, type, method);
    }

    #endregion


}
