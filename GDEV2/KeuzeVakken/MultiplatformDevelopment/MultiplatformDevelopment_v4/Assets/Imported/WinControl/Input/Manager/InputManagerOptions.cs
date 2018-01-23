using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InputManager{

    public static partial class Settings{
        private static bool autoUpdate = true; public static bool AutomaticDeviceUpdate{ get{ return autoUpdate; }set{ autoUpdate = value; } }
    }

}
