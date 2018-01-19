#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DataMeshObject))]
public class DataMeshObjectInspector : Editor {

    Vector2 scrollPos = Vector2.zero;

    public override void OnInspectorGUI() {

        DataMeshObject targ = (DataMeshObject)target;

       //serializedObject.Update();

        //base.OnInspectorGUI();
        Rect r = EditorGUILayout.BeginHorizontal();
        GUILayout.Label("");
        Rect r2 = r;
        r2.position += new Vector2(r.width-50, 0);
        r2.width = 50;
        r.width -= 50;
        r.position += new Vector2(0, 1);
        r.height -= 2;
        //targ.dataMeshName = GUI.TextField(r, targ.dataMeshName, GUIStyle.none);
        r.position -= new Vector2(0, 1);
        r.height += 2;
        EditorGUI.ProgressBar(r, 1, (targ.dataMeshName.ToLower().Contains("datamesh") ? targ.dataMeshName : targ.dataMeshName + "( DataMesh)" ) );
        if(GUI.Button(r, "", GUIStyle.none)){ EditorGUIUtility.PingObject(target); }
        EditorGUI.ProgressBar(r2, 1, (DataMeshToolbar.IsEditingDataMeshObject(targ) ? "editing" : "edit"));
        if(GUI.Button(r2, "", GUIStyle.none)){
            DataMeshToolbar.RequestEdit(targ);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
            Undo.RecordObject(targ, "changed DataMesh name");
            GUILayout.Label("DataMesh name");
            targ.dataMeshName = GUILayout.TextField(targ.dataMeshName);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("");

        int count = 0;
        for(int i=0;i<targ.subObjects.Count; i++){ count += targ.subObjects[i].triangles.Count; }

        GUILayout.Label(count + " triangles");
        GUILayout.Label(targ.subObjects.Count + " virtual objects");

        GUILayout.Label("");

        /*
        r = EditorGUILayout.BeginHorizontal();
        r.position += new Vector2(r.width - 50, 0);
        r.width = 50;
        GUILayout.Label("");
        EditorGUI.ProgressBar(r, 1, "+");
        if (GUI.Button(r, "", GUIStyle.none)) {
            Undo.RecordObject(targ, "edited DataMesh values");
            if (targ.values.Count > 0) {
                targ.values.Add(new DataMeshObject.DataMeshValueAndDescrition(targ.values[targ.values.Count - 1].value + 1, "no descrition"));
            }
            else{
                targ.values.Add(new DataMeshObject.DataMeshValueAndDescrition(0, "no description"));
            }
        }
        EditorGUILayout.EndHorizontal();
        */

        GUILayout.Label("Values and descriptions:");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        {

            {
                r = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("");
                Rect tmpr = r;
                tmpr.width = 50;
                EditorGUI.IntField(tmpr, -1);
                tmpr.position += new Vector2(r.width - 50, 0);
                
                tmpr.position -= new Vector2(r.width - 100, 0);
                tmpr.width = r.width - 100;
                EditorGUI.TextField(tmpr, "clear");

                EditorGUILayout.EndHorizontal();
            }

            for(int i=0; i<targ.values.Count; i++){
                //GUILayout.BeginHorizontal();
                //targ.values[i].value = EditorGUILayout.IntField(targ.values[i].value);
                //targ.values[i].description = EditorGUILayout.TextField(targ.values[i].description);
                //GUILayout.EndHorizontal();
                Undo.RecordObject(targ, "edited DataMesh value or description");
                r = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("");
                Rect tmpr = r;
                tmpr.width = 50;
                targ.values[i].value = EditorGUI.IntField(tmpr, targ.values[i].value);
                tmpr.position += new Vector2(r.width-50, 0);
                EditorGUI.ProgressBar(tmpr, 1, "-");
                if (GUI.Button(tmpr, "", GUIStyle.none)) {
                    targ.values.Remove(targ.values[i]);
                }
                else {
                    tmpr.position -= new Vector2(r.width - 100, 0);
                    tmpr.width = r.width - 150;
                    targ.values[i].description = GUI.TextField(tmpr, targ.values[i].description);
                    tmpr.position += new Vector2(tmpr.width, 0);
                    tmpr.width = 50;
                    targ.values[i].editorRenderColor = EditorGUI.ColorField(tmpr, targ.values[i].editorRenderColor);
                }

                EditorGUILayout.EndHorizontal();
            }

            r = EditorGUILayout.BeginHorizontal();
            r.position += new Vector2(r.width - 50, 0);
            r.width = 50;
            GUILayout.Label("");
            EditorGUI.ProgressBar(r, 1, "+");
            if (GUI.Button(r, "", GUIStyle.none)) {
                Undo.RecordObject(targ, "edited DataMesh values");
                if (targ.values.Count > 0) {
                    targ.values.Add(new DataMeshObject.DataMeshValueAndDescrition(targ.values[targ.values.Count - 1].value + 1, "no descrition"));
                }
                else {
                    targ.values.Add(new DataMeshObject.DataMeshValueAndDescrition(0, "no description"));
                }
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndScrollView();


        //serializedObject.ApplyModifiedProperties();

    }

}
#endif