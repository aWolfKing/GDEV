using System;
using System.Collections.Generic;
using System.Linq;
using FE;


namespace Prototype.Skills {

    public class Proto_DoubleCounter_1 : PrototypeSkill, INamedObject{

        public string Name {
            get {
                return "Double counter 1";
            }
        }

        public string ShortName {
            get {
                return "Double counter 1";
            }
        }

        public string Description {
            get {
                return "When countering, attack twice for every attack this unit would otherwise do.";
            }
        }


        private bool canActivate = true;
        private PrototypeCharacter defender = null;



        public Proto_DoubleCounter_1(){
            PrototypeCombat.Callbacks.CombatStart.Attach(this.GetDefendingCharacter);
            PrototypeCombat.Callbacks.InitiateAttack.Attach(this.DoubleCounter);
            PrototypeCombat.Callbacks.DefendAttack.Attach(this.OnAttacked);
        }


        
        private void DoubleCounter(PrototypeCharacter attacker, PrototypeCharacter defender){
            if(attacker == this.defender){
                if(this.canActivate){ 
                    this.canActivate = false;
                    PrototypeUINotify.NotifySkillActivation(@"""Double counter"", additional attack.");
                    PrototypeCombat.Attack(attacker, defender);
                }
                else{
                    this.canActivate = true;
                }
            }
        }

        private void OnAttacked(PrototypeCharacter attacker, PrototypeCharacter defender){
            this.canActivate = true;
        }

        private void GetDefendingCharacter(PrototypeCharacter attacker, PrototypeCharacter defender){
            this.defender = defender;
            this.canActivate = true;
        }

    }

}
