using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GameManagerScript : MonoBehaviour {

	public bool mobile;
	public bool debugMode;

    //Let's fuck over everything by removing regions and borders
    //Or at least, make regions like full maps
	//edit: still a bunch of fucky stuff around this

    //holy shit, the whole place building / building ghost system is fucked
    //probably fix using a visual scriptableobject
	//edit: we actually fixed this. yay!

    //Singleton time babey
    public static GameManagerScript m;
	public Transform threedeeworld;

    public Material modelMat; //sorry

    //shit now i wanna make this all scriptableobjects
    //probably ill learn when to use scriptableobjects vs managers better sometime also nice that the word scriptableobject lined up right there

    //Scripts (aaaaaaaaaaaaaa)
    InputScript inputManager;
	AudioMangagerScript audioManager;
	public MapScript activeMap;
	public GameStateScript gameState;
	public UIManagerScript uiManager;
	public UIManagerScript uiManagerMobile;

	//Game data
	public GameObjectSetData buildingSet;
    public ResourceData[] resourceTypes;
    public Dictionary<string, GameObject> buildings;
    public Dictionary<string, ResourceData> resources;

	public GameObject regionPrefab;
	public GameObject immigrantWalkerPrefab;

	public int regionSize;

	public Color transparentColor;
	public Color transparentRedColor;
	public Material outlineMat;

	//should make a dictionary of keywords, too?
	public Keyword resourceStoreKeyword;

	public GameObject startingMap;


	[Header("Instance Variables")]
	public int mouseTileX;
	public int mouseTileY;

	public RegionScript selectedRegion;
	public GameObject selectedRegionOutline;


	public GameObject selectedObject;
	public GameObject selectedBorder;
	public int selectedBorderX;
	public int selectedBorderY;

    public GameObject currentBuildingPrefab;
	public int currentBuildingIndex;
	public GameObject buildingGhost;
	public bool buildingGhostRotate;
	public int placeBuilding;//0 - default; 1 - placing building

	public bool placeBorder;

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

		placeBuilding = 0;
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
		if(placeBuilding == 1) {
			buildingGhost.GetComponent<GridObjectRendererScript>().SetPosition(x, y);
			BuildingAreaDisplayScript area = buildingGhost.GetComponent<BuildingAreaDisplayScript>();
			if (area != null) { area.DestroyRadiusDisplay(); area.CreateRadiusDisplay(); }
			if (activeMap.CheckClearToBuild(mouseTileX, mouseTileY, buildingGhost)) {
				buildingGhost.GetComponent<GridObjectRendererScript>().SetSpriteColor(transparentColor);
			} else {
				buildingGhost.GetComponent<GridObjectRendererScript>().SetSpriteColor(transparentRedColor);
			}
		}
	}



	//Input
	public void LeftClick(bool isMouseOverUI) {
		if (placeBuilding == 1) {
			if (isMouseOverUI) {
				RemoveBuildingGhost();
			} else {
				PlaceBuilding(mouseTileX, mouseTileY);
			}
        /*
		} else if (mouseTileX - activeRegionX >= regionSize || mouseTileY - activeRegionY >= regionSize || placeBorder) {
			Debug.Log("Place road?");
			int[] activeBorderCoords = FindActiveBorder();
			if(selectedBorder == null || bordersCross[activeBorderCoords[0], activeBorderCoords[1]] != selectedBorder && bordersCross[activeBorderCoords[0], activeBorderCoords[1]] != null) {
				SelectBorder(activeBorderCoords[0], activeBorderCoords[1]);
			}
            */
		} else {
			SelectBuilding(activeMap.GetTile(mouseTileX, mouseTileY).building);
			RegionScript newSelectedRegion = (activeMap.GetTile(mouseTileX, mouseTileY).region);
			//Select region
			if (newSelectedRegion != selectedRegion) {
				selectedRegion = newSelectedRegion;
				selectedRegion.UpdateAllowedBuildings();
				uiManager.SetRegionPanel(selectedRegion);
				CreateRegionOutline();
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
		if (placeBuilding == 1) {
			RemoveBuildingGhost();
		} else if (placeBorder) {
			selectedBorder = null;
			placeBorder = false;
		} else {
			RemoveBuilding(mouseTileX, mouseTileY);
		}
	}

	public void MiddleClick(bool isMouseOverUI) {
		if (placeBuilding == 1) {
			RotateBuildingGhost();
		}
	}

	/*
	private void FindActiveRegion() {
		int x = mouseTileX; int y = mouseTileY;
		int regionX = x / (regionSize + borderSize);
		int regionY = y / (regionSize + borderSize);
		if (regionX >= 0 && regionY >= 0 && regionX <= world.GetUpperBound(0) && regionY < world.GetUpperBound(1)) {
			SelectRegion(regionX, regionY);
			uiManager.SetRegionInfoPanel(activeRegion);
		}
	}
	

	public void SelectRegion(int x, int y) {
		activeRegion = world[x, y];
		activeRegionX = x * (regionSize + borderSize); activeRegionY = y * (regionSize + borderSize);
	}
	*/

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
	/*
	void CreateWorld(int sizeX, int sizeY) {
		int regionDistance = regionSize + borderSize;
		world = new MapScript[sizeX, sizeY];
		//bordersCross = new GameObject[sizeX + 1, sizeY + 1];
		//bordersX = new GameObject[sizeX, sizeY + 1];
		//bordersY = new GameObject[sizeX + 1, sizeY];

		//CreateRegions
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				GameObject newRegion = Instantiate(regionPrefab, new Vector3((x-y) * regionDistance/ 2f, (x+y) * regionDistance/ 4f, 200), Quaternion.identity, transform);
				newRegion.transform.name = "Region " + x + ", " + y;
				MapScript newRegionScript = newRegion.GetComponent<MapScript>();
                newRegionScript.globalX = x * regionDistance; newRegionScript.globalY = y * regionDistance;
				newRegionScript.uiManager = uiManager;
				newRegionScript.CreateTiles(tilePrefab, regionSize, regionSize, x * (regionSize + borderSize), y * (regionSize + borderSize));
				newRegionScript.districtName = "Regio " + IntToRoman(sizeX * y + x + 1) + " - Artisan Quarter";
				world[x, y] = newRegionScript;
			}
		}
		*/
        /*
		//Crossroads
		for (int x = 0; x <= sizeX; x++) {
			for (int y = 0; y <= sizeY; y++) {
				bordersCross[x, y] = Instantiate(borderCrossPrefab, new Vector3((x - y) * regionDistance / 2f, ((x + y) * regionDistance - borderSize * 2) / 4f, x + y + 199.5f), Quaternion.identity, transform);
				bordersCross[x, y].transform.name = "BorderCross " + x + ", " + y;
				bordersCross[x, y].GetComponent<BorderScript>().x = x; bordersCross[x, y].GetComponent<BorderScript>().y = y;
			}
		}

		//X Borders
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y <= sizeY; y++) {
				bordersX[x, y] = Instantiate(borderXPrefab, new Vector3(((x - y) * regionDistance + borderSize)  / 2f, ((x + y) * regionDistance - borderSize) / 4f, x + y + 200), Quaternion.identity, transform);
				bordersX[x, y].transform.name = "BorderX " + x + ", " + y;
				bordersX[x, y].GetComponent<BorderScript>().x = x; bordersCross[x, y].GetComponent<BorderScript>().y = y;
			}
		}

		//Y Borders
		for (int x = 0; x <= sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				bordersY[x, y] = Instantiate(borderYPrefab, new Vector3(((x - y) * regionDistance - borderSize) / 2f, ((x + y) * regionDistance - borderSize) / 4f, x + y + 200), Quaternion.identity, transform);
				bordersY[x, y].transform.name = "BorderY " + x + ", " + y;
				bordersY[x, y].GetComponent<BorderScript>().x = x; bordersCross[x, y].GetComponent<BorderScript>().y = y;
			}
		}
        */
		/*
	}
	*/

	public void SetCurrentBuildingPrefab(string name) {
		currentBuildingPrefab = buildings[name];
		CreateBuildingGhost();
	}

	public void CreateBuildingGhost() {
		placeBuilding = 1;
        buildingGhost = new GameObject();
        buildingGhost.transform.SetParent(threedeeworld, false);
        GameObject spriteGhost = new GameObject();
        GridObjectRendererScript renderer = buildingGhost.AddComponent<GridObjectRendererScript>();
        renderer.SetData(currentBuildingPrefab.GetComponent<GridObjectRendererScript>().data);
        renderer.CreateRenderers(spriteGhost, buildingGhost);
        renderer.SetPosition(mouseTileX, mouseTileY);
		if (renderer.data.sizeX == renderer.data.sizeY) buildingGhostRotate = false;
		if (buildingGhostRotate) renderer.Flip();
        //buildingGhost = Instantiate(currentBuildingPrefab, new Vector3((mouseTileX - mouseTileY) * 0.5f, (mouseTileX + mouseTileY) * .25f, -10), Quaternion.identity);
        //buildingGhost.GetComponent<SpriteRenderer>().color = transparentColor;
        audioManager.PlaySFX("click01", 0.25f);

		BuildingAreaDisplayScript prefabArea = currentBuildingPrefab.GetComponent<BuildingAreaDisplayScript>();
		if (prefabArea != null) {
			BuildingAreaDisplayScript area = buildingGhost.AddComponent<BuildingAreaDisplayScript>();
			area.SetData(prefabArea.radius, prefabArea.tileHighlightPrefab, activeMap.sizeX, activeMap.sizeY);
			area.CreateRadiusDisplay();
		}
	}

	public void RotateBuildingGhost() {
		GridObjectRendererScript renderer = buildingGhost.GetComponent<GridObjectRendererScript>();
		if (renderer.data.sizeX != renderer.data.sizeY) {
			buildingGhostRotate = !buildingGhostRotate;
			renderer.Flip();
			//buildingGhost.GetComponent<BuildingScript>().Flip();
			audioManager.PlaySFX("click01", 0.25f);
		}
	}

	public void RemoveBuildingGhost() {
		Destroy(buildingGhost);
		buildingGhostRotate = false;
		placeBuilding = 0;
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



}