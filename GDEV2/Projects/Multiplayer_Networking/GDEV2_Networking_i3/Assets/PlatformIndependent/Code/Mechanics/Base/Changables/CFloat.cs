using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class CFloat_static{

        public static void OnChangeAdded(this CFloat cFloat, CInt.FloatChange change){
            cFloat.OnChangeAdded.op_FunctionCall(change);
        }

        public static void OnChangeRemoved(this CFloat cFloat, CInt.FloatChange change){
            cFloat.OnChangeRemoved.op_FunctionCall(change);
        }

    }    



    public class CFloat : Changable<float, CInt.FloatChange>{

        public class OnChangeAddedCallback : Callback<FloatChange>{ 
        
        }

        public class OnChangeRemovedCallback : Callback<FloatChange>{

        }



        private OnChangeAddedCallback onChangeAdded = new OnChangeAddedCallback();
        public OnChangeAddedCallback OnChangeAdded{
            get{
                return this.onChangeAdded;
            }
        }
        private OnChangeRemovedCallback onChangeRemoved = new OnChangeRemovedCallback();
        public OnChangeRemovedCallback OnChangeRemoved{
            get{
                return this.onChangeRemoved;
            }
        }



        public float BaseValue{
            get{
                return this.value;
            }
        }

        public float Value {
            get{
                return this.CalculateTotal();
            }
        }    
    


        internal static void SetBaseValue(CFloat cFloat, float value){
            cFloat.value = value;
        }

        internal static float GetBaseValue(CFloat cFloat){
            return cFloat.value;
        }



        protected override float CalculateTotal() {
            float baseValue = this.value;
            float total = this.value;

            ///Only changes that add a number are added here, percentages might need this.
            float addedValue = 0;

            foreach(var change in this.changes){
                if(!change.IsPercentage){
                    addedValue += change.Value;
                    total += change.Value;
                }
            }
            foreach(var change in this.changes){
                if(change.IsPercentage){
                    switch(change.AddPercentageTo){
                        case CInt.FloatChange.PercentageAddTo.baseValue:
                            total += baseValue * change.Value;
                            break;
                        case CInt.FloatChange.PercentageAddTo.addedValue:
                            total += addedValue * change.Value;
                            break;
                        case CInt.FloatChange.PercentageAddTo.baseValuePlusAddedValue:
                            total += (baseValue + addedValue) * change.Value;
                            break;
                        case CInt.FloatChange.PercentageAddTo.totalValue:
                            total += total * change.Value;
                            break;
                    }
                }
            }             
            

            return total;
        }



        public void AddChange(CInt.FloatChange change){
            if(!this.changes.Contains(change)){ 
                this.changes.Add(change);
                this.OnChangeAdded(change);
            }
        }

        public void RemoveChange(CInt.FloatChange change){
            if(this.changes.Contains(change)){
                this.changes.Remove(change);
                this.OnChangeRemoved(change);
            }
        }



        public CFloat(float value){
            this.value = value;
        }

        public CFloat(){
            this.value = 0;
        }



        public static CFloat operator + (CFloat cInt, CInt.FloatChange change){
            cInt.AddChange(change);
            return cInt;
        }

        public static CFloat operator - (CFloat cInt, CInt.FloatChange change){
            cInt.RemoveChange(change);
            return cInt;
        }

    }

}
