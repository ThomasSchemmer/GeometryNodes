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
        switch (e.type) {
            case EventType.MouseDown:
                HandleStartDrag(e); break;
            case EventType.MouseUp:
                isDragged = false; break;
            case EventType.MouseDrag:
                return HandleDrag(e); 
        }
        return false;
    }

    private void HandleStartDrag(Event e) {
        if (e.button != 0)
            return;
        if (NodeEditorWindow.GetOffset(original).Contains(e.mousePosition)) {
            isDragged = true;
        }
        GUI.changed = true;
    }


    private bool HandleDrag(Event e) {
        if (e.button != 0 || !isDragged)
            return false;
        Drag(e.delta);
        e.Use();
        return true;

    }

    public abstract bool Execute(out List<Node> enables);
}
