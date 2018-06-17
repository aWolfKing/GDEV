using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class Callback {

        public delegate void Method();
        public delegate bool TargetFilter(object taget);



        private List<Delegate> onCall = null;



        public virtual void Attach(Method method){
            this.onCall.Add(method);
        }

        public virtual void Detach(Method method){
            if(this.onCall.Contains(method)){ 
                this.onCall.Remove(method);
            }
        }

        protected void AttachDelegate(Delegate method){
            this.onCall.Add(method);
        }

        protected void DetachDelegate(Delegate method){
            if(this.onCall.Contains(method)){ 
                this.onCall.Remove(method);
            }
        }



        public void Invoke(params object[] parameters){
            op_FunctionCall(parameters);
        }

        public void Invoke(TargetFilter filter, params object[] parameters){
            op_FunctionCall(filter, parameters);
        }



        /// <summary>
        /// Calls all delegates.
        /// </summary>
        /// <param name="parameters"></param>
        /// Named after "()" operator.
        internal void op_FunctionCall(params object[] parameters){
            if(parameters == null){
                parameters = new object[]{ };
            }
            foreach(var call in this.onCall){
                call.DynamicInvoke(parameters);
            }
        }

        /// <summary>
        /// Only calls the delegates where filter(delegate target) returns true.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="parameters"></param>
        /// Named after "()" operator.
        internal void op_FunctionCall(TargetFilter filter, params object[] parameters){
            if(parameters == null){
                parameters = new object[]{ };
            }
            foreach(var call in this.onCall){
                if(filter(call.Target)){
                    call.DynamicInvoke(parameters);
                }
            }
        }



        public Callback(){
            this.onCall = new List<Delegate>();
        }

    }

}
