using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConnectedScript : BuildingScript {

	void UpdateConnections() {
		int connectionData = 0;
		if (x + 1 == map.sizeX || map.GetTile(x+1,y).building != null) {
			if(x + 1 == map.sizeX || map.GetTile(x + 1, y).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				connectionData += 1;
			}
		}
		if (y + 1 == map.sizeY || map.GetTile(x, y+1).building != null) {
			if (y + 1 == map.sizeY || map.GetTile(x, y + 1).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				connectionData += 2;
			}
		}
		if (x == 0 || map.GetTile(x-1, y).building != null) {
			if (x == 0 || map.GetTile(x-1, y).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				connectionData += 4;
			}
		}
		if (y == 0 || map.GetTile(x, y-1).building != null) {
			if (y == 0 || map.GetTile(x, y - 1).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				connectionData += 8;
			}
		}
		//SetSprite(connectionData);
	}

	public override void Flip() {
		//nah
	}

	public void Activate() {
		if (x + 1 != map.sizeX && map.GetTile(x + 1, y).building != null) {
			if (map.GetTile(x + 1, y).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				map.GetTile(x + 1, y).building.GetComponent<BuildingConnectedScript>().UpdateConnections();
			}
		}
		if (y + 1 != map.sizeY && map.GetTile(x, y + 1).building != null) {
			if (map.GetTile(x, y + 1).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				map.GetTile(x, y + 1).building.GetComponent<BuildingConnectedScript>().UpdateConnections();
			}
		}
		if (x != 0 && map.GetTile(x - 1, y).building != null) {
			if (map.GetTile(x - 1, y).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				map.GetTile(x - 1, y).building.GetComponent<BuildingConnectedScript>().UpdateConnections();
			}
		}
		if (y != 0 && map.GetTile(x, y - 1).building != null) {
			if (map.GetTile(x, y - 1).building.GetComponent<BuildingScript>().buildingName == buildingName) {
				map.GetTile(x, y - 1).building.GetComponent<BuildingConnectedScript>().UpdateConnections();
			}
		}
		UpdateConnections();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
