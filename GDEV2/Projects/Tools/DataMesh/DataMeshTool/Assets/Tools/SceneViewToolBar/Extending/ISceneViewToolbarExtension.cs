using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a button or toggle to the SceneView-toolbar.
/// Use attributes to determine which methods get called.
/// </summary>
[System.Obsolete("Use the 'SceneViewToolbarExtension' attribute instead")]
public interface ISceneViewToolbarExtension {

    SceneViewToolbarExtensionOptions sceneViewToolbarExtensionOptions{ get; }

}
