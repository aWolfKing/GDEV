using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class UnitStats : IUnitStats{ 

        public enum Stat{
            Health,
            Strenght,
            Magic,
            Skill,
            Speed,
            Luck,
            Defence,
            Resistance,
            Movement
        }



        private NamedIntStat    health = null,
                                strenght = null,
                                magic = null,
                                skill = null,
                                speed = null,
                                luck = null,
                                defence = null,
                                resistance = null,
                                movement = null;



        public static NamedIntStat MakeUnitStat(Stat stat, int value){
            switch(stat){
                case Stat.Health:
                    return new NamedIntStat(value, "Health", "hp", "Maximum amount of health.");
                case Stat.Strenght:
                    return new NamedIntStat(value, "Strenght", "str", "Influences physical damage dealth.");
                case Stat.Magic:
                    return new NamedIntStat(value, "Magic", "mag", "Influences magical damage dealth.");
                case Stat.Skill:
                    return new NamedIntStat(value, "Skill", "skl", "Influences hit rate and skill activation.");
                case Stat.Speed:
                    return new NamedIntStat(value, "Speed", "spd", "Influences evasion and amount of attacks in combat.");
                case Stat.Luck:
                    return new NamedIntStat(value, "Luck", "lck", "Influences skill activation and other things.");
                case Stat.Defence:
                    return new NamedIntStat(value, "Defence", "def", "Influences physical damage recieved.");
                case Stat.Resistance:
                    return new NamedIntStat(value, "Resistance", "res", "Influences magical damage recieved.");
                case Stat.Movement:
                    return new NamedIntStat(value, "Movement", "mov", "Amount of spaces this unit can walk.");
            }
            return null;
        }




        public NamedIntStat Health{
            get{
                return this.health;
            }
        }

        public NamedIntStat Strenght{ 
            get{
                return this.strenght;
            }
        }

        public NamedIntStat Magic{
            get{
                return this.magic;
            }
        }

        public NamedIntStat Skill{
            get{
                return this.skill;
            }
        }

        public NamedIntStat Speed{
            get{
                return this.speed;
            }
        }

        public NamedIntStat Luck{
            get{
                return this.luck;
            }
        }

        public NamedIntStat Defence{
            get{
                return this.defence;
            }
        }

        public NamedIntStat Resistance{
            get{
                return this.resistance;
            }
        }

        public NamedIntStat Movement{
            get{
                return this.movement;
            }
        }



        public UnitStats(int health, int strenght, int magic, int skill, int speed, int luck, int defence, int resistance, int movement) {
            this.health = UnitStats.MakeUnitStat(UnitStats.Stat.Health, health);
            this.strenght = UnitStats.MakeUnitStat(UnitStats.Stat.Strenght, strenght);
            this.magic = UnitStats.MakeUnitStat(UnitStats.Stat.Magic, magic);
            this.skill = UnitStats.MakeUnitStat(UnitStats.Stat.Skill, skill);
            this.speed = UnitStats.MakeUnitStat(UnitStats.Stat.Speed, speed);
            this.luck = UnitStats.MakeUnitStat(UnitStats.Stat.Luck, luck);
            this.defence = UnitStats.MakeUnitStat(UnitStats.Stat.Defence, defence);
            this.resistance = UnitStats.MakeUnitStat(UnitStats.Stat.Resistance, resistance);
            this.movement = UnitStats.MakeUnitStat(UnitStats.Stat.Movement, movement);
        }

        public UnitStats() {
            this.health = UnitStats.MakeUnitStat(UnitStats.Stat.Health, 0);
            this.strenght = UnitStats.MakeUnitStat(UnitStats.Stat.Strenght, 0);
            this.magic = UnitStats.MakeUnitStat(UnitStats.Stat.Magic, 0);
            this.skill = UnitStats.MakeUnitStat(UnitStats.Stat.Skill, 0);
            this.speed = UnitStats.MakeUnitStat(UnitStats.Stat.Speed, 0);
            this.luck = UnitStats.MakeUnitStat(UnitStats.Stat.Luck, 0);
            this.defence = UnitStats.MakeUnitStat(UnitStats.Stat.Defence, 0);
            this.resistance = UnitStats.MakeUnitStat(UnitStats.Stat.Resistance, 0);
            this.movement = UnitStats.MakeUnitStat(UnitStats.Stat.Movement, 0);
        }

    }

}
