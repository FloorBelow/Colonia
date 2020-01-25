using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : InitScript {

    [Header("Base Data")]
    public string buildingName;
    public int sizeX; public int sizeY;	public int sizeZ;
	public IconData icon;
	public ResourceData[] resources;
	public int[] resourceCounts;



    [Header("Instance Variables")]
    public int x; public int y;
    public bool flip;
    bool active = false;
    public int currentSpriteIndex;
    public MapScript map;

    void Start () {
	}

	public virtual void Flip() {
		int temp = sizeX; sizeX = sizeY; sizeY = temp;
        flip = !flip;
        /*
		if(currentSpriteIndex == 1) {
			SetSprite(0);
		} else {
			SetSprite(1);
		}
        */
	}

    public override void Init(){
        //FIX - faster get?
        active = true;
        /*
        if(modelPrefab != null) {
            model = Instantiate(modelPrefab, GameObject.Find("3dWorld").transform);
            model.transform.localPosition = new Vector3((region.globalX + x) * -1, 0, (region.globalY + y) * -1);
            if (flip) {
                model.transform.localPosition += new Vector3(sizeX * -1, 0, 0);
                model.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
        }
        */
        /*
        int[] path = PathFindScript.Pathfind(region, x, y, 1, 1);
        if(path != null){
            for(int i = 0; i < path.Length; i++) {
                Debug.Log((path[i] % region.sizeX).ToString() + "," + (path[i] / region.sizeX).ToString() + " - " + path[i]);
            }
            PathFindScript.DrawPath(region, gameObject, path);
        }
        */
    }

    public bool IsActive() { return active; }

}
