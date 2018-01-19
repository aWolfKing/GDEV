using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneViewToolbarExtensionOptions {

    public SceneViewToolbarExtensionType extensionType = SceneViewToolbarExtensionType.Button;
    public int requestedPosition = 10;
    public Texture icon = null;
    private string nameIcon = "TE";
    public string NameIcon{ get{ return nameIcon; } }
    public int requestedWidth = 0;
    //public string toolTip = "";   // to be added?

    public SceneViewToolbarExtensionOptions(SceneViewToolbarExtensionType type, int requestedPosition, char iconChar1, char iconChar2) {
        this.requestedPosition = requestedPosition;
        this.nameIcon = iconChar1.ToString() + "" + iconChar2.ToString();
        this.extensionType = type;
    }

    public SceneViewToolbarExtensionOptions(SceneViewToolbarExtensionType type, int requestedPosition, string iconName){
        this.requestedPosition = requestedPosition;
        //this.icon = //load icon
        this.icon = SceneViewToolbarExtensionManager.GetIconFromIconsFolder(iconName);
        this.extensionType = type;
        if(this.icon == null){
            this.nameIcon = iconName;
        }
    }


    public SceneViewToolbarExtensionOptions(SceneViewToolbarExtensionType type, int requestedPosition, char iconChar1, char iconChar2, int requestedWidth) {
        this.requestedPosition = requestedPosition;
        this.nameIcon = iconChar1.ToString() + "" + iconChar2.ToString();
        this.extensionType = type;
        this.requestedWidth = requestedWidth;
    }

    public SceneViewToolbarExtensionOptions(SceneViewToolbarExtensionType type, int requestedPosition, string iconName, int requestedWidth) {
        this.requestedPosition = requestedPosition;
        //this.icon = //load icon
        this.icon = SceneViewToolbarExtensionManager.GetIconFromIconsFolder(iconName);
        this.extensionType = type;
        this.requestedWidth = requestedWidth;
        if (this.icon == null) {
            this.nameIcon = iconName;
        }
    }




    #region Obsolete
    /*
    [System.Obsolete("Use icon name instead")]
    public SceneViewToolbarExtensionOptions(SceneViewToolbarExtensionType type, int requestedPosition, Texture icon) {
        this.requestedPosition = requestedPosition;
        this.icon = icon;
        this.extensionType = type;
    }
    */
    #endregion

}
