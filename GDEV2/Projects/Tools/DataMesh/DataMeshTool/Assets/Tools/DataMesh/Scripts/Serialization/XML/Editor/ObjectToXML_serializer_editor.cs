#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

public static class ToXML_editor{

    #region ToXML

    [MenuItem("Assets/Serialization/XML/ToXML")]
    public static void ToXML(){
        string filePath = AssetDatabase.GetAssetPath(Selection.activeObject).Replace(".asset", "_xml_Serialized.xml");
        XmlSerializer serializer = new XmlSerializer(Selection.activeObject.GetType());
        TextWriter writer = new StreamWriter(filePath);
        serializer.Serialize(writer, Selection.activeObject);
        writer.Close();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Serialization/XML/ToXML", true)]
    public static bool CanBeSerialized(){
        return Selection.activeObject.GetType().BaseType == typeof(ScriptableObject);
    }

    #endregion
    #region FromXML

    [MenuItem("Assets/Serialization/XML/FromXML")]
    public static void FromXML() {
        string filePath = AssetDatabase.GetAssetPath(Selection.activeObject).Replace("_xml_Serialized.xml", "_xml_Deserialized.asset");
        System.Type objectType = null;
        TextReader reader = new StreamReader(AssetDatabase.GetAssetPath(Selection.activeObject));
        string[] lines = reader.ReadToEnd().Split('\n');
        //for (int i =/*0*/ 1; i < 2/*lines.Length*/; i++) {
            try {
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                for (int o = 0; o < assemblies.Length; o++) {
                    objectType = assemblies[o].GetType(lines[/*i*/ 1].Replace("<", "").Split(' ')[0]);
                    if (objectType != null) {
                        break;
                    }
                }
            }
            catch { }
        //}
        reader.Close();
        reader = new StreamReader(AssetDatabase.GetAssetPath(Selection.activeObject));
        XmlSerializer serializer = new XmlSerializer(objectType);

        try {
            Object asset = serializer.Deserialize(reader) as Object;
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch{}

        reader.Close();
    }

    [MenuItem("Assets/Serialization/XML/FromXML", true)]
    public static bool CanBeDeSerialzed() {
        if(AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".xml")){
            TextReader reader = new StreamReader(AssetDatabase.GetAssetPath(Selection.activeObject));
            string[] lines = reader.ReadToEnd().Split('\n');
            //for(int i=/*0*/ 1; i<2/*lines.Length*/; i++){
                try {
                    var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                    for (int o=0; o<assemblies.Length; o++) {
                        if (assemblies[o].GetType(lines[/*i*/ 1].Replace("<", "").Split(' ')[0]) != null) {
                            return true;
                        }
                    }
                }catch{ }
            //}
            reader.Close();
        }
        return false;
    }

    #endregion

}
#endif