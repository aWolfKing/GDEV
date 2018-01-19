#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;


public class SceneViewToolbarExtensionManager : EditorWindow {

    private class SceneViewToolBarExtensionObj{
        public System.Type type = null;
        public SceneViewToolbarExtensionOptions sceneViewToolbarExtensionOptions;
        public bool active = false;
        public SceneViewToolBarExtensionObj(System.Type t, SceneViewToolbarExtensionOptions o){ this.type = t; this.sceneViewToolbarExtensionOptions = o; }
        public MethodInfo[] button = null;
        public MethodInfo[] toggleEnabled = null;
        public MethodInfo[] toggleDisabled = null;
        public void ToggleEnabled(){
            for(int i=0; i<toggleEnabled.Length; i++){ toggleEnabled[i].Invoke(null, new object[] { }); }
            active = true;
        }
        public void ToggleDisabled() {
            for (int i = 0; i < toggleDisabled.Length; i++) { toggleDisabled[i].Invoke(null, new object[] { }); }
            active = false;
        }
        public int SelectedOption = 0;

        public class DropDownOption{
            public MethodInfo method;
            public string optionName = "option";
            public Texture texture = null;
            public int position = 0;
            public string toolTip = "";
            public DropDownOption(MethodInfo method, string optionName, int position){ 
                this.method = method;
                this.texture = GetIconFromIconsFolder(optionName);
                this.optionName = optionName;
                this.position = position;
            }
            public DropDownOption(MethodInfo method, string optionName, int position, string toolTip) {
                this.method = method;
                this.texture = GetIconFromIconsFolder(optionName);
                this.optionName = optionName;
                this.position = position;
                this.toolTip = toolTip;
            }
        }
        public DropDownOption[] dropDownOptions = null;
    }

    private static List<SceneViewToolBarExtensionObj> extensions = new List<SceneViewToolBarExtensionObj>();
    private static readonly int rectToolbarOffsetLeft = 257;
    private static readonly int rectToolbarOffsetRight = 274;
    private static int itemWidth = 25;
    private static int itemOffset = 4;

