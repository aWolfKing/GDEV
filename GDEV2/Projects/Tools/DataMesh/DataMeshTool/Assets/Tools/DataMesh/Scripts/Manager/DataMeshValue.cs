using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataMeshValue {

    private int value = 10000;
    private string description = "no description";

    public int Value{ get{ return value; } }
    public string Description { get{ return description; } }

    public DataMeshValue(int value, string description){
        this.value = value;
        this.description = description;
    }

    public DataMeshValue(int value) {
        this.value = value;
    }

}
