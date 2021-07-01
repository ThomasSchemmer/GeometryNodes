using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InstanceNode : Node {
    public static float WIDTH = 200, HEIGHT = 75;
    public GameObject target;

    public InstanceNode(Vector2 pos) : base(pos, WIDTH, HEIGHT) {
        type = Type.INSTANCE;
        this.inputs.Add(new NodeInput(this, Styles.inputStyle, true));
        this.outputs.Add(new NodeInput(this, Styles.outputStyle, false));
    }

    public override bool Execute(out List<Node> enables) {
        enables = new List<Node>();
        if (!inputs[0].TryGetOtherNodes(out List<Node> others))
            return false;
        bool othersExec = true;
        foreach (Node other in others) {
            othersExec &= other.isExecuted;
        }
        if (!othersExec)
            return false;
        if (isExecuted)
            return true;

        Result result = others[0].result;
        if (!(result is Geometry))
            return false;

        if (target == null)
            return false;

        this.result = InstanceGeometry((Geometry)result);

        enables = new List<Node>();
        foreach (NodeInput output in outputs) {
            if (output.TryGetOtherNodes(out List<Node> nodes)) {
                enables.AddRange(nodes);
            }
        }
        isExecuted = true;
        return true;
    }

    private Geometry InstanceGeometry(Geometry oldGeo) {
        Geometry newGeo = Geometry.CreateEmpty();
        Mesh targetMesh = target.GetComponent<MeshFilter>().sharedMesh;

        for(int i = 0; i < oldGeo.vertices.Count; i++) {
            InstanceObjectToGeo(newGeo, targetMesh, oldGeo.vertices[i]);
        }

        return newGeo;
    }

    private void InstanceObjectToGeo(Geometry newGeo, Mesh targetMesh, Vector3 pos) {
        int lastMax = newGeo.vertices.Count;
        foreach (Vector3 vertex in targetMesh.vertices) {
            newGeo.vertices.Add(vertex + pos);
        }
        foreach (int tri in targetMesh.triangles) {
            newGeo.triangles.Add(tri + lastMax);
        }
    }

    public override void Draw() {
        rect = NodeEditorWindow.GetOffset(original);
        GUI.Box(rect, "", Styles.boxStyle);
        rect.height = 30;
        GUI.Label(rect, "" + Enum.GetName(typeof(Type), type), Styles.boxStyle);
        rect.height = 20;

        rect.y += 35;
        rect.x += 7;
        EditorGUI.LabelField(rect, "Object:");
        rect.x += WIDTH / 2f - 25;
        rect.width = WIDTH / 2f;

        target = (GameObject)EditorGUI.ObjectField(rect, target, typeof(GameObject), true);

        Rect temp = NodeEditorWindow.GetOffset(original);
        Rect iRect = new Rect(temp.x + temp.width - 25, temp.y + 4, 20, 20);
        if (isHovered)
            EditorGUI.LabelField(iRect, EditorGUIUtility.IconContent("CollabDeleted Icon"));
        DrawInputs();
    }
}
