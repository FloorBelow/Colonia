using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UIColorScript))]
public class UIColorEditorScript : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if(GUILayout.Button("Set color of images")) {
			UIColorScript scr = (UIColorScript)target;
			Image[] images = scr.gameObject.GetComponentsInChildren<Image>();
			foreach(Image image in images) {
				image.color = scr.color;
			}
		}
	}
}
