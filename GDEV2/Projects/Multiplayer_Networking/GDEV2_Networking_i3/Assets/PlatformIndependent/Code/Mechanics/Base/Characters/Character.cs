using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class Character : INamedObject, IUnitStats{

        public class Builder{

            private bool    namesSet = false,
                            descriptionSet = false,
                            classSet = false,
                            personalStatChangesSet = false,
                            personalStatGrowthRatesSet = false;



            private Character character = null;



            public void SetNames(string fullName, string shortName){
                this.character.name = fullName;
                this.character.shortName = shortName;
                this.namesSet = true;
            }

            public void SetDescription(string description){
                this.character.description = description;
                this.descriptionSet = true;
            }

            public void SetClass(UnitClass unitClass){
                this.character.SetClass(unitClass);
                this.classSet = true;
            }

            private void SetPersonalStatChange(UnitStats.Stat stat, int value){
                switch(stat){
                    case UnitStats.Stat.Health:
                        CInt.SetBaseValue(this.character.personalStatChanges.Health.BaseStat, value);
                        break;
                    case UnitStats.Stat.Strenght:
                        CInt.SetBaseValue(this.character.personalStatChanges.Strenght.BaseStat, value);
                        break;
                    case UnitStats.Stat.Magic:
                        CInt.SetBaseValue(this.character.personalStatChanges.Magic.BaseStat, value);
                        break;
                    case UnitStats.Stat.Skill:
                        CInt.SetBaseValue(this.character.personalStatChanges.Skill.BaseStat, value);
                        break;
                    case UnitStats.Stat.Speed:
                        CInt.SetBaseValue(this.character.personalStatChanges.Speed.BaseStat, value);
                        break;
                    case UnitStats.Stat.Luck:
                        CInt.SetBaseValue(this.character.personalStatChanges.Luck.BaseStat, value);
                        break;
                    case UnitStats.Stat.Defence:
                        CInt.SetBaseValue(this.character.personalStatChanges.Defence.BaseStat, value);
                        break;
                    case UnitStats.Stat.Resistance:
                        CInt.SetBaseValue(this.character.personalStatChanges.Resistance.BaseStat, value);
                        break;
                    case UnitStats.Stat.Movement:
                        CInt.SetBaseValue(this.character.personalStatChanges.Movement.BaseStat, value);
                        break;
                }
            }

            public void SetPersonalStatChanges(int health, int strenght, int magic, int skill, int speed, int luck, int defence, int resistance, int movement){
                SetPersonalStatChange(UnitStats.Stat.Health, health);
                SetPersonalStatChange(UnitStats.Stat.Strenght, strenght);
                SetPersonalStatChange(UnitStats.Stat.Magic, magic);
                SetPersonalStatChange(UnitStats.Stat.Skill, skill);
                SetPersonalStatChange(UnitStats.Stat.Speed, speed);
                SetPersonalStatChange(UnitStats.Stat.Luck, luck);
                SetPersonalStatChange(UnitStats.Stat.Defence, defence);
                SetPersonalStatChange(UnitStats.Stat.Resistance, resistance);
                SetPersonalStatChange(UnitStats.Stat.Movement, movement);
                this.personalStatChangesSet = true;
            }

            public void SetPersonalStatGrowthRates(float health, float strenght, float magic, float skill, float speed, float luck, float defence, float resistance){
                this.character.personalStatGrowthRates = new UnitStatGrowths(health, strenght, magic, skill, speed, luck, defence, resistance);
                this.personalStatGrowthRatesSet = true;
            }

            public Character Build(){

                if(!this.namesSet || !this.descriptionSet || !this.classSet){
                    string msg = "The builder was not completed: \n";
                    msg += "Names " + (this.namesSet ? "set" : "not set") + "\n";
                    msg += "Description " + (this.descriptionSet ? "set" : "not set") + "\n";
                    msg += "Class " + (this.classSet ? "set" : "not set") + "\n";
                    msg += "Personal stat changes " + (this.personalStatChangesSet ? "set" : "not set") + " (Optional)" + "\n";
                    msg += "Personal stat growth rates " + (this.personalStatGrowthRatesSet ? "set" : "not set") + " (Optional)" + "\n";
                    throw new Exception(msg);
                }

                this.character.classStatChanges.Refresh(this.character);
                this.character.characterStatChanges.Refresh(this.character);
                this.character.levelStatChanges.Refresh(this.character);

                if(this.character.unitClass != null){
                    var classSkills = new List<ISkill>(this.character.unitClass.Skills);
                    foreach(var skill in classSkills){
                        this.character.EquipSkill(skill);
                    }
                }

                return this.character;
            }



            public Builder(){
                this.character = new Character();
            }

        }



        public class LevelStatChanges{

            private CInt.FloatChange    health = null,
                                        strenght = null,
                                        magic = null,
                                        skill = null,
                                        speed = null,
                                        luck = null,
                                        defence = null,
                                        resistance = null,
                                        movement = null;

            private Dictionary<int, UnitStats> growthPerLevel = new Dictionary<int, UnitStats>();


            public void GenerateGrowth(Character character, out UnitStats growth){
                GenerateGrowth(character, true, out growth);
            }

            public void GenerateGrowth(Character character, out UnitStats growth, int minGrowths){
                GenerateGrowth(character, true, out growth, minGrowths);
            }

            public void GenerateGrowthNoApply(Character character, out UnitStats growth) {
                GenerateGrowth(character, false, out growth);
            }

            public void GenerateGrowthNoApply(Character character, out UnitStats growth, int minGrowths) {
                GenerateGrowth(character, false, out growth, minGrowths);
            }

            private void GenerateGrowth(Character character, bool doApply, out UnitStats growth, int minGrowths = 0){
                if(!this.growthPerLevel.ContainsKey(character.Level)){

                    growth = new    UnitStats( 
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Health),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Strenght),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Magic),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Skill),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Speed),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Luck),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Defence),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Resistance),
                                            GetRandomLevelGrowth(character, UnitStats.Stat.Movement)
                                    );


                    if(minGrowths > 0){ 
                        List<UnitStats.Stat> growthStats = new List<UnitStats.Stat>();
                        if(growth.Health.Value > 0){
                            growthStats.Add(UnitStats.Stat.Health);
                        }
                        if(growth.Strenght.Value > 0){
                            growthStats.Add(UnitStats.Stat.Strenght);
                        }
                        if(growth.Magic.Value > 0){
                            growthStats.Add(UnitStats.Stat.Magic);
                        }
                        if(growth.Skill.Value > 0){
                            growthStats.Add(UnitStats.Stat.Skill);
                        }
                        if(growth.Speed.Value > 0){
                            growthStats.Add(UnitStats.Stat.Speed);
                        }
                        if(growth.Luck.Value > 0){
                            growthStats.Add(UnitStats.Stat.Luck);
                        }
                        if(growth.Defence.Value > 0){
                            growthStats.Add(UnitStats.Stat.Defence);
                        }
                        if(growth.Resistance.Value > 0){
                            growthStats.Add(UnitStats.Stat.Resistance);
                        }
                        if(growth.Movement.Value > 0){
                            growthStats.Add(UnitStats.Stat.Movement);
                        }

                        int growthCount = growthStats.Count;
                        if(growthCount < minGrowths){
                            List<UnitStats.Stat> allStats = new List<UnitStats.Stat>(){
                                UnitStats.Stat.Health,
                                UnitStats.Stat.Strenght,
                                UnitStats.Stat.Magic,
                                UnitStats.Stat.Skill,
                                UnitStats.Stat.Speed,
                                UnitStats.Stat.Luck,
                                UnitStats.Stat.Defence,
                                UnitStats.Stat.Resistance,
                                UnitStats.Stat.Movement
                            };
                            for(int i=allStats.Count-1; i>=0; i--){
                                if(growthStats.Contains(allStats[i])){
                                    allStats.RemoveAt(i);
                                }
                            }
                            growthStats = allStats; //growth stats are now the stats that didn't grow.

                            float totGrowth = 0;
                            ///Every growth rate = growth rate + last stat's growth rate.
                            Dictionary<UnitStats.Stat, float> growthRates = new Dictionary<UnitStats.Stat, float>();
                            foreach(var stat in growthStats){
                                float growthChance = character.GetTotalStatGrowthRateFor(stat);
                                if(growthChance > 0){
                                    growthRates.Add(stat, growthChance + totGrowth);
                                    totGrowth += growthChance;
                                }
                            }

                            int health = growth.Health.Value;
                            int strenght = growth.Strenght.Value;
                            int magic = growth.Magic.Value;
                            int skill = growth.Skill.Value;
                            int speed = growth.Speed.Value;
                            int luck = growth.Luck.Value;
                            int defence = growth.Defence.Value;
                            int resistance = growth.Resistance.Value;
                            int movement = growth.Movement.Value;

                            do {
                                if(growthRates.Count > 0 && growthCount < minGrowths){

                                    float totalValue = 0;
                                    foreach(var growthRate in growthRates){
                                        totalValue += growthRate.Value;
                                    }
                                    //float randomNumber = Math.RandomFloat_Unity(0, totalValue);

                                    UnitStats.Stat currectStat = UnitStats.Stat.Luck;
                                    float currentNumber = -1;

                                    foreach(var growthRate in growthRates){
                                        if(growthRate.Value > currentNumber){
                                            currentNumber = growthRate.Value;
                                            currectStat = growthRate.Key;
                                        }
                                    }

                                    if(growthRates.ContainsKey(currectStat)){
                                        growthRates.Remove(currectStat);
                                        growthCount++;

                                        switch(currectStat){
                                            case UnitStats.Stat.Health:
                                                health += 1;
                                                break;
                                            case UnitStats.Stat.Strenght:
                                                strenght += 1;
                                                break;
                                            case UnitStats.Stat.Magic:
                                                magic += 1;
                                                break;
                                            case UnitStats.Stat.Skill:
                                                skill += 1;
                                                break;
                                            case UnitStats.Stat.Speed:
                                                speed += 1;
                                                break;
                                            case UnitStats.Stat.Luck:
                                                luck += 1;
                                                break;
                                            case UnitStats.Stat.Defence:
                                                defence += 1;
                                                break;
                                            case UnitStats.Stat.Resistance:
                                                resistance += 1;
                                                break;
                                            case UnitStats.Stat.Movement:
                                                movement += 1;
                                                break;
                                        }

                                    }

                                }
                                else{
                                    break;
                                }
                            }
                            while(true);

                            growth = new UnitStats(health, strenght, magic, skill, speed, luck, defence, resistance, movement);
                        }


                    }


                    if(doApply){
                        var growthClone = new UnitStats(
                            growth.Health.Value,
                            growth.Strenght.Value,
                            growth.Magic.Value,
                            growth.Skill.Value,
                            growth.Speed.Value,
                            growth.Luck.Value,
                            growth.Defence.Value,
                            growth.Resistance.Value,
                            growth.Movement.Value
                        );
                        this.growthPerLevel.Add(character.level, growthClone);
                    }
                }
                else{
                    growth = this.growthPerLevel[character.level];
                }
            }

            private int GetRandomLevelGrowth(Character character, UnitStats.Stat stat){
                //float randomFloat = Math.RandomFloat_Unity(0, 100);
                float chance = 0;

                if(character.personalStatGrowthRates != null){
                    switch(stat){
                        case UnitStats.Stat.Health:
                            chance += character.personalStatGrowthRates.Health.Value;
                            break;
                        case UnitStats.Stat.Strenght:
                            chance += character.personalStatGrowthRates.Strenght.Value;
                            break;
                        case UnitStats.Stat.Magic:
                            chance += character.personalStatGrowthRates.Magic.Value;
                            break;
                        case UnitStats.Stat.Skill:
                            chance += character.personalStatGrowthRates.Skill.Value;
                            break;
                        case UnitStats.Stat.Speed:
                            chance += character.personalStatGrowthRates.Speed.Value;
                            break;
                        case UnitStats.Stat.Luck:
                            chance += character.personalStatGrowthRates.Luck.Value;
                            break;
                        case UnitStats.Stat.Defence:
                            chance += character.personalStatGrowthRates.Defence.Value;
                            break;
                        case UnitStats.Stat.Resistance:
                            chance += character.personalStatGrowthRates.Resistance.Value;
                            break;
                        case UnitStats.Stat.Movement:
                            chance += character.personalStatGrowthRates.Movement.Value;
                            break;
                    }
                }

                if(character.unitClass != null && character.unitClass.StatGrowthRates != null){
                    switch(stat){
                        case UnitStats.Stat.Health:
                            chance += character.unitClass.StatGrowthRates.Health.Value;
                            break;
                        case UnitStats.Stat.Strenght:
                            chance += character.unitClass.StatGrowthRates.Strenght.Value;
                            break;
                        case UnitStats.Stat.Magic:
                            chance += character.unitClass.StatGrowthRates.Magic.Value;
                            break;
                        case UnitStats.Stat.Skill:
                            chance += character.unitClass.StatGrowthRates.Skill.Value;
                            break;
                        case UnitStats.Stat.Speed:
                            chance += character.unitClass.StatGrowthRates.Speed.Value;
                            break;
                        case UnitStats.Stat.Luck:
                            chance += character.unitClass.StatGrowthRates.Luck.Value;
                            break;
                        case UnitStats.Stat.Defence:
                            chance += character.unitClass.StatGrowthRates.Defence.Value;
                            break;
                        case UnitStats.Stat.Resistance:
                            chance += character.unitClass.StatGrowthRates.Resistance.Value;
                            break;
                        case UnitStats.Stat.Movement:
                            chance += character.unitClass.StatGrowthRates.Movement.Value;
                            break;
                    }
                }

                return Math.RandomBool_Unity(chance) ? 1 : 0;
            }



            public bool HasLevelGrowthForLevel(int level, out UnitStats growth){
                if(this.growthPerLevel.ContainsKey(level)){
                    var levelGrowth = this.growthPerLevel[level];
                    growth = new UnitStats(
                        levelGrowth.Health.Value,
                        levelGrowth.Strenght.Value,
                        levelGrowth.Magic.Value,
                        levelGrowth.Skill.Value,
                        levelGrowth.Speed.Value,
                        levelGrowth.Luck.Value,
                        levelGrowth.Defence.Value,
                        levelGrowth.Resistance.Value,
                        levelGrowth.Movement.Value
                    );
                    return true;
                }
                else{
                    growth = new UnitStats();
                    return false;
                }
            }

            internal void Refresh(Character character) {

                this.health.Value = 0;
                this.strenght.Value = 0;
                this.magic.Value = 0;
                this.skill.Value = 0;
                this.speed.Value = 0;
                this.luck.Value = 0;
                this.defence.Value = 0;
                this.resistance.Value = 0;
                this.movement.Value = 0;

                for(int i=0; i<=character.level; i++){
                    if(this.growthPerLevel.ContainsKey(i)){
                        var growth = this.growthPerLevel[i];
                        this.health.Value += growth.Health.Value;
                        this.strenght.Value += growth.Strenght.Value;
                        this.magic.Value += growth.Magic.Value;
                        this.skill.Value += growth.Skill.Value;
                        this.speed.Value += growth.Speed.Value;
                        this.luck.Value += growth.Luck.Value;
                        this.defence.Value += growth.Defence.Value;
                        this.resistance.Value += growth.Resistance.Value;
                        this.movement.Value += growth.Movement.Value;
                    }
                }

                character.health.BaseStat.AddChange(this.health);
                character.strenght.BaseStat.AddChange(this.strenght);
                character.magic.BaseStat.AddChange(this.magic);
                character.skill.BaseStat.AddChange(this.skill);
                character.speed.BaseStat.AddChange(this.speed);
                character.luck.BaseStat.AddChange(this.luck);
                character.defence.BaseStat.AddChange(this.defence);
                character.resistance.BaseStat.AddChange(this.resistance);
                character.movement.BaseStat.AddChange(this.movement);
            }

            internal CInt.FloatChange GetStatChange(UnitStats.Stat stat){
                switch(stat){
                    case UnitStats.Stat.Health:
                        return this.health;
                    case UnitStats.Stat.Strenght:
                        return this.strenght;
                    case UnitStats.Stat.Magic:
                        return this.magic;
                    case UnitStats.Stat.Skill:
                        return this.skill;
                    case UnitStats.Stat.Speed:
                        return this.speed;
                    case UnitStats.Stat.Luck:
                        return this.luck;
                    case UnitStats.Stat.Defence:
                        return this.defence;
                    case UnitStats.Stat.Resistance:
                        return this.resistance;
                    case UnitStats.Stat.Movement:
                        return this.movement;
                }
                return null;
            }



            public LevelStatChanges(Character character){
                this.health = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.strenght = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.magic = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.skill = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.speed = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.luck = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.defence = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.resistance = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.movement = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                Refresh(character);
            }

        }



        private class ClassStatChanges{

            private CInt.FloatChange    health = null,
                                        strenght = null,
                                        magic = null,
                                        skill = null,
                                        speed = null,
                                        luck = null,
                                        defence = null,
                                        resistance = null,
                                        movement = null;



            internal void Refresh(Character character) {
                if(character.unitClass != null){
                    this.health.Value = character.unitClass.MinLevelStats.Health.Value;
                    this.strenght.Value = character.unitClass.MinLevelStats.Strenght.Value;
                    this.magic.Value = character.unitClass.MinLevelStats.Magic.Value;
                    this.skill.Value = character.unitClass.MinLevelStats.Skill.Value;
                    this.speed.Value = character.unitClass.MinLevelStats.Speed.Value;
                    this.luck.Value = character.unitClass.MinLevelStats.Luck.Value;
                    this.defence.Value = character.unitClass.MinLevelStats.Defence.Value;
                    this.resistance.Value = character.unitClass.MinLevelStats.Resistance.Value;
                    this.movement.Value = character.unitClass.MinLevelStats.Movement.Value;
                }
                else{                    
                    this.health.Value = 0;
                    this.strenght.Value = 0;
                    this.magic.Value = 0;
                    this.skill.Value = 0;
                    this.speed.Value = 0;
                    this.luck.Value = 0;
                    this.defence.Value = 0;
                    this.resistance.Value = 0;
                    this.movement.Value = 0;
                }

                character.health.BaseStat.AddChange(this.health);
                character.strenght.BaseStat.AddChange(this.strenght);
                character.magic.BaseStat.AddChange(this.magic);
                character.skill.BaseStat.AddChange(this.skill);
                character.speed.BaseStat.AddChange(this.speed);
                character.luck.BaseStat.AddChange(this.luck);
                character.defence.BaseStat.AddChange(this.defence);
                character.resistance.BaseStat.AddChange(this.resistance);
                character.movement.BaseStat.AddChange(this.movement);
            }

            internal CInt.FloatChange GetStatChange(UnitStats.Stat stat){
                switch(stat){
                    case UnitStats.Stat.Health:
                        return this.health;
                    case UnitStats.Stat.Strenght:
                        return this.strenght;
                    case UnitStats.Stat.Magic:
                        return this.magic;
                    case UnitStats.Stat.Skill:
                        return this.skill;
                    case UnitStats.Stat.Speed:
                        return this.speed;
                    case UnitStats.Stat.Luck:
                        return this.luck;
                    case UnitStats.Stat.Defence:
                        return this.defence;
                    case UnitStats.Stat.Resistance:
                        return this.resistance;
                    case UnitStats.Stat.Movement:
                        return this.movement;
                }
                return null;
            }



            public ClassStatChanges(Character character){
                this.health = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.strenght = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.magic = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.skill = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.speed = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.luck = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.defence = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.resistance = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.movement = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                Refresh(character);
            }

        }



        private class CharacterStatChanges{

            private CInt.FloatChange    health = null,
                                        strenght = null,
                                        magic = null,
                                        skill = null,
                                        speed = null,
                                        luck = null,
                                        defence = null,
                                        resistance = null,
                                        movement = null;



            internal void Refresh(Character character) {
                if(character.personalStatChanges != null){
                    this.health.Value = character.personalStatChanges.Health.Value;
                    this.strenght.Value = character.personalStatChanges.Strenght.Value;
                    this.magic.Value = character.personalStatChanges.Magic.Value;
                    this.skill.Value = character.personalStatChanges.Skill.Value;
                    this.speed.Value = character.personalStatChanges.Speed.Value;
                    this.luck.Value = character.personalStatChanges.Luck.Value;
                    this.defence.Value = character.personalStatChanges.Defence.Value;
                    this.resistance.Value = character.personalStatChanges.Resistance.Value;
                    this.movement.Value = character.personalStatChanges.Movement.Value;
                }
                else{                    
                    this.health.Value = 0;
                    this.strenght.Value = 0;
                    this.magic.Value = 0;
                    this.skill.Value = 0;
                    this.speed.Value = 0;
                    this.luck.Value = 0;
                    this.defence.Value = 0;
                    this.resistance.Value = 0;
                    this.movement.Value = 0;
                }

                character.health.BaseStat.AddChange(this.health);
                character.strenght.BaseStat.AddChange(this.strenght);
                character.magic.BaseStat.AddChange(this.magic);
                character.skill.BaseStat.AddChange(this.skill);
                character.speed.BaseStat.AddChange(this.speed);
                character.luck.BaseStat.AddChange(this.luck);
                character.defence.BaseStat.AddChange(this.defence);
                character.resistance.BaseStat.AddChange(this.resistance);
                character.movement.BaseStat.AddChange(this.movement);
            }

            internal CInt.FloatChange GetStatChange(UnitStats.Stat stat){
                switch(stat){
                    case UnitStats.Stat.Health:
                        return this.health;
                    case UnitStats.Stat.Strenght:
                        return this.strenght;
                    case UnitStats.Stat.Magic:
                        return this.magic;
                    case UnitStats.Stat.Skill:
                        return this.skill;
                    case UnitStats.Stat.Speed:
                        return this.speed;
                    case UnitStats.Stat.Luck:
                        return this.luck;
                    case UnitStats.Stat.Defence:
                        return this.defence;
                    case UnitStats.Stat.Resistance:
                        return this.resistance;
                    case UnitStats.Stat.Movement:
                        return this.movement;
                }
                return null;
            }



            public CharacterStatChanges(Character character){
                this.health = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.strenght = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.magic = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.skill = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.speed = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.luck = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.defence = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.resistance = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                this.movement = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
                Refresh(character);
            }

        }



        private string name = null;
        private string shortName = null;
        private string description = null;

        private int level = 1;
        private CFloat experienceMultiplier = null;
        private UnitStatGrowths personalStatGrowthRates = null;

        private List<ISkill> equippedSkills = new List<ISkill>();
        private List<ISkill> personalSkills = new List<ISkill>();



        private NamedIntStat    health = null,
                                strenght = null,
                                magic = null,
                                skill = null,
                                speed = null,
                                luck = null,
                                defence = null,
                                resistance = null,
                                movement = null;

        private UnitClass unitClass = null;


        /// <summary>
        /// Stat changes that are always applied.
        /// </summary>
        private UnitStats personalStatChanges = null;

        private ClassStatChanges classStatChanges = null;

        private CharacterStatChanges characterStatChanges = null;

        /// <summary>
        /// Stat changes caused by leveling up.
        /// </summary>
        private LevelStatChanges levelStatChanges = null;


        
        public string Name{
            get{
                return this.name;
            }
        }

        public string ShortName{
            get{
                return this.shortName;
            }
        }

        public string Description{
            get{
                return this.description;
            }
        }



        public int Level{
            get{
                return this.level;
            }
        }

        public CFloat ExperienceMultiplier{
            get{
                return this.experienceMultiplier;
            }
        }



        public UnitClass Class{
            get{
                return this.unitClass;
            }
        }



        public UnitStatGrowths PersonalStatGrowthRates{
            get{
                return this.personalStatGrowthRates;
            }
        }


                                
        public NamedIntStat Health{
            get{
                RefreshStatChanges();
                return this.health;
            }
        }

        public NamedIntStat Strenght{ 
            get{
                RefreshStatChanges();
                return this.strenght;
            }
        }

        public NamedIntStat Magic{
            get{
                RefreshStatChanges();
                return this.magic;
            }
        }

        public NamedIntStat Skill{
            get{
                RefreshStatChanges();
                return this.skill;
            }
        }

        public NamedIntStat Speed{
            get{
                RefreshStatChanges();
                return this.speed;
            }
        }

        public NamedIntStat Luck{
            get{
                RefreshStatChanges();
                return this.luck;
            }
        }

        public NamedIntStat Defence{
            get{
                RefreshStatChanges();
                return this.defence;
            }
        }

        public NamedIntStat Resistance{
            get{
                RefreshStatChanges();
                return this.resistance;
            }
        }

        public NamedIntStat Movement{
            get{
                RefreshStatChanges();
                return this.movement;
            }
        }



        public void SetClass(UnitClass unitClass){
            if(this.unitClass != null) {
                var classSkillList = new List<ISkill>(this.unitClass.Skills);
                for(int i = this.equippedSkills.Count - 1; i >= 0; i--) {
                    var skill = this.equippedSkills[i];
                    if(classSkillList.Contains(skill)) {
                        this.UnEquipSkill(skill);
                    }
                }
            }
            { 
                var classSkillList = new List<ISkill>(unitClass.Skills);
                foreach(var skill in classSkillList){
                    if(!this.equippedSkills.Contains(skill)){
                        this.EquipSkill(skill);
                    }
                }
            }

            this.unitClass = unitClass;
            this.classStatChanges.Refresh(this);
        }



        public void EquipSkill(ISkill skill){
            if(!this.equippedSkills.Contains(skill)){
                this.equippedSkills.Add(skill);
                if(skill is IEquipable){
                    ((IEquipable)skill).Equip(this);
                }
            }
        }

        public void UnEquipSkill(ISkill skill) {
            if(this.equippedSkills.Contains(skill)){
                this.equippedSkills.Remove(skill);
                if(skill is Equipable){
                    ((IEquipable)skill).UnEquip(this);
                }
            }
        }



        /// <summary>
        /// Levels up with at least 1 stat growth.
        /// </summary>
        /// <param name="growths"></param>
        public void LevelUp(out UnitStats growths){
            this.level++;
            this.levelStatChanges.GenerateGrowth(this, out growths, 1);
        }

        public void LevelUp(out UnitStats growths, int minStatGrowths){
            this.level++;
            this.levelStatChanges.GenerateGrowth(this, out growths, minStatGrowths);
        }

        /// <summary>
        /// Levels up with no minimum amount of stat growths.
        /// </summary>
        /// <param name="growths"></param>
        public void LevelUpNoMinGrowths(out UnitStats growths){
            this.level++;
            this.levelStatChanges.GenerateGrowth(this, out growths);
        }

        public float GetTotalStatGrowthRateFor(UnitStats.Stat stat){
            float value = 0;
            switch(stat){
                case UnitStats.Stat.Health:
                    value += this.personalStatGrowthRates.Health.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Health.Value;
                    }
                    break;
                case UnitStats.Stat.Strenght:
                    value += this.personalStatGrowthRates.Strenght.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Strenght.Value;
                    }
                    break;
                case UnitStats.Stat.Magic:
                    value += this.personalStatGrowthRates.Magic.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Magic.Value;
                    }
                    break;
                case UnitStats.Stat.Skill:
                    value += this.personalStatGrowthRates.Skill.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Skill.Value;
                    }
                    break;
                case UnitStats.Stat.Speed:
                    value += this.personalStatGrowthRates.Speed.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Speed.Value;
                    }
                    break;
                case UnitStats.Stat.Luck:
                    value += this.personalStatGrowthRates.Luck.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Luck.Value;
                    }
                    break;
                case UnitStats.Stat.Defence:
                    value += this.personalStatGrowthRates.Defence.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Defence.Value;
                    }
                    break;
                case UnitStats.Stat.Resistance:
                    value += this.personalStatGrowthRates.Resistance.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Resistance.Value;
                    }
                    break;
                case UnitStats.Stat.Movement:
                    value += this.personalStatGrowthRates.Movement.Value;
                    if(this.unitClass != null){
                        value += this.unitClass.StatGrowthRates.Movement.Value;
                    }
                    break;
            }
            return value;
        }        



        private void RefreshStatChanges(){
            this.characterStatChanges.Refresh(this);
            this.classStatChanges.Refresh(this);
            this.levelStatChanges.Refresh(this);
        }



        protected Character(){
            this.health = UnitStats.MakeUnitStat(UnitStats.Stat.Health, 0);
            this.strenght = UnitStats.MakeUnitStat(UnitStats.Stat.Strenght, 0);
            this.magic = UnitStats.MakeUnitStat(UnitStats.Stat.Magic, 0);
            this.skill = UnitStats.MakeUnitStat(UnitStats.Stat.Skill, 0);
            this.speed = UnitStats.MakeUnitStat(UnitStats.Stat.Speed, 0);
            this.luck = UnitStats.MakeUnitStat(UnitStats.Stat.Luck, 0);
            this.defence = UnitStats.MakeUnitStat(UnitStats.Stat.Defence, 0);
            this.resistance = UnitStats.MakeUnitStat(UnitStats.Stat.Resistance, 0);
            this.movement = UnitStats.MakeUnitStat(UnitStats.Stat.Movement, 0);

            this.experienceMultiplier = new CFloat(1);

            this.personalStatChanges = new UnitStats();

            this.classStatChanges = new ClassStatChanges(this);
            this.characterStatChanges = new CharacterStatChanges(this);
            this.levelStatChanges = new LevelStatChanges(this);
            this.personalStatGrowthRates = new UnitStatGrowths();
        }

    }

}
