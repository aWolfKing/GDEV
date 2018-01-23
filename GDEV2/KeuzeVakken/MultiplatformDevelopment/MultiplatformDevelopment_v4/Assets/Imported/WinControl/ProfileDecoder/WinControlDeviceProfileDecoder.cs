using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using HidLibrary;

namespace aWolfKing.WinControl {

    public class WinControlDeviceProfileDecoder {

        public static IEnumerable<WinControlDeviceProfile> GetWinControlDeviceProfiles(){
            System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            for(int t=0; t<types.Length; t++){
                if(types[t].BaseType == typeof(WinControlDeviceProfile)){
                    //MonoBehaviour.print(types[t].Name);
                    yield return (WinControlDeviceProfile)System.Activator.CreateInstance(types[t]);
                }
            }
        }

        public static SingleWinControlDevice DecodeProfile(WinControlDeviceProfile profile){
            SingleWinControlDevice device = null;
            Dictionary<string, string> info = DecodeInformation(profile);
            if(info.ContainsKey("vid") && info.ContainsKey("pid")){
                string vid, pid;
                //MonoBehaviour.print(info["vid"] + ", " + info["pid"]);
                try {
                    //if(info["vid"][1] == 'x') { vid = int.Parse(info["vid"].Substring(2), System.Globalization.NumberStyles.HexNumber); }
                    //else{ vid = int.Parse(info["vid"]); }
                    //if (info["pid"][1] == 'x') { pid = int.Parse(info["pid"].Substring(2), System.Globalization.NumberStyles.HexNumber); }
                    //else { pid = int.Parse(info["pid"]); }
                    string name = "nameless", desc = "no description";
                    if(info.ContainsKey("description")){ desc = info["description"]; }
                    if(info.ContainsKey("devicename")){ name = info["devicename"]; }
                    vid = info["vid"];
                    pid = info["pid"];
                    device = new SingleWinControlDevice(null, vid, pid, desc, name, profile.GetType());
                }catch{ Debug.LogError("VID and/or PID was not the correct datatype (should be int)"); }
            }
            else{
                Debug.LogError("VID and/or PID not found in profile: " + (info.ContainsKey("devicename") ? info["devicename"] : "<nameless>"));
            }
            return device;
        }
        
        private static Dictionary<string, string> DecodeInformation(WinControlDeviceProfile profile){
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string[] info = profile.Information.Split('\n');
            for(int i=0; i<info.Length; i++){
                string prnt = info[i];
                //prnt = prnt.Replace(" ", "");
                while(prnt.StartsWith(" ")){ prnt = prnt.Substring(1); }
                string[] splitted = prnt.Split(':');
                if(splitted.Length > 1 && splitted[1].Length > 0){
                    while (splitted[1].StartsWith(" ")) { splitted[1] = splitted[1].Substring(1); }
                    ret.Add(splitted[0].ToLower().Replace(" ",""), splitted[1].Replace("'", ""));
                }
            }
            return ret;
        }

