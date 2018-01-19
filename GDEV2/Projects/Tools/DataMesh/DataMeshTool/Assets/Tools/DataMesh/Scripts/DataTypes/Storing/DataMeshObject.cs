using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataMeshObject", menuName = "DataMesh/DataMeshObject", order = 1)]
public class DataMeshObject : ScriptableObject {

    [System.Serializable]
    public class DataMeshSubObject{
        public Vector3 position;
        public Quaternion rotation;
        public List<DataMeshTriangle> triangles = new List<DataMeshTriangle>();
        private List<DataMeshTriangle> worldTriangles = new List<DataMeshTriangle>();
        public DataMeshTriangle[] WorldTriangles {
            get {
                if (worldTriangles.Count != triangles.Count) {
                    worldTriangles.Clear();
                    for(int i=0; i<triangles.Count; i++){
                        Vector3 p0 = ToWorld(triangles[i].p0);
                        Vector3 p1 = ToWorld(triangles[i].p1);
                        Vector3 p2 = ToWorld(triangles[i].p2);
                        worldTriangles.Add(new DataMeshTriangle(p0, p1, p2, triangles[i].value));
                    }
                }
                return worldTriangles.ToArray();
            }
        }


        public Vector3 ToWorld(Vector3 local){
            return position + rotation * Vector3.forward * local.z + rotation * Vector3.right * local.x + rotation * Vector3.up * local.y;
        }
        public Vector3 ToLocalWithoutRotation(Vector3 world){
            return world - position;
        }

        public void Clear() {
            triangles.Clear();
        }

        public void ClearValues(params int[] values){
            for(int i=triangles.Count-1; i>=0; i--){
                bool doRemove = false;
                for(int o=0; o<values.Length; o++){ if (triangles[i].value == values[o]){ doRemove = true; break; } }
                if(doRemove){
                    triangles.Remove(triangles[i]);
                }
            }
        }

        public void PushTriangles(DataMeshTriangle[] tris, bool doCheck = true){
            for (int i = 0; i < tris.Length; i++) {
                if (tris[i].value != -1) {
                    Vector3 p0 = ToLocalWithoutRotation(tris[i].p0);
                    Vector3 p1 = ToLocalWithoutRotation(tris[i].p1);
                    Vector3 p2 = ToLocalWithoutRotation(tris[i].p2);
                    bool doPush = true;
                    if (doCheck) {
                        for(int o=0; o<triangles.Count; o++){
                            if(Vector3.Distance(p0, triangles[o].p0) <= 0.01f && Vector3.Distance(p1, triangles[o].p1) <= 0.01f && Vector3.Distance(p2, triangles[o].p2) <= 0.01f) {
                                doPush = false;
                                triangles[o].value = tris[i].value;
                                break;
                            }
                        }
                    }
                    if (doPush) {
                        triangles.Add(new DataMeshTriangle(p0, p1, p2, tris[i].value));
                    }
                }
                else{
                    Vector3 p0 = ToLocalWithoutRotation(tris[i].p0);
                    Vector3 p1 = ToLocalWithoutRotation(tris[i].p1);
                    Vector3 p2 = ToLocalWithoutRotation(tris[i].p2);
                    for (int o = triangles.Count-1; o >= 0; o--) {
                        if (Vector3.Distance(p0, triangles[o].p0) <= 0.01f && Vector3.Distance(p1, triangles[o].p1) <= 0.01f && Vector3.Distance(p2, triangles[o].p2) <= 0.01f) {
                            triangles[o].value = tris[i].value;
                            triangles.Remove(triangles[triangles.Count - 1]);
                            break;
                        }
                    }
                }

            }
        }

        public void PushTrianglesWithoutCheck(params DataMeshTriangle[] tris) {
            PushTriangles(tris, false);
        }

    }

    [System.Serializable]
    public class DataMeshValueAndDescrition{
        public int value = 10000;
        public string description = "no description";
        public Color editorRenderColor = Color.gray;
        public DataMeshValueAndDescrition(){}
        public DataMeshValueAndDescrition(int value, string description){
            this.value = value;
            this.description = description;
        }
    }


    public string dataMeshName = "example datamesh";
    public List<DataMeshSubObject> subObjects = new List<DataMeshSubObject>();
    public List<DataMeshValueAndDescrition> values = new List<DataMeshValueAndDescrition>();

    public string GetDescriptionFromValue(int value){
        if(value == -1){ return "clear"; }
        for(int i=0; i<values.Count; i++){ if (values[i].value == value) { return values[i].description; } }
        return "not registered";
    }

    public Color GetEditorRenderColorFromValue(int value){
        if(value == -1){ return new Color(1, 1, 1, 0); }
        for(int i=0; i<values.Count; i++){ if(values[i].value == value) { return values[i].editorRenderColor; } }
        return new Color(1, 1, 1, 0);
    }


    public int GetValueBelow(Vector3 position) {
        return GetValue(position, -Vector3.up);
    }

    public int GetValue(Vector3 position, Vector3 direction) {
        Vector3 hit = new Vector3(100000,100000,100000);
        DataMeshTriangle closest = null;
        for (int i = 0; i < subObjects.Count; i++) {
            Vector3 localhit;
            DataMeshTriangle localclosest = DataMesh3DMath.GetFirstHitTriangle(position, direction, out localhit, /*subObjects[i].triangles.ToArray()*/ subObjects[i].WorldTriangles);
            if(closest == null){ closest = localclosest; hit = localhit; }
            else{ if (Vector3.Distance(position, localhit) < Vector3.Distance(position, hit)) { closest = localclosest; hit = localhit; } }
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
        for (int i = 0; i < subObjects.Count; i++) {
            Vector3 localhit;
            DataMeshTriangle localclosest = DataMesh3DMath.GetFirstHitTriangle(position, direction, out localhit, subObjects[i].triangles.ToArray());
            if (closest == null) { closest = localclosest; hit = localhit; }
            else { if (Vector3.Distance(position, localhit) < Vector3.Distance(position, _hit)) { closest = localclosest; hit = localhit; } }
        }
        hit = (closest != null ? _hit : Vector3.zero);
        return closest;
    }

}
