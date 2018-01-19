using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class ImportDefine : Attribute{
    public string text = "ImportDefine";
    /// <summary>
    /// Defines "Import_'define'"
    /// </summary>
    /// <param name="define"></param>
    public ImportDefine(string define){
        if(!define.ToLower().StartsWith("import_")){
            define = "Import_" + define;
        }
        this.text = define;
    }
}
