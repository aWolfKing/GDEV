using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class Changable{

        public abstract class ValueChange{

        }



        public class ValueChange<T> : ValueChange{

            protected T value;
            protected object cause;

            public ValueChange(T change, object cause){
                this.value = change;
                this.cause = cause;
            }

        }

    }



    public abstract class Changable<T, C> where C : Changable.ValueChange {

        protected T value;
        protected List<C> changes = new List<C>();

        protected abstract T CalculateTotal();

    }

}
