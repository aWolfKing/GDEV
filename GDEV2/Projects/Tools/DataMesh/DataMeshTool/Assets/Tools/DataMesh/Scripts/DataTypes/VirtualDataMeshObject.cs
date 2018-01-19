using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VirtualDataMeshObject {

    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;

    [System.NonSerialized] public VirtualDataMeshObject linkedTo = null;
    public LocalDataMesh localDataMesh = null;

    [System.NonSerialized] private List<VirtualDataMeshObject> hasControlOver = new List<VirtualDataMeshObject>();

    public void TakeControlOver(VirtualDataMeshObject obj){
        if(obj.linkedTo != null){
            obj.linkedTo.RemoveContolFrom(obj);
        }
        obj.linkedTo = this;
        if(!hasControlOver.Contains(obj)){
            hasControlOver.Add(obj);
        }
    }

    public void RemoveContolFrom(VirtualDataMeshObject obj){
        if(obj.linkedTo.hasControlOver.Contains(obj)){
            obj.linkedTo.hasControlOver.Remove(obj);
        }
        obj.linkedTo = null;
    }


    // Pushing data: pass array of already checked virtual objects to prevent infinite loop.


}
