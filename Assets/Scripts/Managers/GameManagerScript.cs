using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GameManagerScript : MonoBehaviour {

	



    //Let's fuck over everything by removing regions and borders
    //Or at least, make regions like full maps
	//edit: still a bunch of fucky stuff around this

    //holy shit, the whole place building / building ghost system is fucked
    //probably fix using a visual scriptableobject
	//edit: we actually fixed this. yay!

    //Singleton time babey
    public static GameManagerScript m;
	[Header("Game Data")]

	public Transform modelPivot;



    //shit now i wanna make this all scriptableobjects
    //probably ill learn when to use scriptableobjects vs managers better sometime also nice that the word scriptableobject lined up right there

    //Scripts (aaaaaaaaaaaaaa)
    InputScript inputManager;
	AudioMangagerScript audioManager;

	//public GameStateScript gameState;
	public UIManagerScript uiManager;
	public UIManagerScript uiManagerMobile;

	//Game data
	public GameObjectSetData buildingSet;
    public ResourceData[] resourceTypes;
    public Dictionary<string, GameObject> buildings;
    public Dictionary<string, ResourceData> resources;

	public GameObject immigrantWalkerPrefab;




	//should make a dictionary of keywords, too?
	public Keyword resourceStoreKeyword;

	public GameObject startingMap;

	[Header("Art Stuff")]
	public Material modelMat; //sorry
	public Material outlineMat;
	public Color transparentColor;
	public Color transparentRedColor;

	[Header("Instance Variables")]

	public bool mobile;
	public bool debugMode;

	public MapScript activeMap;

	public int mouseTileX;
	public int mouseTileY;

	public RegionScript selectedRegion;
	public GameObject selectedRegionOutline;


	public GameObject selectedObject;

    public GameObject currentBuildingPrefab;
	GameObject buildingGhost;
	List<RoadGhost> roadGhosts;
	public bool buildingGhostRotate;

	public enum ControlState {
		Free,
		PlaceBuilding,
		PlaceRoad,
		PlaceRoadSegment
	}
	public ControlState controlState;

	void Awake() {
		m = this;
		if (mobile) {
			uiManager.gameObject.SetActive(false);
			uiManager = uiManagerMobile;
		} else uiManagerMobile.gameObject.SetActive(false);
	}

    void Start () {
		//Create other managers
		InputScript.OnMouseTileChange += OnMouseTileChange;

        //Build dics
        buildings = new Dictionary<string, GameObject>();
        foreach(GameObject o in buildingSet.objects) {
            buildings.Add(o.GetComponent<BuildingScript>().buildingName, o);
        }
        resources = new Dictionary<string, ResourceData>();
        foreach (ResourceData o in resourceTypes) {
            resources.Add(o.resourceName, o);
        }

        inputManager = gameObject.AddComponent<InputScript>();
        inputManager.gameManager = this;
        audioManager = gameObject.GetComponent<AudioMangagerScript>();

		controlState = ControlState.Free;
		buildingGhostRotate = false;

		activeMap = startingMap.GetComponent<MapScript>();

		for (int i = 0; i < activeMap.sizeX; i++) {
			activeMap.PlaceBuilding(buildings["Road"], i, activeMap.sizeY / 2, sortSprites:false);
		}
		ResourceStorageScript startStorage = activeMap.PlaceBuilding(buildings["Warehouse"], 33, activeMap.sizeY / 2 + 1, sortSprites: false).GetComponent<ResourceStorageScript>();
		activeMap.SortSprites();
		startStorage.AddResource(resources["Wood"], 24);
		startStorage.AddResource(resources["Brick"], 40);


		//gameState = gameObject.AddComponent<GameStateRegionScript>();
		//uiManager.SetRegionInfoPanel(activeMap);

		//activeRegion.CreateRegionOutline(14);
	}


	void Update() {
		mouseTileX = inputManager.mouseTileX;
		mouseTileY = inputManager.mouseTileY;

		if(selectedObject != null) {
			uiManager.SetInfoBox(selectedObject);
		}

		if(selectedRegion != null) uiManager.UpdateRegionPanel();
	}

	void OnMouseTileChange(int x, int y) {
		if(controlState == ControlState.PlaceBuilding) {
			buildingGhost.GetComponent<GridObjectRendererScript>().SetPosition(x, y);
			BuildingAreaDisplayScript area = buildingGhost.GetComponent<BuildingAreaDisplayScript>();
			if (area != null) { area.DestroyRadiusDisplay(); area.CreateRadiusDisplay(); }
			if (activeMap.CheckClearToBuild(x, y, buildingGhost)) {
				buildingGhost.GetComponent<GridObjectRendererScript>().SetSpriteColor(transparentColor);
			} else {
				buildingGhost.GetComponent<GridObjectRendererScript>().SetSpriteColor(transparentRedColor);
			}
		} else if(controlState == ControlState.PlaceRoad) {
			roadGhosts[0].sprite.transform.position = MapScript.TileToPosition(x, y);
			roadGhosts[0] = new RoadGhost(roadGhosts[0].sprite, x, y);
		} else if(controlState == ControlState.PlaceRoadSegment) {
			PlaceRoadSegmentOnMouseTileChange(x, y);
		}
	}



	//Input
	public void LeftClick(bool isMouseOverUI) {
		if (controlState == ControlState.PlaceBuilding) {
			if (isMouseOverUI) {
				RemoveBuildingGhost();
			} else {
				PlaceBuilding(mouseTileX, mouseTileY);
			}
		} else if (controlState == ControlState.PlaceRoad) {
			StartPlaceRoadSegment();
			audioManager.Click();
		} else if (controlState == ControlState.PlaceRoadSegment) {
			PlaceRoadSegment();
		} else {
			SelectBuilding(activeMap.GetTile(mouseTileX, mouseTileY).building);
			RegionScript newSelectedRegion = (activeMap.GetTile(mouseTileX, mouseTileY).region);
			//Select region
			if (newSelectedRegion != selectedRegion) {
				selectedRegion = newSelectedRegion;
				selectedRegion.UpdateAllowedBuildings();
				uiManager.SetRegionPanel(selectedRegion);
				CreateRegionOutline();
				audioManager.Click();
			}
		}

	}

	public void CreateRegionOutline() {
		if (selectedRegionOutline != null) Destroy(selectedRegionOutline);
		selectedRegionOutline = activeMap.CreateRegionOutline(selectedRegion, 299);
	}

	public void ToggleDebug() { debugMode = !debugMode; }

	void SelectBuilding(GameObject building) {
		if (selectedObject != null && selectedObject.GetComponent<BuildingAreaDisplayScript>() != null) selectedObject.GetComponent<BuildingAreaDisplayScript>().DestroyRadiusDisplay();
		selectedObject = building;
		//if (building.GetComponent<BuildingAreaDisplayScript>() != null) building.GetComponent<BuildingAreaDisplayScript>().CreateRadiusDisplay();
	}

	public void RightClick(bool isMouseOverUI) {
		audioManager.Click();
		if (controlState == ControlState.PlaceBuilding) RemoveBuildingGhost();
		else if (controlState == ControlState.PlaceRoad) ExitBuildRoad();
		else if (controlState == ControlState.PlaceRoadSegment) StopPlaceRoadSegment();
		else RemoveBuilding(mouseTileX, mouseTileY);
	}

	public void MiddleClick(bool isMouseOverUI) {
		if (controlState == ControlState.PlaceBuilding) {
			RotateBuildingGhost();
		}
	}

	string IntToRoman(int i) {
		int[] dec = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
		string[] roman = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
		string result = "";
		for(int x = 0; x < dec.Length; x++) {
			while(i % dec[x] < i) {
				result += roman[x];
				i -= dec[x];
			}
		}
		return result;
	}

	public void SetCurrentBuildingPrefab(string name) {
		currentBuildingPrefab = buildings[name];
		if (buildingGhost != null) RemoveBuildingGhost();
		CreateBuildingGhost();
	}

	public void CreateBuildingGhost() {
		controlState = ControlState.PlaceBuilding;
        buildingGhost = new GameObject();
        buildingGhost.transform.SetParent(modelPivot, false);
		GridObjectRendererScript.CreateRenderer(buildingGhost, currentBuildingPrefab.GetComponent<GridObjectRendererScript>().data, mouseTileX, mouseTileY, buildingGhostRotate);
		if (activeMap.CheckClearToBuild(mouseTileX, mouseTileY, buildingGhost)) buildingGhost.GetComponent<GridObjectRendererScript>().SetSpriteColor(transparentColor);
		else buildingGhost.GetComponent<GridObjectRendererScript>().SetSpriteColor(transparentRedColor);
		BuildingAreaDisplayScript prefabArea = currentBuildingPrefab.GetComponent<BuildingAreaDisplayScript>();
		if (prefabArea != null) {
			BuildingAreaDisplayScript area = buildingGhost.AddComponent<BuildingAreaDisplayScript>();
			area.SetData(prefabArea.radius, prefabArea.tileHighlightPrefab, activeMap.sizeX, activeMap.sizeY);
			area.CreateRadiusDisplay();
		}

		audioManager.Click(1);
	}

	public void RotateBuildingGhost() {
		GridObjectRendererScript renderer = buildingGhost.GetComponent<GridObjectRendererScript>();
		if (renderer.data.sizeX != renderer.data.sizeY) {
			buildingGhostRotate = !buildingGhostRotate;
			renderer.Flip();
			//buildingGhost.GetComponent<BuildingScript>().Flip();
			audioManager.Click(1);
		}
	}

	public void RemoveBuildingGhost() {
		Destroy(buildingGhost);
		buildingGhostRotate = false;
		controlState = ControlState.Free;
	}

    public void PlaceBuilding() {
		if(activeMap.CheckClearToBuild(mouseTileX, mouseTileY, buildingGhost)) {
			BuildingScript buildingScript = currentBuildingPrefab.GetComponent<BuildingScript>();
			//Spend resources
			if (!debugMode) {
				for (int i = 0; i < buildingScript.resources.Length; i++) {
					activeMap.RemoveResource(buildingScript.resources[i], buildingScript.resourceCounts[i]);
				}
			}
			activeMap.PlaceBuilding(currentBuildingPrefab, mouseTileX, mouseTileY, buildingGhostRotate);
			audioManager.PlaySFX("build01", 0.5f);
            Destroy(buildingGhost);
			buildingGhost = null;
			CreateBuildingGhost();
		} else {
			Debug.Log("Can't place building here.");
		}
	}

	public void PlaceBuilding(int x, int y) {
		PlaceBuilding();
	}

	public void RemoveBuilding(int x, int y) {
		if (activeMap.RemoveBuilding(x, y)) { audioManager.PlaySFX("build01", 0.5f); }
	}

	public void PurchaseActiveRegion(RegionType type) {
		selectedRegion.regionType = type;
		selectedRegion.UpdateAllowedBuildings();
		uiManager.SetRegionPanel(selectedRegion);
		CreateRegionOutline();
	}

	public void UpgradeActiveRegion() {
		selectedRegion.Upgrade();
		uiManager.SetRegionPanel(selectedRegion);
	}

	public void SaveMap() {
		Debug.Log("Writing map");
		using (StreamWriter stream = new StreamWriter("map.save")) {
			activeMap.Save(stream);
		}
	}

	public void LoadMap() {
		activeMap.LoadMap(File.ReadAllText("map.save"));
	}


	struct RoadGhost {
		public GameObject sprite;
		public int x; public int y;
		public RoadGhost(GameObject sprite, int x, int y) {
			this.sprite = sprite; this.x = x; this.y = y;
		}
	}

	public void EnterBuildRoad() {
		if(controlState == ControlState.PlaceBuilding) RemoveBuildingGhost();
		controlState = ControlState.PlaceRoad;
		if(roadGhosts == null) roadGhosts = new List<RoadGhost>();
		roadGhosts.Add(CreateRoadGhost(mouseTileX, mouseTileY));
	}

	void ExitBuildRoad() {
		foreach (RoadGhost o in roadGhosts) Destroy(o.sprite);
		roadGhosts.Clear();
		controlState = ControlState.Free;
	}

	void PlaceRoadSegmentOnMouseTileChange(int x, int y) {
		int[] path = PathFindScript.PathfindNew(activeMap, roadGhosts[0].x, roadGhosts[0].y, x, y);
		if(path != null) {
			for (int i = 0; i < roadGhosts.Count; i++) {
				Destroy(roadGhosts[i].sprite);
			}
			roadGhosts.Clear();
			for (int i = 0; i < path.Length; i++) {
				if(activeMap.tiles[path[i]].building == null) roadGhosts.Add(CreateRoadGhost(path[i] % activeMap.sizeX, path[i] / activeMap.sizeX));
			}
		}
	}

	void PlaceRoadSegment() {
		foreach(RoadGhost g in roadGhosts) {
			activeMap.PlaceBuilding(buildings["Road"], g.x, g.y, false, false);
		}
		activeMap.SortSprites();
		audioManager.PlaySFX("build01", 0.5f);
		StopPlaceRoadSegment();
	}

	void StartPlaceRoadSegment() {
		controlState = ControlState.PlaceRoadSegment;
	}

	void StopPlaceRoadSegment() {
		for(int i = 1; i < roadGhosts.Count; i++) {
			Destroy(roadGhosts[i].sprite);
		}
		roadGhosts.RemoveRange(1, roadGhosts.Count - 1);
		controlState = ControlState.PlaceRoad;
	}

	RoadGhost CreateRoadGhost(int x, int y) {
		GameObject road = new GameObject();
		road.transform.position = MapScript.TileToPosition(x, y);
		SpriteRenderer s = road.AddComponent<SpriteRenderer>();
		s.sprite = buildings["Road"].GetComponent<GridObjectRendererScript>().data.sprites[0];
		s.color = transparentColor;
		return new RoadGhost(road, x, y);	
	}

}