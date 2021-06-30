using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class NodeConnection 
{
    public NodeInput input, output;
    public Action<NodeConnection> OnClickRemoveConnection;

    public NodeConnection(NodeInput input, NodeInput output, Action<NodeConnection> OnClickRemoveConnection) {
        this.input = input;
        this.output = output;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw() {
        Vector2 start = input != null ? 
            input.GetPosition(true).center : 
            Event.current.mousePosition;
        Vector2 end = output != null ?
            output.GetPosition(false).center :
            Event.current.mousePosition;
        Handles.DrawBezier(
            start,
            end,
            start + Vector2.left * 50f,
            end - Vector2.left * 50f,
            Color.white,
            null,
            2f);

    }
}
