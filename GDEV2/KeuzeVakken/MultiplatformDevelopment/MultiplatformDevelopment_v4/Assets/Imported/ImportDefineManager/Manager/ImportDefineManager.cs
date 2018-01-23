#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class ImportDefineManager {

    [InitializeOnLoadMethod]
    [UnityEditor.Callbacks.DidReloadScripts]
	private static void Manage(){

        List<string> importDefines = new List<string>();

        var types = Assembly.GetExecutingAssembly().GetTypes();
        for(int i=0; i<types.Length; i++){
            var attributes = types[i].GetCustomAttributes();
            foreach(var attribute in attributes){
                if(attribute.GetType() == typeof(ImportDefine)){
                    string t = ((ImportDefine)attribute).text;
                    if(!t.ToLower().StartsWith("import_")){ t = "Import_" + t; }
                    importDefines.Add(t);
                    break;
                }
            } 
        }


        var buildGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defined = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);

        string importDefineString = defined;

        string[] defines = defined.Split(';');
        foreach(string d in defines){
            if(d.StartsWith("Import_")){
                if(!importDefines.Contains(d)){
                    importDefineString = importDefineString.Replace(d, "");
                }
            }
        }

        for(int i=0; i<importDefines.Count; i++){
            if(!importDefineString.Contains(importDefines[i])) {
                if(!importDefineString.EndsWith(";") && importDefineString.Length > 0){ importDefineString += ";"; }
                importDefineString += importDefines[i] + (i + 1 < importDefines.Count ? ";" : "");
            }
        }

        importDefineString.Replace(";;", ";");
        if(!importDefineString.EndsWith(";")){ importDefineString += ";"; }

        //MonoBehaviour.print(importDefineString);

        string was = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
        if (was != importDefineString) {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, importDefineString);
        }

    }

}
#endif
