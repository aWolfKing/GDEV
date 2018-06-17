using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FE;


public class TestPlayerLevelUp : MonoBehaviour{

    [SerializeField] private int numberOfLevels = 10;
    [SerializeField] private int minGrowths = 0;



    private void Start() {

        /*
        UnitClass.Builder classbuilder = new UnitClass.Builder();
        classbuilder.SetNames("Class", "Class");
        classbuilder.SetDescription("Class");
        classbuilder.SetStatGrowths(5, -2, 2, 10, 0, 30, 0, 6);
        classbuilder.SetMinimumBaseStats(0, 0, 0, 0, 0, 0, 0, 0, 0);
        classbuilder.SetMaximumBaseStats(30, 30, 30, 30, 30, 30, 30, 30, 30);
        UnitClass unitClass = classbuilder.Build();
        */

        Character.Builder builder = new Character.Builder();
        builder.SetNames("John Doe", "John");
        builder.SetDescription("Character.");
        builder.SetClass(/*unitClass*/ DefaultUnitClasses.Villager);
        builder.SetPersonalStatChanges(0, 0, 0, 0, 0, 0, 0, 0, 0);
        //builder.SetPersonalStatGrowthRates(30, 50, 10, 20, 30, 15, 25, 5);
        Character character = builder.Build();


        for(int i=0; i<this.numberOfLevels; i++){

            UnitStats growths;
            character.LevelUp(out growths, this.minGrowths);
            PrintStats(character, growths);

        }


    }

    private void PrintStats(Character character,UnitStats growths){
        string print = "";
        print += "health: " + character.Health.BaseValue + " (+" + growths.Health.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Health) + "%)\n";
        print += "strenght: " + character.Strenght.BaseValue + " (+" + growths.Strenght.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Strenght) + "%)\n";
        print += "magic: " + character.Magic.BaseValue + " (+" + growths.Magic.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Magic) + "%)\n";
        print += "skill: " + character.Skill.BaseValue + " (+" + growths.Skill.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Skill) + "%)\n";
        print += "speed: " + character.Speed.BaseValue + " (+" + growths.Speed.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Speed) + "%)\n";
        print += "luck: " + character.Luck.BaseValue + " (+" + growths.Luck.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Luck) + "%)\n";
        print += "defence: "+ character.Defence.BaseValue + " (+" + growths.Defence.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Defence) + "%)\n";
        print += "resistance: " + character.Resistance.BaseValue + " (+" + growths.Resistance.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Resistance) + "%)\n";
        print += "movement: " + character.Movement.BaseValue + " (+" + growths.Movement.Value + ") (" + character.GetTotalStatGrowthRateFor(UnitStats.Stat.Movement) + "%)\n";
        MonoBehaviour.print(print);
    }

}

