using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkedGameManager : NetworkBehaviour {

    private class StoredInfo{
        public bool ready = false;
    }

    private static Dictionary<CommunicationClient, StoredInfo> client_storedInfo = new Dictionary<CommunicationClient, StoredInfo>();

    private static NetworkedGameManager _this = null;

    [SerializeField] private GameObject characterPrefab = null;
    private bool finishedAction = false;

    private Vector3 cameraVelocity = Vector3.zero;
    private float   input_x = 0f,
                    input_y = 0f;

    [SerializeField][HideInInspector][SyncVar]private bool isPlaying = false;

    private Vector3 targetPos = Vector3.zero;

    private float playTime = 0;
    public static float PlayTime{
        get{
            return _this.playTime;
        }
    }


    private Dictionary<int, ControllableCharacter> id_character_pairs = new Dictionary<int, ControllableCharacter>();
    [SerializeField][HideInInspector][SyncVar]private int   currentPlayersTurn = -1,
                                                            currentWaitingPlayer = -1;
    public static bool IsThisPlayersTurn{
        get{
            return CommunicationClient.ClientID == _this.currentPlayersTurn;
        }
    }




    private void Awake() {
        _this = this;
    }

    private void Start() {

        if(!this.isServer){ 

            StartCoroutine(DelayedRegister());

        }

    }

    private void Update() {
        
        if(!this.isPlaying){
            return;
        }

        this.playTime++;

        if(IsThisPlayersTurn){

            this.input_x = Input.GetAxis("Horizontal");
            this.input_y = Input.GetAxis("Vertical");
        
        }

        if(this.isServer){

            try{ 
                CameraRoot.transform.position = Vector3.SmoothDamp(CameraRoot.transform.position, this.id_character_pairs[this.currentPlayersTurn].transform.position, ref this.cameraVelocity, 1f, 6f, Time.smoothDeltaTime);
            }
            catch{}

        }

    }


    private void FixedUpdate() {
                
        if(!this.isPlaying){
            return;
        }

        MonoBehaviour.print(IsThisPlayersTurn + " " + CommunicationClient.ClientID + " " + this.currentPlayersTurn);

        if(IsThisPlayersTurn){

            if(!this.id_character_pairs.ContainsKey(this.currentPlayersTurn)){
                UpdateDictionary();
            }

            this.id_character_pairs[this.currentPlayersTurn].transform.position += new Vector3(this.input_y * Time.fixedDeltaTime * -2, 0, this.input_x * Time.fixedDeltaTime * 2);

        }

    }




    private IEnumerator DelayedRegister(){
        yield return new WaitForSeconds(1.5f);
        this.Register();
    }



    public void StartMatch(){

        if(this.isServer){

            GridSystem.OnMapLoad += this.PostMapLoad;

            CommunicationClient.LocalInstance.CallOnAllClients(GridSystem.LoadMap);

        }

    }




    private void PostMapLoad(){

        //try{ 

            if(!this.isServer){ //Null reference on this line, why?
                return;
            }
        
            StartCoroutine(PostMapLoadCoroutine());

        //}
        //catch{}
    }

    private IEnumerator PostMapLoadCoroutine(){
        yield return this.SpawnPlayersCoroutine();
        yield return this.MatchLoopCoroutine();
    }


    private IEnumerator SpawnPlayersCoroutine(){
        yield return new WaitForEndOfFrame();

        List<Vector3> spawns = new List<Vector3>(GridSystem.GetAllPlayerSpawnPoints());

        int randomIndex = UnityEngine.Random.Range(0, spawns.Count - 1);
        Vector3 spawnPos0 = spawns[randomIndex];
        spawns.RemoveAt(randomIndex);

        randomIndex = UnityEngine.Random.Range(0, spawns.Count - 1);
        Vector3 spawnPos1 = spawns[randomIndex];
        spawns.RemoveAt(randomIndex);


        List<int> ids = new List<int>(CommunicationServer.Clients.GetAllClientIDs());
        GameObject character = GameObject.Instantiate(this.characterPrefab, spawnPos0, Quaternion.identity) as GameObject;
        var _character = character.transform.GetComponent<ControllableCharacter>();
        this.id_character_pairs.Add(ids[0], _character);
        //NetworkServer.SpawnWithClientAuthority(character, CommunicationServer.Clients.GetClientFromID(ids[0]).communicationClient.gameObject);
        NetworkServer.Spawn(character);
        _character.SetController(ids[0]);
        _character.GetComponent<NetworkIdentity>().AssignClientAuthority(CommunicationServer.Clients.GetClientFromID(ids[0]).communicationClient.gameObject.GetComponent<NetworkIdentity>().connectionToClient);

        var wait = new WaitForEndOfFrame();

        Vector3 vel = Vector3.zero;

        do {
            CameraRoot.transform.position = Vector3.SmoothDamp(CameraRoot.transform.position, new Vector3(spawnPos0.x, 0, spawnPos0.z), ref vel, 1f, 6f, Time.smoothDeltaTime);
            yield return wait;
            if(Vector3.Distance(CameraRoot.transform.position, new Vector3(spawnPos0.x, 0, spawnPos0.z)) <= 0.05f){
                break;
            }
        }
        while(true);


        yield return new WaitForSeconds(0.1f);


        if(ids.Count > 1){

            character = GameObject.Instantiate(this.characterPrefab, spawnPos1, Quaternion.identity) as GameObject;
            _character = character.GetComponent<ControllableCharacter>();
            _character.SetController(ids[1]);
            this.id_character_pairs.Add(ids[1], _character);
            //NetworkServer.SpawnWithClientAuthority(character, CommunicationServer.Clients.GetClientFromID(ids[1]).communicationClient.gameObject.gameObject);
            NetworkServer.Spawn(character);
            _character.SetController(ids[1]);
            _character.GetComponent<NetworkIdentity>().AssignClientAuthority(CommunicationServer.Clients.GetClientFromID(ids[1]).communicationClient.gameObject.GetComponent<NetworkIdentity>().connectionToClient);

            vel = Vector3.zero;

            do {
                CameraRoot.transform.position = Vector3.SmoothDamp(CameraRoot.transform.position, new Vector3(spawnPos1.x, 0, spawnPos1.z), ref vel, 1f, 6f, Time.smoothDeltaTime);
                yield return wait;
                if(Vector3.Distance(CameraRoot.transform.position, new Vector3(spawnPos1.x, 0, spawnPos1.z)) <= 0.05f) {
                    break;
                }
            }
            while(true);

        }


        int startingPlayer = ids[UnityEngine.Random.Range(0, ids.Count - 1)];

        this.currentPlayersTurn = startingPlayer;

        if(this.currentPlayersTurn == ids[0] && ids.Count > 1){

            do {
                CameraRoot.transform.position = Vector3.SmoothDamp(CameraRoot.transform.position, new Vector3(spawnPos0.x, 0, spawnPos0.z), ref vel, 0.5f, 12f, Time.smoothDeltaTime);
                yield return wait;
                if(Vector3.Distance(CameraRoot.transform.position, new Vector3(spawnPos0.x, 0, spawnPos0.z)) <= 0.05f){
                    break;
                }
            }
            while(true);

        }

        if(this.currentPlayersTurn == ids[0] && ids.Count > 1){
            this.currentWaitingPlayer = ids[1];
        }
        else if(ids.Count > 1 && this.currentPlayersTurn == ids[1]){
            this.currentPlayersTurn = ids[0];
        }


        UIFeedbackLogger.Log("Player " + this.currentPlayersTurn + " starts.");

        this.isPlaying = true;

    }


    private IEnumerator MatchLoopCoroutine(){
        yield return new WaitForEndOfFrame();

        this.finishedAction = false;

        CommunicationServer.Clients.GetClientFromID(this.currentPlayersTurn).communicationClient.CallOnTargetClient(AlertTurn);
        var waitingClient = CommunicationServer.Clients.GetClientFromID(this.currentWaitingPlayer);
        if(waitingClient != null){
            waitingClient.communicationClient.CallOnTargetClient(AlertAwaitOthersTurn);
        }

        var wait = new WaitForEndOfFrame();
        do {
            yield return wait;
            if(this.finishedAction){
                break;
            }
        }
        while(true);

        this.finishedAction = false;

    }


    private static void AlertTurn(){
        UIFeedbackLogger.Log("Your turn started!");
    }

    private static void AlertAwaitOthersTurn(){
        UIFeedbackLogger.Log("Your opponent's turn started.");
    }


    internal static void SwitchTurns(GameObject caller){

        //CommunicationServer.Clients.GetClientFromID(_this.currentPlayersTurn).communicationClient.CallOnTargetClient(ControllableCharacter.UpdateController);
        //CommunicationServer.Clients.GetClientFromID(_this.currentWaitingPlayer).communicationClient.CallOnTargetClient(ControllableCharacter.UpdateController);

        //CommunicationServer.Clients.GetClientFromID(_this.currentPlayersTurn).communicationClient.CallOnTargetClient(UpdateDictionary);
        var waiter_player = CommunicationServer.Clients.GetClientFromID(_this.currentWaitingPlayer);
        if(waiter_player != null){
            waiter_player.communicationClient.CallOnTargetClient(UpdateDictionary);
        }

        List<int> ids = new List<int>();

        int player = 0,
            waiter = 0;

        foreach(var pair in _this.id_character_pairs){
            ids.Add(pair.Key);
        }

        if(_this.currentPlayersTurn == ids[0]){
            if(ids.Count > 1){
                player = ids[1];
                waiter = ids[0];
                _this.currentPlayersTurn = player;

                CommunicationServer.Clients.GetClientFromID(player).communicationClient.CallOnTargetClient(AlertTurn);
                CommunicationServer.Clients.GetClientFromID(waiter).communicationClient.CallOnTargetClient(AlertAwaitOthersTurn);
            }
        }
        else if(ids.Count > 1 && _this.currentPlayersTurn == ids[1]){
            player = ids[0];
            waiter = ids[1];
            _this.currentPlayersTurn = player;

            CommunicationServer.Clients.GetClientFromID(player).communicationClient.CallOnTargetClient(AlertTurn);
            CommunicationServer.Clients.GetClientFromID(waiter).communicationClient.CallOnTargetClient(AlertAwaitOthersTurn);
        }

    }


    internal static void UpdateDictionary(){

            
        _this.id_character_pairs.Clear();

        var characters = GameObject.FindObjectsOfType<ControllableCharacter>();

        foreach(var character in characters){
            if(!_this.id_character_pairs.ContainsKey(character.PlayerID)){ 
                _this.id_character_pairs.Add(character.PlayerID, character);
            }
        }

        MonoBehaviour.print(_this.id_character_pairs.Count + " characters found");
        

    }



    public void Register(){
        CommunicationClient.LocalInstance.CallOnServer(_Register);
    }

    ///Should be called on server.
    private static void _Register(GameObject client){
        var _client = client.GetComponent<CommunicationClient>();
        if(_client != null){
            if(!client_storedInfo.ContainsKey(_client)){
                client_storedInfo.Add(_client, new StoredInfo());
            }
        }
    }


    public void SetReady(){
        CommunicationClient.LocalInstance.CallOnServer(_SetReady);
    }

    ///Should be called on server.
    private static void _SetReady(GameObject client){

        var allClients = GameObject.FindObjectsOfType<CommunicationClient>();
        foreach(var __client in allClients){
            if(__client != null) {
                if(!client_storedInfo.ContainsKey(__client)) {
                    client_storedInfo.Add(__client, new StoredInfo());
                }
            }
        }

        var _client = client.GetComponent<CommunicationClient>();
        if(_client != null){ 
            if(!client_storedInfo.ContainsKey(_client)){
                client_storedInfo.Add(_client, new StoredInfo());
            }
            client_storedInfo[_client].ready = true;

            bool everyoneIsReady = true;
            foreach(var pair in client_storedInfo){
                if(!pair.Value.ready){
                    everyoneIsReady = false;
                    break;
                }
            }
            if(everyoneIsReady){
                _this.StartMatch();
            }

        }
    }

}

