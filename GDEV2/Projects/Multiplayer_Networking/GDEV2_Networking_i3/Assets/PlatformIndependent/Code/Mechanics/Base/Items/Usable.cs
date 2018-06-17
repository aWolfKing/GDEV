using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class Usable : OwnedObject, IUsable {

        public class OnUseCallback : Callback<object>{

        }



        private OnUseCallback onUse = new OnUseCallback();
        public OnUseCallback OnUse{
            get{
                return this.onUse;
            }
        }



        public virtual bool CanBeUsed(object by) {
            return true;
        }

        public virtual void Use(object by) {
            this.OnUse(by);
        }

    }

}
