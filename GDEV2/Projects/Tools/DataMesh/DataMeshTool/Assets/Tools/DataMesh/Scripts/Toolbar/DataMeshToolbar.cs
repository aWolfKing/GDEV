#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[SceneViewToolbarExtension(SceneViewToolbarExtensionType.Toggle, -1000, 'D', 'M')]
public class DataMeshToolbar : EditorWindow {

    private static bool isOpen = false;
    private static float toolbarHeight = 14;


    private static class DataMeshGUIStyles { 
        public static GUIStyle toolbarStyle = null;
        public static GUIStyle toolbarButtonStyle = null;
        public static GUIStyle toolbarTextFieldStyle = null;
        public static GUIStyle toolbarDropDownStyle = null;
    }


    private static class DataMeshEditorTextures {           //Textures must have the same name as the fields, Fields must be public
        public static Texture meshesLinkedTexture = null;
        public static Texture dataMeshObject = null;
        public static Texture paintBrushIcon = null;
        public static Texture moreIcon = null;
        public static Texture moveAble = null;
        public static void FindTextures(){
            FieldInfo[] fields = typeof(DataMeshEditorTextures).GetFields(BindingFlags.Public | BindingFlags.Static);
            for(int i=0; i<fields.Length; i++){
                try{
                    fields[i].SetValue(null, (Texture)AssetDatabase.LoadMainAssetAtPath("Assets/Tools/DataMesh/Icons/" + fields[i].Name + ".png"));
                }
                catch{ Debug.LogWarning("Texture " + "'" + fields[i].Name + "'" + " was not found"); }
            }
        }
    }

    private static class DataMeshGUIOptions{
        public static bool twoD = false;
        public static bool showDataMeshObjects = true;
        public static bool showDataMeshLinks = true;
        public static bool showSmallToolButtons = false;
        public static bool showAllDataMeshes = false;
        public static bool showToolbarItemToolTips = false;
        public static bool showToolBox = false;
        public static bool showPaintSelectedButtons = false;
        public static bool simpleToolbar = true;
        public static float iconSize = 20f;
        public static bool showHelp = false;
        public static bool dataMeshTitleRightMouse = false;
        public static bool paintSelectionRightMouse = false;
        public static bool clearSelectionRightMouse = false;
    }

    private static class InputOptions{
        public static KeyCode paintToggleKey = KeyCode.P;
        public static KeyCode toolboxToggleKey = KeyCode.B;
    }

    private static class Variables{
        public static bool inputIsUsed = false;
        public static bool isSelecting = false;
        public static bool leftMouseIsDown = false;
        public static bool rightMouseIsDown = false;
        public static bool shiftIsDown = false;
        public static bool controlIsDown = false;
        public static List<Vector2> selectionPoints = new List<Vector2>();
        public static bool cameraRotated = false;
        public static Vector2 lastMousePosition = Vector2.zero;
        public static GameObject rightClickSelectedGameObject = null;
        public static Rect valueWindowRect = new Rect() { position = new Vector2(718, 17), width = 207, height = 14 };
        public static Rect valueWindowPinnedRect = new Rect() { position = new Vector2(718, 17), width = 207, height = 14 };
        public static int selectedPaintValue = -1;
        public static DataMeshObject editingDataMesh = null;
        public static Rect sceneViewRect = new Rect();
        public static bool isPainting = false;
        public static DataMeshObject lastDragged = null;
    }

    private static class Painting{
        public static List<DataMeshTriangle> virtualTriangles = new List<DataMeshTriangle>();
        private static DataMeshEditorRendererHolder renderHolder = null;
        public static DataMeshEditorRendererHolder RenderHolder{ 
            get{ 
                if(renderHolder == null){
                    renderHolder = GameObject.FindObjectOfType<DataMeshEditorRendererHolder>();
                    if(renderHolder == null){
                        GameObject obj = new GameObject("DataMeshEditorRenderHolder");
                        obj.transform.position = Vector3.zero;
                        renderHolder = obj.AddComponent<DataMeshEditorRendererHolder>();
                    }
                }
                return renderHolder;
            } 
        }
        public class PaintingRenderHolder{ public MeshFilter meshFilter; public MeshRenderer meshRenderer; public Mesh mesh; public void Clear() { GameObject.Destroy(meshFilter.gameObject); mesh.Clear(); mesh = null; } }
        public static Dictionary<int, PaintingRenderHolder> renderers = new Dictionary<int, PaintingRenderHolder>();
        public static Dictionary<int, Material> paintingRenderMaterials = new Dictionary<int, Material>();
    }

    private static class EditorDataMesh{
        private static DataMeshEditorVirtualObjectHolder virtualObjectHolder = null;
        public static DataMeshEditorVirtualObjectHolder VirtualObjectHolder{ 
            get{ 
                if(virtualObjectHolder == null){ virtualObjectHolder = GameObject.FindObjectOfType<DataMeshEditorVirtualObjectHolder>(); }
                if(virtualObjectHolder == null){ GameObject obj = new GameObject("VirtualObjectHolder"); virtualObjectHolder = obj.AddComponent<DataMeshEditorVirtualObjectHolder>(); }
                return virtualObjectHolder;
            } 
        }
        public static List<DataMeshObject.DataMeshSubObject> selectedVirtualObjects = new List<DataMeshObject.DataMeshSubObject>();
        public static DataMeshObject.DataMeshSubObject rightClickedVirtualObject = null;
        public static bool openCreateVirtualObjectMenu = false;
        public static Vector2 CreateVirtualObjectMenuPosition = Vector2.zero;
    }

    private static class SubObjectLinks{
        public static Dictionary<DataMeshObject.DataMeshSubObject, List<DataMeshObject.DataMeshSubObject>> control_overs = new Dictionary<DataMeshObject.DataMeshSubObject, List<DataMeshObject.DataMeshSubObject>>();
        public static Dictionary<DataMeshObject.DataMeshSubObject, DataMeshObject.DataMeshSubObject> listening_tos = new Dictionary<DataMeshObject.DataMeshSubObject, DataMeshObject.DataMeshSubObject>();

        public static void TakeControlOver(DataMeshObject.DataMeshSubObject dm, DataMeshObject.DataMeshSubObject takeOver) {
            if (dm != takeOver) {
                if (!SubObjectLinks.control_overs.ContainsKey(dm)) { SubObjectLinks.control_overs.Add(dm, new List<DataMeshObject.DataMeshSubObject>()); }
                if (!SubObjectLinks.control_overs[dm].Contains(takeOver)) { SubObjectLinks.control_overs[dm].Add(takeOver); }
                if(!listening_tos.ContainsKey(takeOver)){ listening_tos.Add(takeOver, dm); }
                else{ control_overs[listening_tos[takeOver]].Remove(takeOver); listening_tos[takeOver] = dm; }
            }
        }
        public static void RemoveControlFrom(DataMeshObject.DataMeshSubObject dm, DataMeshObject.DataMeshSubObject takenOver){
            if(SubObjectLinks.control_overs.ContainsKey(dm)){ if(SubObjectLinks.control_overs[dm].Contains(takenOver)){ SubObjectLinks.control_overs[dm].Remove(takenOver); } }
            if(listening_tos.ContainsKey(takenOver)){ control_overs[listening_tos[takenOver]].Remove(takenOver); }
        }
        public static bool HasAnyControl(DataMeshObject.DataMeshSubObject dm){
            if (SubObjectLinks.control_overs.ContainsKey(dm)){ return SubObjectLinks.control_overs[dm].Count > 0; }
            return false;
        }
        public static DataMeshObject.DataMeshSubObject ListeningTo(DataMeshObject.DataMeshSubObject dm){
            if(listening_tos.ContainsKey(dm)){ return listening_tos[dm]; }
            return null;
        }
        public static void CopyLinksFrom(DataMeshObject.DataMeshSubObject dm, DataMeshObject.DataMeshSubObject to){
            if(!listening_tos.ContainsKey(to) && listening_tos.ContainsKey(dm)){ listening_tos.Add(to, listening_tos[dm]); }
        }
        public static void Delete(DataMeshObject.DataMeshSubObject dm){
            foreach(var d in listening_tos){
                if(d.Value == dm){ RemoveControlFrom(d.Value, dm); }
            }
            if(listening_tos.ContainsKey(dm)){ listening_tos.Remove(dm); }
        }
    }



