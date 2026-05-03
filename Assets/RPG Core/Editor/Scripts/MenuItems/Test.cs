using System;
using Mentor.RpgCore.Database;
using Mentor.RpgCore.Editor;
using UnityEngine;
using UnityEditor;

public class TestWindow : RpgBaseWindowTemplate<RpgBaseDBEntry>
{}

namespace Mentor.RPGCore.Editor
{
    public class Test : EditorWindow
    {
        [MenuItem("RPG Core/Editors/Test/Base Window")]
        public static void ShowWindow()
        {
            Type specificType = typeof(RpgBaseDBEntry);

            var window = GetWindow<TestWindow>();
            window.InitializeWindow(specificType);
            window.titleContent = new GUIContent("Base Template");
            window.minSize = new Vector2(880, 450);
            window.maxSize = new Vector2(880, 450);
        }
    }
}