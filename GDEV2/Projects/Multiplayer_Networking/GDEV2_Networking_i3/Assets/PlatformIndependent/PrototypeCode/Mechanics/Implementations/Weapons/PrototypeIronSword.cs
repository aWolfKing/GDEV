using System;
using System.Collections.Generic;
using FE;


namespace Prototype.Weapons{

    public class PrototypeIronSword : OwnedObject, IPrototypeWeapon {

        public Equipable.OnEquipCallback OnEquip {
            get {
                throw new NotImplementedException();
            }
        }

        public Equipable.OnUnEquipCallback OnUnEquip {
            get {
                throw new NotImplementedException();
            }
        }


        private CInt.FloatChange damageDone = null;
        private PrototypeCharacter  attacker = null,
                                    defender = null;


        public PrototypeIronSword(){
            this.damageDone = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
            PrototypeCombat.Callbacks.AttackStart.Attach(this.GetCombatInfo);
            PrototypeCombat.Callbacks.CombatEnd.Attach(this.ResetCombatInfo);
        }


        public void CalculateDamageDone() {
            this.damageDone.Value = 0;
            PrototypeCombat.DamageDone.AddChange(this.damageDone);
            PrototypeCharacter owner = OwnedObject.GetOwning<PrototypeCharacter>(this);
            if(owner != null && owner == this.attacker && this.defender != null){
                int damageValue = this.attacker.Strenght.Value - this.defender.Defence.Value;
                if(damageValue < 0){
                    damageValue = 0;
                }
                this.damageDone.Value = damageValue;
            }
        }


        private void GetCombatInfo(PrototypeCharacter attacker, PrototypeCharacter defender){
            this.attacker = attacker;
            this.defender = defender;
        }

        private void ResetCombatInfo(PrototypeCharacter attacker, PrototypeCharacter defender) {
            this.attacker = null;
            this.defender = null;
        }


        public bool CanBeEquiped(object by) {
            return true;
        }

        public bool CanBeUnEquipped(object by) {
            return true;
        }

        public void Equip(object by) {
            this.SetOwner(by);
        }

        public void UnEquip(object by) {
            this.SetOwner(null);
        }
    }

}
