using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FE {

    public static class DefaultUnitClasses {

        [System.AttributeUsage(AttributeTargets.Property)]
        private class DefaultClassID : System.Attribute{
            private int m_id = -1;
            public int ID{
                get{
                    return this.m_id;
                }
            }
            public DefaultClassID(int id){
                this.m_id = id;
            }
        }



        private static PropertyInfo[] properties = null;



        [DefaultClassID(0)]
        public static UnitClass Villager{
            get{
                UnitClass.Builder builder = new UnitClass.Builder();

                builder.SetNames("Villager", "Villager");
                builder.SetDescription("A unit that hasn't made its choice of class jet.");
                builder.SetMinimumBaseStats(7, 5, 2, 2, 4, 10, 4, 3, 6);
                builder.SetMaximumBaseStats(25, 20, 19, 21, 18, 24, 17, 14, 6);
                builder.SetStatGrowths(20, 11, 8, 13, 15, 14, 10, 8);

                builder.SetClassSkills(new Skills.EnthousiasticLearning());

                return builder.Build();
            }
        }



        public static UnitClass GetClassFromID(int id){
            if(properties == null){
                var tmp_properties = new List<PropertyInfo>(typeof(DefaultUnitClasses).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
                for(int i=tmp_properties.Count-1; i>=0; i--){
                    var idAttribute = tmp_properties[i].GetCustomAttributes(typeof(DefaultClassID), true);
                    if(idAttribute == null || idAttribute.Length == 0){
                        tmp_properties.RemoveAt(i);
                    }
                }
                properties = tmp_properties.ToArray();
            }
            foreach(var property in properties){
                var idAttribute = property.GetCustomAttributes(typeof(DefaultClassID), true)[0] as DefaultClassID;
                if(idAttribute.ID == id){
                    return property.GetValue(null, null) as UnitClass;
                }
            }
            return null;
        }

    }

}
