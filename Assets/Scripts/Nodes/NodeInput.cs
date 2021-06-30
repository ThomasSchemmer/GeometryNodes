using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInput {
    public enum InputType { In, Out }

    public Node parent;
    public List<NodeConnection> connections;
    public GUIStyle style;
    public Rect rect, original;
    public InputType type;

    public NodeInput(Node parent, GUIStyle style, bool isInput) { 
        this.parent = parent;
        this.type = isInput ? InputType.In : InputType.Out;
        this.style = style;
        original = new Rect(0, 0, 10f, 20);
        original.y = parent.rect.y + (parent.rect.height * 0.5f) - rect.height * 0.5f;
        switch (type) {
            case InputType.In:
                original.x = parent.rect.x - original.width + 8f;
                break;
            default:
                original.x = parent.rect.x + parent.rect.width - 8f;
                break;
        }
        connections = new List<NodeConnection>();
    }

    public void Draw(int i, int max) {
        rect = GetPosition(i, max);
        GUI.Box(rect, "", style);
    }

    public bool ProcessEvent(Event e) {
        if (e.type != EventType.MouseDown)
            return false;
        if (!rect.Contains(e.mousePosition))
            return false;

        if(e.button == 0) {
            NodeEditorWindow.onClickConnectionPoint(this, false);
            e.Use();
            return true;
        }
        if(e.button == 1) {
            NodeEditorWindow.onClickConnectionPoint(this, true);
            e.Use();
            return true;
        }
        return false;
    }

    public Rect GetPosition(int i, int max) {
        Rect rect = NodeEditorWindow.GetOffset(original);
        Rect pPos = NodeEditorWindow.GetOffset(parent.original);
        rect.y = pPos.y + (pPos.height / (max + 1) * (i + 1)) - rect.height * 0.5f;
        switch (type) {
            case InputType.In:
                rect.x = pPos.x - rect.width + 8f;
                break;
            default:
                rect.x = pPos.x + pPos.width - 8f;
                break;
        }
        return rect;
    }

    public Rect GetPosition(bool isInput) {
        int i = isInput ? parent.inputs.IndexOf(this) : parent.outputs.IndexOf(this);
        int max = isInput ? parent.inputs.Count : parent.outputs.Count;
        return GetPosition(i, max);
    }

    public bool TryGetOtherNodes(out List<Node> others) {
        others = null;
        if (connections.Count == 0)
            return false;
        others = new List<Node>();
        if (type == InputType.In && connections[0].output == null)
            return false;

        if (type == InputType.Out && connections[0].input == null)
            return false;


        foreach (NodeConnection connection in connections) {
            others.Add(type == InputType.In ? connection.output.parent : connection.input.parent);
        }
        return true;
    }
}
