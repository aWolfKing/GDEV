using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using aWolfKing.WinControl;

public static partial class InputManager {

    public static partial class Touch {

        public static bool GetTouch(out Vector2 position, int index, int indexOffset = 0) {
            position = new Vector2(0.5f, 0.5f);
            for (int i = 0; i < Input.touches.Length; i++) {
                if (Input.touches[i].fingerId == index) { position = new Vector2(1f / Screen.width * Input.touches[i].position.x, 1f / Screen.height * Input.touches[i].position.y); return true; }
            }
            if (InputManager.MainController != null) {
                InputStruct p = InputManager.MainController.GetInput("Touch" + (index + indexOffset));
                InputStruct x = InputManager.MainController.GetInput("Touch" + (index + indexOffset) + "X");
                InputStruct y = InputManager.MainController.GetInput("Touch" + (index + indexOffset) + "Y");
                if (x != null && y != null) {
                    position = new Vector2(x.Value, y.Value);
                    return (p != null ? (p.Value > 0) : (p != null));
                }
            }
            return false;
        }
        public static bool GetTouch(out Vector2 position, int index, SingleWinControlDevice device, int indexOffset = 0) {
            if(device == null){ return GetTouch(out position, index, indexOffset); }
            position = new Vector2(0.5f, 0.5f);
            InputStruct p = device.GetInput("Touch" + (index + indexOffset));
            InputStruct x = device.GetInput("Touch" + (index + indexOffset) + "X");
            InputStruct y = device.GetInput("Touch" + (index + indexOffset) + "Y");
            if (x != null && y != null) {
                position = new Vector2(x.Value, y.Value);
                return (p != null ? (p.Value > 0) : (p != null));
            }
            return false;
        }
        

        public static void GetPinchOpen(System.Action onPinch, float timeToPinch, float maxPinchDuration = 1.5f, System.Action onFailed = null) {
            GetPinchOpen(onPinch, timeToPinch, null, maxPinchDuration, onFailed);
        }
        public static void GetPinchOpen(System.Action onPinch, float timeToPinch, SingleWinControlDevice device, float maxPinchDuration = 1.5f, System.Action onFailed = null) {
            aWolfKing.WinControl.MonoBehaviour.CoroutineDummy.Instance.StartCoroutine(PinchCoroutine(onPinch, timeToPinch, device, maxPinchDuration, -1, onFailed));
        }
        
        public static void GetPinchClose(System.Action onPinch, float timeToPinch, float maxPinchDuration = 1.5f, System.Action onFailed = null) {
            GetPinchClose(onPinch, timeToPinch, null, maxPinchDuration, onFailed);
        }
        public static void GetPinchClose(System.Action onPinch, float timeToPinch, SingleWinControlDevice device, float maxPinchDuration = 1.5f, System.Action onFailed = null) {
            aWolfKing.WinControl.MonoBehaviour.CoroutineDummy.Instance.StartCoroutine(PinchCoroutine(onPinch, timeToPinch, device, maxPinchDuration, 1, onFailed));
        }


