using System;
using System.Collections.Generic;
using FE;


namespace Prototype{ 

    public class PrototypeCombat {

        public static class Callbacks{ 
            private static Callback<PrototypeCharacter, PrototypeCharacter> combatStart = new Callback<PrototypeCharacter, PrototypeCharacter>();
            public static Callback<PrototypeCharacter, PrototypeCharacter> CombatStart {
                get{
                    return combatStart;
                }
            }

            private static Callback<PrototypeCharacter, PrototypeCharacter> attackStart = new Callback<PrototypeCharacter, PrototypeCharacter>();
            public static Callback<PrototypeCharacter, PrototypeCharacter> AttackStart{
                get{
                    return attackStart;
                }
            }

            private static Callback<PrototypeCharacter, PrototypeCharacter> attackEnd = new Callback<PrototypeCharacter, PrototypeCharacter>();
            public static Callback<PrototypeCharacter, PrototypeCharacter> AttackEnd{
                get{
                    return attackEnd;
                }
            }

            private static Callback<PrototypeCharacter, PrototypeCharacter> initiateAttack = new Callback<PrototypeCharacter, PrototypeCharacter>();
            public static Callback<PrototypeCharacter, PrototypeCharacter> InitiateAttack{
                get{
                    return initiateAttack;
                }
            }

            private static Callback<PrototypeCharacter, PrototypeCharacter> defendAttack = new Callback<PrototypeCharacter, PrototypeCharacter>();
            public static Callback<PrototypeCharacter, PrototypeCharacter> DefendAttack{
                get{
                    return defendAttack;
                }
            }

            private static Callback<PrototypeCharacter, PrototypeCharacter> combatEnd = new Callback<PrototypeCharacter, PrototypeCharacter>();
            public static Callback<PrototypeCharacter, PrototypeCharacter> CombatEnd{
                get{
                    return combatEnd;
                }
            }
        }



        private static int attackerAttacks = 0;
        private static int defenderAttacks = 0;
        private static PrototypeCharacter   attacker = null,
                                            defender = null;

        private static bool doEndCombatEarly = false;

        private static CInt damageDone = new CInt();
        public static CInt DamageDone{
            get{
                return damageDone;
            }
        }



        public static void StartCombat(PrototypeCharacter attacker, PrototypeCharacter defender){

            attackerAttacks = 0;
            defenderAttacks = 0;
            doEndCombatEarly = false;
            PrototypeCombat.attacker = attacker;
            PrototypeCombat.defender = defender;

            int health_atk_was = attacker.Health.Value;
            int health_def_was = defender.Health.Value;

            PrototypeUINotify.Notify("Combat starts, attacker hp: " + attacker.Health.Value + ", defender hp: " + defender.Health.Value + ".");

            Callbacks.CombatStart.Invoke(attacker, defender);

            if(!doEndCombatEarly){

                if(attackerAttacks == 0){
                    Attack(attacker, defender);
                }

                if(!doEndCombatEarly){ 
                    if(defenderAttacks == 0){
                        Attack(defender, attacker);
                    }
                }

                if(!doEndCombatEarly){

                    if(attacker.Speed.Value - 5 >= defender.Speed.Value){
                        PrototypeUINotify.Notify("Attacker spd - 5 >= defender spd; attacker gains additional attack.");
                        Attack(attacker, defender);
                    }
                    else if(defender.Speed.Value - 5 >= attacker.Speed.Value){
                        PrototypeUINotify.Notify("Defender spd - 5 >= attacker spd; defender gains additional attack.");
                        Attack(defender, attacker);
                    }

                }

            }

            Callbacks.CombatEnd.Invoke(attacker, defender);

            PrototypeUINotify.Notify("Combat ended, attacker hp: " + attacker.Health.Value + " (" + (attacker.Health.Value - health_atk_was) + "), defender hp: " + defender.Health.Value  + " (" + (defender.Health.Value - health_def_was) + ").");

        }


        public static void Attack(PrototypeCharacter attacker, PrototypeCharacter defender){

            damageDone = new CInt();

            Callbacks.AttackStart.Invoke(attacker, defender);

            if(!doEndCombatEarly){ 

                Callbacks.InitiateAttack.Invoke((object _target) => { return OwnedObject.GetOwning<PrototypeCharacter>(_target) == attacker; }, attacker, defender);
                if(!doEndCombatEarly){ 
                    Callbacks.DefendAttack.Invoke((object _target) => { return OwnedObject.GetOwning<PrototypeCharacter>(_target) == defender; }, attacker, defender);
                    if(attacker == PrototypeCombat.attacker){
                        attackerAttacks++;
                    }
                    else if(attacker == PrototypeCombat.defender){
                        defenderAttacks++;
                    }

                    attacker.EquippedWeapon.CalculateDamageDone();

                    if(!doEndCombatEarly){

                        PrototypeUINotify.Notify((attacker == PrototypeCombat.defender ? "Attacker" : "Defender") + " took " + damageDone.Value + " damage.");
                        defender.RecieveDamage(damageDone.Value);

                    }

                }

            }

            Callbacks.AttackEnd.Invoke(attacker, defender);

        }

        public static void EndCombatEarly(){
            doEndCombatEarly = true;
        }



        public static int GetAttacksDone(PrototypeCharacter character){
            if(character == attacker){
                return attackerAttacks;
            }
            else if(character == defender){
                return defenderAttacks;
            }
            return 0;
        }

    }

}