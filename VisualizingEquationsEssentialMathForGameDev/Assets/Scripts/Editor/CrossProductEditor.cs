using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CrossProductEditor : CommonEditor, IUpdateSceneGUI
{
    public Vector3 p;
    public Vector3 q;
    public Vector3 pxq;

    private SerializedObject obj;
    private SerializedProperty propP;
    private SerializedProperty propQ;
    private SerializedProperty propPxQ;
    private GUIStyle guiStyle = new GUIStyle();
    [MenuItem("Tools/Cross Product")]
    private static void ShowWindow()
    {
        var window = GetWindow(typeof(CrossProductEditor), true, "Cross Product");
        window.Show();
    }
    void OnEnable() 
    {
        if (this.p == Vector3.zero && this.q == Vector3.zero)
        {
            InitValues();
        }
        this.obj = new SerializedObject(this);
        this.propP = this.obj.FindProperty("p");
        this.propQ = this.obj.FindProperty("q");
        this.propPxQ = this.obj.FindProperty("pxq");
        this.guiStyle.fontSize = 25;
        this.guiStyle.fontStyle = FontStyle.Bold;
        this.guiStyle.normal.textColor = Color.white;
        SceneView.duringSceneGui += SceneGUI;
        Undo.undoRedoPerformed += RepaintOnGUI;
    }
    void OnDisable() 
    {
        SceneView.duringSceneGui -= SceneGUI;
        Undo.undoRedoPerformed -= RepaintOnGUI;
    }
    void OnGUI() 
    {
        this.obj.Update();

        DrawBlockUI("P", this.propP);
        DrawBlockUI("Q", this.propQ);
        DrawBlockUI("PxQ", this.propPxQ);
        if (this.obj.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Reset Values"))
        {
            InitValues();
        }
    }
    protected override void InitValues()
    {
        this.p = new Vector3(0.0f, 1.0f, 0.0f);
        this.p = new Vector3(0.0f, 1.0f, 0.0f);
    }
    public void SceneGUI(SceneView sceneView)
    {
        Vector3 p = Handles.PositionHandle(this.p, Quaternion.identity);
        Vector3 q = Handles.PositionHandle(this.q, Quaternion.identity);

        Handles.color = Color.blue;
        Vector3 pxq = CrossProduct(p, q);
        Handles.DrawSolidDisc(pxq, Vector3.forward, 0.05f);

        if (this.p != p || this.q != q)
        {
            Undo.RecordObject(this, "Tool Move");
            this.p = p;
            this.q = q;
            this.pxq = pxq;
            RepaintOnGUI();
        }

        DrawLineGUI(p, "P", Color.green);
        DrawLineGUI(q, "Q", Color.red);
        DrawLineGUI(pxq, "PxQ", Color.blue);
    }
    private void DrawLineGUI(Vector3 pos, string text, Color color)
    {
        Handles.color = color;
        Handles.Label(pos, text, this.guiStyle);
        Handles.DrawAAPolyLine(3f, pos, Vector3.zero);
    }
    private void RepaintOnGUI()
    {
        Repaint();
    }
    // private Vector3 CrossProduct(Vector3 p, Vector3 q)
    // {
    //     float x = p.y * q.z - p.z * q.y;
    //     float y = p.z * q.x - p.x * q.z;
    //     float z = p.x * q.y - p.y * q.x;
    //     return new Vector3(x, y, z);
    // }
    private Vector3 CrossProduct(Vector3 p, Vector3 q)
    {
        Matrix4x4 m = new Matrix4x4();

        m[0, 0] = 0;
        m[0, 1] = q.z;
        m[0, 2] = -q.y;

        m[1, 0] = -q.z;
        m[1, 1] = 0;
        m[1, 2] = q.x;
        
        m[2, 0] = q.y;
        m[2, 1] = -q.x;
        m[2, 2] = 0;

        return m * p;
    }
}
