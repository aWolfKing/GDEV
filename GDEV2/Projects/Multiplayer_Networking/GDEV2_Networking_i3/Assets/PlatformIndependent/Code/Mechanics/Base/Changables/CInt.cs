using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class CInt_static{

        public static void OnChangeAdded(this CInt cInt, CInt.FloatChange change){
            cInt.OnChangeAdded.op_FunctionCall(change);
        }

        public static void OnChangeRemoved(this CInt cInt, CInt.FloatChange change){
            cInt.OnChangeRemoved.op_FunctionCall(change);
        }

    }



    public class CInt : Changable<int, CInt.FloatChange>{

        public class FloatChange : FE.FloatChange{

            public enum PercentageAddTo{
                baseValue,
                addedValue,
                baseValuePlusAddedValue,
                /// <summary>
                /// Not recommended, because the order of changes added can drastically influence the calculated value.
                /// </summary>
                totalValue
            }



            private PercentageAddTo percentageAddTo = PercentageAddTo.baseValuePlusAddedValue;
            public PercentageAddTo AddPercentageTo{
                get{
                    return this.percentageAddTo;
                }
                set{
                    this.percentageAddTo = value;
                }
            }



            public FloatChange(float value, object cause, ChangeType changeType) : base(value, cause, changeType) {

            }
            public FloatChange(float value, object cause, ChangeType changeType, PercentageAddTo percentageAddTo) : base(value, cause, changeType){
                this.percentageAddTo = percentageAddTo;
            }

        }



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



        public int BaseValue{
            get{
                return this.value;
            }
        }

        public int Value{
            get{
                return this.CalculateTotal();
            }
        }    
    


        internal static void SetBaseValue(CInt cInt, int value){
            cInt.value = value;
        }

        internal static int GetBaseValue(CInt cInt){
            return cInt.value;
        }



        protected override int CalculateTotal() {
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
                        case FloatChange.PercentageAddTo.baseValue:
                            total += baseValue * change.Value;
                            break;
                        case FloatChange.PercentageAddTo.addedValue:
                            total += addedValue * change.Value;
                            break;
                        case FloatChange.PercentageAddTo.baseValuePlusAddedValue:
                            total += (baseValue + addedValue) * change.Value;
                            break;
                        case FloatChange.PercentageAddTo.totalValue:
                            total += total * change.Value;
                            break;
                    }
                }
            }             
            

            return Math.RoundTowardsZero(total);
        }



        public void AddChange(FloatChange change){
            if(!this.changes.Contains(change)){ 
                this.changes.Add(change);
                this.OnChangeAdded(change);
            }
        }

        public void RemoveChange(FloatChange change){
            if(this.changes.Contains(change)){
                this.changes.Remove(change);
                this.OnChangeRemoved(change);
            }
        }



        public CInt(int value){
            this.value = value;
        }

        public CInt(){
            this.value = 0;
        }



        public static CInt operator + (CInt cInt, FloatChange change){
            cInt.AddChange(change);
            return cInt;
        }

        public static CInt operator - (CInt cInt, FloatChange change){
            cInt.RemoveChange(change);
            return cInt;
        }

    }

}
