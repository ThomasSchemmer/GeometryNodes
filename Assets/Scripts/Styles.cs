using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Styles 
{
    public static GUIStyle boxStyle, inputStyle, outputStyle;

    public static void Init() {

        boxStyle = new GUIStyle();
        boxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        boxStyle.border = new RectOffset(12, 12, 12, 12);
        boxStyle.alignment = TextAnchor.MiddleCenter;
        boxStyle.normal.textColor = Color.white;

        inputStyle = new GUIStyle();
        inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inputStyle.border = new RectOffset(4, 4, 12, 12);

        outputStyle = new GUIStyle();
        outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outputStyle.border = new RectOffset(4, 4, 12, 12);

    }
}
