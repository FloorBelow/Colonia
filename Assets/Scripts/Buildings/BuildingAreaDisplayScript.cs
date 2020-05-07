using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAreaDisplayScript : MonoBehaviour
{
	public GameObject tileHighlightPrefab;
	public float radius;

	float powRadius;
	Transform parent;
	int sizeX; int sizeY;

	BuildingScript building;
	GridObjectRendererScript renderer;
	HashSet<GameObject> buildingsInRange;
	
    // Start is called before the first frame update
    void Start()
    {
		building = gameObject.GetComponent<BuildingScript>();
		if (building != null) { sizeX = building.map.sizeX; sizeY = building.map.sizeY; }
		renderer = gameObject.GetComponent<GridObjectRendererScript>();
    }

	public void SetData(float radius, GameObject tileHighlightPrefab, int x, int y) {
		this.radius = radius; this.tileHighlightPrefab = tileHighlightPrefab; sizeX = x; sizeY = y;
		renderer = gameObject.GetComponent<GridObjectRendererScript>();
	}

    public void CreateRadiusDisplay() {
		int buildingX = (renderer.flip ? renderer.data.sizeY : renderer.data.sizeX);
		int buildingY = (renderer.flip ? renderer.data.sizeX : renderer.data.sizeY);
		float xPos = renderer.x + ((float)buildingX - 1) / 2;
		float yPos = renderer.y + ((float)buildingY - 1) / 2;
		powRadius = Mathf.Pow(radius, 2);

		parent = new GameObject("Radius Display").transform;
		parent.SetParent(renderer.spriteObject.transform);

		for (int y = renderer.y - (int)radius; y < renderer.y + buildingY + (int)radius; y++ ) {
			for (int x = renderer.x - (int)radius; x < renderer.x + buildingX + (int)radius; x++) {
				if(y >= 0 && y < sizeY && x >= 0 && x < sizeX) {
					if (Mathf.Pow((float)x - xPos, 2) + Mathf.Pow((float)y - yPos, 2) < powRadius) {
						GameObject highlight = Instantiate(tileHighlightPrefab, parent.transform);
						highlight.transform.position = MapScript.TileToPosition(x, y, 0);
					}
				}
			}
		}
	}

	public HashSet<GameObject> GetBuildingsInRange() {
		if (buildingsInRange == null) buildingsInRange = new HashSet<GameObject>();
		else buildingsInRange.Clear();
		if (renderer == null) renderer = gameObject.GetComponent<GridObjectRendererScript>();
		if (building == null) { building = gameObject.GetComponent<BuildingScript>(); sizeX = building.map.sizeX; sizeY = building.map.sizeY; }
		int buildingX = (renderer.flip ? renderer.data.sizeY : renderer.data.sizeX);
		int buildingY = (renderer.flip ? renderer.data.sizeX : renderer.data.sizeY);
		float xPos = renderer.x + ((float)buildingX - 1) / 2;
		float yPos = renderer.y + ((float)buildingY - 1) / 2;
		powRadius = Mathf.Pow(radius, 2);
		for (int y = renderer.y - (int)radius; y < renderer.y + buildingY + (int)radius; y++) {
			for (int x = renderer.x - (int)radius; x < renderer.x + buildingX + (int)radius; x++) {
				if (y >= 0 && y < sizeY && x >= 0 && x < sizeX) {
					if (Mathf.Pow((float)x - xPos, 2) + Mathf.Pow((float)y - yPos, 2) < powRadius) {
						if (building.map.GetTile(x,y).building != null) buildingsInRange.Add(building.map.GetTile(x, y).building);
					}
				}
			}
		}
		return buildingsInRange;
	}

	public void DestroyRadiusDisplay() { if(parent != null) Destroy(parent.gameObject); }
}