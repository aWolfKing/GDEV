#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DataMeshEditorWindow : EditorWindow {

    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Tools/DataMesh/Scene DataMesh manager")]
    public static void OpenWindow(){
        EditorWindow.GetWindow<DataMeshEditorWindow>();
    }


    private void OnGUI() {

        GUILayout.Label("");
        Rect r = EditorGUILayout.BeginHorizontal();
        r.height = 18;
        GUILayout.Label("");
        EditorGUI.ProgressBar(r, 1, EditorSceneManager.GetActiveScene().name != "" ? ("Editing scene: " + EditorSceneManager.GetActiveScene().name) : "Editing scene: Nameless");
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < DataMesh.LocalSceneManager.dataMeshes.Count; i++) {
            if (DataMesh.LocalSceneManager.dataMeshes[i] != null) {
                r = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("");
                Rect tmpr = r;
                tmpr.width = r.width - 50 - 20;
                tmpr.position += new Vector2(10, 0);
                if(GUI.Button(tmpr, DataMesh.LocalSceneManager.dataMeshes[i].dataMeshName, EditorStyles.toolbarButton)){
                    EditorGUIUtility.PingObject(DataMesh.LocalSceneManager.dataMeshes[i]);
                }

                tmpr.position -= new Vector2(10, 0);
                tmpr.position += new Vector2(r.width - 50 - 10, 0);
                tmpr.width = 50;
                tmpr.height = 18;
                EditorGUI.ProgressBar(tmpr, 1, "-");
                if(GUI.Button(tmpr, "", GUIStyle.none)){
                    Undo.RecordObject(DataMesh.LocalSceneManager, "Removed DataMesh from scene");
                    DataMesh.LocalSceneManager.dataMeshes.Remove(DataMesh.LocalSceneManager.dataMeshes[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
            else{
                r = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("");
                Rect tmpr = r;
                tmpr.width = r.width - 50 - 20;
                tmpr.position += new Vector2(10, 0);
                //if (GUI.Button(tmpr, DataMesh.LocalSceneManager.dataMeshes[i].dataMeshName, EditorStyles.toolbarButton)) {
                //    EditorGUIUtility.PingObject(DataMesh.LocalSceneManager.dataMeshes[i]);
                //}
                {
                    Undo.RecordObject(DataMesh.LocalSceneManager, "Added DataMesh to scene");
                    DataMesh.LocalSceneManager.dataMeshes[i] = (DataMeshObject)EditorGUI.ObjectField(tmpr, DataMesh.LocalSceneManager.dataMeshes[i], typeof(DataMeshObject), false);
                }

                tmpr.position -= new Vector2(10, 0);
                tmpr.position += new Vector2(r.width - 50 - 10, 0);
                tmpr.width = 50;
                tmpr.height = 18;
                EditorGUI.ProgressBar(tmpr, 1, "-");
                if (GUI.Button(tmpr, "", GUIStyle.none)) {
                    Undo.RecordObject(DataMesh.LocalSceneManager, "Removed DataMesh from scene");
                    DataMesh.LocalSceneManager.dataMeshes.Remove(DataMesh.LocalSceneManager.dataMeshes[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        {

            r = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("");
            Rect tmpr = r;
            tmpr.width = r.width - 50 - 20;
            tmpr.position += new Vector2(10, 0);
            //if (GUI.Button(tmpr, DataMesh.LocalSceneManager.dataMeshes[i].dataMeshName, EditorStyles.toolbarButton)) {
            //    EditorGUIUtility.PingObject(DataMesh.LocalSceneManager.dataMeshes[i]);
            //}

            tmpr.position -= new Vector2(10, 0);
            tmpr.position += new Vector2(r.width - 50 - 10, 0);
            tmpr.width = 50;
            tmpr.height = 18;
            EditorGUI.ProgressBar(tmpr, 1, "+");
            if (GUI.Button(tmpr, "", GUIStyle.none)) {
                //Undo.RecordObject(DataMesh.LocalSceneManager, "Added DataMesh to scene");
                DataMesh.LocalSceneManager.dataMeshes.Add(null);
            }
            else {
                DataMeshObject tmpdm = (DataMeshObject)EditorGUI.ObjectField(tmpr, null, typeof(DataMeshObject), false);
                if(tmpdm != null){
                    Undo.RecordObject(DataMesh.LocalSceneManager, "Added DataMesh to scene");
                    DataMesh.LocalSceneManager.dataMeshes.Add(tmpdm);
                }
                EditorGUI.ProgressBar(tmpr, 1, "+");
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndScrollView();

    }

}
#endif