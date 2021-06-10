using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

namespace ARRT.Firebase.Database
{
    public static class DBRealtime
    {
        public static Action onChangeData { get; set; }
        public static FirebaseDatabase database { get; private set; }
        public static DatabaseReference referenceRoot { get; private set; }


        static DBRealtime()
        {
            database = FirebaseDatabase.DefaultInstance;
            referenceRoot = database.RootReference;

            referenceRoot.ChildChanged += (sender, args) =>
            {
                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                onChangeData?.Invoke();
            };
        }

        public static void Fetch(DatabaseReference reference, Action<object> response, Action error=null)
        {
           reference.GetValueAsync().ContinueWith((task) =>
           {  
               DataSnapshot snapshot = task.Result;
               if(snapshot.Exists)
               {
                   response?.Invoke(snapshot.Value);
               }
               else
               {
                   Debug.LogWarning("Not snapshot found");
                   error?.Invoke();
               }
           },TaskScheduler.FromCurrentSynchronizationContext());
           
       }

       public static void Fetch(string path, Action<object> response, Action error = null)
       {
           var reference = database.GetReference(path);
           Fetch(reference, response, error);
       }

       public static void FetchAll(Action<object> response)
       {
           referenceRoot.GetValueAsync().ContinueWith((task) =>
           {
               var snapshot = task.Result;
               if (snapshot.Exists)
               {
                   response?.Invoke(snapshot.Value);
               }
           },TaskScheduler.FromCurrentSynchronizationContext());
       }

        public static void SubscribeToFetch(DatabaseReference reference, Action<object> response)
        {
            reference.ValueChanged += (sender, args) =>
            {
                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                response.Invoke(args.Snapshot.Value);
            };

            reference.ChildChanged += (sender, args) =>
            {
                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                response.Invoke(args.Snapshot.Value);
            };
        }

        public static void SubscribeToFetch(string path, Action<object> response)
        {
            var reference = database.GetReference(path);
            SubscribeToFetch(reference, response);
        }

        public static void SetRawJsonValue(DatabaseReference reference, object data)
        {
            var json = JsonUtility.ToJson(data);
            reference.SetRawJsonValueAsync(json);
        }

        public static void SetRawJsonValue(string path, object data)
        {
            var reference = database.GetReference(path);
            SetRawJsonValue(reference, data);
        }

        public static void SetValue(DatabaseReference reference, object data)
        {
            reference.SetValueAsync(data);
        }

        public static void SetValue(string path, object data)
        {
            var reference = database.GetReference(path);
            SetValue(reference, data);
        }

        public static void Remove(DatabaseReference reference)
        {
            reference.RemoveValueAsync();
        }

        public static void Remove(string path)
        {
            var reference = database.GetReference(path);
            Remove(reference);
        }

        public static void RemoveAll()
        {
            Remove(referenceRoot);
        }
    }
}