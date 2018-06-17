using System;
using System.Collections.Generic;
using FE;


namespace Prototype.Skills {
    public class Proto_HealthyStrenght_1 : PrototypeSkill, INamedObject {


        public string Name {
            get {
                return "Healthy strenght 1";
            }
        }

        public string ShortName {
            get {
                return "Healthy str 1";
            }
        }

        public string Description {
            get {
                return "Boost damage done by ((20% of strenght) / max health * current health).";
            }
        }


        private CInt.FloatChange damageBoost = null;


        public Proto_HealthyStrenght_1(){
            this.damageBoost = new CInt.FloatChange(0, this, FloatChange.ChangeType.add);
            PrototypeCombat.Callbacks.AttackStart.Attach(this.AttackStart);
        }


        private void AttackStart(PrototypeCharacter attacker, PrototypeCharacter defender){
            PrototypeCharacter character = OwnedObject.GetOwning<PrototypeCharacter>(this);
            if(character == attacker){
                this.damageBoost.Value = FE.Math.RoundTowardsZero((attacker.Strenght.Value * 0.2f) / character.Health.BaseValue * character.Health.Value);
                PrototypeUINotify.NotifySkillActivation(@"""Healthy strenght"", added " + this.damageBoost.Value + " to damage done.");
                PrototypeCombat.DamageDone.AddChange(this.damageBoost);
            }
        }

    }
}
