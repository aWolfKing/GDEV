#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RubensSplineReader {

    [SceneAndProjectDragAndDrop.Attributes.DropValidate(DragAndDropVisualMode.Link)]
	private static bool ValidateRead(Object[] selection, Object target){
        if (selection.Length == 1 && selection[0] != null && target.GetType() == typeof(DataMeshObject)) {
            try{
                if(AssetDatabase.GetAssetPath(selection[0]).EndsWith(".json")){
                    string[] lines = System.IO.File.ReadAllLines(AssetDatabase.GetAssetPath(selection[0]));
                    if(lines[0] == "SplineComponent"){
                        for(int i=0; i<lines.Length; i++){
                            if(lines[i].ToLower().Contains("\"anchor\": {") && i+4 < lines.Length){
                                if(lines[i+1].Contains("\"x\":") && lines[i+2].Contains("\"y\":") && lines[i+3].Contains("\"z\":") && lines[i+4].Contains("},")){
                                    float x, y, z;
                                    string s_x, s_y, s_z;
                                    try {
                                        s_x = lines[i + 1].Split(':')[1].Replace(" ", "").Replace(",", "");
                                        s_y = lines[i + 2].Split(':')[1].Replace(" ", "").Replace(",", "");
                                        s_z = lines[i + 3].Split(':')[1].Replace(" ", "").Replace(",", "");
                                        return float.TryParse(s_x, out x) && float.TryParse(s_y, out y) && float.TryParse(s_z, out z);
                                    }
                                    catch{ return false; }
                                }
                            }
                        }
                    }
                }
            }
            catch{ return false; }
        }

        return false;
    }

    [SceneAndProjectDragAndDrop.Attributes.DropOperation]
    private static bool Read(Object[] selection, Object target){
        if (selection.Length == 1 && selection[0] != null && target.GetType() == typeof(DataMeshObject)) {
            try {
                if (AssetDatabase.GetAssetPath(selection[0]).EndsWith(".json")) {
                    string[] lines = System.IO.File.ReadAllLines(AssetDatabase.GetAssetPath(selection[0]));
                    if (lines[0] == "SplineComponent") {
                        List<Vector3> positions = new List<Vector3>();
                        for (int i = 0; i < lines.Length; i++) {
                            if (lines[i].ToLower().Contains("\"anchor\": {") && i + 4 < lines.Length) {
                                if (lines[i + 1].Contains("\"x\":") && lines[i + 2].Contains("\"y\":") && lines[i + 3].Contains("\"z\":") && lines[i + 4].Contains("},")) {
                                    float x, y, z;
                                    string s_x, s_y, s_z;
                                    try {
                                        s_x = lines[i + 1].Split(':')[1].Replace(" ", "").Replace(",", "").Replace(".", ",");
                                        s_y = lines[i + 2].Split(':')[1].Replace(" ", "").Replace(",", "").Replace(".", ",");
                                        s_z = lines[i + 3].Split(':')[1].Replace(" ", "").Replace(",", "").Replace(".", ",");
                                        float.TryParse(s_x, out x);
                                        float.TryParse(s_y, out y);
                                        float.TryParse(s_z, out z);
                                        positions.Add(new Vector3(x, y, z));
                                    }
                                    catch { continue; }
                                }
                            }
                        }
                        List<DataMeshTriangle> tris = new List<DataMeshTriangle>();
                        MonoBehaviour.print("Spline anchors: " + positions.Count);
                        for(int i=0; i<positions.Count; i++){
                            DataMeshTriangle tri = new DataMeshTriangle();
                            DataMeshTriangle tri2 = new DataMeshTriangle();
                            tri.p0 = positions[i] + new Vector3(0.5f, 0, 0.5f);
                            tri.p1 = positions[i] + new Vector3(0.5f, 0, -0.5f);
                            tri.p2 = positions[i] + new Vector3(-0.5f, 0, -0.5f);
                            tri2.p0 = positions[i] + new Vector3(-0.5f, 0, -0.5f);
                            tri2.p1 = positions[i] + new Vector3(-0.5f, 0, 0.5f);
                            tri2.p2 = positions[i] + new Vector3(0.5f, 0, 0.5f);
                            tri.value = 0;
                            tri2.value = 0;
                            tris.Add(tri);
                            tris.Add(tri2);
                            //MonoBehaviour.print(positions[i]);
                            if(i+1 < positions.Count){
                                Debug.DrawLine(positions[i], positions[i + 1], Color.red, 40f);
                            }
                        }

                        DataMeshObject obj = (DataMeshObject)target;
                        if(obj.subObjects.Count == 0){
                            var subObj = new DataMeshObject.DataMeshSubObject();
                            subObj.position = Vector3.zero;
                            obj.subObjects.Add(subObj);
                        }
                        obj.subObjects[0].PushTriangles(tris.ToArray(), true);
                    }
                }
            }
            catch { }
        }
        return true;
    }

}
#endif