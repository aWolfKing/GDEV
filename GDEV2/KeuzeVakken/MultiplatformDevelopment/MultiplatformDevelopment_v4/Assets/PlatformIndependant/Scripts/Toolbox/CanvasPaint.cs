using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPaint : MonoBehaviour {

    private static CanvasPaint _this;
    public static CanvasPaint Instance{ get{ return _this; } }

    private new MeshCollider collider;
    private new Camera camera;

    private Transform canvasTransform, cornerLU, cornerRU, cornerRD, cornerLD;

    [SerializeField] private RawImage image;
    private Texture2D texture;
    [SerializeField] private int pixelWidth = 960, pixelHeight = 540;


    private void Awake() {
        _this = this;
        texture = new Texture2D(pixelWidth, pixelHeight);
        image.texture = texture;
        Color[] cols = new Color[pixelWidth*pixelHeight];
        for(int i=0; i<cols.Length; i++){
            cols[i] = Color.white;
        }
        texture.SetPixels(cols);
        texture.Apply();
    }

    private void OnDestroy() {
        Destroy(texture);
        texture = null;
    }

    private void FixedUpdate() {
        RaycastHit hit;
        Vector2 vp;
        if (PaintInput.GetPaintInput(out vp)) {
            Ray r = camera.ViewportPointToRay(vp);
            if (collider.Raycast(r, out hit, Mathf.Infinity)) {
                Debug.DrawLine(hit.point, hit.point - Vector3.forward, Color.red, 0.2f);
                Vector2 coord = Vector2.zero;
                coord.x = 1f / Vector3.Distance(cornerLU.position, cornerRU.position) * Vector3.Project((hit.point - cornerLU.position), (cornerRU.position - cornerLU.position).normalized).magnitude;
                coord.y = 1f / Vector3.Distance(cornerLU.position, cornerLD.position) * Vector3.Project((hit.point - cornerLD.position), (cornerLD.position - cornerLU.position).normalized).magnitude;
                //coord.y = 1 - coord.y;
                //MonoBehaviour.print(coord);
                Paint(coord, HSVSliders.Color);
            }
        }
    }


    public static void SetMesh(Mesh m){
        if(_this.collider == null){
            GameObject obj = new GameObject("PaintCollider");
            obj.transform.SetParent(_this.canvasTransform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _this.collider = obj.AddComponent<MeshCollider>();
        }
        _this.collider.sharedMesh = m;
    }

    public static void SetTransforms(Transform c, Transform lu, Transform ru, Transform rd, Transform ld, Camera cam){
        _this.canvasTransform = c;
        _this.cornerLU = lu;
        _this.cornerRU = ru;
        _this.cornerRD = rd;
        _this.cornerLD = ld;
        _this.camera = cam;
    }


    private static void Paint(Vector2 pos, Color col){
        Color[] cols = _this.texture.GetPixels();
        //MonoBehaviour.print(cols.Length + " " + (_this.pixelHeight * _this.pixelWidth).ToString());
        int pixelX = Mathf.RoundToInt(_this.pixelWidth * pos.x);
        int pixelY = Mathf.RoundToInt(_this.pixelHeight * pos.y);
        int pixel = (pixelY * _this.pixelWidth) + pixelX;
        if (pixel >= 0 && pixel < _this.pixelWidth * _this.pixelHeight) {
            cols[pixel] = col;
        }
        _this.texture.SetPixels(cols);
        _this.texture.Apply();
    }


}
