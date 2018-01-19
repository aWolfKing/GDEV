using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static partial class SceneAndProjectDragAndDrop {


    private static class Variables{
        public static string assetHoverGuid = "";
        public static int instanceHoverId = -1;
        public static Object[] lastSelection = new Object[0];
        public static Rect assetHoverRect = new Rect() { position = Vector2.zero, size = Vector2.zero };
        public static Rect instanceHoverRect = new Rect() { position = Vector2.zero, size = Vector2.zero };
        public static bool mouseIsInAssetRect = false;
        public static bool mouseIsInInstanceRect = false;
        public static SceneView sceneView = null;
        public static bool mouseIsInSceneView = false;
    }


    [InitializeOnLoadMethod]
    private static void Awake(){
        EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyItem;
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyItem;
        EditorApplication.projectWindowItemOnGUI -= DrawProjectItem;
        EditorApplication.projectWindowItemOnGUI += DrawProjectItem;
        SceneView.onSceneGUIDelegate -= DrawScene;
        SceneView.onSceneGUIDelegate += DrawScene;
        SceneAndProjectDragAndDrop.AttributeManager.Awake();
        //EditorApplication.update -= //wow dit bestaat
    }

    private static void DrawHierarchyItem(int instanceId, Rect rect){

        if (rect.Contains(Event.current.mousePosition)) {
            SceneAndProjectDragAndDrop.Variables.instanceHoverId = instanceId;
            SceneAndProjectDragAndDrop.Variables.instanceHoverRect = rect;
        }

        CheckMouse();

    }

    private static void DrawProjectItem(string guid, Rect rect){

        if (rect.Contains(Event.current.mousePosition)){
            SceneAndProjectDragAndDrop.Variables.assetHoverGuid = guid;
            SceneAndProjectDragAndDrop.Variables.assetHoverRect = rect;
        }

        CheckMouse();
        
    }

    private static void DrawScene(SceneView sceneView){

        SceneAndProjectDragAndDrop.Variables.mouseIsInSceneView = true;
        SceneAndProjectDragAndDrop.Variables.sceneView = sceneView;

        CheckMouse();

    }

    private static void CheckMouse(){

        if(new System.Diagnostics.StackFrame(1).GetMethod().Name != "DrawScene"){
            SceneAndProjectDragAndDrop.Variables.mouseIsInSceneView = false;
        }

        if /*CAN DROP*/ (SceneAndProjectDragAndDrop.Variables.assetHoverRect.Contains(Event.current.mousePosition)) {
            //DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;
            if (ValidateDrop(SceneAndProjectDragAndDrop.Variables.lastSelection, AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(SceneAndProjectDragAndDrop.Variables.assetHoverGuid)), out visualMode)) {
                if(visualMode != DragAndDropVisualMode.None){
                    DragAndDrop.visualMode = visualMode;
                }
            }
        }
        else if /*CAN DROP*/ (SceneAndProjectDragAndDrop.Variables.instanceHoverRect.Contains(Event.current.mousePosition)) {
            //DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;
            if (ValidateDrop(SceneAndProjectDragAndDrop.Variables.lastSelection, EditorUtility.InstanceIDToObject(SceneAndProjectDragAndDrop.Variables.instanceHoverId), out visualMode)) {
                if (visualMode != DragAndDropVisualMode.None) {
                    DragAndDrop.visualMode = visualMode;
                }
            }
        }
        else if (SceneAndProjectDragAndDrop.Variables.mouseIsInSceneView){
            DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;
            if(ValidateDrop(SceneAndProjectDragAndDrop.Variables.lastSelection, SceneAndProjectDragAndDrop.Variables.sceneView, out visualMode)){
                if(visualMode != DragAndDropVisualMode.None){
                    DragAndDrop.visualMode = visualMode;
                }
            }
        }

        if /*DROPPED*/ (Event.current.rawType == EventType.DragExited) {
            if (SceneAndProjectDragAndDrop.Variables.lastSelection.Length > 0) {

                //do what needed when dropped
                if (SceneAndProjectDragAndDrop.Variables.mouseIsInAssetRect /*SceneAndProjectDragAndDrop.Variables.assetHoverRect.Contains(Event.current.mousePosition)*/){
                    PerformDrop(SceneAndProjectDragAndDrop.Variables.lastSelection, AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(SceneAndProjectDragAndDrop.Variables.assetHoverGuid)));
                }
                else if (SceneAndProjectDragAndDrop.Variables.mouseIsInInstanceRect /*SceneAndProjectDragAndDrop.Variables.instanceHoverRect.Contains(Event.current.mousePosition)*/){
                    PerformDrop(SceneAndProjectDragAndDrop.Variables.lastSelection, EditorUtility.InstanceIDToObject(SceneAndProjectDragAndDrop.Variables.instanceHoverId));
                }
                else if (SceneAndProjectDragAndDrop.Variables.mouseIsInSceneView){
                    PerformDrop(SceneAndProjectDragAndDrop.Variables.lastSelection, SceneAndProjectDragAndDrop.Variables.sceneView);
                }

            }
        }
        else if(Event.current.rawType == EventType.DragUpdated){
            SceneAndProjectDragAndDrop.Variables.lastSelection = DragAndDrop.objectReferences;
            SceneAndProjectDragAndDrop.Variables.mouseIsInAssetRect = SceneAndProjectDragAndDrop.Variables.assetHoverRect.Contains(Event.current.mousePosition);
            SceneAndProjectDragAndDrop.Variables.mouseIsInInstanceRect = SceneAndProjectDragAndDrop.Variables.instanceHoverRect.Contains(Event.current.mousePosition);
        }

    }


    private static void PerformDrop(Object[] dropped, Object target){
        var pair = AttributeManager.GetOperationExecutionPair(dropped, target);
        if(pair != null){
            try{
                pair.execution(dropped, target);
            }
            catch{}
        }
    }

    private static bool ValidateDrop(Object[] selection, Object target, out DragAndDropVisualMode cursorMode){
        cursorMode = DragAndDropVisualMode.None;
        var pair = AttributeManager.GetOperationExecutionPair(selection, target);
        if (pair != null) {
            cursorMode = pair.visualMode;
        }
        return pair != null;
    }


}
