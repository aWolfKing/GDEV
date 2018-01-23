using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using aWolfKing.WinControl;

public class DualShock4Profile : WinControlDeviceProfile {

    private string information      = @"
                                            Device name: 'Dualshock4'
                                            VID: '0x054C'
                                            PID: '0x05C4'
                                            Description: 'Dualshock4 controller'
                                       "
                                    ;

    private string wiredProfile     = @"
                                            OverallOffset = -1
                                            Axes:
                                                LX, LeftJoystickX: joy(1,0,255)
                                                LY, LeftJoystickY: joy(2,0,255) [inverted]
                                                RX, RightJoystickX: joy(3,0,255)
                                                RY, RightJoystickY: joy(4,0,255)

                                                L2, LeftTrigger: trig(8,0,255)
                                                R2, RightTrigger: trig(9,0,255)


                                            Buttons:
                                                Action0, Square: 5.4
                                                Action1, Cross: 5.5
                                                Action2, Circle: 5.6
                                                Action3, Triangle: 5.7

                                                L3, LeftJoystick: 6.6
                                                R3, RightJoystick: 6.7

                                                Options, Start: 6.5
                                                Share: 6.4

                                                L2, LeftTrigger: 6.2
                                                R2, RightTrigger: 6.3
                                                
                                                L1, LeftBumper: 6.0
                                                R1, RightBumper: 6.1

                                                Trackpad, Touchpad: 7.1
                                                PS, Home: 7.0

                                                TrackPad1, Touchpad1, TouchScreen1, Touch1, Touch: 35.7 [inverted]
                                                TrackPad2, Touchpad2, TouchScreen2, Touch2: 39.7 [inverted]
                                                

                                            Other:
                                                BatteryLevel: 12

                                                AccelZ
                                                AccelY
                                                AccelX

                                                GyroX: =GyroX
                                                GyroY: =GyroY
                                                GyroZ: =GyroZ

                                                TrackPad1X, Touchpad1X, TouchScreen1X, Touch1X, TouchX: =TrackPad1X
                                                TrackPad1Y, Touchpad1Y, TouchScreen1Y, Touch1Y, TouchY: =TrackPad1Y
                                                TrackPad2X, Touchpad2X, TouchScreen2X, Touch2X: =TrackPad2X
                                                TrackPad2Y, Touchpad2Y, TouchScreen2Y, Touch2Y: =TrackPad2Y
                                                                                                
                                                DpadX, DPadX: =DpadX
                                                DpadY, DPadY: =DpadY


                                       "
                                    ;

    private string bluetoothProfile = @"
                                                Axes:
                                                Buttons:
                                                Other:
                                       "
                                    ;



    public override string Information      { get { return information      ; } }
    public override string WiredProfile     { get { return wiredProfile     ; } }
    public override string BluetoothProfile { get { return bluetoothProfile ; } }

    public static int WiredByteOffset { get{ return -1; } }


    #region Specific calculations

    public static float TrackPad1X(byte[] inp){ //byte bit1, byte bit2) {      //=trig(36 + ((37 & 0xF)*255),0,1912)
        //return InputStruct.Trigger(bit1 + ((bit2 & 0xF) * 255), 0, 1912);
        return InputStruct.Trigger(inp[36+WiredByteOffset] + ((inp[37+WiredByteOffset] & 0xF) * 255), 0, 1912);
    }

    public static float TrackPad1Y(byte[] inp){ //(byte bit1, byte bit2) {      //=trig((37 & 0xF0) >> 4) +(bytes[38] * 16,0,916)
        //return InputStruct.Trigger(((bit1 & 0xF0) >> 4) + (bit2 * 16), 0, 916);
        return InputStruct.Trigger(((inp[37+WiredByteOffset] & 0xF0) >> 4) + (inp[38+WiredByteOffset] * 16), 0, 916);
    }

    public static float TrackPad2X(byte[] inp){ //(byte bit1, byte bit2) {     //=trig(40 + ((41 & 0xF) * 255),0,1912)
        //return InputStruct.Trigger(bit1 + ((bit2 & 0xF) * 255), 0, 1912);
        return InputStruct.Trigger(inp[40+WiredByteOffset] + ((inp[41+WiredByteOffset] & 0xF) * 255), 0, 1912);
    }

    public static float TrackPad2Y(byte[] inp){ //(byte bit1, byte bit2) {     //=trig(41 & 0xF0) >> 4) + (42 * 16,0,916)
        //return InputStruct.Trigger(((bit1 & 0xF0) >> 4) +(bit2 * 16), 0, 916);
        return InputStruct.Trigger(((inp[41+WiredByteOffset] & 0xF0) >> 4) + (inp[42+WiredByteOffset] * 16), 0, 916);
    }


    public static float DpadX(byte[] inp) {
        // inp[byte] & 00001111 //is laatste 4 bits
        int val = ((inp[5 + WiredByteOffset] & 00000111));
        bool left = val <= 7 && val >= 5;
        bool right = val <= 3 && val >= 1;
        return (right ? 1:0)-(left ? 1:0);
        //return val;
    }

    public static float DpadY(byte[] inp) {
        int val = ((inp[5 + WiredByteOffset] & 00000111));
        bool up = val == 7 || val == 1 || val == 0;
        bool down = val <= 5 && val >= 3;
        return (up ? 1 : 0) - (down ? 1 : 0);
        //return val;
    }


    public static float GyroX(byte[] inp){
        return -1 * (short)((ushort)(inp[20 + WiredByteOffset] << 8) | inp[21 + WiredByteOffset]) / 64;
    }

    public static float GyroY(byte[] inp) {
        return (short)((ushort)(inp[22 + WiredByteOffset] << 8) | inp[23 + WiredByteOffset]) / 64;
    }

    public static float GyroZ(byte[] inp) {
        return (short)((ushort)(inp[24 + WiredByteOffset] << 8) | inp[25 + WiredByteOffset]) / 64;
    }


    #endregion


}