    [InitializeOnLoadMethod]
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Manage(){
        extensions.Clear();
        System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        for(int i=0; i<types.Length; i++){
            //.net 4.6
            //SceneViewToolbarExtension extension = types[i].GetCustomAttribute<SceneViewToolbarExtension>();
            //older .net
            SceneViewToolbarExtension[] possibleExtensions = (SceneViewToolbarExtension[])types[i].GetCustomAttributes(typeof(SceneViewToolbarExtension), false);
            SceneViewToolbarExtension extension = null;
            if(possibleExtensions.Length > 0){ extension = possibleExtensions[0]; }
            //end
            if(extension != null){
                if (extensions.Count > 0) {
                    bool isAdded = false;
                    for (int o = 0; o < extensions.Count; o++) {
                        if (extensions[o].sceneViewToolbarExtensionOptions.requestedPosition > extension.Options.requestedPosition) {
                            extensions.Insert(o, new SceneViewToolBarExtensionObj(types[i], extension.Options));
                            isAdded = true;
                            break;
                        }
                    }
                    if (!isAdded) {
                        extensions.Add(new SceneViewToolBarExtensionObj(types[i], extension.Options));
                    }
                }
                else {
                    extensions.Add(new SceneViewToolBarExtensionObj(types[i], extension.Options));
                }
            }
            #region Obsolete
            /*
            else if(types[i].GetInterface(typeof(ISceneViewToolbarExtension).ToString()) != null){
                //MonoBehaviour.print(types[i].Name);
                ISceneViewToolbarExtension a = (ISceneViewToolbarExtension)System.Activator.CreateInstance(types[i]);
                
                if(extensions.Count > 0){
                    for(int o=0; o<extensions.Count; o++){
                        if(extensions[o].sceneViewToolbarExtensionOptions.requestedPosition > a.sceneViewToolbarExtensionOptions.requestedPosition){
                            extensions.Insert(o, new SceneViewToolBarExtensionObj(types[i], a.sceneViewToolbarExtensionOptions));
                            break;
                        }
                    }
                    extensions.Add(new SceneViewToolBarExtensionObj(types[i], a.sceneViewToolbarExtensionOptions));
                }
                else{
                    extensions.Add(new SceneViewToolBarExtensionObj(types[i], a.sceneViewToolbarExtensionOptions));
                }
            }
            */
            #endregion
        }
        for(int i=0; i<extensions.Count; i++){
            MethodInfo[] methods = extensions[i].type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            List<MethodInfo> buttons = new List<MethodInfo>();
            List<MethodInfo> toggleEnabled = new List<MethodInfo>();
            List<MethodInfo> toggleDisabled = new List<MethodInfo>();
            //List<MethodInfo> dropDownOptions = new List<MethodInfo>();
            List<SceneViewToolBarExtensionObj.DropDownOption> dropDownOptions = new List<SceneViewToolBarExtensionObj.DropDownOption>();
            if(extensions[i].sceneViewToolbarExtensionOptions.extensionType == SceneViewToolbarExtensionType.Button){
                for(int o=0; o<methods.Length; o++){
                    //.net 4.6
                    //SceneViewToolbarButton button = methods[o].GetCustomAttribute<SceneViewToolbarButton>();
                    //older .net
                    SceneViewToolbarButton[] possibleButtons = (SceneViewToolbarButton[])methods[o].GetCustomAttributes(typeof(SceneViewToolbarButton), false);
                    SceneViewToolbarButton button = null;
                    if(possibleButtons.Length > 0){ button = possibleButtons[0]; }
                    //end
                    if(button != null && methods[o].GetParameters().Length == 0){
                        buttons.Add(methods[o]);
                    }
                }
                if(buttons.Count == 0){
                    Debug.LogWarning("'" + extensions[i].type.Name + "' expected a parameterless static method with the 'SceneViewToolbarButton' attribute.");
                }
                else{
                    extensions[i].button = buttons.ToArray();
                }
            }
            else if(extensions[i].sceneViewToolbarExtensionOptions.extensionType == SceneViewToolbarExtensionType.Toggle){
                for (int o = 0; o < methods.Length; o++) {
                    //.net 4.6
                    //SceneViewToolbarToggleEnable enabled = methods[o].GetCustomAttribute<SceneViewToolbarToggleEnable>();
                    //SceneViewToolbarToggleDisable disabled = methods[o].GetCustomAttribute<SceneViewToolbarToggleDisable>();
                    //older .net
                    SceneViewToolbarToggleEnable[] possibleEnable = (SceneViewToolbarToggleEnable[])methods[o].GetCustomAttributes(typeof(SceneViewToolbarToggleEnable), false);
                    SceneViewToolbarToggleDisable[] possibleDisable = (SceneViewToolbarToggleDisable[])methods[o].GetCustomAttributes(typeof(SceneViewToolbarToggleDisable), false);
                    SceneViewToolbarToggleEnable enabled = null;
                    SceneViewToolbarToggleDisable disabled = null;
                    if(possibleEnable.Length > 0){ enabled = possibleEnable[0]; }
                    if(possibleDisable.Length > 0){ disabled = possibleDisable[0]; }
                    //end
                    if (enabled != null && methods[o].GetParameters().Length == 0) {
                        toggleEnabled.Add(methods[o]);
                    }
                    if (disabled != null && methods[o].GetParameters().Length == 0) {
                        toggleDisabled.Add(methods[o]);
                    }
                }
                if(toggleEnabled.Count == 0){
                    Debug.LogWarning("'" + extensions[i].type.Name + "' expected a parameterless static method with the 'SceneViewToolbarToggleEnable' attribute.");
                }
                else{
                    extensions[i].toggleEnabled = toggleEnabled.ToArray();
                }
                if (toggleDisabled.Count == 0) {
                    Debug.LogWarning("'" + extensions[i].type.Name + "' expected a parameterless static method with the 'SceneViewToolbarToggleDisable' attribute.");
                }
                else{
                    extensions[i].toggleDisabled = toggleDisabled.ToArray();
                }
            }
            else if(extensions[i].sceneViewToolbarExtensionOptions.extensionType == SceneViewToolbarExtensionType.DropDown){
                for(int o=0; o<methods.Length; o++){
                    SceneViewToolbarDropdownOption[] possibleOptions = (SceneViewToolbarDropdownOption[])methods[o].GetCustomAttributes(typeof(SceneViewToolbarDropdownOption));
                    SceneViewToolbarDropdownOption option = null;
                    if(possibleOptions.Length > 0){ option = possibleOptions[0]; }
                    if(option != null && methods[o].GetParameters().Length == 0){
                        //dropDownOptions.Add(methods[o]);
                        dropDownOptions.Add(new SceneViewToolBarExtensionObj.DropDownOption(methods[o], option.content, option.position, option.toolTip));
                    }
                }
                if(dropDownOptions.Count == 0){
                    Debug.LogWarning("'" + extensions[i].type.Name + "' expected one or more parameterless static methods with the 'SceneViewToolbarDropdownOption' attribute.");
                }
                else{

                    List<SceneViewToolBarExtensionObj.DropDownOption> inOrder = new List<SceneViewToolBarExtensionObj.DropDownOption>();
                    for (int o = 0; o < dropDownOptions.Count; o++) {
                        if (inOrder.Count != 0) {
                            for (int j = 0; j < inOrder.Count; j++) {
                                if(dropDownOptions[o].position < inOrder[j].position){
                                    inOrder.Insert(j, dropDownOptions[o]);
                                    continue;
                                }
                            }
                            inOrder.Add(dropDownOptions[o]);
                        }
                        else{ inOrder.Add(dropDownOptions[o]); }
                    }
                    extensions[i].dropDownOptions = inOrder.ToArray();
                }
            }
        }
        SceneView.onSceneGUIDelegate -= RenderToolbar;
        SceneView.onSceneGUIDelegate += RenderToolbar;
    }

