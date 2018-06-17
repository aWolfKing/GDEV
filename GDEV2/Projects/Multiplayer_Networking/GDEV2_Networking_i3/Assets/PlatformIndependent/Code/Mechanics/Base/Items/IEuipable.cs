using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class IEquipable_static{

        public static void OnEquip(this IEquipable equipable, object by){
            equipable.OnEquip.op_FunctionCall(by);
        }

        public static void OnUnEquip(this IEquipable equipable, object by){
            equipable.OnUnEquip.op_FunctionCall(by);
        }

    }



    public interface IEquipable {

        Equipable.OnEquipCallback OnEquip{ get; }
        Equipable.OnUnEquipCallback OnUnEquip{ get; }

        bool CanBeEquiped(object by);
        bool CanBeUnEquipped(object by);

        void Equip(object by);
        void UnEquip(object by);

    }

}
