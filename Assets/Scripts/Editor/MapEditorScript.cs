using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapScript))]
public class MapEditorScript : Editor {
	Texture2D regionTexture;
	int sizeX; int sizeY;
	bool showTiles;
	Color[] regionColors;
	public override void OnInspectorGUI() {
		MapScript map = (MapScript)target;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("x:");
		sizeX = EditorGUILayout.IntField(sizeX);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("y:");
		sizeY = EditorGUILayout.IntField(sizeY);
		EditorGUILayout.EndHorizontal();
		if(GUILayout.Button("Create map")) {
			if (map.tiles != null) map.tiles = null;
			map.sizeX = sizeX; map.sizeY = sizeY;
			map.CreateTiles();
		}
		EditorGUILayout.LabelField("Regions");
		regionTexture = (Texture2D)EditorGUILayout.ObjectField(regionTexture, typeof(Texture2D), true);
		if( GUILayout.Button("Import region texture")) {
			if (regionTexture.width < map.sizeX || regionTexture.height < map.sizeY) {
				Debug.LogError("Region texture too small to create map");
			} else {
				List<Color> regionColorList = new List<Color>();
				for (int y = 0; y < regionTexture.height; y++) {
					for (int x = 0; x < regionTexture.height; x++) {
						Color pixColor = regionTexture.GetPixel(x, y);
						if (!regionColorList.Contains(pixColor)) regionColorList.Add(pixColor);
					}
				}
				regionColors = regionColorList.ToArray();

				map.regions = new RegionScript[regionColors.Length];
				for (int i = 0; i < regionColors.Length; i++) {
					GameObject regionObject = new GameObject();
					regionObject.name = "Region " + i.ToString();
					regionObject.transform.SetParent(map.transform);
					map.regions[i] = regionObject.AddComponent<RegionScript>();
					map.regions[i].map = map;
					map.regions[i].color = regionColors[i];
				}
				for(int i = 0; i < map.tiles.Length; i++) {
					map.tiles[i].region = map.regions[System.Array.IndexOf(regionColors, regionTexture.GetPixel(i % map.sizeX, i / map.sizeX))];
				}
			}
		}
		if(regionColors != null) foreach(Color color in regionColors) {
			EditorGUILayout.ColorField(color);
		}
		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		DrawDefaultInspector();
		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
		if(showTiles) {
			for (int i = 0; i < map.tiles.Length; i++) {
				if (map.tiles[i].region != null) EditorGUILayout.LabelField(map.tiles[i].region.name);
			}
		}

	}


}
