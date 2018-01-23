using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace aWolfKing.WinControl {

    public class InputStruct {

        public enum InputType{ axis, button, other}
        private InputType inputType = InputType.axis;                   public InputType TypeOfInput{ get{ return inputType; } }
        public enum CalculationType{ none, joystick, trigger}
        private CalculationType calculationType = CalculationType.none;
        private float deadZone = 0.1f;

        private float value;    public float Value{ get{ return value; }set{ this.value = value; } }
        private int m_byte;     public int Byte{ get{ return m_byte; } }
        private int bit;        public int Bit{ get{ return bit; } }
        private float calcMin;  public float CalcMin{ get{ return calcMin; } }
        private float calcMax;  public float CalcMax{ get{ return calcMax; } }
        private string[] names; public string[] Names{ get{ return names; } }
        private bool inverted;  public bool Inverted{ get{ return inverted; }set{ inverted = value; } }

        private System.Func<byte[],float> calculation = null;


        public InputStruct(InputType inputType, CalculationType calculationType, int _byte, int bit, /*int[] bits,*/ System.Func<byte[],float> calc, params string[] names){
            this.inputType = inputType;
            this.calculationType = calculationType;
            this.m_byte = _byte;
            //this.bits = bits;
            this.bit = bit;
            this.calculation = calc;
            this.names = names;
        }

        public InputStruct(InputType inputType, CalculationType calculationType, int _byte, int bit, float calcMin, float calcMax, /*int[] bits,*/ System.Func<byte[], float> calc, params string[] names) {
            this.inputType = inputType;
            this.calculationType = calculationType;
            this.m_byte = _byte;
            //this.bits = bits;
            this.bit = bit;
            this.calculation = calc;
            this.names = names;
            this.calcMin = calcMin;
            this.calcMax = calcMax;
        }

        public InputStruct(InputType inputType, CalculationType calculationType, int _byte, int bit, float calcMin, float calcMax, bool invert, System.Func<byte[], float> calc, params string[] names) {
            this.inputType = inputType;
            this.calculationType = calculationType;
            this.m_byte = _byte;
            //this.bits = bits;
            this.bit = bit;
            this.calculation = calc;
            this.names = names;
            this.calcMin = calcMin;
            this.calcMax = calcMax;
            this.inverted = invert;
        }




        public static implicit operator float(InputStruct s){
            return Mathf.Abs(s.value) >= s.deadZone ? s.value : 0;
        }

        public static implicit operator bool(InputStruct s){
            return s.value >= s.deadZone;
        }

        public float CalculateValue(byte[] inp){
            if (calculation != null) {
                value = calculation(inp);
                if(inverted){
                    switch(inputType){
                        case InputType.axis:
                            value *= -1;
                            break;
                        case InputType.button:
                            value = 1 - value;
                            break;
                    }
                }
            }
            else{
                if (calculationType == CalculationType.none) {
                    value = (bit >= 0 ? BitFromByte(inp[this.Byte], bit) : inp[this.Byte]);
                }
                else if(calculationType == CalculationType.joystick){
                    value = JoyStick(inp[this.Byte], calcMin, calcMax);
                }
                else if(calculationType == CalculationType.trigger){
                    value = Trigger(inp[this.Byte], calcMin, calcMax);
                }

                if (inverted) {
                    switch (inputType) {
                        case InputType.axis:
                            value *= -1;
                            break;
                        case InputType.button:
                            value = 1 - value;
                            break;
                    }
                }

            }
            return value;
        }


        public static float Trigger(float value, float min, float max){
            return Mathf.Clamp(1 / (max - min) * value, -1, 1);
        }

        public static float JoyStick(float value, float min, float max){
            return Mathf.Clamp(((1 / (max-min)*value) - 0.5f)*2, -1, 1);
        }

        public static float BitFromByte(byte b, int bit){
            return ((b & (1 << bit)) != 0) ? 1 : 0;
        }

    }

}