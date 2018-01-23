using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllClasses : MonoBehaviour {

    [SerializeField] private Text text;

    private void Start() {
        string txt = "";
        Transform[] obj = GameObject.FindObjectsOfType<Transform>();
        for(int i=0; i<obj.Length; i++){
            Component[] components = obj[i].GetComponents<MonoBehaviour>();
            for(int o=0; o<components.Length; o++){
                txt += components[o].GetType() + "\n";
            }
        }
        text.text = txt;
    }

}
