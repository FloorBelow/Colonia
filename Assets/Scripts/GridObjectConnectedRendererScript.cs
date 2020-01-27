using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectConnectedRendererScript : GridObjectRendererScript
{
	public int connectionFlags;
	bool nocenter;

	//Needs to be on a building
    public override void SetPosition(int x, int y, int z = 0) {
		base.SetPosition(x, y, z);
		connectionFlags = 0;
		UpdateAdjacentConnections();
	}

	void UpdateAdjacentConnections(bool remove = false) {
		MapScript map = gameObject.GetComponent<BuildingScript>().map;
		if (x + 1 != map.sizeX && map.GetTile(x + 1, y).building != null) {
			//TODO Should probably change to building name rather than basing off render data, but that should come after making buildings scriptableobjects
			GridObjectRendererScript renderer = map.GetTile(x+1, y).building.GetComponent<GridObjectRendererScript>();
			if (data == renderer.data) {
				connectionFlags += 1;
				int i = remove ? -4 : 4;
				((GridObjectConnectedRendererScript)renderer).UpdateConnectedSprite(i);
			}
		}
		if (y + 1 != map.sizeY && map.GetTile(x, y + 1).building != null) {
			GridObjectRendererScript renderer = map.GetTile(x, y+1).building.GetComponent<GridObjectRendererScript>();
			if (data == renderer.data) {
				connectionFlags += 2;
				int i = remove ? -8 : 8;
				((GridObjectConnectedRendererScript)renderer).UpdateConnectedSprite(i);
			}
		}
		if (x != 0 && map.GetTile(x - 1, y).building != null) {
			GridObjectRendererScript renderer = map.GetTile(x-1, y).building.GetComponent<GridObjectRendererScript>();
			if (data == renderer.data) {
				connectionFlags += 4;
				int i = remove ? -1 : 1;
				((GridObjectConnectedRendererScript)renderer).UpdateConnectedSprite(i);
			}
		}
		if (y != 0 && map.GetTile(x, y - 1).building != null) {
			GridObjectRendererScript renderer = map.GetTile(x, y-1).building.GetComponent<GridObjectRendererScript>();
			if (data == renderer.data) {
				connectionFlags += 8;
				int i = remove ? -2 : 2;
				((GridObjectConnectedRendererScript)renderer).UpdateConnectedSprite(i);
			}
		}
		if(!remove) UpdateConnectedSprite(0);
	}

	private void OnDestroy() {
		UpdateAdjacentConnections(true);
		if (spriteObject != gameObject) Destroy(spriteObject);
		if (modelObject != gameObject) Destroy(modelObject);
	}

	public void UpdateConnectedSprite(int i) {
		connectionFlags += i;
		//Debug.Log(connectionFlags);

		if (spriteObject != null) {
			if (connectionFlags >= data.sprites.Length) {
				spriteObject.GetComponent<SpriteRenderer>().enabled = false;
			} else {
				spriteObject.GetComponent<SpriteRenderer>().enabled = true;
				spriteObject.GetComponent<SpriteRenderer>().sprite = data.sprites[connectionFlags];
			}
		}

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
