using System;
using System.Collections.Generic;
using FE;
using Prototype.Skills;


namespace Prototype{ 

    public static class PrototypeSkills {

        private static Dictionary<int, Type> id_skill_pairs = new Dictionary<int, Type>(){
            { 1, typeof(Proto_DamageMirror_1) },
            { 2, typeof(Proto_HealthyStrenght_1) },
            { 3, typeof(Proto_DoubleCounter_1) },
        };



        public static PrototypeSkill GetSkillFromID(int id){
            if(id_skill_pairs.ContainsKey(id)){
                return Activator.CreateInstance(id_skill_pairs[id]) as PrototypeSkill;
            }
            return null;
        }

    }
    
}