using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TranslationNode : Node {
    public static float WIDTH = 200, HEIGHT = 325;

    public Vector3 offset, scale, rotation;

    public TranslationNode(Vector2 pos)
        : base(pos, WIDTH, HEIGHT) {
        this.type = Type.TRANSLATION;
        this.inputs.Add(new NodeInput(this, Styles.inputStyle, true));
        this.outputs.Add(new NodeInput(this, Styles.outputStyle, false));
        this.scale = new Vector3(1, 1, 1);
    }

    public override bool Execute(out List<Node> enables) {
        enables = new List<Node>();
        if (isExecuted)
            return true;
        if (!this.inputs[0].TryGetOtherNode(out Node other))
            return false;

        if (!other.isExecuted || !(other.result is Geometry))
            return false;

        Geometry geo = (Geometry)other.result;
        Matrix4x4 trs = Matrix4x4.TRS(offset, Quaternion.Euler(rotation), scale);
        for(int i = 0; i < geo.vertices.Count; i++) {
            geo.vertices[i] = trs.MultiplyPoint(geo.vertices[i]);
        }
        foreach (NodeInput output in outputs) {
            if (output.TryGetOtherNode(out Node node))
                enables.Add(node);
        }
        result = geo;
        isExecuted = true;
        return true;
    }

    public override void Draw() {
        rect = NodeEditorWindow.GetOffset(original);
        GUI.Box(rect, "", Styles.boxStyle);
        rect.height = 30;
        GUI.Label(rect, "" + Enum.GetName(typeof(Type), type), Styles.boxStyle);

        rect.height = 20;
        rect.width -= 40;
        rect.y += 20;
        rect.x += 7;
        DrawVector(ref rect, "Translation:", ref offset);
        DrawVector(ref rect, "Scale:", ref scale);
        DrawVector(ref rect, "Rotation:", ref rotation);

        DrawInputs();
    }

    private void DrawVector(ref Rect pos, String header, ref Vector3 variable) {
        EditorGUI.LabelField(pos, header);
        pos.y += 20;
        pos.x += 5;
        Vector3 newVar = new Vector3();
        EditorGUI.LabelField(pos, "x");
        pos.x += 15;
        newVar.x = EditorGUI.FloatField(pos, variable.x);
        pos.y += 25;
        pos.x -= 15;
        EditorGUI.LabelField(pos, "y");
        pos.x += 15;
        newVar.y = EditorGUI.FloatField(pos, variable.y);
        pos.y += 25;
        pos.x -= 15;
        EditorGUI.LabelField(pos, "z");
        pos.x += 15;
        newVar.z = EditorGUI.FloatField(pos, variable.z);
        pos.y += 25;
        pos.x -= 20;
        variable = newVar;
    }

}
