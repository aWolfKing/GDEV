using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using aWolfKing.WinControl;

public static partial class InputManager {

    private static bool isSetup = false;

    private static List<SingleWinControlDevice> devices = new List<SingleWinControlDevice>();
    private static SingleWinControlDevice mainController = null;
    public static SingleWinControlDevice MainController{ 
        get{
            #if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            if(!isSetup){ SetupAllDevices(); isSetup = true; }
            if(mainController == null && devices.Count > 0){ mainController = devices[0]; }
            return mainController;
            #else
            return null;
            #endif
        } 
        set{ mainController = value; }
    }


    #if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR

    public static void SetupAllDevices(/*string name*/){
        foreach(var a in WinControlDeviceProfileDecoder.GetWinControlDeviceProfiles()){
            SetupDevicesWithProfile(a);
        }
        if(GameObject.FindObjectOfType<aWolfKing.WinControl.MonoBehaviour.AutoDeviceUpdater>() == null){
            GameObject obj = new GameObject("InputManager automatic device-updater");
            obj.AddComponent<aWolfKing.WinControl.MonoBehaviour.AutoDeviceUpdater>();
        }
    }


    public static void SetupDevicesFromDeviceName(string name){
        foreach(var a in WinControlDeviceProfileDecoder.GetWinControlDeviceProfiles()){
            SetupDevicesWithProfile(a);
        }
    }

    public static void SetupDevicesWithProfile(WinControlDeviceProfile profile){
        List<SingleWinControlDevice> d = new List<SingleWinControlDevice>(WinControlDeviceProfileDecoder.GetDevicesFromProfile(profile));
        foreach (var _d in d) {
            bool alreadyExisted = false;
            for (int i = 0; i < devices.Count; i++) {
                if (devices[i].HidDevice == _d.HidDevice){
                    alreadyExisted = true;
                    break;
                }
            }
            if(!alreadyExisted){
                devices.Add(_d);
            }
        }
    }


    public static IEnumerable<SingleWinControlDevice> GetDevicesFromDeviceName(string name) {
        foreach (var a in WinControlDeviceProfileDecoder.GetWinControlDeviceProfiles()) {
            foreach (var b in GetDevicesWithProfile(a)) {
                yield return b;
            }
        }
    }

    public static IEnumerable<SingleWinControlDevice> GetDevicesWithProfile(WinControlDeviceProfile profile){
        for(int i=0; i<devices.Count; i++){
            if(devices[i].Profile == profile.GetType()){
                yield return devices[i];
            }
        }
    }


    public static void UpdateAllDevices(){
        for(int i=0; i<devices.Count; i++){
            devices[i].UpdateInput();
        }
    }


    public static SingleWinControlDevice GetDeviceFromIndex(int index){
        if(!isSetup){ SetupAllDevices(); isSetup = true; }
        if(devices.Count > index){
            return devices[index];
        }
        return null;
    }


    #endif

}
