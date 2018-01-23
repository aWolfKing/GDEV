using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace aWolfKing.WinControl.MonoBehaviour {

    public class AutoDeviceUpdater : UnityEngine.MonoBehaviour {
        #if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        private void Update() {
            if (InputManager.Settings.AutomaticDeviceUpdate) {
                InputManager.UpdateAllDevices();
            }
        }
        #endif
    }

}