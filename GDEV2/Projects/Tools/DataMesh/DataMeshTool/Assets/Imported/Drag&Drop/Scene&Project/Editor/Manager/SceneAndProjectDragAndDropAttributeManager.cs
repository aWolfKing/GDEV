using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public static partial class SceneAndProjectDragAndDrop {

    private static class AttributeManager{

        public delegate bool DropOperationDelegate(UnityEngine.Object[] dropped, UnityEngine.Object target);

        public class ValidationExecutionPair{
            public DropOperationDelegate validation;
            public DropOperationDelegate execution;
            public UnityEditor.DragAndDropVisualMode visualMode = UnityEditor.DragAndDropVisualMode.None;
            public ValidationExecutionPair() {}
            public ValidationExecutionPair(DropOperationDelegate validation, DropOperationDelegate execution) {
                this.validation = validation;
                this.execution = execution;
            }
        }

        private static List<ValidationExecutionPair> pairs = new List<ValidationExecutionPair>();

        public static void Awake(){
            pairs.Clear();
            Dictionary<System.Type, ValidationExecutionPair> sourceOperationPairs = new Dictionary<Type, ValidationExecutionPair>();
            var assemlies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemlies){
                var classes = assembly.GetTypes();
                foreach(var type in classes){
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    foreach(var method in methods){
                        var validate = method.GetCustomAttributes<SceneAndProjectDragAndDrop.Attributes.DropValidate>() as SceneAndProjectDragAndDrop.Attributes.DropValidate[];
                        var execute = method.GetCustomAttributes<SceneAndProjectDragAndDrop.Attributes.DropOperation>() as SceneAndProjectDragAndDrop.Attributes.DropOperation[];
                        if(validate != null && validate.Length > 0 && validate[0] != null){
                            if(!sourceOperationPairs.ContainsKey(type)){
                                sourceOperationPairs.Add(type, new ValidationExecutionPair());
                            }
                            try{
                                sourceOperationPairs[type].validation = DropOperationDelegate.CreateDelegate(typeof(DropOperationDelegate), method) as DropOperationDelegate;
                            }
                            catch{}
                            try{
                                sourceOperationPairs[type].visualMode = validate[0].visualMode;
                            }
                            catch{}
                        }
                        if (execute != null && execute.Length > 0 && execute[0] != null) {
                            if (!sourceOperationPairs.ContainsKey(type)) {
                                sourceOperationPairs.Add(type, new ValidationExecutionPair());
                            }
                            try{
                                sourceOperationPairs[type].execution = DropOperationDelegate.CreateDelegate(typeof(DropOperationDelegate), method) as DropOperationDelegate;
                            }
                            catch{}
                        }
                    }
                }
            }
            foreach (var pair in sourceOperationPairs){
                if(pair.Value.validation != null && pair.Value.execution != null){
                    pairs.Add(pair.Value);
                }
            }
            sourceOperationPairs.Clear();
        }


        public static ValidationExecutionPair GetOperationExecutionPair(UnityEngine.Object[] dropped, UnityEngine.Object target){
            foreach (var pair in pairs){
                if(pair.validation(dropped, target)){
                    return pair;
                }
            }
            return null;
        }


    }
   
}
