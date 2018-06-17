using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class NamedIntStat : IntStat, INamedObject{

        private string name = null;
        private string shortName = null;
        private string description = null;



        public string Name{
            get{
                return this.name;
            }
        }

        public string ShortName{
            get{
                return this.shortName;
            }
        }

        public string Description{
            get{
                return this.description;
            }
        }
        


        public NamedIntStat(int value, string name, string shortName, string description) : base(value){
            this.name = name;
            this.shortName = shortName;
            this.description = description;
        }

        public NamedIntStat(int value, string name) : base(value){
            this.name = name;
            this.shortName = name.Substring(0, 3);
            this.description = "";
        }

    }

}
