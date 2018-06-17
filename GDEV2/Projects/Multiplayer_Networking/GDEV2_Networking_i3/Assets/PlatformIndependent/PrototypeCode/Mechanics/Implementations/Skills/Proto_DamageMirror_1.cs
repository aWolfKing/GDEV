using System;
using System.Collections.Generic;
using FE;


namespace Prototype.Skills{ 

    public class Proto_DamageMirror_1 : PrototypeSkill, IEquipable, INamedObject {

        private Equipable.OnEquipCallback onEquipCallback = new Equipable.OnEquipCallback();
        private Equipable.OnUnEquipCallback onUnEquipCallback = new Equipable.OnUnEquipCallback();



        public Equipable.OnEquipCallback OnEquip {
            get {
                return this.onEquipCallback;
            }
        }

        public Equipable.OnUnEquipCallback OnUnEquip {
            get {
                return this.onUnEquipCallback;
            }
        }



        public string Name {
            get {
                return "Damage mirror 1";
            }
        }

        public string ShortName {
            get {
                return "Dmg mirror 1";
            }
        }

        public string Description {
            get {
                return "Inflicts 10% of damage taken to the opposing character in combat (minimum of 1).";
            }
        }



        public bool CanBeEquiped(object by) {
            return true;
        }

        public bool CanBeUnEquipped(object by) {
            return true;
        }



        public void Equip(object by) {
            this.SetOwner(by);
            PrototypeCombat.Callbacks.CombatStart.Attach(this.OnCombatStart);
            PrototypeCombat.Callbacks.CombatEnd.Attach(this.OnCombatEnd);
            PrototypeCombat.Callbacks.AttackStart.Attach(this.OnAttackStart);
            PrototypeCharacter.DamageRecieved.Attach(this.MirrorDamage);
            PrototypeCharacter character = OwnedObject.GetOwning<PrototypeCharacter>(by);
            if(character != null){
                this.thisUnit = character;
            }
        }

        public void UnEquip(object by) {
            this.SetOwner(null);
            PrototypeCombat.Callbacks.CombatStart.Detach(this.OnCombatStart);
            PrototypeCombat.Callbacks.CombatEnd.Detach(this.OnCombatEnd);
            PrototypeCombat.Callbacks.AttackStart.Detach(this.OnAttackStart);
            PrototypeCharacter.DamageRecieved.Detach(this.MirrorDamage);
        }



        private PrototypeCharacter  attacker = null, 
                                    defender = null,
                                    thisUnit = null;

        private static bool canActivate = true;



        public Proto_DamageMirror_1() {

        }
        


        private void MirrorDamage(int damageRecieved){
            if(!canActivate){ return; }
            canActivate = false;
            int mirrorDamage = FE.Math.RoundTowardsZero(damageRecieved * 0.1f);
            if(mirrorDamage <= 0){
                mirrorDamage = 1;
            }
            if(this.thisUnit != null && this.attacker != null && this.defender != null){
                if(this.attacker == this.thisUnit && this.defender != this.thisUnit){
                    PrototypeUINotify.NotifySkillActivation(@"""Damage mirror"", mirrored " + mirrorDamage + " damage.");
                    this.defender.RecieveDamage(mirrorDamage);
                }
                else if(this.attacker != this.thisUnit && this.defender == this.thisUnit){
                    PrototypeUINotify.NotifySkillActivation(@"""Damage mirror"", mirrored " + mirrorDamage + " damage.");
                    this.attacker.RecieveDamage(mirrorDamage);
                }
            }
        }

        private void OnCombatStart(PrototypeCharacter attacker, PrototypeCharacter defender){
            this.attacker = attacker;
            this.defender = defender;
            canActivate = true;
        }

        private void OnAttackStart(PrototypeCharacter attacker, PrototypeCharacter defender){
            canActivate = true;
        }

        private void OnCombatEnd(PrototypeCharacter attacker, PrototypeCharacter defender) {
            this.attacker = null;
            this.defender = null;
            canActivate = false;
        }

    }

}