    /*
    [SceneViewToolbarToggleDisable][SceneViewToolbarToggleEnable]
    [MenuItem("Tools/DataMesh/Toolbar &m")]
    public static void ToggleToolbar(){
        if(isOpen){
            isOpen = false;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }
        else{
            isOpen = true;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            if (DataMeshGUIStyles.toolbarStyle == null) { DataMeshGUIStyles.toolbarStyle = CloneStyle(EditorStyles.toolbar); }
            if (DataMeshGUIStyles.toolbarButtonStyle == null){ DataMeshGUIStyles.toolbarButtonStyle = CloneStyle(EditorStyles.toolbarButton); }
            if (DataMeshGUIStyles.toolbarTextFieldStyle == null){ DataMeshGUIStyles.toolbarTextFieldStyle = CloneStyle(EditorStyles.textField); }
            if (DataMeshGUIStyles.toolbarDropDownStyle == null){ DataMeshGUIStyles.toolbarDropDownStyle = CloneStyle(EditorStyles.toolbarDropDown); }
            DataMeshEditorTextures.FindTextures();
        }
    }
    */



    [SceneViewToolbarToggleEnable]
    public static void ToggleToolbarEnable(){
        isOpen = true;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        SceneView.onSceneGUIDelegate -= ClosedToolbarEnableDataMeshDragToScene;
        if (DataMeshGUIStyles.toolbarStyle == null) { DataMeshGUIStyles.toolbarStyle = CloneStyle(EditorStyles.toolbar); }
        if (DataMeshGUIStyles.toolbarButtonStyle == null) { DataMeshGUIStyles.toolbarButtonStyle = CloneStyle(EditorStyles.toolbarButton); }
        if (DataMeshGUIStyles.toolbarTextFieldStyle == null) { DataMeshGUIStyles.toolbarTextFieldStyle = CloneStyle(EditorStyles.textField); }
        if (DataMeshGUIStyles.toolbarDropDownStyle == null) { DataMeshGUIStyles.toolbarDropDownStyle = CloneStyle(EditorStyles.toolbarDropDown); }
        DataMeshEditorTextures.FindTextures();
    }

    [InitializeOnLoadMethod][UnityEditor.Callbacks.DidReloadScripts]
    [SceneViewToolbarToggleDisable]
    public static void ToggleToolbarDisable(){
        isOpen = false;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate -= ClosedToolbarEnableDataMeshDragToScene;
        SceneView.onSceneGUIDelegate += ClosedToolbarEnableDataMeshDragToScene;
        Variables.isPainting = false;
    }

    [MenuItem("Tools/DataMesh/Toolbar &m")]
    public static void ToggleToolbarHotkeys(){
        SceneViewToolbarExtensionManager.RequestToggle<DataMeshToolbar>(!isOpen);
    }



    private static void OnSceneGUI(SceneView sceneView){
        Handles.BeginGUI();
        Variables.sceneViewRect = sceneView.position;
        Variables.inputIsUsed = false;
        EnableDataMeshObjectDragToScene(sceneView);
        ToolbarInteraction(sceneView);
        DrawSelectedVirtualObjectHandles(sceneView);
        DrawVirtualObjectGUI(sceneView);
        DrawRightClickedVirtualObject(sceneView);
        DrawDataMeshTitleDropDown(sceneView);
        DrawPaintSelectedDropDown(sceneView);
        DrawClearSelectedDropDown(sceneView);
        DrawHelp(sceneView);
        DrawToolbar(sceneView);
        HandleInput(sceneView);
        Paint();
        //RenderSelectionCircle(sceneView);
        GetRightClickedVirtualObject(sceneView);
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Variables.lastMousePosition = Event.current.mousePosition;
        Handles.EndGUI();
    }
    

    private static void ClosedToolbarEnableDataMeshDragToScene(SceneView sceneView){
        if (Variables.editingDataMesh == null || (Variables.lastDragged != Variables.editingDataMesh && Variables.lastDragged != null)) {

            if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0].GetType() == typeof(DataMeshObject)) {
                Rect r2 = new Rect() { position = Event.current.mousePosition + new Vector2(15, 10), width = 100, height = toolbarHeight };
                Handles.BeginGUI();
                GUI.Label(r2, "Release to edit");
                Handles.EndGUI();
            }

