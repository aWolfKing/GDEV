using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class UIFeedbackLogger : NetworkBehaviour {

    private static UIFeedbackLogger _this = null;

    [SerializeField] private Text text = null;
    private List<string> messages = new List<string>();
    [SerializeField] [HideInInspector] private SyncListString serverMessages = new SyncListString();
    [SerializeField] private int maxMessagesToDisplay = 9;


    private void Awake() {
        _this = this;
        this.serverMessages.Callback += this.SyncedMessge;
    }


    public static void Log(string txt){
        _this.messages.Add(txt);
        _this.UpdateText();
    }

    public static void Clear(){
        _this.messages.Clear();
        _this.UpdateText();
    }


    private void SyncedMessge(SyncListString.Operation operation, int index){
        switch(operation){
            case SyncList<string>.Operation.OP_ADD:
                this.messages.Add(this.serverMessages[index]);
                break;
        }
        _this.UpdateText();
    }


    private void UpdateText(){
        if(this.messages.Count == 0){ return; }
        List<string> lines = new List<string>();
        for(int i=this.messages.Count-1; i>=0; i--){
            string[] subLines = this.messages[i].Split('\n');
            if(lines.Count + subLines.Length <= this.maxMessagesToDisplay){
                for(int s=subLines.Length-1; s>=0; s--){
                    lines.Add(subLines[s]);
                }
            }
        }
        string text = "";
        for(int i=lines.Count-1; i>=0; i--){
            text += lines[i] + "\n";
        }
        this.text.text = text;
    }

}
