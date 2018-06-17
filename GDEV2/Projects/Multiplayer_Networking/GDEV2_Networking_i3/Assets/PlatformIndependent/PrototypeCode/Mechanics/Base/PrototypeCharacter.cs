using System;
using System.Collections.Generic;
using System.Reflection;
using FE;


namespace Prototype{ 

    public class PrototypeCharacter : Character {

        private static Callback<int> damageRecieved = new Callback<int>();
        public static Callback<int> DamageRecieved{
            get{
                return damageRecieved;
            }
        }



        private CInt.FloatChange damageTaken = null;



        private IPrototypeWeapon equippedWeapon = null;
        public IPrototypeWeapon EquippedWeapon {
            get{
                return this.equippedWeapon;
            }
            set{
                this.equippedWeapon = value;
                this.equippedWeapon.Equip(this);
            }
        }



        public void RecieveDamage(int amount){
            amount *= -1;
            if(this.damageTaken == null){
                this.damageTaken = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
            }
            this.damageTaken.Value += amount;
            this.Health.AddChange(this.damageTaken);

            damageRecieved.Invoke((object _target) => { return OwnedObject.GetOwning<PrototypeCharacter>(_target) == this; }, amount);
            if(this.Health.Value <= 0){
                PrototypeCombat.EndCombatEarly();
            }
        }



        public void EquipSkill(PrototypeSkill skill){
            skill.SetOwner(this);
            if(skill is IEquipable){
                ((IEquipable)skill).Equip(this);
            }
        }

        public void UnEquipSkill(PrototypeSkill skill){
            skill.SetOwner(null);
            if(skill is IEquipable) {
                ((IEquipable)skill).UnEquip(this);
            }
        }



        public PrototypeCharacter() : base(){

        }

        public PrototypeCharacter(string data) : base(){

            int health = 30, strenght = 30, magic = 30, skill = 30, speed = 30, luck = 30, defence = 23, resistance = 23, movement = 7;
            string name = "name", description = "description";
            int[] skillIds = new int[] { };
            IPrototypeWeapon weapon = new Prototype.Weapons.PrototypeIronSword();

            UnitClass.Builder classBuilder = new UnitClass.Builder();
            classBuilder.SetNames("Protype unit", "Proto");
            classBuilder.SetDescription("A prototype unit, with high stats.");
            classBuilder.SetMinimumBaseStats(0, 0, 0, 0, 0, 0, 0, 0, 0);
            classBuilder.SetMaximumBaseStats(50, 50, 50, 50, 50, 50, 50, 50, 7);
            classBuilder.SetStatGrowths(0, 0, 0, 0, 0, 0, 0, 0);
            UnitClass protoTypeClass = classBuilder.Build();

            this.SetClass(protoTypeClass);
            CInt.SetBaseValue(this.Health.BaseStat, health);
            CInt.SetBaseValue(this.Strenght.BaseStat, strenght);
            CInt.SetBaseValue(this.Magic.BaseStat, magic);
            CInt.SetBaseValue(this.Skill.BaseStat, skill);
            CInt.SetBaseValue(this.Speed.BaseStat, speed);
            CInt.SetBaseValue(this.Luck.BaseStat, luck);
            CInt.SetBaseValue(this.Defence.BaseStat, defence);
            CInt.SetBaseValue(this.Resistance.BaseStat, resistance);
            CInt.SetBaseValue(this.Movement.BaseStat, movement);

            //this.GetType().GetField("name", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, name);
            //this.GetType().GetField("shortName", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, name);
            //this.GetType().GetField("description", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, description);

            this.EquippedWeapon = weapon;

            foreach(var id in skillIds){
                var _skill = PrototypeSkills.GetSkillFromID(id);
                if(_skill != null){
                    this.EquipSkill(_skill);
                }
            }

        }

    }

}