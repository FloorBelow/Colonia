using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*
	BUILDING:
	name,x,y,flip,STORE(optional)|
	STORE:
	STORE,resourcename,count,)






*/

public class MapReader
{
	int i;
	string text;

	public void ReadMap(string inputText, MapScript map) {
		i = 0;
		text = inputText;
		GameObject building = null;
		while (i < text.Length - 1) {
			if (PeekNext() == "STORE") {
				ResourceStorageScript store = building.GetComponent<ResourceStorageScript>();
				SeekNext();
				while (PeekNext() != ")") {
					string resourceName = ReadNext();
					int count = Int32.Parse(ReadNext());
					store.AddResource(UtilityScript.GetResource(resourceName), count);
				}
				SeekNext();
			}
			string buildingName = ReadNext();
			if (!GameManagerScript.m.buildings.ContainsKey(buildingName)) Debug.LogError(buildingName);
			int x = Int32.Parse(ReadNext());
			int y = Int32.Parse(ReadNext());
			bool flip = ReadNext() == "1";
			building = map.PlaceBuilding(GameManagerScript.m.buildings[buildingName], x, y, flip, false);
		}
		map.SortSprites();
	}

	string PeekNext() { return text.Substring(i, text.IndexOf(',', i) - i); }

	string ReadNext() {
		int newI = text.IndexOf(',', i);
		string s = text.Substring(i, newI - i);
		i = newI + 1;
		return s;
	}

	void SeekNext() {
		i = text.IndexOf(',', i) + 1;
	}

}
