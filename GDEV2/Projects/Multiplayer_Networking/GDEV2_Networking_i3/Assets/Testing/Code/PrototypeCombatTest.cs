using System;
using System.Collections.Generic;
using UnityEngine;


public class PrototypeCombatTest : MonoBehaviour{

    private void Start() {

        var attacker = new Prototype.PrototypeCharacter("");
        var defender = new Prototype.PrototypeCharacter("");

        defender.Speed.AddChange(new FE.CInt.FloatChange(5, this, FE.FloatChange.ChangeType.add));

        attacker.EquipSkill(Prototype.PrototypeSkills.GetSkillFromID(2));
        defender.EquipSkill(Prototype.PrototypeSkills.GetSkillFromID(3));

        Prototype.PrototypeCombat.StartCombat(attacker, defender);

    }

}

