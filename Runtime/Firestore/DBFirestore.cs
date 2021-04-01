using UnityEngine;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using Firebase.Extensions;

namespace ARRT.Firebase.Firestore
{
    public static class DBFirestore
    {
        private static FirebaseFirestore m_Database;

        public static FirebaseFirestore database { get => m_Database ?? (m_Database = FirebaseFirestore.DefaultInstance); }


        public static void GetDocument<T>(string collectionName, string documentName, Action<T> response)
        {
            DocumentReference docRef = database.Collection(collectionName).Document(documentName);
            docRef.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.LogFormat("Document {0} accepted!", documentName);
                    var doc = snapshot.ConvertTo<T>();

                    response?.Invoke(doc);
                }
                else
                {
                    Debug.LogWarningFormat("Document {0} does not exist!", documentName);
                }
            });
        }

        public static void GetDocument(string collectionName, string documentName, Action<Dictionary<string, object>> response)
        {
            DocumentReference docRef = database.Collection(collectionName).Document(documentName);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.LogFormat("Document {0} accepted!", documentName);
                    response?.Invoke(snapshot.ToDictionary());
                }
                else
                {
                    Debug.LogWarningFormat("Document {0} does not exist!", documentName);
                }
            });
        }

        public static void GetAllDocuments(string collectionName, Action<List<Dictionary<string, object>>> response)
        {
            CollectionReference colRef = database.Collection(collectionName);
            colRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;
                if (snapshot != null)
                {
                    Debug.LogFormat("Document {0} accepted!", snapshot.Metadata);

                    List<Dictionary<string, object>> docList = new List<Dictionary<string, object>>();
                    foreach (DocumentSnapshot document in snapshot)
                    {
                        docList.Add(document.ToDictionary());
                    }
                    
                    if (docList.Count > 0)
                    {
                        response?.Invoke(docList);
                    }
                    else
                    {
                        Debug.LogWarningFormat("Collection {0} can not ConvertTo<T>!", collectionName);
                    }
                }
                else
                {
                    Debug.LogWarningFormat("Collection {0} does not exist!", collectionName);
                }
            });
        }

        public static void SetDocument(string collectionName, string documentName, Dictionary<string, object> response)
        {
            DocumentReference docRef = database.Collection(collectionName).Document(documentName);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                
                if (snapshot.Exists)
                    Debug.LogFormat("Document {0} update!", documentName);
                else
                    Debug.LogWarningFormat("Document {0} create!", documentName);
                
                docRef.SetAsync(response);
            });
        }

        public static void DeleteDocument(string collectionName, string documentName)
        {
            DocumentReference docRef = database.Collection(collectionName).Document(documentName);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.LogFormat("Document {0} delete!", documentName);
                    docRef.DeleteAsync();
                }
                else
                {
                    Debug.LogWarningFormat("Document {0} does not exist!", documentName);
                }
            });
        }

        public static void DeleteAllDocuments(string collectionName, string IfYouShureWrite_Yes)
        {
            if (IfYouShureWrite_Yes != "Yes")
            {
                Debug.LogWarning("Pass `Yes` in IfYouShureWrite_Yes variable to confirm");
                return;
            }

            CollectionReference colRef = database.Collection(collectionName);
            colRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;
                if (snapshot[0].Exists)
                {
                    Debug.LogFormat("Collection {0} cleared!", collectionName);
                    
                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        document.Reference.DeleteAsync();
                    }
                }
                else
                {
                    Debug.LogWarningFormat("Collection {0} does not exist!", collectionName);
                }
            });
        }
    }
}
