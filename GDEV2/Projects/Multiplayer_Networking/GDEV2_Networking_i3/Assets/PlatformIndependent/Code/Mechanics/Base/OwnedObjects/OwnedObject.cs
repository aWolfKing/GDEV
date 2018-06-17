using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FE {

    public static class OwnedObject_static{

        /// <summary>
        /// Fakes an 'function call' operator together with the OwnedObject.OwnerChange property.
        /// </summary>
        /// <param name="ownedObject"></param>
        public static void OwnerChange(this OwnedObject ownedObject, object oldOwner, object newOwner){
            ownedObject.OwnerChange.op_FunctionCall(oldOwner, newOwner);
        }

    }



    public class OwnedObject {

        public class OwnerChangeCallback : Callback<object, object>{
            
        }



        private object owner = null;
        private OwnerChangeCallback ownerChange = null;
        public OwnerChangeCallback OwnerChange{
            get{
                return this.ownerChange;
            }
        }



        public static T GetOwning<T>(object ownedObject) where T : class{
            if(ownedObject is T){
                return (T)ownedObject;
            }
            if(ownedObject is OwnedObject){
                object checkingOwner = ((OwnedObject)ownedObject).owner;
                do{
                    ///Object is not owned, return null.
                    if(checkingOwner == null){
                        return null;
                    }
                    ///Object is owned by an object of type T, return that object.
                    else if(checkingOwner is T){
                        return (T)checkingOwner;
                    }
                    ///Object is owned by an OwnedObject that is not of type T, check that objects owner.
                    else if(checkingOwner is OwnedObject){
                        checkingOwner = ((OwnedObject)checkingOwner).owner;
                    }
                    ///Object is not owned by an OwnedObject and is not owned by an object of type T, return null.
                    else{
                        return null;
                    }
                }
                while(true);
            }
            else{
                return null;
            }
        }



        public void SetOwner(object owner){
            this.OwnerChange(this.owner, owner);    //Calls all delegates that are assosiated with this owner change.
            this.owner = owner;
        }



        public OwnedObject(){
            this.ownerChange = new OwnerChangeCallback();
        }

        public OwnedObject(object owner) : this(){
            this.owner = owner;
        }

    }

}
