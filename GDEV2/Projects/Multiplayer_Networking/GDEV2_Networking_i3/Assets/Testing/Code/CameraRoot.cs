using System;
using System.Collections.Generic;
using UnityEngine;


public class CameraRoot : MonoBehaviour {

    private static CameraRoot _this = null;

    public static new Transform transform{
        get{
            return ((MonoBehaviour)_this).transform;
        }
    }

    private void Awake() {
        _this = this;
    }

}

