using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class IntStat : CInt{

        private CInt baseValue = new CInt(0);



        public CInt BaseStat{
            get{
                return this.baseValue;
            }
        }



        public new int BaseValue{
            get{
                return this.baseValue.Value;
            }
        }



        protected override int CalculateTotal() {
            this.value = this.baseValue.Value;
            return base.CalculateTotal();
        }



        public IntStat(int value){
            this.baseValue = new CInt(value);
            this.value = 0;
        }

    }

}