    private static float barWidth = 0;



    public static void RenderToolbar(SceneView sceneView){
        barWidth = sceneView.position.width - rectToolbarOffsetRight - 274;
        Rect r = new Rect() { position = new Vector2(rectToolbarOffsetLeft, -1), width = sceneView.position.width - rectToolbarOffsetRight - 274, height = 18 };
        GUI.Window(14041190, r, RenderToolBarExtensions, "", GUIStyle.none);
    }

    private static void RenderToolBarExtensions(int id){
        Rect r = new Rect() { position = Vector2.zero, height = 18, width = itemWidth };
        for (int i = 0; i < extensions.Count; i++) {

            int localwidth = (extensions[i].sceneViewToolbarExtensionOptions.requestedWidth != 0 ? extensions[i].sceneViewToolbarExtensionOptions.requestedWidth : itemWidth);
            r.width = localwidth;

            if (r.position.x + r.width <= barWidth) {
                if (extensions[i].sceneViewToolbarExtensionOptions.extensionType == SceneViewToolbarExtensionType.Button) {
                    bool wasClicked = false;
                    if (extensions[i].sceneViewToolbarExtensionOptions.icon != null) {
                        if (GUI.Button(r, extensions[i].sceneViewToolbarExtensionOptions.icon, EditorStyles.toolbarButton)) {
                            wasClicked = true;
                        }
                    }
                    else {
                        if (GUI.Button(r, extensions[i].sceneViewToolbarExtensionOptions.NameIcon, EditorStyles.toolbarButton)) {
                            wasClicked = true;
                        }
                    }
                    if (wasClicked) {
                        if (extensions[i].button.Length > 0) {
                            for (int o = 0; o < extensions[i].button.Length; o++) {
                                extensions[i].button[o].Invoke(null, new object[] { });
                            }
                        }
                    }
                }
                else if (extensions[i].sceneViewToolbarExtensionOptions.extensionType == SceneViewToolbarExtensionType.Toggle) {
                    bool wasActive = extensions[i].active;
                    bool isActive = wasActive;
                    if (extensions[i].sceneViewToolbarExtensionOptions.icon != null) {
                        isActive = GUI.Toggle(r, wasActive, extensions[i].sceneViewToolbarExtensionOptions.icon, EditorStyles.toolbarButton);
                    }
                    else {
                        isActive = GUI.Toggle(r, wasActive, extensions[i].sceneViewToolbarExtensionOptions.NameIcon, EditorStyles.toolbarButton);
                    }
                    if (wasActive && !isActive) {
                        if (extensions[i].toggleDisabled.Length > 0) {
                            for (int o = 0; o < extensions[i].toggleDisabled.Length; o++) {
                                extensions[i].toggleDisabled[o].Invoke(null, new object[] { });
                            }
                        }
                        extensions[i].active = false;
                    }
                    else if (!wasActive && isActive) {
                        if (extensions[i].toggleEnabled.Length > 0) {
                            for (int o = 0; o < extensions[i].toggleEnabled.Length; o++) {
                                extensions[i].toggleEnabled[o].Invoke(null, new object[] { });
                            }
                        }
                        extensions[i].active = true;
                    }
                }
                else if (extensions[i].sceneViewToolbarExtensionOptions.extensionType == SceneViewToolbarExtensionType.DropDown){
                    GUIContent[] content = new GUIContent[extensions[i].dropDownOptions.Length];
                    for(int o=0; o<extensions[i].dropDownOptions.Length; o++){
                        content[o] = new GUIContent(extensions[i].dropDownOptions[o].optionName);
                        if(extensions[i].dropDownOptions[o].toolTip != ""){ content[o].tooltip = extensions[i].dropDownOptions[o].toolTip; }
                        /*
                        if(extensions[i].sceneViewToolbarExtensionOptions.icon != null){
                            content[o].image = extensions[i].dropDownOptions[o].texture;
                        }
                        else{
                            content[o].text = extensions[i].dropDownOptions[o].optionName;
                        }
                        */
                    }

                    /*
                    if (extensions[i].dropDownOptions[extensions[i].SelectedOption].texture != null) {
                        content[extensions[i].SelectedOption].image = extensions[i].dropDownOptions[extensions[i].SelectedOption].texture;
                        content[extensions[i].SelectedOption].text = "";
                    }
                    else{
                        content[extensions[i].SelectedOption].image = null;
                        content[extensions[i].SelectedOption].text = extensions[i].dropDownOptions[extensions[i].SelectedOption].optionName;
                    }
                    */

                    int was = extensions[i].SelectedOption;
                    extensions[i].SelectedOption = EditorGUI.Popup(r, extensions[i].SelectedOption, content, EditorStyles.toolbarDropDown);
                    if(was != extensions[i].SelectedOption){
                        //RequestIconChange(extensions[i].type, extensions[i].dropDownOptions[extensions[i].SelectedOption].texture);
                        extensions[i].dropDownOptions[extensions[i].SelectedOption].method.Invoke(null, new object[] { });
                    }
                }
                r.position += new Vector2(/*(itemWidth)*/ localwidth + itemOffset, 0);
            }
            else{
                break;
            }
        }

    }