        private static IEnumerator PinchCoroutine(System.Action onPinch, float timeToPinch, SingleWinControlDevice device, float maxPinchDuration = 1.5f, int direction = 0 /* 1 = close, -1 = open */, System.Action onFailed = null) {
            float counter = 0;
            List<Vector2> f1 = new List<Vector2>();
            List<Vector2> f2 = new List<Vector2>();
            Vector2 pos1;
            Vector2 pos2;
            bool pressed = false;
            do {
                if(GetTouch(out pos1, 0, device, 1) && GetTouch(out pos2, 1, device, 1)){
                    pressed = true;
                    f1.Add(pos1);
                    f2.Add(pos2);
                    break; 
                }
                counter += Time.smoothDeltaTime;
                yield return new WaitForEndOfFrame();
            }
            while (counter < timeToPinch);
            if(pressed){

                counter = 0;
                do {
                    if (GetTouch(out pos1, 0, device, 1) && GetTouch(out pos2, 1, device, 1)) {
                        if(Vector2.Distance(f1[f1.Count - 1], pos1) >= 0.01f && Vector2.Distance(f2[f2.Count - 1], pos2) >= 0.01f){
                            f1.Add(pos1);
                            f2.Add(pos2);
                            Vector2 dir = (f2[0] - f1[0]).normalized * direction;
                            if(Vector2.Dot(dir, (f1[f1.Count-1]-f1[0]).normalized) <= 0 && Vector2.Dot(dir*-1, (f2[f2.Count - 1] - f2[0]).normalized) <= 0) {
                                if (onFailed != null) { onFailed(); }
                                break;
                            }
                        }
                    }
                    else {
                        Vector2 dir = (f2[0] - f1[0]).normalized * direction;
                        if (Vector2.Dot(dir, (f1[f1.Count - 1] - f1[0]).normalized) > 0 && Vector2.Dot(dir * -1, (f2[f2.Count - 1] - f2[0]).normalized) > 0) {
                            if (onPinch != null) { onPinch(); }
                            break;
                        }
                    }
                    counter += Time.smoothDeltaTime;
                    yield return new WaitForEndOfFrame();
                }
                while (counter < maxPinchDuration);

            }
            else{
                if(onFailed != null){ onFailed(); }
            }
        }



        public static void GetTwoFingerSwipe(Vector2 direction, System.Action onSwipe, float timeToSwipe, float maxSwipeDuration = 1.5f, System.Action onFailed = null){
            GetTwoFingerSwipe(direction, onSwipe, timeToSwipe, null, maxSwipeDuration, onFailed);
        }
        public static void GetTwoFingerSwipe(Vector2 direction, System.Action onSwipe, float timeToSwipe, SingleWinControlDevice device, float maxSwipeDuration = 1.5f, System.Action onFailed = null) {
            aWolfKing.WinControl.MonoBehaviour.CoroutineDummy.Instance.StartCoroutine(TwoFingerSwipeCoroutine(direction, onSwipe, timeToSwipe, device, maxSwipeDuration, onFailed));
        }

        private static IEnumerator TwoFingerSwipeCoroutine(Vector2 direction, System.Action onSwipe, float timeToSwipe, SingleWinControlDevice device, float maxSwipeDuration = 1.5f, System.Action onFailed = null){
            float counter = 0;
            List<Vector2> f1 = new List<Vector2>();
            List<Vector2> f2 = new List<Vector2>();
            Vector2 pos1;
            Vector2 pos2;
            bool pressed = false;
            do {
                if (GetTouch(out pos1, 0, device, 1) && GetTouch(out pos2, 1, device, 1)) {
                    pressed = true;
                    f1.Add(pos1);
                    f2.Add(pos2);
                    break;
                }
                counter += Time.smoothDeltaTime;
                yield return new WaitForEndOfFrame();
            }
            while (counter < timeToSwipe);
            if (pressed) {

                counter = 0;
                do {
                    if (GetTouch(out pos1, 0, device, 1) && GetTouch(out pos2, 1, device, 1)) {
                        if (Vector2.Distance(f1[f1.Count - 1], pos1) >= 0.01f && Vector2.Distance(f2[f2.Count - 1], pos2) >= 0.01f) {
                            f1.Add(pos1);
                            f2.Add(pos2);
                            Vector2 dir = direction;
                            if (!(Vector2.Dot(dir, (f1[f1.Count - 1] - f1[0]).normalized) > 0.5f && Vector2.Dot(dir, (f2[f2.Count - 1] - f2[0]).normalized) > 0.5f)) {
                                if (onFailed != null) { onFailed(); }
                                break;
                            }
                        }
                    }
                    else {
                        Vector2 dir = direction;
                        if (Vector2.Dot(dir, (f1[f1.Count - 1] - f1[0]).normalized) > 0.5f && Vector2.Dot(dir, (f2[f2.Count - 1] - f2[0]).normalized) > 0.5f) {
                            if (onSwipe != null) { onSwipe(); }
                            break;
                        }
                    }
                    counter += Time.smoothDeltaTime;
                    yield return new WaitForEndOfFrame();
                }
                while (counter < maxSwipeDuration);

            }
            else {
                if (onFailed != null) { onFailed(); }
            }
        }


    }

}
