using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridObjectRendererScript))]
public class GridObjectRendererEditorScript : Editor
{
    public GridObjectRendererData data;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        data = (GridObjectRendererData) EditorGUILayout.ObjectField("data", data, typeof(GridObjectRendererData));
        if(GUILayout.Button("Change Data")) {
            GridObjectRendererScript s = (GridObjectRendererScript)target;
            s.UpdateData(data);
        }
    }
}
