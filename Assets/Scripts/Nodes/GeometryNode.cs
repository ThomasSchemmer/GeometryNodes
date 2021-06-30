using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class GeometryNode : Node {
    public static float WIDTH = 200, HEIGHT = 100;
    public Geometry.Type geometryType;
    public GameObject target;

    public GeometryNode(Vector2 pos)
        : base(pos, WIDTH, HEIGHT) {
        type = Type.GEOMETRY;
        geometryType = Geometry.Type.Cube;
        this.outputs.Add(new NodeInput(this, Styles.outputStyle, false));
    }

    public override bool Execute(out List<Node> enables) {
        switch (geometryType) {
            case Geometry.Type.Cube:
                result = Geometry.Create(Geometry.Type.Cube); break;
            case Geometry.Type.Plane:
                result = Geometry.Create(Geometry.Type.Plane); break;
            case Geometry.Type.Custom:
                result = CreateResultFromTarget(); break;
        }
        enables = new List<Node>();
        foreach(NodeInput output in outputs) {
            if (output.TryGetOtherNode(out Node node))
                enables.Add(node);
        }
        isExecuted = true;
        return true;
    }

    private Result CreateResultFromTarget() {
        Geometry geo = Geometry.CreateEmpty();
        if (!target)
            return geo;
        MeshFilter filter = target.GetComponent<MeshFilter>();
        if (!filter)
            return geo;
        Mesh mesh = filter.sharedMesh;
        if (!mesh)
            return geo;
        return new Geometry(mesh.vertices, mesh.triangles);
    }

    public override void Draw() {
        rect = NodeEditorWindow.GetOffset(original);
        GUI.Box(rect, "", Styles.boxStyle);
        rect.height = 30;
        GUI.Label(rect, "" + Enum.GetName(typeof(Type), type), Styles.boxStyle);

        rect.height = 20;
        rect.y += 23;
        rect.x += 7;
        EditorGUI.LabelField(rect, "Type:");
        rect.width = WIDTH / 2f;
        rect.x += WIDTH / 2f - 25;
        rect.y += 5;
        geometryType = (Geometry.Type)EditorGUI.Popup(
                                        rect,
                                        "",
                                        (int)geometryType,
                                        Enum.GetNames(typeof(Geometry.Type)));
        if (geometryType == Geometry.Type.Custom) {
            original.height = 100;
            rect.x -= WIDTH / 2f - 25;
            rect.y += 25;
            EditorGUI.LabelField(rect, "Target:");
            rect.x += WIDTH / 2f - 25;

            target = (GameObject) EditorGUI.ObjectField(rect, target, typeof(GameObject), true);
        } else {
            original.height = 65;
        }

        DrawInputs();
    }

}
