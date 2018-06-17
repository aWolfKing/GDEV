using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class CommunicationServer {

    internal class Clients{

        internal class Client{
            internal int clientID = -1;
            internal CommunicationClient communicationClient = null;
            public Client(CommunicationClient client){
                this.clientID = GetUnusedID();
                this.communicationClient = client;
                this.communicationClient.SetID(this.clientID);
            }
        }



        private static List<Client> clients = new List<Client>();
        private static int idCounter = 1;



        internal static void RegisterClient(CommunicationClient client){
            if(_this.isServer){
                bool alreadyExists = false;
                foreach(var c in clients){
                    if(c.communicationClient == client){
                        alreadyExists = true;
                        break;
                    }
                }
                if(!alreadyExists){
                    clients.Add(new Client(client));
                }
            }
        }

        internal static void DeRegisterClient(CommunicationClient client){
            if(_this.isServer){
                for(int i=clients.Count-1; i>=0; i--){
                    if(clients[i].communicationClient == client){
                        clients.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        internal static Client GetClientFromID(int id){
            foreach(var client in clients){
                if(client.clientID == id){
                    return client;
                }
            }
            return null;
        }

        private static int GetUnusedID(){
            int id = idCounter;
            idCounter++;
            return id;
        }



        public static int[] GetAllClientIDs(){
            List<int> result = new List<int>();
            foreach(var client in clients){
                result.Add(client.clientID);
            }
            return result.ToArray();
        }

    }

}
