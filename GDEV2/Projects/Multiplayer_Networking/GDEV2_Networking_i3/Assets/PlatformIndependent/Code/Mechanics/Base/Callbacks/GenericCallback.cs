using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public class Callback<T0> : Callback {

        public new delegate void Method(T0 t0);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0){
            base.op_FunctionCall(t0);
        }

        public void Invoke(TargetFilter filter, T0 t0){
            base.op_FunctionCall(filter, t0);
        }

    }

    public class Callback<T0, T1> : Callback {

        public new delegate void Method(T0 t0, T1 t1);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1){
            base.op_FunctionCall(t0, t1);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1) {
            base.op_FunctionCall(filter, t0, t1);
        }

    }

    public class Callback<T0, T1, T2> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2){
            base.op_FunctionCall(t0, t1, t2);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2) {
            base.op_FunctionCall(filter, t0, t1, t2);
        }

    }

    public class Callback<T0, T1, T2, T3> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3){
            base.op_FunctionCall(t0, t1, t2, t3);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3) {
            base.op_FunctionCall(filter, t0, t1, t2, t3);
        }

    }

    public class Callback<T0, T1, T2, T3, T4> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4){
            base.op_FunctionCall(t0, t1, t2, t3, t4);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4) {
            base.op_FunctionCall(filter, t0, t1, t2, t3, t4);
        }

    }

    public class Callback<T0, T1, T2, T3, T4, T5> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            base.op_FunctionCall(t0, t1, t2, t3, t4, t5);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            base.op_FunctionCall(filter, t0, t1, t2, t3, t4, t5);
        }

    }

    public class Callback<T0, T1, T2, T3, T4, T5, T6> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            base.op_FunctionCall(t0, t1, t2, t3, t4, t5, t6);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            base.op_FunctionCall(filter, t0, t1, t2, t3, t4, t5, t6);
        }

    }

    public class Callback<T0, T1, T2, T3, T4, T5, T6, T7> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            base.op_FunctionCall(t0, t1, t2, t3, t4, t5, t6, t7);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            base.op_FunctionCall(filter, t0, t1, t2, t3, t4, t5, t6, t7);
        }

    }

    public class Callback<T0, T1, T2, T3, T4, T5, T6, T7, T8> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            base.op_FunctionCall(t0, t1, t2, t3, t4, t5, t6, t7, t8);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            base.op_FunctionCall(filter, t0, t1, t2, t3, t4, t5, t6, t7, t8);
        }

    }

    public class Callback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : Callback {

        public new delegate void Method(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);

        public void Attach(Method method) {
            base.AttachDelegate(method);
        }

        public void Detach(Method method) {
            base.DetachDelegate(method);
        }

        public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            base.op_FunctionCall(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }

        public void Invoke(TargetFilter filter, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            base.op_FunctionCall(filter, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }

    }

}