        private static IEnumerable<InputStruct> DecodeWiredInput(WinControlDeviceProfile profile){
            string[] inp_ = profile.WiredProfile.Split('\n');
            string title = "none";
            int offset = 0;
            for(int i=0; i<inp_.Length; i++){
                if (inp_[i].ToLower().Replace(" ", "").StartsWith("overalloffset=")) {
                    try{
                        offset = int.Parse(inp_[i].ToLower().Replace(" ", "").Split('=')[1]);
                    }
                    catch{ }
                }
                else{
                    string[] inp = inp_[i].Replace(" ", "").Split(':');
                    if (inp.Length > 1 && !(inp[0].ToLower().Contains("buttons") || inp[0].ToLower().Contains("axes") || inp[0].ToLower().Contains("other"))) {

                        string[] names = inp[0].Split(',');
                        System.Func<byte[],float> calculation = null;
                        InputStruct.InputType inputType = InputStruct.InputType.axis;
                        InputStruct.CalculationType calculationType = InputStruct.CalculationType.none;
                        int _byte = 0;
                        int _bit = -1;

                        float calcMin = 0; 
                        float calcMax = 255;
                        bool invert = false;

                        if(title == "buttons"){ inputType = InputStruct.InputType.button; }
                        else if(title == "axes"){ inputType = InputStruct.InputType.axis; }
                        else if(title == "other"){ inputType = InputStruct.InputType.other; }


                        if (inp[1].Contains("[") && inp[1].Contains("]")) {
                            string inpString = "";
                            string tags = "";
                            bool inTags = false;
                            for(int o=0; o<inp[1].Length; o++){
                                if(inp[1][o] == '['){ inTags = true; }
                                else if(inp[1][o] == ']'){ inTags = false; }
                                if (inTags) { tags += inp[1][o]; }
                                else if(inp[1][o] != '[' && inp[1][o] != ']'){
                                    inpString += inp[1][o];
                                }
                            }
                            tags = tags.Replace("[", "").Replace("]", "");
                            string[] t = tags.Split(',');
                            for(int j=0; j<t.Length; j++){
                                if(t[j].ToLower() == "inverted"){
                                    invert = true;
                                }
                            }
                            inp[1] = inpString;
                        }


                        if (inp[1].Contains("=")) {
                            calculation = FindDelegate(profile, inp[1]);
                        }
                        if (inp[1].Contains("joy")){
                            string[] split = inp[1].Replace("joy","").Replace("(","").Replace(")","").Split(',');
                            if (split.Length == 3) {
                                try{
                                    _byte = int.Parse(split[0]);
                                    calcMin = float.Parse(split[1]);
                                    calcMax = float.Parse(split[2]);
                                    calculationType = InputStruct.CalculationType.joystick;
                                }
                                catch{ }
                            }
                        }
                        if (inp[1].Contains("trig")){
                            string[] split = inp[1].Replace("trig", "").Replace("(", "").Replace(")", "").Split(',');
                            if (split.Length == 3) {
                                try {
                                    _byte = int.Parse(split[0]);
                                    calcMin = float.Parse(split[1]);
                                    calcMax = float.Parse(split[2]);
                                    calculationType = InputStruct.CalculationType.trigger;
                                }
                                catch { }
                            }
                        }
                        else {

                            if (inp[1].Contains(".")) {
                                try {
                                    string[] split = inp[1].Split('.');
                                    _byte = int.Parse(split[0]);
                                    _bit = int.Parse(split[1]);
                                }
                                catch { }
                            }
                            else{
                                try{
                                    _byte = int.Parse(inp[1]);
                                }
                                catch{ }
                            }

                        }

                        /*
                        if(inp[1].Contains("=trig(")){
                            try{
                                calculationType = InputStruct.CalculationType.trigger;
                            }
                            catch{ }
                        }
                        else if(inp[1].Contains("=joy(")){
                            try{

                                calculationType = InputStruct.CalculationType.joystick;
                            }
                            catch{ }
                        }
                        */

                        _bit = Mathf.Clamp(_bit, -1, 7);

                        //MonoBehaviour.print(inputType + ", " + calculationType + ", " + (_byte + offset) + ", " + _bit + ", " + calculation + ", " + names[0]);

                        yield return new InputStruct(inputType, calculationType, _byte + offset, _bit, calcMin, calcMax, invert, calculation, names);

                    }
                    else {
                        if (inp[0].ToLower() == "buttons" || inp[0].ToLower() == "axes" || inp[0].ToLower() == "other") {
                            title = inp[0].ToLower();
                        }
                    }
                }
            }
        }


        public static IEnumerable<HidDevice> GetHIDDevicesFromProfile(WinControlDeviceProfile profile){
            List<HidDevices.DeviceInfo> connectedDevices = new List<HidDevices.DeviceInfo>(HidDevices.EnumerateDevices());
            SingleWinControlDevice winControlDevice = DecodeProfile(profile);
            for(int i=0; i<connectedDevices.Count; i++){
                HidDevice hidDevice = HidDevices.GetDevice(connectedDevices[i].Path);
                if(hidDevice.Attributes.VendorHexId.ToString() == winControlDevice.VID && hidDevice.Attributes.ProductHexId.ToString() == winControlDevice.PID){
                    yield return hidDevice;
                }
            }
        }

        public static IEnumerable<SingleWinControlDevice> GetDevicesFromProfile(WinControlDeviceProfile profile) {
            List<HidDevices.DeviceInfo> connectedDevices = new List<HidDevices.DeviceInfo>(HidDevices.EnumerateDevices());
            SingleWinControlDevice winControlDevice = DecodeProfile(profile);
            for (int i = 0; i < connectedDevices.Count; i++) {
                HidDevice hidDevice = HidDevices.GetDevice(connectedDevices[i].Path);
                if (hidDevice.Attributes.VendorHexId.ToString() == winControlDevice.VID && hidDevice.Attributes.ProductHexId.ToString() == winControlDevice.PID) {
                    SingleWinControlDevice device = new SingleWinControlDevice(hidDevice, winControlDevice.VID, winControlDevice.VID, winControlDevice.Description, winControlDevice.Name, profile.GetType());
                    List<InputStruct> input = new List<InputStruct>(DecodeWiredInput(profile));
                    device.SetInput(input);
                    yield return device;
                }
            }
        }



        private static System.Func<byte[],float> FindDelegate(WinControlDeviceProfile profile, string val){
            val = val.Replace(" ", "");
            string funcName = "";
            bool foundName = false;
            for(int i=0;i<val.Length; i++){
                if (val[i] != '=') {
                    if (val[i] == '(') {
                        break;
                    }
                    else if (!foundName) {
                        funcName += val[i];
                    }
                }
            }
            //MonoBehaviour.print(funcName + "\n" + val.Substring(funcName.Length+1));

            MethodInfo info = profile.GetType().GetMethod(funcName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            ParameterInfo[] p = info.GetParameters();
            if (p.Length == 1 && p[0].ParameterType == typeof(byte[])){
                try {
                    return (System.Func<byte[], float>)System.Func<byte[], float>.CreateDelegate(typeof(System.Func<byte[], float>), info);
                }
                catch{ }
            }

            return null;
        }



    }

}