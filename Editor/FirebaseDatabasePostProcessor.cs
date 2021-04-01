using UnityEditor;
using System.IO;

namespace ARRTEditor.Firebase.DB
{
    public class FirebaseDatabasePostprocessor : AssetPostprocessor
    {
        private const string m_FileName = "Firebase.Database.dll";
        private const string m_Define = "USE_FIREBASE_DATABASE";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            DetectFirebaseDeletion(deletedAssets);
            DetectFirebaseImport(importedAssets);
        }

        private static void DetectFirebaseImport(string[] importedAssets)
        {
            foreach (string assetPath in importedAssets)
            {
                if (Path.GetFileName(assetPath).Equals(m_FileName))
                {
                    DefineManager.TryAddDefine(m_Define, EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }
        }

        private static void DetectFirebaseDeletion(string[] deletedAssets)
        {
            foreach (string assetPath in deletedAssets)
            {
                if (Path.GetFileName(assetPath).Equals(m_FileName))
                {
                    DefineManager.TryRemoveDefine(m_Define, EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }
        }
    }
}
