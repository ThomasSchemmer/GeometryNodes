using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInput {
    public enum InputType { In, Out }

    public Node parent;
    public NodeConnection connection;
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
    }

    public void Draw(int i, int max) {
        rect = GetPosition(i, max);
        if (GUI.Button(rect, "", style)) {
            NodeEditorWindow.onClickConnectionPoint(this);
        }
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

    public bool TryGetOtherNode(out Node other) {
        other = null;
        if (connection == null)
            return false;
        if (type == InputType.In && connection.output == null)
            return false;
        if (type == InputType.Out && connection.input == null)
            return false;

        other = type == InputType.In ? connection.output.parent : connection.input.parent;
        return true;
    }
}
