using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceData))]
public class ResourceDataEditorScript : Editor
{
    Object spriteAtlas;
    Object modelAtlas;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        spriteAtlas = EditorGUILayout.ObjectField("sprite atlas", spriteAtlas, typeof(Object));
        modelAtlas = EditorGUILayout.ObjectField("model atlas", modelAtlas, typeof(Object));
        if (GUILayout.Button("Get Assets")) {
            ResourceData r = (ResourceData)target;
            r.sprites = Resources.LoadAll<Sprite>(spriteAtlas.name);
            r.models = Resources.LoadAll<Mesh>(modelAtlas.name);
            Debug.Log(r.sprites.Length.ToString() + ", " + r.models.Length.ToString());
            EditorUtility.SetDirty(r);
            AssetDatabase.SaveAssets();
        }
    }
}
