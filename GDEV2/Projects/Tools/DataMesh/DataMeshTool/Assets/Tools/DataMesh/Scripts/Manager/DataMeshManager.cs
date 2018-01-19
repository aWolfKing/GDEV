using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.ObjectModel;

/*
public class DataMeshManager : MonoBehaviour {
}
*/

public static class DataMesh{

    private static Dictionary<string, DataMeshObjectReference> datameshObjects = new Dictionary<string, DataMeshObjectReference>();
    private static SceneDataMeshManager localSceneManager = null;
    public static SceneDataMeshManager LocalSceneManager{ 
        get{ 
            if(localSceneManager == null){
                SceneManager.activeSceneChanged -= ChangedScene;
                SceneManager.activeSceneChanged += ChangedScene;
                localSceneManager = GameObject.FindObjectOfType<SceneDataMeshManager>();
                if(localSceneManager == null){
                    GameObject obj = new GameObject("SceneDataMeshManager");
                    obj.transform.position = Vector3.zero;
                    localSceneManager = obj.AddComponent<SceneDataMeshManager>();
                }
            }
            return localSceneManager;
        } 
    }

    private static void ChangedScene(Scene was, Scene now){ localSceneManager = null; datameshObjects.Clear(); }
    
    private static class EditorFunctionality{

        public static DataMeshObject GetDataMesh(string name){
            for (int i = 0; i < DataMesh.LocalSceneManager.dataMeshes.Count; i++) {
                if(DataMesh.LocalSceneManager.dataMeshes[i].dataMeshName == name){
                    return DataMesh.LocalSceneManager.dataMeshes[i];
                }
            }
            return null;
        }

    }

    public class DataMeshObjectReference{

        private DataMeshObject dataMeshObject;

        public DataMeshObjectReference(DataMeshObject obj){
            this.dataMeshObject = obj;
        }


        public int GetValueBelow(Vector3 position) {
            return GetValue(position, -Vector3.up);
        }
        public int GetValue(Vector3 position, Vector3 direction) {
            Vector3 hit = new Vector3(100000, 100000, 100000);
            DataMeshTriangle closest = null;
            for (int i = 0; i < dataMeshObject.subObjects.Count; i++) {

                /*
                for(int o=0; o<dataMeshObject.subObjects[i].WorldTriangles.Length; o++){
                    Debug.DrawLine(dataMeshObject.subObjects[i].WorldTriangles[o].p0, dataMeshObject.subObjects[i].WorldTriangles[o].p1, Color.yellow, 0.4f);
                    Debug.DrawLine(dataMeshObject.subObjects[i].WorldTriangles[o].p1, dataMeshObject.subObjects[i].WorldTriangles[o].p2, Color.yellow, 0.4f);
                    Debug.DrawLine(dataMeshObject.subObjects[i].WorldTriangles[o].p2, dataMeshObject.subObjects[i].WorldTriangles[o].p0, Color.yellow, 0.4f);
                }
                */

                Vector3 localhit;
                DataMeshTriangle localclosest = DataMesh3DMath.GetFirstHitTriangle(position, direction, out localhit, /*dataMeshObject.subObjects[i].triangles.ToArray()*/ dataMeshObject.subObjects[i].WorldTriangles);
                if (closest == null) { closest = localclosest; hit = localhit; }
                else { if (Vector3.Distance(position, localhit) < Vector3.Distance(position, hit)) { closest = localclosest; hit = localhit; } }
            }
            if (closest != null) {
                return closest.value;
            }
            else {
                return -1;
            }
        }
        public DataMeshTriangle GetTriangle(Vector3 position, Vector3 direction) {
            Vector3 hit;
            return GetTriangle(position, direction, out hit);
        }
        public DataMeshTriangle GetTriangle(Vector3 position, Vector3 direction, out Vector3 hit) {
            //return DataMesh3DMath.GetFirstHitTriangle(position, direction, out hit, triangles.ToArray());
            Vector3 _hit = new Vector3(100000, 100000, 100000);
            DataMeshTriangle closest = null;
            for (int i = 0; i < dataMeshObject.subObjects.Count; i++) {
                Vector3 localhit;
                DataMeshTriangle localclosest = DataMesh3DMath.GetFirstHitTriangle(position, direction, out localhit, dataMeshObject.subObjects[i].triangles.ToArray());
                if (closest == null) { closest = localclosest; hit = localhit; }
                else { if (Vector3.Distance(position, localhit) < Vector3.Distance(position, _hit)) { closest = localclosest; hit = localhit; } }
            }
            hit = (closest != null ? _hit : Vector3.zero);
            return closest;
        }

    }


    public static DataMeshObjectReference Get(string name){
        if(datameshObjects.ContainsKey(name)){ return datameshObjects[name]; }
        else{ 
            for(int i=0; i<LocalSceneManager.dataMeshes.Count; i++){
                if(LocalSceneManager.dataMeshes[i].dataMeshName == name){
                    datameshObjects.Add(name, new DataMeshObjectReference(LocalSceneManager.dataMeshes[i]));
                    return datameshObjects[name];
                }
            }
            #if UNITY_EDITOR
            Debug.LogError("Datamesh '" + name + "' is not active in this scene.");
            #endif
            return null;
        }
    }


}