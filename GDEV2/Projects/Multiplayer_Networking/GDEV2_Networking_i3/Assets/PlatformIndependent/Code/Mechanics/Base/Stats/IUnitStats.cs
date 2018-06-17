using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class IUnitStats_static {

        public static NamedIntStat GetUnitStat(this IUnitStats unitStats, UnitStats.Stat stat){
            switch(stat){
                case UnitStats.Stat.Health:
                    return unitStats.Health;
                case UnitStats.Stat.Strenght:
                    return unitStats.Strenght;
                case UnitStats.Stat.Magic:
                    return unitStats.Magic;
                case UnitStats.Stat.Skill:
                    return unitStats.Skill;
                case UnitStats.Stat.Speed:
                    return unitStats.Speed;
                case UnitStats.Stat.Luck:
                    return unitStats.Luck;
                case UnitStats.Stat.Defence:
                    return unitStats.Defence;
                case UnitStats.Stat.Resistance:
                    return unitStats.Resistance;
                case UnitStats.Stat.Movement:
                    return unitStats.Movement;
            }
            return null;
        }

    }



    public interface IUnitStats {

        NamedIntStat Health{ get; }
        NamedIntStat Strenght{ get; }
        NamedIntStat Magic{ get; }
        NamedIntStat Skill{ get; }
        NamedIntStat Speed{ get; }
        NamedIntStat Luck{ get; }
        NamedIntStat Defence{ get; }
        NamedIntStat Resistance{ get; }
        NamedIntStat Movement{ get; }

    }

}
