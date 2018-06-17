using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class IUsable_static{

        public static void OnUse(this IUsable usable, object by){
            usable.OnUse.op_FunctionCall(by);
        }

    }



    public interface IUsable {

        Usable.OnUseCallback OnUse{ get; }

        bool CanBeUsed(object by);

        void Use(object by);

    }

}
