using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace aWolfKing.WinControl {

    public abstract class WinControlDeviceProfile {

        public abstract string Information { get; }

        public abstract string WiredProfile { get; }
        public abstract string BluetoothProfile { get; }

    }

}