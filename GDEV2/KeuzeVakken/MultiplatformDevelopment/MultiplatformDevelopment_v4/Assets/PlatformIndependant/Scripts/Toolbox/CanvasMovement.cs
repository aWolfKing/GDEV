using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMovement : MonoBehaviour {

    [SerializeField] private Transform canvasTransform;
    [SerializeField] private new Camera camera;

    [SerializeField] private float zReference = 9.37f;
    [SerializeField] private float xPosOffset = 19;
    [SerializeField] private float yPosOffset = 10.5f;
    [SerializeField] private float zPosMax = 200;

    [SerializeField] private Transform cornerLU, cornerRU, cornerRD, cornerLD;
    [SerializeField] private UnityEngine.UI.Slider zoomSlider;

    private Mesh mesh = null;

    private void Awake() {
        zoomSlider.maxValue = zPosMax;
        zoomSlider.minValue = 0.3f;
        zoomSlider.value = canvasTransform.position.z;
        CanvasPaint.SetTransforms(canvasTransform, cornerLU, cornerRU, cornerRD, cornerLD, camera);
        if(mesh == null){
            mesh = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            verts.Add(canvasTransform.InverseTransformPoint(cornerRU.position));
            verts.Add(canvasTransform.InverseTransformPoint(cornerRD.position));
            verts.Add(canvasTransform.InverseTransformPoint(cornerLD.position));
            verts.Add(canvasTransform.InverseTransformPoint(cornerLU.position));
            mesh.vertices = verts.ToArray();
            int[] tris = new int[] { 0, 1, 2, 2, 3, 0 };
            mesh.triangles = tris;
        }
        CanvasPaint.SetMesh(mesh);
    }

    private void OnDestroy() {
        mesh.Clear();
        Destroy(mesh);
        mesh = null;
    }

    private void FixedUpdate() {
        Vector3 pos = canvasTransform.position;
        float zoomInput = PaintInput.GetZoomInput();
        Vector2 moveInput = PaintInput.GetMoveInput();
        if(zoomInput != 0){
            pos.z = Mathf.Clamp(pos.z + zoomInput, 0.3f, zPosMax);
            zoomSlider.value = pos.z;
        }
        else if(zoomInput == 0) {
            pos.z = Mathf.Clamp(zoomSlider.value, 0.3f, zPosMax);
            zoomInput = 0.01f;
        }
        if (moveInput != Vector2.zero || zoomInput != 0) {
            Vector2 maxXY = GetMaxXY(pos.z);
            pos.x = Mathf.Clamp(pos.x + moveInput.x, -maxXY.x, maxXY.x);
            pos.y = Mathf.Clamp(pos.y + moveInput.y, -maxXY.y, maxXY.y);
        }

        canvasTransform.Rotate(Vector3.forward * PaintInput.GetRotateInput());

        canvasTransform.position = pos;
    }

    private Vector2 GetMaxXY(float z){
        int xSide = canvasTransform.position.x > 0 ? 1 : canvasTransform.position.x == 0 ? 0 : -1;
        int ySide = canvasTransform.position.y > 0 ? 1 : canvasTransform.position.y == 0 ? 0 : -1;
        Transform t = canvasTransform;
        if(xSide > 0){
            if(ySide > 0){
                t = cornerLD;
            }
            else if(ySide < 0){
                t = cornerLU;
            }
        }
        else if(xSide < 0){
            if(ySide > 0){
                t = cornerRD;
            }
            else if(ySide < 0){
                t = cornerRU;
            }
        }
        else if(xSide == 0){
            if(ySide == 0){
                return new Vector2(100, 100);
            }
        }
        Vector3 maxCornerWorld = camera.ViewportPointToRay(new Vector2((xSide/2f)+0.5f, (ySide/2f)+0.5f)).direction;//(z / zReference) * zReference;
        maxCornerWorld.z = z;
        maxCornerWorld.x = (z / zReference) * maxCornerWorld.x;
        maxCornerWorld.y = (z / zReference) * maxCornerWorld.y;
        Vector3 maxWorld = maxCornerWorld + (canvasTransform.position - t.position);
        return new Vector2(Mathf.Abs(maxWorld.x), Mathf.Abs(maxWorld.y));
    }


}
