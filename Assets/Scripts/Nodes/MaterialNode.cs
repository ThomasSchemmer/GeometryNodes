using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialNode : Node {
    public static float WIDTH = 200, HEIGHT = 75;
    public Geometry.Type geometryType;
    public Material target;

    public MaterialNode(Vector2 pos)
       : base(pos, WIDTH, HEIGHT) {
        type = Type.MATERIAL;
        geometryType = Geometry.Type.Cube;
        this.outputs.Add(new NodeInput(this, Styles.outputStyle, false));
        this.inputs.Add(new NodeInput(this, Styles.inputStyle, true));
    }

    public override void Draw() {
        rect = NodeEditorWindow.GetOffset(original);
        GUI.Box(rect, "", Styles.boxStyle);
        rect.height = 30;
        GUI.Label(rect, "" + Enum.GetName(typeof(Type), type), Styles.boxStyle);
        rect.height = 20;

        rect.y += 35;
        rect.x += 7;
        EditorGUI.LabelField(rect, "Material:");
        rect.x += WIDTH / 2f - 25;
        rect.width = WIDTH / 2f;

        target = (Material)EditorGUI.ObjectField(rect, target, typeof(Material), true);

        DrawInputs();
    }

    public override bool Execute(out List<Node> enables) {
        enables = new List<Node>();
        if (!inputs[0].TryGetOtherNode(out Node other) ||
            !other.isExecuted)
            return false;
        if (isExecuted)
            return true;

        Result result = other.result;
        if (!(result is Geometry))
            return false;

        if (target == null)
            return false;

        this.result = new MaterialGeometry(target, (Geometry)result);

        enables = new List<Node>();
        foreach (NodeInput output in outputs) {
            if (output.TryGetOtherNode(out Node node))
                enables.Add(node);
        }
        isExecuted = true;
        return true;
    }
}
