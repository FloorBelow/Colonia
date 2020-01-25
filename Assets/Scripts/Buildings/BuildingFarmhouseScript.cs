using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFarmhouseScript : InitScript {


	public GameObject farmTilePrefab;
	BuildingScript building;

	public override void Init() {
		building = GetComponent<BuildingScript>();
		Activate();
	}

	public void Activate() {
		for (int y = 0; y < building.map.sizeY; y++) {
			for (int x = 0; x < building.map.sizeX; x++) {
				if(building.map.GetTile(x,y).region == building.map.GetTile(building.x, building.y).region && building.map.CheckClearToBuild(x, y, farmTilePrefab)) {
					CreateFarmTile(x, y);
				}
			}
		}
		building.map.SortSprites();
	}

	void CreateFarmTile(int x, int y) {
		//GameObject farmTile = Instantiate(farmTilePrefab, new Vector3((x - y) * 0.5f + map.transform.position.x, (x + y) * .25f + map.transform.position.y, -10), Quaternion.identity);
		building.map.PlaceBuilding(farmTilePrefab, x, y, sortSprites:false);
		//egionScript.PlaceBuilding(x, y, farmTile);
	}
}
