using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE.Skills {

    public class EnthousiasticLearning : IEquipableSkill, INamedObject {

        private const float percentage = 10;

        private CInt.FloatChange floatChange = null;

        private Equipable.OnEquipCallback onEquipCallback = new Equipable.OnEquipCallback();
        private Equipable.OnUnEquipCallback onUnEquipCallback = new Equipable.OnUnEquipCallback();



        public Equipable.OnEquipCallback OnEquip {
            get {
                return this.onEquipCallback;
            }
        }

        public Equipable.OnUnEquipCallback OnUnEquip {
            get{
                return this.onUnEquipCallback;
            }
        }



        public string Name {
            get {
                return "Enthousiastic learning";
            }
        }

        public string ShortName {
            get{
                return "Learning boost";
            }
        }

        public string Description {
            get{
                return "Boosts the growth rate of all skills except movement by " + percentage + ".";
            }
        }



        public bool CanBeEquiped(object by) {
            return true;
        }

        public bool CanBeUnEquipped(object by) {
            return true;
        }



        public void Equip(object by) {
            var character = OwnedObject.GetOwning<Character>(by);
            if(character != null){
                character.PersonalStatGrowthRates.Health.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Strenght.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Magic.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Skill.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Speed.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Luck.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Defence.AddChange(this.floatChange);
                character.PersonalStatGrowthRates.Resistance.AddChange(this.floatChange);
            }
        }

        public void UnEquip(object by) {
            var character = OwnedObject.GetOwning<Character>(by);
            if(character != null){
                character.PersonalStatGrowthRates.Health.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Strenght.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Magic.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Skill.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Speed.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Luck.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Defence.RemoveChange(this.floatChange);
                character.PersonalStatGrowthRates.Resistance.RemoveChange(this.floatChange);
            }
        }



        public EnthousiasticLearning(){
            this.floatChange = new CInt.FloatChange(percentage, this, FloatChange.ChangeType.add);
        }

    }

}
