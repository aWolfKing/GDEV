using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {
   
    public class FloatChange : Changable.ValueChange<float>{

        public enum ChangeType{
            add,
            percentage
        }    



        private bool isPercentage = false;
        public bool IsPercentage{
            get{
                return this.isPercentage;
            }
            set{
                this.isPercentage = value;
            }
        }

        public float Value{
            get{
                return this.value;
            }
            set{
                this.value = value;
            }
        }



        public FloatChange(float value, object cause, ChangeType changeType) : base(value, cause){
            this.isPercentage = changeType == ChangeType.percentage;
        }
        
    }    

}
