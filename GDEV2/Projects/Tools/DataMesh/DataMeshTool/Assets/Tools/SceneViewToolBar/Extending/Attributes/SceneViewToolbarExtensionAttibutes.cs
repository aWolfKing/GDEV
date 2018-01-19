using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[AttributeUsage(AttributeTargets.Method)]
public class SceneViewToolbarButton : Attribute{
}

[AttributeUsage(AttributeTargets.Method)]
public class SceneViewToolbarToggleEnable : Attribute{
}

[AttributeUsage(AttributeTargets.Method)]
public class SceneViewToolbarToggleDisable : Attribute{
}

[AttributeUsage(AttributeTargets.Method)]
public class SceneViewToolbarDropdownOption : Attribute{
    public int position = 0;
    public string content = "option";
    public string toolTip = "";
    public SceneViewToolbarDropdownOption(string iconNameOrText){ this.content = iconNameOrText; }
    public SceneViewToolbarDropdownOption(int position, string text){ this.position = position; this.content = text; }
    public SceneViewToolbarDropdownOption(int position, string text, string toolTip) { this.position = position; this.content = text; this.toolTip = toolTip; }
    public SceneViewToolbarDropdownOption(string text, string toolTip) { this.content = text; this.toolTip = toolTip; }
}



[AttributeUsage(AttributeTargets.Class)]
public class SceneViewToolbarExtension : Attribute{
    private SceneViewToolbarExtensionOptions options;
    public SceneViewToolbarExtensionOptions Options{ get{ return options; } }

    /*
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, Texture icon) {
        options = new SceneViewToolbarExtensionOptions(type, 1000, icon);
    }
    */
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, char iconChar1, char iconChar2) {
        options = new SceneViewToolbarExtensionOptions(type, 1000, iconChar1, iconChar2);
    }
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, int requestedPosition, char iconChar1, char iconChar2) {
        options = new SceneViewToolbarExtensionOptions(type, requestedPosition, iconChar1, iconChar2);
    }
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, string iconName){
        options = new SceneViewToolbarExtensionOptions(type, 1000, iconName);
    }
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, int requestedPosition, string iconName) {
        options = new SceneViewToolbarExtensionOptions(type, requestedPosition, iconName);
    }

    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, char iconChar1, char iconChar2, int requestedWidth) {
        options = new SceneViewToolbarExtensionOptions(type, 1000, iconChar1, iconChar2, requestedWidth);
    }
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, int requestedPosition, char iconChar1, char iconChar2, int requestedWidth) {
        options = new SceneViewToolbarExtensionOptions(type, requestedPosition, iconChar1, iconChar2, requestedWidth);
    }
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, string iconName, int requestedWidth) {
        options = new SceneViewToolbarExtensionOptions(type, 1000, iconName, requestedWidth);
    }
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, int requestedPosition, string iconName, int requestedWidth) {
        options = new SceneViewToolbarExtensionOptions(type, requestedPosition, iconName, requestedWidth);
    }


    /*
    public SceneViewToolbarExtension(SceneViewToolbarExtensionType type, int requestedPosition, Texture icon){
        options = new SceneViewToolbarExtensionOptions(type, requestedPosition, icon);
    }
    */
}