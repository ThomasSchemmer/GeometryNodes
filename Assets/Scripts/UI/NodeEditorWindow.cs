using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class NodeEditorWindow : EditorWindow {
    public static NodeEditorWindow instance;
    public static Vector2 offset, oldOffset;
    public static int lastMouseButton = -1;

    public static Action<NodeInput> onClickConnectionPoint;

    public List<Node> nodes;

    private string path = "\\NodeEditor\\nodes.sav";
    private List<NodeConnection> connections;

    private Vector2 dragStart;

    private NodeInput selectedInput;
    private NodeConnection currentConnection;

    [MenuItem("Window/NodeEditor")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(NodeEditorWindow));
    }

    private void OnEnable() {
        instance = this;
        offset = new Vector2(0, 0);
        oldOffset = new Vector2(0, 0);
        onClickConnectionPoint = OnClickOnInput;
        Styles.Init();
        if (nodes == null)
            nodes = new List<Node>();
        if (connections == null)
            connections = new List<NodeConnection>();
        LoadNodes();
    }

    private void OnDisable() {
        ClearSave();
        foreach(Node node in nodes) {
            SaveNode(node);
        }
    }

    void OnGUI() {
        DrawNodes();
        DrawConnections();
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        if (GUI.changed)
            Repaint();
    }

    private void DrawNodes() {
        if (nodes == null)
            return;
        foreach(Node node in nodes) {
            node.Draw();
        }
        
    }

    private void DrawConnections() {
        if (connections == null)
            return;
        if (currentConnection != null)
            currentConnection.Draw();
        foreach(NodeConnection conn in connections) {
            conn.Draw();
        }
    }

    private void ProcessNodeEvents(Event e) {
        if (nodes == null)
            return;
        foreach(Node node in nodes) {
            if (node.ProcessEvents(e))
                GUI.changed = true;
        }
    }

    private void ProcessEvents(Event e) {
        switch (e.type) {
            case EventType.MouseDown:
                HandleRightClick(e);
                break;
        }
        HandleView(e);
    }

    private void HandleRightClick(Event e) {
        if (e.button != 1)
            return;
        if (currentConnection != null) {
            currentConnection = null;
            selectedInput = null;
            return;
        }
        HandleContextMenu(e);
    }

    private void HandleView(Event e) {
        if (e.button != 2)
            return;
        switch (e.type) {
            case EventType.MouseDown:
                dragStart = e.mousePosition;
                break;
            case EventType.MouseDrag:
                offset = dragStart - e.mousePosition + oldOffset;
                break;
            case EventType.MouseUp:
                oldOffset = offset;
                break;

        }
    }



    private void HandleContextMenu(Event e) {
        GenericMenu genMenu = new GenericMenu();
        genMenu.AddItem(new GUIContent("Add GeometryNode"), false, () => OnClickAddNode(e.mousePosition, Node.Type.GEOMETRY));
        genMenu.AddItem(new GUIContent("Add OutputNode"), false, () => OnClickAddNode(e.mousePosition, Node.Type.OUTPUT));
        genMenu.AddItem(new GUIContent("Add TranslationNode"), false, () => OnClickAddNode(e.mousePosition, Node.Type.TRANSLATION));
        genMenu.AddItem(new GUIContent("Add MaterialNode"), false, () => OnClickAddNode(e.mousePosition, Node.Type.MATERIAL));

        genMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition, Node.Type type) {
        Node n;
        switch (type) {
            case Node.Type.GEOMETRY:
                n = new GeometryNode(mousePosition); break;
            case Node.Type.OUTPUT:
                if (!OutputNode.IsAllowed())
                    return;
                n = new OutputNode(mousePosition); break;
            case Node.Type.TRANSLATION:
                n = new TranslationNode(mousePosition); break;
            case Node.Type.MATERIAL:
                n = new MaterialNode(mousePosition); break;
            default:
                Debug.LogError("Invalid Node Type!");
                return;
        }
        nodes.Add(n);
        SaveNode(n);
        OutputNode.StartExecution();
    }

    private void OnClickRemoveNodeConnection(NodeConnection connection) {
        if (connection.input != null)
            connection.input.connection = null;
        if (connection.output != null)
            connection.output.connection = null;
        connections.Remove(connection);
    }

    private void OnClickOnInput(NodeInput input) {
        if (lastMouseButton == 1) {
            OnClickRemoveNodeConnection(input.connection);
            return;
        }
        
        //if we are currently not creating a connection, we are starting so now
        if (selectedInput == null) {
            selectedInput = input;
            currentConnection = new NodeConnection(
                input.type == NodeInput.InputType.In ? input : null,
                input.type == NodeInput.InputType.In ? null : input,
                OnClickRemoveNodeConnection
                );
            return;
        }
        //cannot connect to self
        if (selectedInput == input) {
            selectedInput = null;
            currentConnection = null;
            return;
        }
        //check for double connections on the same input/output
        //prohibit linking input with input
        if (input.type == NodeInput.InputType.In ) {
            if (currentConnection.input != null || input.connection != null)
                return;
            currentConnection.input = input;
            input.connection = currentConnection;
            currentConnection.output.connection = currentConnection;

        } else {
            if (currentConnection.output != null || input.connection != null)
                return;
            currentConnection.output = input;
            input.connection = currentConnection;
            currentConnection.input.connection = currentConnection;
        }
            

        connections.Add(currentConnection);
        currentConnection = null;
        selectedInput = null;

        OutputNode.StartExecution();
    }

    private void LoadNodes() {
        if (!Directory.Exists(Application.persistentDataPath + "\\NodeEditor"))
            Directory.CreateDirectory(Application.persistentDataPath + "\\NodeEditor");

        if (!File.Exists(GetDataPath()))
            return;

        string[] lines = File.ReadAllLines(GetDataPath());
        foreach(string line in lines) {
            LoadNode(line);
        }
    }

    private void SaveNode(Node n) {
        string line = "";
        line += n.original.x + " " + n.original.y + " ";
        line += (int)n.type + "\n";

        if (!File.Exists(GetDataPath()))
            File.Create(GetDataPath()).Close();
        File.AppendAllText(GetDataPath(), line);
    }


    private void LoadNode(string line) {
        string[] arr = line.Split(' ');
        Vector2 pos = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
        Node.Type type = (Node.Type)int.Parse(arr[2]);
        switch (type) {
            case Node.Type.GEOMETRY:
                nodes.Add(new GeometryNode(pos)); break;
            case Node.Type.OUTPUT:
                nodes.Add(new OutputNode(pos)); break;
            case Node.Type.TRANSLATION:
                nodes.Add(new TranslationNode(pos)); break;
            case Node.Type.MATERIAL:
                nodes.Add(new MaterialNode(pos)); break;
        }
    }

    private string GetDataPath() {
        return Application.persistentDataPath + path;
    }

    private void ClearSave() {
        OutputNode.instance = null;
        if (!File.Exists(GetDataPath()))
            return;
        File.WriteAllText(GetDataPath(), "");
    }

    private void Update() {
        Repaint();
        if (Input.GetMouseButtonDown(0))
            lastMouseButton = 0;
        if (Input.GetMouseButtonDown(1))
            lastMouseButton = 1;
        if (Input.GetMouseButtonDown(2))
            lastMouseButton = 2;
    }

    public static Rect GetOffset(Rect original) {
        return new Rect(original.x - offset.x,
            original.y - offset.y,
            original.width,
            original.height);
    }

}
