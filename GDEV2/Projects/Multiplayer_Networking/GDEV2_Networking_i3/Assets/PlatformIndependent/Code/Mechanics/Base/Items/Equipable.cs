using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class Equipable : OwnedObject, IEquipable {

        public class OnEquipCallback : Callback<object>{

        }

        public class OnUnEquipCallback : Callback<object>{

        }



        private OnEquipCallback onEquip = new OnEquipCallback();
        public OnEquipCallback OnEquip{
            get{
                return this.onEquip;
            }
        }

        private OnUnEquipCallback onUnEquip = null;
        public OnUnEquipCallback OnUnEquip{
            get{
                return this.onUnEquip;
            }
        }



        public virtual bool CanBeEquiped(object by) {
            return true;
        }

        public bool CanBeUnEquipped(object by) {
            return true;
        }


        public virtual void Equip(object by) {
            this.OnEquip(by);
        }

        public virtual void UnEquip(object by) {
            this.OnUnEquip(by);
        }

    }

}