    public static void RequestToggle<T>(bool value){
        for(int i=0; i<extensions.Count; i++){
            if(extensions[i].type == typeof(T)){
                if(extensions[i].active && !value){
                    extensions[i].ToggleDisabled();
                }
                else if(!extensions[i].active && value){
                    extensions[i].ToggleEnabled();
                }
            }
        }
    }

    public static void RequestIconChange<T>(Texture newIcon){
        for (int i = 0; i < extensions.Count; i++) {
            if (extensions[i].type == typeof(T)) {
                extensions[i].sceneViewToolbarExtensionOptions.icon = newIcon;
            }
        }
    }

    /*
    public static void RequestIconChange(System.Type t, Texture newIcon) {
        for (int i = 0; i < extensions.Count; i++) {
            if (extensions[i].type == t) {
                extensions[i].sceneViewToolbarExtensionOptions.icon = newIcon;
            }
        }
    }
    */

    public static Texture GetIconFromIconsFolder(string iconName){
        if(!iconName.ToLower().EndsWith(".png")){ iconName = iconName + ".png"; }
        string[] paths = Directory.GetDirectories(Application.dataPath, "Icons", SearchOption.AllDirectories);
        for(int i=0; i<paths.Length; i++){
            string[] files = Directory.GetFiles(paths[i], iconName);
            for(int o=0; o<files.Length; o++){
                //MonoBehaviour.print(files[o]);
                if(files[o].ToLower().EndsWith(".png")){
                    string path = "";
                    path = files[o].Substring(files[o].IndexOf("Assets"));
                    files[o] = path;
                    Texture t = (Texture)AssetDatabase.LoadAssetAtPath((files[o].ToLower().EndsWith(".png") ? files[o] : (files[o] + ".png")), typeof(Texture));
                    if(t != null){ return t; }
                }
            }           
        }
        return null;
    }


}
#endif