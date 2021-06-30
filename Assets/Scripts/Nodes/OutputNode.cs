using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OutputNode : Node {
    public static float WIDTH = 200, HEIGHT = 75;
    public static OutputNode instance;

    public OutputNode(Vector2 pos)
        : base(pos, WIDTH, HEIGHT) {
        this.type = Type.OUTPUT;
        this.inputs.Add(new NodeInput(this,Styles.inputStyle, true));
        instance = this;
    }

    private void _StartExecution() {
        List<Node> activeNodes = new List<Node>();
        foreach(Node node in NodeEditorWindow.instance.nodes) {
            node.isExecuted = false;
            if (node.GetType() == typeof(GeometryNode))
                activeNodes.Add(node);
        }

        bool isChanged;
        int counter = 0;
        while(activeNodes.Count > 0 && counter < 2) {
            List<Node> newActiveNodes = new List<Node>();
            isChanged = false;
            foreach(Node node in activeNodes) {
                if (!node.Execute(out List<Node> enables)) {
                    newActiveNodes.Add(node);
                } else {
                    isChanged = true;
                    counter = 0;
                    newActiveNodes.AddRange(enables);
                }
            }
            activeNodes = newActiveNodes;
            if (!isChanged)
                counter++;
        }
    }

    public override void Draw() {
        rect = NodeEditorWindow.GetOffset(original);
        GUI.Box(rect, title, Styles.boxStyle);
        rect.height = 30;
        GUI.Label(rect, "" + Enum.GetName(typeof(Type), type), Styles.boxStyle);
        rect.y += 30;
        rect.x += 50;
        rect.width -= 100;
        if(GUI.Button(rect, "Execute")) {
            StartExecution();
        }

        DrawInputs();
    }

    public override bool Execute(out List<Node> enables) {
        enables = new List<Node>();
        if (!inputs[0].TryGetOtherNode(out Node other) ||
            !other.isExecuted)
            return false;
        if (isExecuted)
            return true;

        GameObject root = GameObject.Find("NodeEditor/Output");
        if (!root) {
            GameObject ne = new GameObject("NodeEditor");
            root = new GameObject("Output");
            root.transform.parent = ne.transform;
        }
        Result result = other.result;
        if (!(result is Geometry))
            return false;
        Geometry geometry = (Geometry)result;
        Mesh mesh = new Mesh();
        mesh.vertices = geometry.vertices.ToArray();
        mesh.triangles = geometry.triangles.ToArray();
        mesh.normals = geometry.vertices.ToArray();
        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

        MeshRenderer rend = root.GetComponent<MeshRenderer>();
        if (!rend)
            rend = root.AddComponent<MeshRenderer>();
        if (result is MaterialGeometry)
            rend.sharedMaterial = ((MaterialGeometry)result).material;
        else
            rend.sharedMaterial = new Material(Shader.Find("Custom/Debug"));

        MeshFilter filter = root.GetComponent<MeshFilter>();
        if (!filter)
            filter = root.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        isExecuted = true;
        return true;
    }

    public static bool IsAllowed() {
        return instance == null;
    }

    public static void StartExecution() {
        if (instance != null)
            instance._StartExecution();
    }
}
