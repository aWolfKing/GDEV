using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HSVSliders : MonoBehaviour {

    [SerializeField] private Image block_img;

    [SerializeField] private Slider hue;
    [SerializeField] private Slider saturation;
    [SerializeField] private Slider value;

    [SerializeField] private Image hue_img;
    [SerializeField] private Image saturation_img;
    [SerializeField] private Image value_img;

    [SerializeField] private RectTransform selectionDot;


    [SerializeField] private static Color color;
    public static Color Color { get { return color; } set { color = value; } }
    private float lastH, lastS, lastV;

    //private UnityEngine.EventSystems.EventSystem eventSystem;



    private void Awake() {
        color = Color.HSVToRGB(hue.value, saturation.value, value.value);
        SetHSVToMaterial(hue_img);
        SetHSVToMaterial(saturation_img);
        SetHSVToMaterial(value_img);
        SetHSVToMaterial(block_img);
        UpdateSelectionDotPosition();
        lastH = hue.value;
        lastS = saturation.value;
        lastV = value.value;
        //eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
    }

    private void FixedUpdate() {
        color = Color.HSVToRGB(hue.value, saturation.value, value.value);
        if (hue.value != lastV || saturation.value != lastS || value.value != lastV){
            SetHSVToMaterial(hue_img);
            SetHSVToMaterial(saturation_img);
            SetHSVToMaterial(value_img);
            SetHSVToMaterial(block_img);
            UpdateSelectionDotPosition();
        }
        lastH = hue.value;
        lastS = saturation.value;
        lastV = value.value;
        //MonoBehaviour.print(eventSystem.IsPointerOverGameObject());
    }

    private void SetHSVToMaterial(Image img){
        img.material.SetFloat("_Hue", hue.value);
        img.material.SetFloat("_Saturation", saturation.value);
        img.material.SetFloat("_Value", value.value);
    }

    private void UpdateSelectionDotPosition(){
        //selectionDot.anchoredPosition = new Vector2(saturation.value, value.value);
        selectionDot.anchorMin = new Vector2(saturation.value, value.value);
        selectionDot.anchorMax = new Vector2(saturation.value, value.value);
    }


    private void ChangeToolboxToOrientation(){
        #if UNITY_ANDROID || UNITY_IOS
        if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight){
            
        }
        else if(Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown){

        }
        #endif
    }


}
