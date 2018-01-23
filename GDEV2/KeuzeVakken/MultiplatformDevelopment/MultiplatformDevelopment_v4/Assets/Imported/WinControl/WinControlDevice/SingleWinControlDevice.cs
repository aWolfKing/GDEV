using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HidLibrary;

namespace aWolfKing.WinControl {

    public class SingleWinControlDevice : WinControlDeviceBase {

        private string  vid         = "";   public string VID           { get{ return vid           ; } }
        private string  pid         = "";   public string PID           { get{ return pid           ; } }
        private string  description = "";   public string Description   { get{ return description   ; } }
        private string  name        = "";   public string Name          { get{ return name          ; } }

        private List<InputStruct> inputStructs = new List<InputStruct>();
        private HidDevice hidDevice = null; public HidDevice HidDevice  { get{ return hidDevice     ; } }

        private System.Type profile;        public System.Type Profile  { get{ return profile       ; } }


        public SingleWinControlDevice(HidDevice hidDevice, string vid, string pid, string description, string name, System.Type profile){
            this.hidDevice = hidDevice;
            this.vid = vid;
            this.pid = pid;
            this.description = description;
            this.name = name;
            this.profile = profile;
        }

        public override InputStruct this[string inputName] {
            get {
                for(int i=0; i<inputStructs.Count; i++){
                    for(int o=0; o<inputStructs[i].Names.Length; o++){
                        if(inputName == inputStructs[i].Names[o]){
                            return inputStructs[i];
                        }
                    }
                }
                return null;
            }
        }

        public override InputStruct GetAxis(string inputName) {
            for (int i = 0; i < inputStructs.Count; i++) {
                for (int o = 0; o < inputStructs[i].Names.Length; o++) {
                    if (inputName == inputStructs[i].Names[o] && inputStructs[i].TypeOfInput == InputStruct.InputType.axis) {
                        return inputStructs[i];
                    }
                }
            }
            return null;
        }

        public override InputStruct GetButton(string inputName) {
            for (int i = 0; i < inputStructs.Count; i++) {
                for (int o = 0; o < inputStructs[i].Names.Length; o++) {
                    if (inputName == inputStructs[i].Names[o] && inputStructs[i].TypeOfInput == InputStruct.InputType.button) {
                        return inputStructs[i];
                    }
                }
            }
            return null;
        }



        public override void SetInput(List<InputStruct> input) {
            inputStructs = input;
        }


        public override void UpdateInput() {
            byte[] bytes = hidDevice.ReadReport().Data;
            for(int i=0; i<inputStructs.Count; i++){
                if(inputStructs[i].Byte < bytes.Length){
                    inputStructs[i].Value = inputStructs[i].CalculateValue(bytes);
                }
            }
        }
    }

}
