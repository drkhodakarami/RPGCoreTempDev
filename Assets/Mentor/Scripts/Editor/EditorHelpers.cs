using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mentor.Editor
{
    public class EditorHelpers
    {
        public static string GetSelectedPath()
        {
            // Try to get the selected folder in the Project View
            string path = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (Directory.Exists(path))
                    return path;
                else
                    return Path.GetDirectoryName(path);
            }

            return path;
        }

        public static string GetProjectWindowPath()
        {
            var obj = Selection.activeObject;

            if (obj == null)
                return "Assets";

            var path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path))
                return "Assets";

            return path;
        }

        public static void CreateFolder(string rootPath, string relativePath)
        {
            string fullPath = Path.Combine(rootPath, relativePath);
            CreateFolder(fullPath);
        }

        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}