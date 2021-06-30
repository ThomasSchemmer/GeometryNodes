using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public abstract class Node
{
    public enum Type { NONE, GEOMETRY, OUTPUT, TRANSLATION, MATERIAL}

    public Rect original, rect;
    public string title;
    public Type type;
    public bool isExecuted;
    public Result result;
    public bool isHovered;

    protected bool isDragged;

    public List<NodeInput> inputs, outputs;

    public Node(Vector2 pos, float w, float h) {
        this.original = new Rect(pos, new Vector2(w, h));
        this.inputs = new List<NodeInput>();
        this.outputs = new List<NodeInput>();
    }

    public void Drag(Vector2 delta) {
        original.position += delta;
    }

    public virtual void Draw() {
        rect = NodeEditorWindow.GetOffset(original);
        GUI.Box(rect, title, Styles.boxStyle);
        EditorGUI.LabelField(rect, "" + System.Enum.GetName(typeof(Type), type));

        DrawInputs();
    }

    public void DrawInputs() {
        int i = 0;
        foreach (NodeInput input in inputs) {
            input.Draw(i, inputs.Count);
            i++;
        }
        i = 0;
        foreach (NodeInput output in outputs) {
            output.Draw(i, outputs.Count);
        }
    }

    public bool ProcessEvents(Event e) {
        isHovered = NodeEditorWindow.GetOffset(original).Contains(e.mousePosition);

        switch (e.type) {
            case EventType.MouseDown:
                return 
                    ProcessNodeInputEvents(e) ||
                    HandleStartDrag(e); 
            case EventType.MouseUp:
                isDragged = false;
                return ProcessClose(e);
            case EventType.MouseDrag:
                return HandleDrag(e); 
        }
        return false;
    }

    private bool ProcessClose(Event e) {
        Rect temp = NodeEditorWindow.GetOffset(original);
        Rect iRect = new Rect(temp.x + temp.width - 25, temp.y + 4, 20, 20);
        if (iRect.Contains(e.mousePosition)) {
            NodeEditorWindow.MarkNodeToDelete(this);
            return true;
        }
        return false;
    }

    private bool ProcessNodeInputEvents(Event e) {
        foreach(NodeInput input in inputs) {
            if (input.ProcessEvent(e))
                return true;
        }
        foreach(NodeInput output in outputs) {
            if (output.ProcessEvent(e))
                return true;
        }
        return false;
    }

    private bool HandleStartDrag(Event e) {
        if (e.button != 0)
            return false;
        if (NodeEditorWindow.GetOffset(original).Contains(e.mousePosition)) {
            isDragged = true;
            return true;
        }
        return false;
    }


    private bool HandleDrag(Event e) {
        if (e.button != 0 || !isDragged)
            return false;
        Drag(e.delta);
        e.Use();
        return true;

    }

    public virtual void Delete() {
        foreach (NodeInput input in this.inputs) {
            NodeEditorWindow.removeConnectionPoint(input);
        }
        foreach (NodeInput output in this.outputs) {
            NodeEditorWindow.removeConnectionPoint(output);
        }
    }

    public abstract bool Execute(out List<Node> enables);
}
