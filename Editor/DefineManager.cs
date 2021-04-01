using UnityEditor;

namespace ARRTEditor.Firebase.DB
{
    public static class DefineManager
    {
        public static void TryAddDefine(string define, BuildTargetGroup buildTargetGroup)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (defines == null) { defines = define; }
            else if (defines.Length == 0) { defines = define; }
            else { if (defines.IndexOf(define, 0) < 0) { defines += ";" + define; } }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }

        public static void TryRemoveDefine(string define, BuildTargetGroup buildTargetGroup)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (defines.StartsWith(define + ";"))
            {
                defines = defines.Remove(0, define.Length + 1);
            }
            else if (defines.StartsWith(define))
            {
                defines = defines.Remove(0, define.Length);
            }
            else if (defines.EndsWith(";" + define))
            {
                defines = defines.Remove(defines.Length - define.Length - 1, define.Length + 1);
            }
            else
            {
                var index = defines.IndexOf(define, 0, System.StringComparison.Ordinal);
                if (index >= 0) { defines = defines.Remove(index, define.Length + 1); }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }
    }
}
