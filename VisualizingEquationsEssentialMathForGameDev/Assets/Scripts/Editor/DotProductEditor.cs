using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DotProductEditor : CommonEditor, IUpdateSceneGUI
{
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 c;

    private SerializedObject obj;
    private SerializedProperty propP0;
    private SerializedProperty propP1;
    private SerializedProperty propC;
    private GUIStyle guiStyle = new GUIStyle();

    [MenuItem("Tools/Dot Product Window")]
    private static void ShowWindow() 
    {
        var window = GetWindow(typeof(DotProductEditor), true, "Dot Product");
        window.Show();
    }
    private void OnEnable() 
    {
        if (this.p0 == Vector3.zero && this.p1 == Vector3.zero)
        {
            InitValues();
        }
        this.obj = new SerializedObject(this);
        this.propP0 = obj.FindProperty("p0");
        this.propP1 = obj.FindProperty("p1");
        this.propC = obj.FindProperty("c");
        this.guiStyle.fontSize = 25;
        this.guiStyle.fontStyle = FontStyle.Bold;
        this.guiStyle.normal.textColor = Color.white;
        SceneView.duringSceneGui += SceneGUI;
    }
    private void OnDisable() 
    {
        SceneView.duringSceneGui -= SceneGUI;
    }
    private void OnGUI() 
    {
        this.obj.Update();

        DrawBlockUI("p0", this.propP0);
        DrawBlockUI("p1", this.propP1);
        DrawBlockUI("c", this.propC);

        if (this.obj.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }
    }
    public void SceneGUI(SceneView view)
    {
        Handles.color = Color.red;
        Vector3 p0 = SetMovePoint(this.p0);
        Handles.color = Color.green;
        Vector3 p1 = SetMovePoint(this.p1);
        Handles.color = Color.white;
        Vector3 c = SetMovePoint(this.c);
        if (this.p0 != p0 || this.p1 != p1 || this.c != c)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.c = c;
            Repaint();
        }
        DrawLabel(p0, p1, c);
    }
    protected override void InitValues()
    {
        this.p0 = new Vector3(0.0f, 1.0f, 0.0f);
        this.p1 = new Vector3(0.5f, 0.5f, 0.0f);
        this.c = Vector3.zero;
    }
    Vector3 SetMovePoint(Vector3 pos)
    {
        float size = HandleUtility.GetHandleSize(Vector3.zero) * 0.15f;
        return Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.SphereHandleCap);
    }
    float DotProduct(Vector3 p0, Vector3 p1, Vector3 c)
    {
        Vector3 a = (p0 - c).normalized;
        Vector3 b = (p1 - c).normalized;

        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }
    void DrawLabel(Vector3 p0, Vector3 p1, Vector3 c)
    {
        Handles.Label(c, DotProduct(p0, p1, c).ToString("F1"), this.guiStyle);
        Handles.color = Color.black;

        var cLeft = WorldRotation(p0, c, new Vector3(0.0f, 1.0f, 0.0f));
        var cRight = WorldRotation(p0, c, new Vector3(0.0f, -1.0f, 0.0f));

        Handles.DrawAAPolyLine(5f, p0, c);
        Handles.DrawAAPolyLine(5f, p1, c);
        Handles.DrawAAPolyLine(5f, c, cLeft);
        Handles.DrawAAPolyLine(5f, c, cRight);
    }
    Vector3 WorldRotation(Vector3 p, Vector3 c, Vector3 pos)
    {
        Vector3 dir = (p - c).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        return c + rot * pos;
    }
}