using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class SceneAndProjectDragAndDrop {

    public static class Attributes{

        [AttributeUsage(AttributeTargets.Method)]
        public class DropValidate : Attribute{
            public UnityEditor.DragAndDropVisualMode visualMode;
            public DropValidate(UnityEditor.DragAndDropVisualMode cursor){
                this.visualMode = cursor;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class DropOperation : Attribute{}

    }

}
