using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public interface INamedObject {

        string Name{ get; }
        string ShortName{ get; }
        string Description{ get; }

    }

}
