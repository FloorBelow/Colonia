using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class MapScript : MonoBehaviour {



	public Queue<PeriodicUpdate> updateQueue;
	float updateTime = 1;
	float updateTimer;

	public int test;
	public UIManagerScript uiManager;
    public int globalX;
    public int globalY;
    public int sizeX;
    public int sizeY;
	[UnityEngine.Serialization.FormerlySerializedAs("tiles2")]
	public Tile[] tiles;
    public List<GameObject> buildings;
	public List<BuildingHouseScript> houses;
	public List<BuildingWorkerScript> workBuildings;
	public List<ResourceStorageScript> stores;
	public List<GridObjectRendererScript> renderers;
	//public SetGameObject buildingsSet;
	public int population;
	public int workersRequired;
	public string districtName;
	//Keep a list of all resources
	public Dictionary<ResourceData, int> resourceCounts;
	public RegionScript[] regions;


	[System.Serializable]
	public struct Tile {
		public GameObject building;
		public RegionScript region;
	}

	void Start () {
		updateQueue = new Queue<PeriodicUpdate>();
		//buildings = new List<GameObject>();
		resourceCounts = new Dictionary<ResourceData, int>();
		foreach(ResourceData resource in GameManagerScript.m.resourceTypeSet.objects) {
			resourceCounts[resource] = 0;
		}
		updateTimer = updateTime;
	}

	private void Update() {

		if(updateQueue.Count != 0) {
			PeriodicUpdate u = updateQueue.Dequeue();
			u.PeriodicUpdate();
			if (u.IsPeriodicUpdateEnabled()) updateQueue.Enqueue(u);
		}


		int p = 0;
		//Occupants Arriving
		foreach(BuildingHouseScript house in houses) {
			if(house.GetVacancy() > 0) {
				int[] path = PathFindScript.Pathfind(this, 0, sizeY / 2, house.gameObject.GetComponent<BuildingScript>().x, house.gameObject.GetComponent<BuildingScript>().y, true, true);
				if (path != null) {
					//Debug.Log("new immigrant arriving");
					//WalkerScript immigrant = Instantiate(GameManagerScript.m.immigrantWalkerPrefab, new Vector3(-16.5f, 7.75f, 0), Quaternion.identity).GetComponent<WalkerScript>();
					WalkerScript immigrant = Instantiate(GameManagerScript.m.immigrantWalkerPrefab, GameManagerScript.m.modelPivot).GetComponent<WalkerScript>();
					immigrant.Init(this);
					immigrant.AddJob(new WalkerScript.JobWalk(immigrant, path));
					immigrant.AddJob(new WalkerScript.JobAddPopulation(immigrant, house));
					house.occupantsArriving = true;
				}
			}
			p += house.population;
		}
		if(population != p) {
			population = p;
			//Worker distribution
			UpdateWorkers();
		}
		UpdateResourceCounts();
	}

	public Tile GetTile(int x, int y) {
		if (x >= sizeX || y >= sizeY || x < 0 || y < 0) return new Tile();
		return tiles[x + y * sizeX];
	}

	void UpdateResourceCounts() {
		foreach(ResourceData resource in resourceCounts.Keys.ToArray()) {
			resourceCounts[resource] = 0;
		}
		foreach(ResourceStorageScript storage in stores) {
			foreach(ResourceData resource in storage.totals.Keys) {
				resourceCounts[resource] += storage.totals[resource];
			}
		}
	}

	public void RemoveResource(ResourceData resource, int number) {
		foreach(ResourceStorageScript storage in stores) {
			if(storage.GetCount(resource) >= number) {
				storage.RemoveResource(resource, number);
				return;
			} else {
				//Debug.LogError("Not enough " + resource.name);
			}
		}
	}

	void UpdateWorkers() {
		int i = population;
		int w = 0;
		foreach (BuildingWorkerScript workBuilding in workBuildings) {
			i = workBuilding.AddWorkers(i);
			w += workBuilding.workersRequired;
		}
		workersRequired = w;
	}

	//HoLY SHIT
	public void CreateTiles() {
		tiles = new Tile[sizeX * sizeY];
	}

	//Probably should be put into a static helper class somewhere
	public static Vector3 TileToPosition(int x, int y, int depthOffset) {
		return new Vector3((x - y) * 0.5f, (x + y) * .25f, depthOffset);
	}

	public static Vector3 TileToPosition(int x, int y) {
		return TileToPosition(x, y, 0);
	}


	//TODO kind of a problem that this uses renderers rather than buildingscripts. some stuff with building ghost to sort out
	public bool CheckClearToBuild(int x, int y, GameObject buildingPrefab) {

		GridObjectRendererScript renderer = buildingPrefab.GetComponent<GridObjectRendererScript>();
		int buildingX = renderer.flip ? renderer.data.sizeY : renderer.data.sizeX; 
        int buildingY = renderer.flip ? renderer.data.sizeX : renderer.data.sizeY;
		RegionScript region = GameManagerScript.m.selectedRegion;
        if (x >= 0 && y >= 0 && x + buildingX <= sizeX && y + buildingY <= sizeY) {
			for (int u = 0; u < buildingX; u++) {
				for (int v = 0; v < buildingY; v++) {
					if (GetTile(x + u, y + v).building != null || GetTile(x+u,y+v).region != region) {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}

    public GameObject PlaceBuilding(GameObject prefab, int x, int y, bool flip = false, bool sortSprites = true) {
        GameObject newBuilding = Instantiate(prefab, GameManagerScript.m.modelPivot);

		buildings.Add(newBuilding);

		if (newBuilding.GetComponent<BuildingHouseScript>() != null) houses.Add(newBuilding.GetComponent<BuildingHouseScript>());

		if (newBuilding.GetComponent<BuildingWorkerScript>() != null) {
			workBuildings.Add(newBuilding.GetComponent<BuildingWorkerScript>());
			UpdateWorkers();
		}
		if(newBuilding.HasKeyword(GameManagerScript.m.resourceStoreKeyword)) {
			stores.Add(newBuilding.GetComponent<ResourceStorageScript>());
		}

		BuildingScript buildingScript = newBuilding.GetComponent<BuildingScript>();
		GetTile(x,y).region.AddBuilding(buildingScript);
		//newBuilding.transform.name = buildingScript.buildingName + " " + buildings.Count;
		buildingScript.x = x; buildingScript.y = y;
		buildingScript.map = this;



		//Need to do renderer after setting up building data. Is that okay? :w
		GridObjectRendererScript renderer = newBuilding.GetComponent<GridObjectRendererScript>();


		//there's some fragile ordering stuff going on here i need to look more closely into later

		renderer.CreateRenderers(new GameObject(buildingScript.buildingName + " " + buildings.Count + " Sprite"), newBuilding);
		if (renderer.data.isTerrainOnly) renderer.SetPosition(x, y, sizeX * sizeY); else renderer.SetPosition(x, y);

		if (buildingScript.sizeX == buildingScript.sizeY) flip = false;
		if (flip) {
			renderer.Flip();
			buildingScript.Flip();
		}

		for (int u = 0; u < buildingScript.sizeX; u++) {
			for (int v = 0; v < buildingScript.sizeY; v++) {
				tiles[x + u + (y + v) * sizeX].building = newBuilding;
			}
		}
		//Sort grid renderers (do we need to put nongrid renderers here as well?
		//GridObjectRendererScript[] renderers = new GridObjectRendererScript[buildings.Count];
		if (!renderer.data.isTerrainOnly) {
			renderer.CheckOverlaps(renderers);
			renderers.Add(renderer);
			if (sortSprites) GridObjectRendererScript.SortSprites(renderers);
		}


        InitScript[] scripts = buildingScript.gameObject.GetComponents<InitScript>();
        foreach (InitScript script in scripts) script.Init();


		
	    

		//Debug.Log("Placed building");

		//Create event system?
		/*
		if(newBuilding.GetComponent<BuildingConnectedScript>() != null) {
			newBuilding.GetComponent<BuildingConnectedScript>().Activate();
		}
		*/
		//UNSAFE FOR MULTIPLE DISTRICT TYPES
		//((DistrictResidentialScript)district).UpdateNeeds();
		return newBuilding;
    }

	public void SortSprites() { GridObjectRendererScript.SortSprites(renderers); }

	public bool RemoveBuilding(int x, int y) {
		GameObject buildingToDestroy = GetTile(x, y).building;
		if (buildingToDestroy != null) {
			buildings.Remove(buildingToDestroy);
			houses.Remove(buildingToDestroy.GetComponent<BuildingHouseScript>());
			workBuildings.Remove(buildingToDestroy.GetComponent<BuildingWorkerScript>());
			stores.Remove(buildingToDestroy.GetComponent<ResourceStorageScript>());

			GridObjectRendererScript gridRenderer = buildingToDestroy.GetComponent<GridObjectRendererScript>();
			renderers.Remove(gridRenderer);
			for(int i = 0; i < renderers.Count; i++) {
				renderers[i].objectsBehindMe.Remove(gridRenderer);
			}
			Destroy(buildingToDestroy);
			return true;
		}
		return false;
	}

	public GameObject CreateRegionOutline(RegionScript region, int z) {
		//RegionScript region = regions[i];
		LineRenderer line = new GameObject("Outline").AddComponent<LineRenderer>();
		Vector3[] positions = GetRegionOutlinePositions(region, z);

		line.loop = false;
		line.widthMultiplier = 0.04f;
		line.material = GameManagerScript.m.outlineMat;
		line.startColor = region.regionType.outlineColor; line.endColor = region.regionType.outlineColor;
		line.positionCount = positions.Length;
		line.SetPositions(positions);
		return line.gameObject;
	}

	public Vector3[] GetRegionOutlinePositions(RegionScript region, int z) {
		List<Vector3> positions = new List<Vector3>();

		int x = 0; int y = 0;
		//Find start
		for (int v = 0; v<sizeY; v++) {
			for (int u = 0; u<sizeX; u++) {
				if(GetTile(u, v).region == region) {

					x = u; y = v;
					positions.Add(TileToPosition(x + 1, y, z));
					positions.Add(TileToPosition(x, y, z));
					positions.Add(TileToPosition(x, y + 1, z));
					goto End;
				}
			}
		}
		End:

		int debugLimit = 0;
		int side = 2;
		while(debugLimit< 1000) {
			switch(side) {
				case 0:
					if (GetTile(x, y-1).region == region) {
						y--;
						side = 3;
					} else {
						//Debug.Log("Drawing left");
						positions.Add(TileToPosition(x, y, z));
						side++;
					}
					break;
				case 1:
					if (GetTile(x-1, y).region == region) {
						x--;
						side--;
					} else {
						//Debug.Log("Drawing up");
						positions.Add(TileToPosition(x, y+1, z));
						side++;
					}
					break;
				case 2:
					if(GetTile(x, y+1).region == region) {
						y++;
						side--;
					} else {
						//Debug.Log("Drawing right");
						positions.Add(TileToPosition(x+1, y+1, z));
						side++;
					}
					break;
				case 3:
					if (GetTile(x+1, y).region == region) {
						x++;
						side--;
					} else {
						//Debug.Log("Drawing down");
						positions.Add(TileToPosition(x+1, y, z));
						side = 0;
					}
					break;
			}
			if(positions[0] == positions[positions.Count - 1]) {
				break;
			}
			debugLimit++;
		}
		return positions.ToArray();
	}

	public void Save(StreamWriter stream) {
		stream.NewLine = ",";
		for(int i = 0; i < buildings.Count; i++) {
			BuildingScript building = buildings[i].GetComponent<BuildingScript>();
			stream.WriteLine(building.buildingName);
			stream.WriteLine(building.x);
			stream.WriteLine(building.y);
			if (building.flip) stream.WriteLine("1"); else stream.WriteLine("0");
			ResourceStorageScript store = building.GetComponent<ResourceStorageScript>();
			if(store != null) {
				bool startwritestore = false;
				foreach(ResourceData resource in store.totals.Keys) {
					if (store.totals[resource] > 0) {
						if (!startwritestore) { stream.WriteLine("STORE"); startwritestore = true; }
						stream.WriteLine(resource.resourceName);
						stream.WriteLine(store.totals[resource]);
					}
				}
				if (startwritestore) stream.WriteLine(")");
			}

		}
	}

	public void LoadMap(string map) {
		ClearMap();

		MapReader reader = new MapReader();
		reader.ReadMap(map, this);
	}

	void ClearMap() {
		for(int i = buildings.Count - 1; i >= 0; i--) {
			DestroyImmediate(buildings[i]);
		}
		foreach (WalkerScript walker in GameManagerScript.m.modelPivot.GetComponentsInChildren<WalkerScript>()) {
			DestroyImmediate(walker.gameObject);
		}
		buildings.Clear();
		houses.Clear();
		workBuildings.Clear();
		stores.Clear();
		renderers.Clear();
		updateQueue.Clear();
	}

}

public interface PeriodicUpdate {
	bool IsPeriodicUpdateEnabled();
	void PeriodicUpdate();
}
