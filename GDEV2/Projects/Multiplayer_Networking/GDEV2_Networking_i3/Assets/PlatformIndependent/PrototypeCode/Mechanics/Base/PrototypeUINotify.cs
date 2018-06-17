using System;
using System.Collections.Generic;
using UnityEngine;


public class PrototypeUINotify {

    public static void Notify(string message){
        MonoBehaviour.print(message);
    }

    public static void NotifySkillActivation(string skill_text){
        MonoBehaviour.print("Skill activated: " + skill_text);
    }

}

