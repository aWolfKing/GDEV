using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DatabaseConnection : MonoBehaviour {

    [SerializeField] private InputField emailInput = null,
                                        passwordInput = null;

    [SerializeField] private List<GameObject> disableOnLogin = new List<GameObject>();
    [SerializeField] private List<GameObject> enableOnLogin = new List<GameObject>();

    private string sessionID = null;
    private int playerID = -1;


    private void Awake() {
        DontDestroyOnLoad(this);
        foreach(var enable in this.disableOnLogin){
            enable.SetActive(true);
        }
        foreach(var disable in this.enableOnLogin){
            disable.SetActive(false);
        }
    }


    public void Login(){
        this.Login(this.emailInput.text, this.passwordInput.text);
    }


    public void AddPlaytimeAsScore(){
        AddScore(Mathf.FloorToInt(NetworkedGameManager.PlayTime));
    }


    public void Login(string email, string password){

        StartCoroutine(LoginCoroutine(email, password));

    }

    private IEnumerator LoginCoroutine(string email, string password){
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        WWW www = new WWW("http://studenthome.hku.nl/~daan.vanwesterlaak/GDEV2_Database/Functionality/Login.php", form);

        yield return www;

        MonoBehaviour.print(www.text);

        string[] feedback = www.text.Split('\n');

        for(int i=0; i<feedback.Length; i++){
            if(feedback[i].ToLower().StartsWith("sessionid:")){
                this.sessionID = feedback[i].Split(':')[1];
                MonoBehaviour.print("Session ID: " + this.sessionID);
                UIFeedbackLogger.Log("Login succesfull!");

                foreach(var disable in this.disableOnLogin){
                    disable.SetActive(false);
                }
                foreach(var enable in this.enableOnLogin){
                    enable.SetActive(true);
                }

                break;
            }
            else if(feedback[i].ToLower().StartsWith("error:")){
                if(feedback[i].ToLower().Contains("user not found")){
                    UIFeedbackLogger.Log("Could not find an user with this email-password combination.");
                }
                else if(feedback[i].ToLower().Contains("please fill in all fields")){
                    UIFeedbackLogger.Log("Please fill in all fields.");
                }
            }
        }


        form = new WWWForm();
        www = new WWW("http://studenthome.hku.nl/~daan.vanwesterlaak/GDEV2_Database/Functionality/GetPlayerID.php?sessionID=" + this.sessionID, form);

        yield return www;

        MonoBehaviour.print(www.text);

        feedback = www.text.Split('\n');

        for(int i = 0; i < feedback.Length; i++) {
            if(feedback[i].ToLower().StartsWith("userid:")) {
                this.playerID = int.Parse(feedback[i].Split(':')[1]);
                MonoBehaviour.print("Player ID: " + this.playerID);
                break;
            }
        }

    }

    public void AddScore(int score){
        StartCoroutine(this.AddScoreCoroutine(score));
    }

    private IEnumerator AddScoreCoroutine(int score){

        if(this.playerID < 0){
            yield break;
        }

        WWW www = new WWW("http://studenthome.hku.nl/~daan.vanwesterlaak/GDEV2_Database/Functionality/AddScore.php?sessionID=" + this.sessionID + "&gameID=3&score=" + score + "&sessionLenght_seconds=" + NetworkedGameManager.PlayTime);

        yield return www;

        MonoBehaviour.print(www.text);

        Application.Quit();
    }


    public void ShowHighScores(){
        Application.OpenURL("http://studenthome.hku.nl/~daan.vanwesterlaak/GDEV2_Database/Functionality/SelectGame.php?sessionID=" + this.sessionID + "&GameID=3");
    }

    public void ShowCreateAccountURL(){
        Application.OpenURL("http://studenthome.hku.nl/~daan.vanwesterlaak/GDEV2_Database/Functionality/CreateAccount.php");
    }

}

