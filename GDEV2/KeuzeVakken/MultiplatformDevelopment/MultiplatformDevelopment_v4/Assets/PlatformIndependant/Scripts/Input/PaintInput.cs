using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintInput : MonoBehaviour {

    private static PaintInput _this;    

    private Vector2 lastTouch1, lastTouch2;
    private Vector2 touch1, touch2;
    private bool lastTouching1 = false, lastTouching2 = false;
    private bool touching1 = false, touching2 = false;

    private UnityEngine.EventSystems.EventSystem eventSystem;

    private void Awake() {
        _this = this;
        eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
    }

    private void FixedUpdate() {
        lastTouch1 = touch1;
        lastTouch2 = touch2;
        lastTouching1 = touching1;
        lastTouching2 = touching2;
        #if Import_WinControl
        touching1 = InputManager.Touch.GetTouch(out touch1, 0, InputManager.MainController, 1);
        if(touching1 && !lastTouching1){
            lastTouch1 = touch1;
        }
        touching2 = InputManager.Touch.GetTouch(out touch2, 1, InputManager.MainController, 1);
        if(touching2 && !lastTouching2){
            lastTouch2 = touch2;
        }
        #else
        
        #endif
    }


    public static float GetZoomInput(){
        if(Input.GetAxis("Mouse ScrollWheel") != 0 && !Input.GetKey(KeyCode.LeftControl)){
            return Input.GetAxis("Mouse ScrollWheel");
        }
        else if(InputManager.MainController != null){
            var rY = InputManager.MainController.GetAxis("RY");
            if(rY != null && Mathf.Abs(rY.Value) >= 0.05f){
                return rY.Value;
            }
        }
        if(Vector2.Distance(_this.lastTouch1, _this.touch1) >= 0.001f && Vector2.Distance(_this.lastTouch2, _this.touch2) >= 0.001f){
            int i = -1;
            do {
                if (Vector2.Angle((_this.touch1 - _this.lastTouch1).normalized, (_this.lastTouch2 - _this.lastTouch1).normalized * i) <= 60) {
                    if (Vector2.Angle((_this.touch2 - _this.lastTouch2).normalized, (_this.lastTouch1 - _this.lastTouch2).normalized * i) <= 60) {
                        return i * (Vector2.Distance(_this.lastTouch1, _this.touch1) + Vector2.Distance(_this.lastTouch2, _this.touch2)) * 2;
                    }
                }
                i += 2;
            }
            while (i < 2);
        }
        return 0;
    }	

    public static float GetRotateInput(){
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetKey(KeyCode.LeftControl)) {
            return Input.GetAxis("Mouse ScrollWheel") * 10;
        }
        else if (InputManager.MainController != null) {
            var r2 = InputManager.MainController.GetAxis("R2");
            var l2 = InputManager.MainController.GetAxis("L2");
            if (r2 != null && l2 != null && (r2.Value >= 0.05f || l2.Value >= 0.05f)) {
                return l2.Value - r2.Value;
            }
        }
        if(Vector2.Distance(_this.lastTouch1, _this.touch1) >= 0.001f && Vector2.Distance(_this.lastTouch2, _this.touch2) >= 0.001f){
            int i = -1;
            do {
                if(Vector2.Angle((_this.touch1 - _this.lastTouch1).normalized, new Vector2(0,1*i)) <= 50){
                    if (Vector2.Angle((_this.touch2 - _this.lastTouch2).normalized, new Vector2(0, -1*i)) <= 50) {
                        return i;
                    }
                }
                i +=2 ;
            }
            while (i < 2);
        }
        return 0;
    }



    public static Vector2 GetMoveInput(){
        Vector2 ret = Vector2.zero;        

        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
            ret.x = Input.GetAxis("Horizontal");
            ret.y = Input.GetAxis("Vertical");
        }
        else{
            if(InputManager.MainController != null){
                var x = InputManager.MainController.GetAxis("LX");
                var y = InputManager.MainController.GetAxis("LY");
                var l1 = InputManager.MainController.GetInput("L1");
                bool b = (l1 == null || (l1 != null && l1.Value < 0.5f));
                if(b && x != null && y != null && Mathf.Abs(x.Value) >= 0.05f && Mathf.Abs(y.Value) >= 0.05f){
                    ret.x = x.Value;
                    ret.y = y.Value;
                }
                else{
                    ret = GetTouchMoveInput();
                }
            }
            else{
                ret = GetTouchMoveInput();
            }
        }
        
        return ret;
    }

    private static Vector2 GetTouchMoveInput(){
        if(Vector2.Distance(_this.touch1, _this.lastTouch1) >= 0.001f && Vector2.Distance(_this.touch2, _this.lastTouch2) >= 0.001f){
            if(Vector2.Angle((_this.touch1-_this.lastTouch1).normalized, (_this.touch2-_this.lastTouch2).normalized) <= 40){
                Vector2 ret = (_this.touch1 - _this.lastTouch1).normalized;
                ret.x *= -1;
                return ret * 0.5f;
            }
        }
        return Vector2.zero;
    }

    public static bool GetPaintInput(out Vector2 viewPortPos){
        //0.77, 0.95 - 0.05
        if(Input.GetKey(KeyCode.Mouse0)){
            viewPortPos = new Vector2(1f / Screen.width * Input.mousePosition.x, 1f / Screen.height * Input.mousePosition.y);
            return !(viewPortPos.x >= 0.77f && (viewPortPos.y >= 0.05f && viewPortPos.y <= 0.95f));
        }
        Vector2 touchPos1, touchPos2;
        if(InputManager.Touch.GetTouch(out touchPos1, 0, InputManager.MainController, 1) && !InputManager.Touch.GetTouch(out touchPos2, 1, InputManager.MainController, 1)){
            viewPortPos = touchPos1;
            return !(viewPortPos.x >= 0.77f && (viewPortPos.y >= 0.05f && viewPortPos.y <= 0.95f));
        }
        viewPortPos = new Vector2(0.5f, 0.5f);
        return false;
    }


}