            if (Variables.lastDragged != null && DragAndDrop.objectReferences.Length == 0) {
                Variables.editingDataMesh = Variables.lastDragged;
                SceneViewToolbarExtensionManager.RequestToggle<DataMeshToolbar>(true);
                return;
            }
        }
        Variables.lastDragged = null;
        if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0].GetType() == typeof(DataMeshObject)) {
            Variables.lastDragged = (DataMeshObject)DragAndDrop.objectReferences[0];
        }

    }

    private static void EnableDataMeshObjectDragToScene(SceneView sceneView){
        if (Variables.editingDataMesh == null || (Variables.lastDragged != Variables.editingDataMesh && Variables.lastDragged != null)) {
            Rect r = new Rect() { position = new Vector2(0, toolbarHeight), height = sceneView.position.height - toolbarHeight, width = sceneView.position.width };

            if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0].GetType() == typeof(DataMeshObject)) {
                Rect r2 = new Rect() { position = Event.current.mousePosition + new Vector2(15, 10), width = 100, height = toolbarHeight };
                GUI.Label(r2, "Release to edit");
            }

            if (Variables.lastDragged != null && DragAndDrop.objectReferences.Length == 0) {
                Variables.editingDataMesh = Variables.lastDragged;
            }
        }
        Variables.lastDragged = null;
        if(DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0].GetType() == typeof(DataMeshObject)){
            Variables.lastDragged = (DataMeshObject)DragAndDrop.objectReferences[0];
        }

    }


    private static void ToolbarInteraction(SceneView sceneView){
        Rect toolbarRect = new Rect() { width = sceneView.position.width, height = toolbarHeight, position = Vector2.zero };
        
        if(toolbarRect.Contains(Event.current.mousePosition)){ Variables.inputIsUsed = true; }
    }


    private static void DrawToolbar(SceneView sceneView){
        Rect toolbarRect = new Rect() { width = sceneView.position.width, height = toolbarHeight, position = Vector2.zero };

        GUI.Label(toolbarRect, "", DataMeshGUIStyles.toolbarStyle);

        Rect toolbarItemRect = toolbarRect;
        toolbarItemRect.width = 120;
        toolbarItemRect.position = new Vector2(5, 0);
        GUI.Label(toolbarItemRect, "DataMesh", DataMeshGUIStyles.toolbarButtonStyle);
        if(Event.current.type == EventType.MouseUp && Event.current.button == 1){ if (toolbarItemRect.Contains(Event.current.mousePosition)) { DataMeshGUIOptions.dataMeshTitleRightMouse = !DataMeshGUIOptions.dataMeshTitleRightMouse; Variables.inputIsUsed = true; Variables.rightClickSelectedGameObject = null; EditorDataMesh.openCreateVirtualObjectMenu = false; } }


        toolbarItemRect.width = 25;
        toolbarItemRect.position = new Vector2(131, 0);
        DataMeshGUIOptions.showHelp = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.showHelp, "?", DataMeshGUIStyles.toolbarButtonStyle);


        if (DataMeshGUIOptions.showSmallToolButtons && !DataMeshGUIOptions.simpleToolbar) {

            toolbarItemRect.width = 26;
            toolbarItemRect.position = new Vector2(162, 0);
            DataMeshGUIOptions.showDataMeshObjects = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.showDataMeshObjects, DataMeshEditorTextures.dataMeshObject, DataMeshGUIStyles.toolbarButtonStyle);
            ToolbarItemToolTip(toolbarItemRect, "Show DataMesh objects?", 170);

            toolbarItemRect.width = 26;
            toolbarItemRect.position = new Vector2(188, 0);
            DataMeshGUIOptions.showDataMeshLinks = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.showDataMeshLinks, DataMeshEditorTextures.meshesLinkedTexture, DataMeshGUIStyles.toolbarButtonStyle);
            ToolbarItemToolTip(toolbarItemRect, "Show DataMesh links?", 170);

        }

        if (!DataMeshGUIOptions.simpleToolbar) {
            Rect moreRect = toolbarItemRect;
            moreRect.width = 30;
            moreRect.height = 18;
            moreRect.position = new Vector2((DataMeshGUIOptions.showSmallToolButtons ? /*254*/ 219 : /*196*/ 193), -1);
            GUI.Label(moreRect, DataMeshEditorTextures.moreIcon);
            DataMeshGUIOptions.showSmallToolButtons = GUI.Toggle(moreRect, DataMeshGUIOptions.showSmallToolButtons, "", GUIStyle.none);
            ToolbarItemToolTip(moreRect, (DataMeshGUIOptions.showSmallToolButtons ? "Hide" : "Show"), 50);


            toolbarItemRect.width = 25;
            toolbarItemRect.position = new Vector2(251, 0);
            DataMeshGUIOptions.showAllDataMeshes = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.showAllDataMeshes, "All", DataMeshGUIStyles.toolbarButtonStyle);
            ToolbarItemToolTip(toolbarItemRect, "Show all DataMeshes?", 170);


            toolbarItemRect.width = 25;
            toolbarItemRect.position = new Vector2(282, 0);
            DataMeshGUIOptions.showToolbarItemToolTips = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.showToolbarItemToolTips, "TT", DataMeshGUIStyles.toolbarButtonStyle);
            if (toolbarItemRect.Contains(Event.current.mousePosition)) { Rect tooltipRect = new Rect() { position = new Vector2(Event.current.mousePosition.x, toolbarHeight), width = 100, height = toolbarHeight }; GUI.Label(tooltipRect, "Enable tooltips?", DataMeshGUIStyles.toolbarButtonStyle); }
        }

        toolbarItemRect.width = (!DataMeshGUIOptions.simpleToolbar ? 70 : 89 );
        toolbarItemRect.position = new Vector2( (!DataMeshGUIOptions.simpleToolbar ? 313 : 162 ), 0);
        DataMeshGUIOptions.showToolBox = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.showToolBox, "Toolbox" + (DataMeshGUIOptions.showHelp ? " (" + InputOptions.toolboxToggleKey.ToString().ToLower() + ")" : ""), DataMeshGUIStyles.toolbarButtonStyle);

        if (!DataMeshGUIOptions.simpleToolbar) {
            Rect moreRect = toolbarItemRect;
            moreRect.position = new Vector2(389, -1);
            moreRect.width = 30;
            moreRect.height = 18;
            GUI.Label(moreRect, DataMeshEditorTextures.moreIcon);
            DataMeshGUIOptions.showPaintSelectedButtons = GUI.Toggle(moreRect, DataMeshGUIOptions.showPaintSelectedButtons, "", GUIStyle.none);
            ToolbarItemToolTip(moreRect, (!DataMeshGUIOptions.showPaintSelectedButtons ? "More" : "Less"), 50);
        }

        if(DataMeshGUIOptions.showPaintSelectedButtons || DataMeshGUIOptions.simpleToolbar){

            toolbarItemRect.width = (!DataMeshGUIOptions.simpleToolbar ? 122 : 194 );
            toolbarItemRect.position = new Vector2( (!DataMeshGUIOptions.simpleToolbar ? 413 : 257 ), 0);
            if (EditorDataMesh.selectedVirtualObjects.Count > 0) {
                GUI.Label(toolbarItemRect, (!Variables.isPainting ? "Paint selected virtual object"+(EditorDataMesh.selectedVirtualObjects.Count>1?"s":"") : "Stop painting") + (DataMeshGUIOptions.showHelp ? " (" + InputOptions.paintToggleKey.ToString().ToLower() + ")" : ""), DataMeshGUIStyles.toolbarButtonStyle);
                if(Event.current.type == EventType.MouseUp && Event.current.button == 0 && toolbarItemRect.Contains(Event.current.mousePosition)){ Variables.isPainting = !Variables.isPainting; }
                else if (Event.current.type == EventType.MouseUp && Event.current.button == 1 && toolbarItemRect.Contains(Event.current.mousePosition)) { DataMeshGUIOptions.paintSelectionRightMouse = !DataMeshGUIOptions.paintSelectionRightMouse; }
            }
            else{
                GUI.Label(toolbarItemRect, "Select a virtual object first", DataMeshGUIStyles.toolbarButtonStyle);
            }

            toolbarItemRect.width = (!DataMeshGUIOptions.simpleToolbar ? 122 : 194);
            toolbarItemRect.position = new Vector2( (!DataMeshGUIOptions.simpleToolbar ? 535 : 457), 0);
            if (EditorDataMesh.selectedVirtualObjects.Count > 0) {
                GUI.Label(toolbarItemRect, "Clear selected virtual object"+(EditorDataMesh.selectedVirtualObjects.Count>1?"s":""), DataMeshGUIStyles.toolbarButtonStyle);
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && toolbarItemRect.Contains(Event.current.mousePosition)) { for (int i = 0; i < EditorDataMesh.selectedVirtualObjects.Count; i++) { EditorDataMesh.selectedVirtualObjects[i].Clear(); } }
                else if(Event.current.type == EventType.MouseUp && Event.current.button == 1 && toolbarItemRect.Contains(Event.current.mousePosition)){ DataMeshGUIOptions.clearSelectionRightMouse = !DataMeshGUIOptions.clearSelectionRightMouse; }
            }
            else{
                GUI.Label(toolbarItemRect, "Select a virtual object first", DataMeshGUIStyles.toolbarButtonStyle);
            }

        }


        toolbarItemRect.width = 55;
        toolbarItemRect.position = new Vector2(657, 0);
        DataMeshGUIOptions.simpleToolbar = GUI.Toggle(toolbarItemRect, DataMeshGUIOptions.simpleToolbar, "Simple", DataMeshGUIStyles.toolbarButtonStyle);

        Variables.valueWindowRect = GUI.Window(0, Variables.valueWindowRect, PaintValueMiniWindow, "", DataMeshGUIStyles.toolbarButtonStyle);
        if (Variables.lastMousePosition != Event.current.mousePosition) {
            if (Variables.valueWindowPinnedRect.Contains(Event.current.mousePosition + new Vector2(0, 17)) && Variables.valueWindowRect.Contains(Event.current.mousePosition + new Vector2(0, 17))) {
                Variables.valueWindowRect = Variables.valueWindowPinnedRect;
            }            
        }

    }


    private static void DrawHelp(SceneView sceneView){
        if (DataMeshGUIOptions.showHelp) {
            Rect r = new Rect() { position = new Vector2(0, toolbarHeight), height = toolbarHeight, width = sceneView.position.width };
            GUI.Label(r, (Variables.editingDataMesh != null ? "right click in scene and/or on buttons for more actions" : "please select a datamesh to edit first (create a DataMeshObject and click edit in the inspector, or drag the DataMeshObject into the scene)"), DataMeshGUIStyles.toolbarButtonStyle);
        }
    }



    private static void RenderSelectionCircle(SceneView sceneView) {
        if (Variables.selectionPoints.Count > 1) {
            for (int i = 0; i + 1 < Variables.selectionPoints.Count; i++) {
                Ray r = HandleUtility.GUIPointToWorldRay(Variables.selectionPoints[i]);
                Vector3 p0 = r.origin + r.direction * 0.01f;
                r = HandleUtility.GUIPointToWorldRay(Variables.selectionPoints[i + 1]);
                Vector3 p1 = r.origin + r.direction * 0.01f;
                // what now?
            }
        }
    }


    private static void HandleInput(SceneView sceneView) {
        /*Mouse buttons up/down*/ {
            if (Event.current.type == EventType.MouseDown) {
                if (Event.current.button == 0) { Variables.leftMouseIsDown = true; }
            }
            else if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 0) {
                    Variables.leftMouseIsDown = false;
                    HandleVirtualObjectSelection(sceneView);
                }
            }
            if (Event.current.type == EventType.MouseDown) {
                if (Event.current.button == 1) { 
                    Variables.rightMouseIsDown = true;
                    Variables.cameraRotated = false;
                }
            }
            else if (Event.current.type == EventType.MouseUp) {
                if (Event.current.button == 1) { Variables.rightMouseIsDown = false; }
            }
            Variables.shiftIsDown = Event.current.shift;
            Variables.controlIsDown = Event.current.control;

            if (Variables.rightMouseIsDown && Vector2.Distance(Event.current.mousePosition, Variables.lastMousePosition) >= 1){
                Variables.cameraRotated = true;
            }

            if(Event.current.type == EventType.KeyUp) {
                if (Event.current.keyCode == InputOptions.paintToggleKey) {
                    if (Variables.isPainting) { EndPainting(); }
                    else if (!Variables.isPainting) { BeginPainting(); }
                    Variables.isPainting = !Variables.isPainting;
                }
                if (Event.current.keyCode == InputOptions.toolboxToggleKey){
                    DataMeshGUIOptions.showToolBox = !DataMeshGUIOptions.showToolBox;
                }
            }

        }
        /*Adding SelectionPoints*/ {
            Variables.isSelecting = Variables.leftMouseIsDown;
            if (Variables.isSelecting) {
                Variables.selectionPoints.Add(Event.current.mousePosition);
            }
        }
    }


    private static void DrawVirtualObjectGUI(SceneView sceneView) {
        if (Variables.editingDataMesh != null) {
            for (int i = 0; i < Variables.editingDataMesh.subObjects.Count; i++) {
                Vector2 iconPos = HandleUtility.WorldToGUIPoint(Variables.editingDataMesh.subObjects[i].position);

                if (SubObjectLinks.ListeningTo(Variables.editingDataMesh.subObjects[i]) != null) {
                    Vector2 icon2Pos = HandleUtility.WorldToGUIPoint(SubObjectLinks.ListeningTo(Variables.editingDataMesh.subObjects[i]).position);
                    if (
                    (iconPos.x >= 0 && iconPos.x <= sceneView.position.width && iconPos.y >= 0 && iconPos.y <= sceneView.position.height)
                    ||
                    (icon2Pos.x >= 0 && icon2Pos.x <= sceneView.position.width && icon2Pos.y >= 0 && icon2Pos.y <= sceneView.position.height)
                    ) {
                        Color was = Handles.color;
                        Handles.EndGUI();
                        Color lineColor = Color.gray;
                        lineColor.a = 0.5f;
                        Handles.color = lineColor;
                        Handles.DrawDottedLine(SubObjectLinks.ListeningTo(Variables.editingDataMesh.subObjects[i]).position, Variables.editingDataMesh.subObjects[i].position, 10f);
                        Handles.BeginGUI();
                        Handles.color = was;
                    }
                }

                if (iconPos.x >= 0 && iconPos.x <= sceneView.position.width && iconPos.y >= 0 && iconPos.y <= sceneView.position.height) {
                    float iconSize = DataMeshGUIOptions.iconSize;
                    Rect r = new Rect() { position = iconPos - new Vector2(iconSize / 2, iconSize / 2), width = iconSize, height = iconSize };
                    GUI.Label(r, DataMeshEditorTextures.dataMeshObject);
                    if (SubObjectLinks.ListeningTo(Variables.editingDataMesh.subObjects[i]) != null) {
                        r.position += new Vector2(7, 7);
                        GUI.Label(r, DataMeshEditorTextures.meshesLinkedTexture);
                    }
                }
            }
        }
    }


    private static void HandleVirtualObjectSelection(SceneView sceneView){
        if (Variables.editingDataMesh != null && !Variables.isPainting) {
            if (!Variables.inputIsUsed) {
                if (Variables.selectionPoints.Count > 2 && Variables.editingDataMesh.subObjects.Count > 0) {
                    if (!Variables.shiftIsDown && !Variables.controlIsDown) {
                        EditorDataMesh.selectedVirtualObjects.Clear();
                    }
                    Vector3 middle = new Vector3(Variables.selectionPoints[0].x, Variables.selectionPoints[0].y, 0);
                    for (int i = 1; i < Variables.selectionPoints.Count; i++) {
                        middle = (middle + new Vector3(Variables.selectionPoints[i].x, Variables.selectionPoints[i].y, 0)) / 2;
                    }
                    for (int i = 0; i + 1 < Variables.selectionPoints.Count; i++) {
                        Vector3 p0 = new Vector3(Variables.selectionPoints[i].x, Variables.selectionPoints[i].y, 0);
                        Vector3 p1 = new Vector3(Variables.selectionPoints[i + 1].x, Variables.selectionPoints[i + 1].y, 0);
                        Vector3 p2 = middle;
                        for (int o = 0; o < Variables.editingDataMesh.subObjects.Count; o++) {
                            Vector2 p = HandleUtility.WorldToGUIPoint(Variables.editingDataMesh.subObjects[o].position);
                            Vector3 pos = new Vector3(p.x, p.y, 0);
                            if (DataMesh3DMath.IsInTriangle(p0, p1, p2, pos, new Vector3(0, 0, 1))) {
                                if (!Variables.controlIsDown) {
                                    if (!EditorDataMesh.selectedVirtualObjects.Contains(Variables.editingDataMesh.subObjects[o])) {
                                        EditorDataMesh.selectedVirtualObjects.Add(Variables.editingDataMesh.subObjects[o]);
                                    }
                                }
                                else {
                                    if (EditorDataMesh.selectedVirtualObjects.Contains(Variables.editingDataMesh.subObjects[o])) {
                                        EditorDataMesh.selectedVirtualObjects.Remove(Variables.editingDataMesh.subObjects[o]);
                                    }
                                }
                            }
                        }
                    }
                    EditorDataMesh.openCreateVirtualObjectMenu = false;
                    EditorDataMesh.rightClickedVirtualObject = null;
                }
            }
        }
        Variables.selectionPoints.Clear();
    }


    private static void DrawSelectedVirtualObjectHandles(SceneView sceneView){
        Handles.EndGUI();
        EditorGUI.BeginChangeCheck();
        for(int i=0; i<EditorDataMesh.selectedVirtualObjects.Count; i++){
            if (Tools.current == Tool.Move) {
                EditorDataMesh.selectedVirtualObjects[i].position = Handles.PositionHandle(EditorDataMesh.selectedVirtualObjects[i].position, (Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : EditorDataMesh.selectedVirtualObjects[i].rotation));
                Handles.Label(EditorDataMesh.selectedVirtualObjects[i].position, EditorDataMesh.selectedVirtualObjects[i].position.x.ToString().Replace(",",".") + ", " + EditorDataMesh.selectedVirtualObjects[i].position.y.ToString().Replace(",", ".") + ", " + EditorDataMesh.selectedVirtualObjects[i].position.z.ToString().Replace(",", "."));
            }
            else if(Tools.current == Tool.Rotate){
                EditorDataMesh.selectedVirtualObjects[i].rotation = Handles.RotationHandle(EditorDataMesh.selectedVirtualObjects[i].rotation, EditorDataMesh.selectedVirtualObjects[i].position);
                Handles.Label(EditorDataMesh.selectedVirtualObjects[i].position, EditorDataMesh.selectedVirtualObjects[i].rotation.eulerAngles.x.ToString().Replace(",", ".") + ", " + EditorDataMesh.selectedVirtualObjects[i].rotation.eulerAngles.y.ToString().Replace(",", ".") + ", " + EditorDataMesh.selectedVirtualObjects[i].rotation.eulerAngles.z.ToString().Replace(",", "."));
            }
            else if(Tools.current == Tool.None){
                Handles.Label(EditorDataMesh.selectedVirtualObjects[i].position, "selected");
            }
        }
        if(EditorGUI.EndChangeCheck()){
            Variables.inputIsUsed = true;
        }
        Handles.BeginGUI();
        if(EditorDataMesh.selectedVirtualObjects.Count == 1){
            Rect r = new Rect() { position = new Vector2(5, sceneView.position.height - (14*2) - 6), height = 14, width = 110 };
            EditorDataMesh.selectedVirtualObjects[0].position.x = EditorGUI.FloatField(r, EditorDataMesh.selectedVirtualObjects[0].position.x, DataMeshGUIStyles.toolbarTextFieldStyle);
            r.position += new Vector2(115, 0);
            EditorDataMesh.selectedVirtualObjects[0].position.y = EditorGUI.FloatField(r, EditorDataMesh.selectedVirtualObjects[0].position.y, DataMeshGUIStyles.toolbarTextFieldStyle);
            r.position += new Vector2(115, 0);
            EditorDataMesh.selectedVirtualObjects[0].position.z = EditorGUI.FloatField(r, EditorDataMesh.selectedVirtualObjects[0].position.z, DataMeshGUIStyles.toolbarTextFieldStyle);
        }
    }


    private static void GetRightClickedVirtualObject(SceneView sceneView){
        if (Variables.editingDataMesh != null) {
            if (!Variables.inputIsUsed) {
                if (Event.current.type == EventType.MouseUp) {
                    if (Event.current.button == 1 && !Variables.cameraRotated) {
                        EditorDataMesh.rightClickedVirtualObject = null;
                        for (int i = 0; i < Variables.editingDataMesh.subObjects.Count; i++) {
                            Vector2 iconPos = HandleUtility.WorldToGUIPoint(Variables.editingDataMesh.subObjects[i].position);
                            if (iconPos.x >= 0 && iconPos.x <= sceneView.position.width && iconPos.y >= 0 && iconPos.y <= sceneView.position.height) {
                                float iconSize = DataMeshGUIOptions.iconSize;
                                Rect r = new Rect() { position = iconPos - new Vector2(iconSize / 2, iconSize / 2), width = iconSize, height = iconSize };
                                if (r.Contains(Event.current.mousePosition)) {
                                    EditorDataMesh.rightClickedVirtualObject = Variables.editingDataMesh.subObjects[i];
                                    EditorDataMesh.openCreateVirtualObjectMenu = false;
                                    break;
                                }
                            }
                        }

                        if (EditorDataMesh.rightClickedVirtualObject == null) {
                            EditorDataMesh.openCreateVirtualObjectMenu = !EditorDataMesh.openCreateVirtualObjectMenu && !Variables.cameraRotated;
                            if (EditorDataMesh.openCreateVirtualObjectMenu) {
                                EditorDataMesh.CreateVirtualObjectMenuPosition = Event.current.mousePosition;
                                RaycastHit hit;
                                if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, 1000f)) {
                                    Variables.rightClickSelectedGameObject = hit.transform.gameObject;
                                }
                                else {
                                    Variables.rightClickSelectedGameObject = null;
                                }
                            }
                            else {
                                Variables.rightClickSelectedGameObject = null;
                            }
                        }
                    }
                    else if(!Variables.inputIsUsed && !Variables.cameraRotated){
                        Variables.rightClickSelectedGameObject = null;
                        EditorDataMesh.openCreateVirtualObjectMenu = false;
                    }
                }
            }
        }
        if(!Variables.inputIsUsed){ 
            if(Event.current.type == EventType.MouseUp){ DataMeshGUIOptions.dataMeshTitleRightMouse = false; }
        }
    }


    private static void DrawRightClickedVirtualObject(SceneView sceneView){
        if(Variables.editingDataMesh != null){
            if (EditorDataMesh.rightClickedVirtualObject != null) {
                Rect r = new Rect() { position = HandleUtility.WorldToGUIPoint(EditorDataMesh.rightClickedVirtualObject.position) + new Vector2(15, 0), height = 14, width = 160 };
                if (GUI.Button(r, "Select", DataMeshGUIStyles.toolbarButtonStyle)) {
                    EditorDataMesh.selectedVirtualObjects.Clear();
                    EditorDataMesh.selectedVirtualObjects.Add(EditorDataMesh.rightClickedVirtualObject);
                    EditorDataMesh.rightClickedVirtualObject = null;
                    return;
                }
                r.position += new Vector2(0, 15);
                if (GUI.Button(r, "Duplicate", DataMeshGUIStyles.toolbarButtonStyle)) {
                    if (EditorDataMesh.selectedVirtualObjects.Count > 0) {
                        Undo.RecordObject(Variables.editingDataMesh, "Virtual object duplication");
                        for (int i = 0; i < EditorDataMesh.selectedVirtualObjects.Count; i++) {
                            DataMeshObject.DataMeshSubObject obj = new DataMeshObject.DataMeshSubObject() { position = EditorDataMesh.selectedVirtualObjects[i].position, rotation = EditorDataMesh.selectedVirtualObjects[i].rotation };
                            obj.position += new Vector3(0, 0.1f, 0);
                            Variables.editingDataMesh.subObjects.Add(obj);
                            EditorDataMesh.rightClickedVirtualObject = null;
                            SubObjectLinks.CopyLinksFrom(EditorDataMesh.selectedVirtualObjects[i], obj);
                        }
                    }
                    else {
                        Undo.RecordObject(Variables.editingDataMesh, "Virtual object duplication");
                        DataMeshObject.DataMeshSubObject obj = new DataMeshObject.DataMeshSubObject() { position = EditorDataMesh.rightClickedVirtualObject.position, rotation = EditorDataMesh.rightClickedVirtualObject.rotation };
                        obj.position += new Vector3(0, 0.1f, 0);
                        Variables.editingDataMesh.subObjects.Add(obj);
                        EditorDataMesh.rightClickedVirtualObject = null;
                        SubObjectLinks.CopyLinksFrom(EditorDataMesh.selectedVirtualObjects[0], obj);
                    }
                    return;
                }
                if (EditorDataMesh.selectedVirtualObjects.Count == 1 && EditorDataMesh.selectedVirtualObjects[0] != EditorDataMesh.rightClickedVirtualObject) {
                    r.position += new Vector2(0, 15);
                    if (GUI.Button(r, "Give control to selection", DataMeshGUIStyles.toolbarButtonStyle)) {
                        Undo.RecordObject(Variables.editingDataMesh, "Virtual object give control");
                        SubObjectLinks.TakeControlOver(EditorDataMesh.selectedVirtualObjects[0], EditorDataMesh.rightClickedVirtualObject);
                        EditorDataMesh.rightClickedVirtualObject = null;
                        return;
                    }
                }
                if (SubObjectLinks.ListeningTo(EditorDataMesh.rightClickedVirtualObject) != null) {
                    r.position += new Vector2(0, 15);
                    if (GUI.Button(r, "Link both ways", DataMeshGUIStyles.toolbarButtonStyle)) {
                        Undo.RecordObject(Variables.editingDataMesh, "Virtual object link both ways");
                        SubObjectLinks.TakeControlOver(EditorDataMesh.rightClickedVirtualObject, SubObjectLinks.ListeningTo(EditorDataMesh.rightClickedVirtualObject));
                        EditorDataMesh.rightClickedVirtualObject = null;
                        return;
                    }
                }
                if (SubObjectLinks.ListeningTo(EditorDataMesh.rightClickedVirtualObject) != null) {
                    r.position += new Vector2(0, 15);
                    if (GUI.Button(r, "Detach from controller", DataMeshGUIStyles.toolbarButtonStyle)) {
                        Undo.RecordObject(Variables.editingDataMesh, "Virtual object detach from controller");
                        SubObjectLinks.RemoveControlFrom(SubObjectLinks.ListeningTo(EditorDataMesh.rightClickedVirtualObject), EditorDataMesh.rightClickedVirtualObject);
                        EditorDataMesh.rightClickedVirtualObject = null;
                        return;
                    }
                }
                r.position += new Vector2(0, 15);
                if (GUI.Button(r, "Clear", DataMeshGUIStyles.toolbarButtonStyle)) {
                    Undo.RecordObject(Variables.editingDataMesh, "Virtual object clear");
                    if (EditorDataMesh.selectedVirtualObjects.Count > 0) {
                        for (int i = 0; i < EditorDataMesh.selectedVirtualObjects.Count; i++) {
                            EditorDataMesh.selectedVirtualObjects[i].Clear();
                        }
                    }
                    else {
                        EditorDataMesh.rightClickedVirtualObject.Clear();
                    }
                    EditorDataMesh.rightClickedVirtualObject = null;
                    return;
                }
                r.position += new Vector2(0, 15);
                if (GUI.Button(r, "Delete", DataMeshGUIStyles.toolbarButtonStyle)) {
                    Undo.RecordObject(Variables.editingDataMesh, "Virtual object delete");
                    if (EditorDataMesh.selectedVirtualObjects.Count > 0) {
                        for (int i = EditorDataMesh.selectedVirtualObjects.Count - 1; i >= 0; i--) {
                            var a = EditorDataMesh.selectedVirtualObjects[i];
                            Variables.editingDataMesh.subObjects.Remove(a);
                            EditorDataMesh.selectedVirtualObjects.Remove(a);
                        }
                    }
                    else {
                        Variables.editingDataMesh.subObjects.Remove(EditorDataMesh.rightClickedVirtualObject);
                    }
                    SubObjectLinks.Delete(EditorDataMesh.rightClickedVirtualObject);
                    EditorDataMesh.rightClickedVirtualObject = null;
                    for (int i = 0; i < Variables.editingDataMesh.subObjects.Count; i++) {
                        SubObjectLinks.Delete(Variables.editingDataMesh.subObjects[i]);
                    }
                    return;
                }
                
            }
            else if (EditorDataMesh.openCreateVirtualObjectMenu) {
                Rect r = new Rect() { position = EditorDataMesh.CreateVirtualObjectMenuPosition + new Vector2(15, 0), height = 14, width = 160 };
                if (GUI.Button(r, "Create Virtual Object", DataMeshGUIStyles.toolbarButtonStyle)) {
                    Undo.RecordObject(Variables.editingDataMesh, "Virtual object create");
                    Ray ray = HandleUtility.GUIPointToWorldRay(EditorDataMesh.CreateVirtualObjectMenuPosition);
                    DataMeshObject.DataMeshSubObject obj = new DataMeshObject.DataMeshSubObject() { position = ray.origin + ray.direction * 5f };
                    Variables.editingDataMesh.subObjects.Add(obj);
                    EditorDataMesh.openCreateVirtualObjectMenu = false;
                }
                if (EditorDataMesh.selectedVirtualObjects.Count == 1) {
                    r.position += new Vector2(0, 15);
                    if (GUI.Button(r, "Create Linked Virtual Object", DataMeshGUIStyles.toolbarButtonStyle)) {
                        Undo.RecordObject(Variables.editingDataMesh, "Virtual obejct create linked");
                        Ray ray = HandleUtility.GUIPointToWorldRay(EditorDataMesh.CreateVirtualObjectMenuPosition);
                        DataMeshObject.DataMeshSubObject obj = new DataMeshObject.DataMeshSubObject() { position = ray.origin + ray.direction * 5f };
                        Variables.editingDataMesh.subObjects.Add(obj);
                        EditorDataMesh.openCreateVirtualObjectMenu = false;
                        SubObjectLinks.TakeControlOver(EditorDataMesh.selectedVirtualObjects[0], obj);
                    }
                }
                if (Variables.rightClickSelectedGameObject != null) {
                    r.position += new Vector2(0, 15);
                    if (GUI.Button(r, "Select this GameObject", DataMeshGUIStyles.toolbarButtonStyle)) {
                        Selection.activeGameObject = Variables.rightClickSelectedGameObject;
                        Variables.rightClickSelectedGameObject = null;
                        EditorDataMesh.openCreateVirtualObjectMenu = false;
                    }
                }
                if (Selection.activeGameObject != null) {
                    r.position += new Vector2(0, 15);
                    if (GUI.Button(r, "Deselect all GameObjects", DataMeshGUIStyles.toolbarButtonStyle)) {
                        Selection.activeGameObject = null;
                        Variables.rightClickSelectedGameObject = null;
                        EditorDataMesh.openCreateVirtualObjectMenu = false;
                    }
                }
            }
        }
    }


    private static void DrawDataMeshTitleDropDown(SceneView sceneView){
        if (DataMeshGUIOptions.dataMeshTitleRightMouse) {
            int itemNum = 3;
            Rect dropDownBox = new Rect() { height = itemNum * (toolbarHeight+2) + 4, position = new Vector2(7 + 2, toolbarHeight - 2), width = 241 - 4 };
            GUI.Label(dropDownBox, "", DataMeshGUIStyles.toolbarStyle);
            {
                Rect itemRect = new Rect() { position = new Vector2(7, toolbarHeight + 2), height = toolbarHeight, width = 241 };
                if(GUI.Button(itemRect, "currently editing: " + (Variables.editingDataMesh != null ? Variables.editingDataMesh.dataMeshName : "none"), DataMeshGUIStyles.toolbarButtonStyle)){
                    EditorGUIUtility.PingObject(Variables.editingDataMesh);
                }
                itemRect.position += new Vector2(0, toolbarHeight + 2);
                if(GUI.Button(itemRect, "DataMesh scene manager", DataMeshGUIStyles.toolbarButtonStyle)){
                    EditorWindow.GetWindow<DataMeshEditorWindow>();
                }
                itemRect.position += new Vector2(0, toolbarHeight + 2);
                if(GUI.Button(itemRect, (Variables.editingDataMesh != null ? "Close currently editing" : "Please select a DataMesh to edit"), DataMeshGUIStyles.toolbarButtonStyle)){
                    Variables.editingDataMesh = null;
                    Variables.isPainting = false;
                    DataMeshGUIOptions.dataMeshTitleRightMouse = false;
                    SceneViewToolbarExtensionManager.RequestToggle<DataMeshToolbar>(false);
                }
            }
        }
    }

    private static void DrawPaintSelectedDropDown(SceneView sceneView){
        if(DataMeshGUIOptions.paintSelectionRightMouse && EditorDataMesh.selectedVirtualObjects.Count > 0){
            Rect toolbarItemRect = new Rect() { height = toolbarHeight };
            toolbarItemRect.width = (!DataMeshGUIOptions.simpleToolbar ? 122 : 194);
            toolbarItemRect.position = new Vector2((!DataMeshGUIOptions.simpleToolbar ? 413 : 257), toolbarHeight);
            if(GUI.Button(toolbarItemRect, "Paint all triangles from selection", DataMeshGUIStyles.toolbarButtonStyle)){
                Debug.Log("To be added");
            }
            toolbarItemRect.position += new Vector2(0, toolbarHeight);
            if (GUI.Button(toolbarItemRect, "Paint all clear tris from selection", DataMeshGUIStyles.toolbarButtonStyle)){
                Debug.Log("To be added");
            }
        }
    }

    private static void DrawClearSelectedDropDown(SceneView sceneView){
        if(DataMeshGUIOptions.clearSelectionRightMouse && EditorDataMesh.selectedVirtualObjects.Count > 0) {
            Rect toolbarItemRect = new Rect() { height = toolbarHeight };
            toolbarItemRect.width = (!DataMeshGUIOptions.simpleToolbar ? 122 : 194);
            toolbarItemRect.position = new Vector2((!DataMeshGUIOptions.simpleToolbar ? 535 : 457), toolbarHeight);
            if (GUI.Button(toolbarItemRect, "Clear all tris with selected value", DataMeshGUIStyles.toolbarButtonStyle)) {
                for(int i=0; i<EditorDataMesh.selectedVirtualObjects.Count; i++){
                    EditorDataMesh.selectedVirtualObjects[i].ClearValues(Variables.selectedPaintValue);
                }
            }
        }
    }



    private static void BeginPainting(){
        Selection.selectionChanged -= SelectionChangedWhilePainting;
        Selection.selectionChanged += SelectionChangedWhilePainting;
        ChangePaintableVirtualTriangles();
        Tools.current = Tool.None;
        EnableRenderersForPainting();
        UpdateRenderersForPainting(Variables.editingDataMesh.subObjects.ToArray());
    }

    private static void EndPainting(){
        Selection.selectionChanged -= SelectionChangedWhilePainting;
        if (Painting.virtualTriangles.Count > 0 && EditorDataMesh.selectedVirtualObjects.Count > 0) {
            Undo.RecordObject(Variables.editingDataMesh, "Pushed triangles");
            for (int i = 0; i < EditorDataMesh.selectedVirtualObjects.Count; i++) {
                if (EditorDataMesh.selectedVirtualObjects[i].triangles.Count < 10000) {
                    Undo.RecordObject(Variables.editingDataMesh, "Pushed triangles");
                    EditorDataMesh.selectedVirtualObjects[i].PushTriangles(Painting.virtualTriangles.ToArray());
                }
                else {
                    Undo.RecordObject(Variables.editingDataMesh, "Pushed triangles");
                    EditorDataMesh.selectedVirtualObjects[i].PushTrianglesWithoutCheck(Painting.virtualTriangles.ToArray());
                }
            }
            Painting.virtualTriangles.Clear();
        }
        DisableRenderersForPainting();
    }

    private static void SelectionChangedWhilePainting(){
        ChangePaintableVirtualTriangles();
    }

    private static void Paint(){
        if(EditorDataMesh.selectedVirtualObjects.Count == 0){ EndPainting(); Variables.isPainting = false; }
        if (Variables.isPainting) {
            
            if ((Vector2.Distance(Event.current.mousePosition, Variables.lastMousePosition) > 0 && Variables.leftMouseIsDown) || (Event.current.type == EventType.MouseUp && Event.current.button == 0)) {
                Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                var hitTriangle = DataMesh3DMath.GetFirstHitTriangle(r.origin, r.direction, Painting.virtualTriangles.ToArray());
                if (hitTriangle != null) {

                    Color colwas = Handles.color;
                    Handles.color = Color.Lerp(Color.yellow, Color.red, 0.6f);
                    Handles.EndGUI();
                    Handles.DrawLine(hitTriangle.p0, hitTriangle.p1);
                    Handles.DrawLine(hitTriangle.p1, hitTriangle.p2);
                    Handles.DrawLine(hitTriangle.p2, hitTriangle.p0);
                    Handles.BeginGUI();
                    Handles.color = colwas;

                    int valueWas = hitTriangle.value;
                    hitTriangle.value = Variables.selectedPaintValue;

                    if (valueWas != hitTriangle.value) {
                        List<DataMeshObject.DataMeshSubObject> alreadyPushed = new List<DataMeshObject.DataMeshSubObject>();
                        for (int i = 0; i < EditorDataMesh.selectedVirtualObjects.Count; i++) {
                            if (!alreadyPushed.Contains(EditorDataMesh.selectedVirtualObjects[i])) {
                                EditorDataMesh.selectedVirtualObjects[i].PushTriangles(new DataMeshTriangle[] { hitTriangle });
                                alreadyPushed.Add(EditorDataMesh.selectedVirtualObjects[i]);
                            }

                            List<DataMeshObject.DataMeshSubObject> pnt = new List<DataMeshObject.DataMeshSubObject>(EditorDataMesh.selectedVirtualObjects);

                            if(SubObjectLinks.control_overs.ContainsKey(EditorDataMesh.selectedVirtualObjects[i])){
                                foreach(var o in SubObjectLinks.control_overs[EditorDataMesh.selectedVirtualObjects[i]]){
                                    if (!alreadyPushed.Contains(o)) {
                                        Vector3 p0 = hitTriangle.p0-EditorDataMesh.selectedVirtualObjects[i].position + o.position;
                                        Vector3 p1 = hitTriangle.p1-EditorDataMesh.selectedVirtualObjects[i].position + o.position;
                                        Vector3 p2 = hitTriangle.p2-EditorDataMesh.selectedVirtualObjects[i].position + o.position;
                                        o.PushTriangles(new DataMeshTriangle[] { new DataMeshTriangle(p0,p1,p2, hitTriangle.value) });
                                        alreadyPushed.Add(o);
                                        pnt.Add(o);
                                    }
                                }
                            }

                            UpdateRenderersForPainting(pnt.ToArray());

                        }
                    }

                }
            }
            else {

                Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                var hitTriangle = DataMesh3DMath.GetFirstHitTriangle(r.origin, r.direction, Painting.virtualTriangles.ToArray());
                if (hitTriangle != null) {

                    Color colwas = Handles.color;
                    Handles.color = Color.Lerp(Color.yellow, Color.red, 0.6f);
                    Handles.EndGUI();
                    Handles.DrawLine(hitTriangle.p0, hitTriangle.p1);
                    Handles.DrawLine(hitTriangle.p1, hitTriangle.p2);
                    Handles.DrawLine(hitTriangle.p2, hitTriangle.p0);
                    Handles.BeginGUI();
                    Handles.color = colwas;

                }

            }

        }
    }

    private static void ChangePaintableVirtualTriangles(){
        if(Painting.virtualTriangles.Count > 0 && EditorDataMesh.selectedVirtualObjects.Count > 0){
            for(int i=0; i<EditorDataMesh.selectedVirtualObjects.Count; i++){
                if (EditorDataMesh.selectedVirtualObjects[i].triangles.Count < 10000) {
                    EditorDataMesh.selectedVirtualObjects[i].PushTriangles(Painting.virtualTriangles.ToArray());
                }
                else{
                    EditorDataMesh.selectedVirtualObjects[i].PushTrianglesWithoutCheck(Painting.virtualTriangles.ToArray());
                }
            }
            Painting.virtualTriangles.Clear();
        }
        List<GameObject> selected = new List<GameObject>();
        foreach(var a in Selection.objects){ if (a.GetType() == typeof(GameObject)) { if(((GameObject)a).activeInHierarchy && ((GameObject)a).GetComponent<MeshFilter>() != null){ selected.Add((GameObject)a); } } }
        for(int i=0; i<selected.Count; i++){
            Mesh m = selected[i].GetComponent<MeshFilter>().sharedMesh;
            for(int t=0; t+2<m.triangles.Length; t+=3){
                Vector3 vert0 = selected[i].transform.TransformPoint(m.vertices[m.triangles[t]]);
                Vector3 vert1 = selected[i].transform.TransformPoint(m.vertices[m.triangles[t+1]]);
                Vector3 vert2 = selected[i].transform.TransformPoint(m.vertices[m.triangles[t+2]]);
                Painting.virtualTriangles.Add(new DataMeshTriangle(vert0, vert1, vert2, -1));
            }
        }
        if(selected.Count == 0){ EndPainting(); }
    }


    private static void UpdateRenderersForPainting(params DataMeshObject.DataMeshSubObject[] objs){

        Dictionary<int, List<Vector3>> vals = new Dictionary<int, List<Vector3>>();

        for(int i=0; i<objs.Length; i++){
            for(int o=0; o<objs[i].triangles.Count; o++){
                if(!vals.ContainsKey(objs[i].triangles[o].value)){
                    vals.Add(objs[i].triangles[o].value, new List<Vector3>());
                }
                vals[objs[i].triangles[o].value].Add(objs[i].ToWorld(objs[i].triangles[o].p0));
                vals[objs[i].triangles[o].value].Add(objs[i].ToWorld(objs[i].triangles[o].p1));
                vals[objs[i].triangles[o].value].Add(objs[i].ToWorld(objs[i].triangles[o].p2));
            }
        }

        foreach(var pair in vals){

            if(!Painting.paintingRenderMaterials.ContainsKey(pair.Key)){
                Shader shader = Shader.Find("Tools/DataMesh/DataMeshEditorVisualisation");
                Painting.paintingRenderMaterials.Add(pair.Key, new Material(shader));
            }

            if(!Painting.renderers.ContainsKey(pair.Key)){
                Painting.renderers.Add(pair.Key, new Painting.PaintingRenderHolder());
                GameObject obj = null;
                for(int i=0; i<Painting.RenderHolder.transform.childCount; i++){ if (Painting.RenderHolder.transform.GetChild(i).name == "Renderer_" + pair.Key) { obj = Painting.RenderHolder.transform.GetChild(i).gameObject; break; } }
                if (obj == null) {
                    obj = new GameObject("Renderer_" + pair.Key);
                    obj.transform.position = Vector3.zero;
                    obj.transform.parent = Painting.RenderHolder.transform;
                }
                Painting.renderers[pair.Key].mesh = new Mesh();
                Painting.renderers[pair.Key].meshFilter = (obj.GetComponent<MeshFilter>() == null ? obj.AddComponent<MeshFilter>() : obj.GetComponent<MeshFilter>());
                Painting.renderers[pair.Key].meshRenderer = (obj.GetComponent<MeshRenderer>() == null ? obj.AddComponent<MeshRenderer>() : obj.GetComponent<MeshRenderer>());
                Painting.renderers[pair.Key].meshFilter.sharedMesh = Painting.renderers[pair.Key].mesh;
                Painting.renderers[pair.Key].meshRenderer.sharedMaterial = Painting.paintingRenderMaterials[pair.Key]; //Painting.paintingRendererSharedMaterial;
            }
            if(Painting.renderers[pair.Key].meshRenderer == null){
                GameObject obj = null;
                for (int i = 0; i < Painting.RenderHolder.transform.childCount; i++) { if (Painting.RenderHolder.transform.GetChild(i).name == "Renderer_" + pair.Key) { obj = Painting.RenderHolder.transform.GetChild(i).gameObject; break; } }
                if (obj == null) {
                    obj = new GameObject("Renderer_" + pair.Key);
                    obj.transform.position = Vector3.zero;
                    obj.transform.parent = Painting.RenderHolder.transform;
                }
                obj.transform.position = Vector3.zero;
                obj.transform.parent = Painting.RenderHolder.transform;
                Painting.renderers[pair.Key].meshFilter = (obj.GetComponent<MeshFilter>() == null ? obj.AddComponent<MeshFilter>() : obj.GetComponent<MeshFilter>());
                Painting.renderers[pair.Key].meshRenderer = (obj.GetComponent<MeshRenderer>() == null ? obj.AddComponent<MeshRenderer>() : obj.GetComponent<MeshRenderer>());
                Painting.renderers[pair.Key].meshFilter.sharedMesh = Painting.renderers[pair.Key].mesh;
                Painting.renderers[pair.Key].meshRenderer.sharedMaterial = Painting.paintingRenderMaterials[pair.Key]; //Painting.paintingRendererSharedMaterial;
            }
            {
                Painting.renderers[pair.Key].meshRenderer.sharedMaterial.SetColor("_Color", Variables.editingDataMesh.GetEditorRenderColorFromValue(pair.Key));
                Painting.renderers[pair.Key].meshRenderer.sharedMaterial.SetInt("_IsSelected", (Variables.selectedPaintValue == pair.Key ? 1 : 0));

                Painting.renderers[pair.Key].mesh.Clear();
                if (Painting.renderers[pair.Key].meshFilter.sharedMesh != Painting.renderers[pair.Key].mesh){
                    Painting.renderers[pair.Key].meshFilter.sharedMesh = Painting.renderers[pair.Key].mesh;
                }

                List<int> tris = new List<int>();
                for(int i=0; i<vals[pair.Key].Count; i++){
                    tris.Add(i);
                }
                Painting.renderers[pair.Key].mesh.vertices = vals[pair.Key].ToArray();
                Painting.renderers[pair.Key].mesh.triangles = tris.ToArray();
            }
        }

    }

    private static void DisableRenderersForPainting(){
        for(int i=0; i<Painting.RenderHolder.transform.childCount; i++){
            Painting.RenderHolder.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private static void EnableRenderersForPainting(){
        for (int i = 0; i < Painting.RenderHolder.transform.childCount; i++) {
            Painting.RenderHolder.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    private static void PaintValueMiniWindow(int i){
        if(Variables.valueWindowRect.position.y < 17){ Variables.valueWindowRect.position = new Vector2(Variables.valueWindowRect.position.x, 17); }
        if(Variables.valueWindowRect.position.y > Variables.sceneViewRect.height - toolbarHeight){ Variables.valueWindowRect.position = new Vector2(Variables.valueWindowRect.position.x, Variables.sceneViewRect.height - toolbarHeight); }
        if(Variables.valueWindowRect.position.x < 0){ Variables.valueWindowRect.position = new Vector2(0, Variables.valueWindowRect.position.y); }
        if(Variables.valueWindowRect.position.x > Variables.sceneViewRect.width - 15){ Variables.valueWindowRect.position = new Vector2(Variables.sceneViewRect.width - 15, Variables.valueWindowRect.position.y); }
        Rect itemRect = new Rect() { position = new Vector2(0, 0), width = 70, height = 14 };
        GUI.Label(itemRect, DataMeshEditorTextures.moveAble);
        itemRect.position = new Vector2(15, 0);

        if (Variables.editingDataMesh != null) {
            int valueWas = Variables.selectedPaintValue;
            Variables.selectedPaintValue = EditorGUI.IntField(itemRect, Variables.selectedPaintValue, DataMeshGUIStyles.toolbarTextFieldStyle);
            itemRect.position += new Vector2(70, 0);
            itemRect.width = 122;
            GUI.Label(itemRect, Variables.editingDataMesh.GetDescriptionFromValue(Variables.selectedPaintValue), DataMeshGUIStyles.toolbarButtonStyle);
            if(Variables.selectedPaintValue != valueWas){ 
                foreach(var pair in Painting.paintingRenderMaterials){
                    pair.Value.SetInt("_IsSelected", (Variables.selectedPaintValue == pair.Key ? 1 : 0));
                }
            }
        }
        else{
            itemRect.width = 192;
            GUI.Label(itemRect, "Please select a DataMesh to edit", DataMeshGUIStyles.toolbarButtonStyle);
        }
        GUI.DragWindow();
    }


    public static void RequestEdit(DataMeshObject target){
        if(!isOpen){ SceneViewToolbarExtensionManager.RequestToggle<DataMeshToolbar>(true); }
        Variables.editingDataMesh = target;

        if(!DataMesh.LocalSceneManager.dataMeshes.Contains(target)){
            Undo.RecordObject(DataMesh.LocalSceneManager, "added DataMesh to scene");
            DataMesh.LocalSceneManager.dataMeshes.Add(target);
        }
        EditorGUIUtility.PingObject(target);
    }

    public static bool IsEditingDataMeshObject(DataMeshObject target){
        return Variables.editingDataMesh == target;
    }




    #region Not being used anymore, different script

    /*
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void CallbackTest() {
        SceneView.onSceneGUIDelegate -= SceneViewToolbarExtension;
        SceneView.onSceneGUIDelegate += SceneViewToolbarExtension;
    }
    

    private static void SceneViewToolbarExtension(SceneView sceneView){
        Handles.BeginGUI();
        Rect toolbarToggleRect = new Rect() { position = new Vector2(257, -1), width = 25, height = 18 };
        GUI.Window(1, toolbarToggleRect, SceneViewToolbarToggle, "", EditorStyles.toolbarButton);
        Handles.EndGUI();
    }

    private static void SceneViewToolbarToggle(int i){
        bool openWas = isOpen;
        bool openIs = isOpen;
        Rect r = new Rect() { position = Vector2.zero, width = 25, height = 18 };
        openIs = GUI.Toggle(r, openWas, "DM", EditorStyles.toolbarButton);
        if(openWas != openIs){ ToggleToolbar(); }
    }
    */

    #endregion


    private static GUIStyle CloneStyle(GUIStyle original){
        GUIStyle ret = new GUIStyle();
        ret.active = original.active;
        ret.alignment = original.alignment;
        ret.border = original.border;
        ret.clipping = original.clipping;
        ret.contentOffset = original.contentOffset;
        ret.fixedHeight = 0;
        ret.fixedWidth = 0;
        ret.focused = original.focused;
        ret.font = original.font;
        ret.fontSize = original.fontSize;
        ret.fontStyle = original.fontStyle;
        ret.hover = original.hover;
        ret.imagePosition = original.imagePosition;
        ret.margin = original.margin;
        ret.name = original.name + "_clone_DataMeshToolbar";
        ret.normal = original.normal;
        ret.onActive = original.onActive;
        ret.onFocused = original.onFocused;
        ret.onHover = original.onHover;
        ret.onNormal = original.onNormal;
        ret.overflow = original.overflow;
        ret.padding = original.padding;
        ret.richText = original.richText;
        ret.stretchHeight = original.stretchHeight;
        ret.stretchWidth = original.stretchWidth;
        ret.wordWrap = original.wordWrap;
        return ret;
    }


    private static void ToolbarItemToolTip(Rect r, string text, float width){
        if (DataMeshGUIOptions.showToolbarItemToolTips) {
            if (r.Contains(Event.current.mousePosition)) {
                Rect tooltipRect = new Rect() { position = new Vector2(Event.current.mousePosition.x, toolbarHeight), width = width, height = toolbarHeight };
                GUI.Label(tooltipRect, text, DataMeshGUIStyles.toolbarButtonStyle);
            }
        }
    }


}
#endif