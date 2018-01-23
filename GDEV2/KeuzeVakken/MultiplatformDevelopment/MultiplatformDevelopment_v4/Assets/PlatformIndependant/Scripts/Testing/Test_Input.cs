using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Input : MonoBehaviour {

    [SerializeField] private Text text;

    private void Start() {
        text.text = typeof(InputManager.Touch).ToString() + " " + Input.touchSupported + " " + InputManager.MainController;
    }

    private void Update() {
        //text.text = (1f/Time.deltaTime).ToString();
        Vector2 touchPos;
        InputManager.Touch.GetTouch(out touchPos, 0, InputManager.MainController, 1);
        //InputManager.Touch.GetTouch(out touchPos, 0, 0);
        //text.text = touchPos.ToString();
        text.text = "x:" + touchPos.x + ", y:" + touchPos.y;
    }

}
