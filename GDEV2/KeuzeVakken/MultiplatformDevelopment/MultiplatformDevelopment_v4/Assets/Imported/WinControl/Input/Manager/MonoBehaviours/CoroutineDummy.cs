using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace aWolfKing.WinControl.MonoBehaviour {

    public class CoroutineDummy : UnityEngine.MonoBehaviour {
        private static CoroutineDummy instance = null;
        public static CoroutineDummy Instance {
            get {
                if (instance == null) {
                    instance = GameObject.FindObjectOfType<CoroutineDummy>();
                    if (instance == null) {
                        GameObject obj = new GameObject("CoroutineDummy");
                        instance = obj.AddComponent<CoroutineDummy>();
                        obj.transform.position = Vector3.zero;
                    }
                }
                return instance;
            }
        }
    }

}
