using System;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationDelegates {
    public delegate void Method();
}

public class CommunicationDelegates<T> {
    public delegate void Method(T p0);
}

public class CommunicationDelegates<T0, T1> {
    public delegate void Method(T0 p0, T1 p1);
}

public class CommunicationDelegates<T0, T1, T2> {
    public delegate void Method(T0 p0, T1 p1, T2 p2);
}

public class CommunicationDelegates<T0, T1, T2, T3> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3);
}

public class CommunicationDelegates<T0, T1, T2, T3, T4> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4);
}

public class CommunicationDelegates<T0, T1, T2, T3, T4, T5> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
}

public class CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
}

public class CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6, T7> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);
}

public class CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6, T7, T8> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);
}

public class CommunicationDelegates<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> {
    public delegate void Method(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9);
}