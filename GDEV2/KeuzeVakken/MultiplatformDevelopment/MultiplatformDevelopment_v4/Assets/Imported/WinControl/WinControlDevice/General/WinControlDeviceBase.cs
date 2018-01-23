using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace aWolfKing.WinControl {

    public abstract class WinControlDeviceBase {

        public abstract InputStruct this[string inputName] { get; }
        public virtual InputStruct GetInput(string inputName) { return this[inputName]; }
        public abstract InputStruct GetAxis(string inputName);
        public abstract InputStruct GetButton(string inputName);

        public abstract void SetInput(List<InputStruct> input);
        public abstract void UpdateInput();

    }

}