using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class UnitClass : /*IUnitStats,*/ INamedObject{

        public class Builder{

            private bool    namesSet = false,
                            descriptionSet = false,
                            minStatsSet = false,
                            maxStatsSet = false,
                            statGrowthset = false,
                            unitClassSkillsSet = false;

            private UnitClass unitClass = null;



            public void SetNames(string fullName, string shortName){
                this.unitClass.name = fullName;
                this.unitClass.shortName = shortName;
                this.namesSet = true;
            }

            public void SetDescription(string description){
                this.unitClass.description = description;
                this.descriptionSet = true;
            }

            public void SetMinimumBaseStats(int health, int strenght, int magic, int skill, int speed, int luck, int defence, int resistance, int movement){
                CInt.SetBaseValue(this.unitClass.baseStats.Health.BaseStat, health);
                CInt.SetBaseValue(this.unitClass.baseStats.Strenght.BaseStat, strenght);
                CInt.SetBaseValue(this.unitClass.baseStats.Magic.BaseStat, magic);
                CInt.SetBaseValue(this.unitClass.baseStats.Skill.BaseStat, skill);
                CInt.SetBaseValue(this.unitClass.baseStats.Speed.BaseStat, speed);
                CInt.SetBaseValue(this.unitClass.baseStats.Luck.BaseStat, luck);
                CInt.SetBaseValue(this.unitClass.baseStats.Defence.BaseStat, defence);
                CInt.SetBaseValue(this.unitClass.baseStats.Resistance.BaseStat, resistance);
                CInt.SetBaseValue(this.unitClass.baseStats.Movement.BaseStat, movement);
                this.minStatsSet = true;
            }

            public void SetMaximumBaseStats(int health, int strenght, int magic, int skill, int speed, int luck, int defence, int resistance, int movement){
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Health.BaseStat, health);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Strenght.BaseStat, strenght);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Magic.BaseStat, magic);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Skill.BaseStat, skill);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Speed.BaseStat, speed);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Luck.BaseStat, luck);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Defence.BaseStat, defence);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Resistance.BaseStat, resistance);
                CInt.SetBaseValue(this.unitClass.baseStatsCaps.Movement.BaseStat, movement);
                this.maxStatsSet = true;
            }

            public void SetStatGrowths(float health, float strenght, float magic, float skill, float speed, float luck, float defence, float resistance){
                this.unitClass.statGrowthRates = new UnitStatGrowths(health, strenght, magic, skill, speed, luck, defence, resistance);
                this.statGrowthset = true;
            }

            public void SetClassSkills(params ISkill[] skills){
                this.unitClass.unitClassSkills = new List<ISkill>(skills);
                this.unitClassSkillsSet = true;
            }

            public UnitClass Build(){
                if(!this.namesSet || !this.descriptionSet || !this.minStatsSet || !this.maxStatsSet || !this.statGrowthset){
                    string msg = "The builder was not completed: \n";
                    msg += "Names " + (this.namesSet ? "set" : "not set") + "\n";
                    msg += "Description " + (this.descriptionSet ? "set" : "not set") + "\n";
                    msg += "Minimum stats " + (this.minStatsSet ? "set" : "not set") + "\n";
                    msg += "Maximum stats " + (this.maxStatsSet ? "set" : "not set") + "\n";
                    msg += "Stat growths " + (this.statGrowthset ? "set" : "not set") + "\n";
                    msg += "Class skills " + (this.unitClassSkillsSet ? "set" : "not set") + " (Optional)" + "\n";
                    throw new Exception(msg);
                }
                return this.unitClass;
            }



            public Builder(){
                this.unitClass = new UnitClass();
            }

        }



        private string name = null;
        private string shortName = null;
        private string description = null;

        private UnitStats baseStats = null;
        private UnitStats baseStatsCaps = null;

        private UnitStatGrowths statGrowthRates = null;

        private List<ISkill> unitClassSkills = null;



        public string Name {
            get {
                return this.name;
            }
        }

        public string ShortName {
            get {
                return this.shortName;
            }
        }

        public string Description {
            get {
                return this.description;
            }
        }       

        public UnitStats MinLevelStats{
            get{
                return this.baseStats;
            }
        }

        public UnitStats MaximumStats{
            get{
                return this.baseStatsCaps;
            }
        }

        public UnitStatGrowths StatGrowthRates{
            get{
                return this.statGrowthRates;
            }
        }

        public ISkill[] Skills{
            get{
                return this.unitClassSkills.ToArray();
            }
        }



        private UnitClass(){
            this.unitClassSkills = new List<ISkill>();
            this.statGrowthRates = new UnitStatGrowths();
            this.baseStats = new UnitStats();
            this.baseStatsCaps = new UnitStats();
        }

    }

}
