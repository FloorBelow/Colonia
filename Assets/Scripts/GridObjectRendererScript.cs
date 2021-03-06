﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectRendererScript : MonoBehaviour
{

    //Honestly it wouldn't be hard to change this to all objects just by using floats
    //What do we do about animated sprites, tho?
    public GridObjectRendererData data;
    public GridObjectRendererData fadeData;
    public int x; public int y; public int z;
    public GameObject spriteObject; //Public for sorting sprites, should be moved here? static method on this[] LATER COMMENT I DONT KNOW WHAT THIS MEANS
    public GameObject modelObject;
	GameObject spriteOverlayObject;
    public bool flip;
    public List<GridObjectRendererScript> objectsBehindMe;
    public HashSet<WalkerScript> walkersBehindMe;
    public bool visited;

    public void CreateRenderers(GameObject spriteObject, GameObject modelObject) {

        walkersBehindMe = new HashSet<WalkerScript>();

        //2d
        this.spriteObject = spriteObject; this.modelObject = modelObject;
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.spriteNorm;
		if(data.spriteOverlay != null) {
			spriteOverlayObject = Instantiate(spriteObject, spriteObject.transform);
			spriteOverlayObject.GetComponent<SpriteRenderer>().sprite = data.spriteOverlay;
			spriteOverlayObject.transform.localPosition = new Vector3(0, 0, -0.09f);
		}
        //3d 
        if(!GameManagerScript.m.mobile && data.model != null) {
            modelObject.layer = 8;
            MeshFilter meshFilter = modelObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = data.model;
            MeshRenderer meshRenderer = modelObject.AddComponent<MeshRenderer>();
            meshRenderer.material = UtilityScript.data.modelMat;
        }
    }

    public static GameObject CreateRenderer(GameObject rendererObject, GridObjectRendererData data, int x, int y, bool flip) {
        GameObject spriteGhost = new GameObject();
        GridObjectRendererScript renderer = rendererObject.AddComponent<GridObjectRendererScript>();
        renderer.SetData(data);
        renderer.CreateRenderers(spriteGhost, rendererObject);
        renderer.SetPosition(x, y);
        if (flip) renderer.Flip();

        return rendererObject;
    }

    public void SetData(GridObjectRendererData data) {
        this.data = data;
    }

    public void UpdateData(GridObjectRendererData data) {
        this.data = data;
        spriteObject.GetComponent<SpriteRenderer>().sprite = data.spriteNorm;
        if (spriteOverlayObject != null) spriteOverlayObject.GetComponent<SpriteRenderer>().sprite = data.spriteOverlay;
        modelObject.GetComponent<MeshFilter>().sharedMesh = data.model;
    }

    public void SetSpriteColor(Color color) {
        spriteObject.GetComponent<SpriteRenderer>().color = color;
		if(spriteOverlayObject != null) spriteOverlayObject.GetComponent<SpriteRenderer>().color = color;
	}

    private void OnDestroy() {
        if (spriteObject != gameObject) Destroy(spriteObject);
        if (modelObject != gameObject) Destroy(modelObject);
    }

    public virtual void SetPosition(int x, int y, int z = 0) {
        this.x = x; this.y = y; this.z = z;
        spriteObject.transform.localPosition = data.isTerrainOnly ? new Vector3((x - y) * .5f, (x + y) * .25f, z) : new Vector3((x - y) * .5f, (x + y) * .25f, z * .5f);
        modelObject.transform.localPosition = new Vector3(x * -1, z, y * -1);
        if (flip) modelObject.transform.localPosition += new Vector3(data.sizeY * -1, 0, 0);
    }

    public void Flip() {
        if (flip) {
            spriteObject.transform.localScale = Vector3.one;
			modelObject.transform.localRotation = Quaternion.identity;
            modelObject.transform.localPosition += new Vector3(data.sizeY, 0, 0);

        } else {
            spriteObject.transform.localScale = new Vector3(-1, 1, 1);
			modelObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
            modelObject.transform.localPosition += new Vector3(data.sizeY * -1, 0, 0);
        }
        flip = !flip;
    }

    public void CheckOverlaps(List<GridObjectRendererScript> objects) {
        if (objectsBehindMe == null) { objectsBehindMe = new List<GridObjectRendererScript>(); }
        for (int i = 0; i < objects.Count; i++) {
            if(objects[i] != this) {
                int overlap = CheckOverlap(objects[i]);
                if (overlap == 1) {
                    //Debug.Log("New building is in front of " + objects[i].name);
                    objectsBehindMe.Add(objects[i]);
                } else if (overlap == 2) {
                    //Debug.Log("New building is behind building " + objects[i].name);
                    objects[i].objectsBehindMe.Add(this);
                } else {
                    //Debug.Log("New building does not overlap with building " + objects[i].name);
                }
            }
        }
    }

    int CheckOverlap(GridObjectRendererScript b) {
        bool isOverlap = true;
        int aSizeX = flip ? data.sizeY : data.sizeX; 
        int aSizeY = flip ? data.sizeX : data.sizeY; 
        int aSizeZ = data.sizeZ;
        int bSizeX = b.flip ? b.data.sizeY : b.data.sizeX;
        int bSizeY = b.flip ? b.data.sizeX : b.data.sizeY; 
        int bSizeZ = b.data.sizeZ;
		//Debug.Log(string.Format("A:({0},{1},{2}) ({3},{4}) B:({5},{6},{7}) ({8},{9})", aSizeX, aSizeY, aSizeZ, x, y, bSizeX, bSizeY, bSizeZ, b.x, b.y));
        if ((x >= (b.x + bSizeX + bSizeZ)) || (b.x >= (x + aSizeX + aSizeZ))) {
            isOverlap = false;
        }
        if ((y >= (b.y + bSizeY + bSizeZ)) || (b.y >= (y + aSizeY + aSizeZ))) {
            isOverlap = false;
        }
        if ((x - y + aSizeX <= b.x - b.y - bSizeY) || (b.x - b.y + bSizeX <= x - y - aSizeY)) {
            isOverlap = false;
        }

        if (isOverlap) {
            if (x >= (b.x + bSizeX)) {
                return 2;
            } else if (b.x >= (x + aSizeX)) {
                return 1;
            }
            if (y >= (b.y + bSizeY)) {
                return 2;
            } else if (b.y >= (y + aSizeY)) {
                return 1;
            }
        }
        return 0;
    }

    bool CheckOverlap(WalkerScript walker) {
        int walkerX = (int)walker.transform.position.x * -1;
        int walkerY = (int)walker.transform.position.z * -1;
        return CheckOverlap(walkerX, walkerY);
    }

    public bool CheckOverlap(int walkerX, int walkerY) {
        int sizeX = flip ? data.sizeY : data.sizeX;
        int sizeY = flip ? data.sizeX : data.sizeY;
        int sizeZ = data.sizeZ;
        if (walkerX < x || walkerY < y || walkerX >= x + sizeX + sizeZ || walkerY >= y + sizeY + sizeZ || walkerX - walkerY < x - y - sizeY || walkerX - walkerY > x - y + sizeX) return false;
        return true;
    }

    //Toposort
    public static void SortSprites(List<GridObjectRendererScript> objects, List<WalkerScript> walkers) {
        int depth = objects.Count + walkers.Count;
		for(int i = 0; i < objects.Count; i++) {
			depth = VisitObject(objects[i], depth);
		}
        for (int i = 0; i < objects.Count; i++) {
            objects[i].visited = false;
        }
        for (int i = 0; i < walkers.Count; i++) {
            walkers[i].visited = false;
        }

    }

	public static int VisitObject(GridObjectRendererScript obj, int depth) {
		if (obj.visited) return depth;
		obj.visited = true;
		for(int i = 0; i < obj.objectsBehindMe.Count; i++) {
			depth = VisitObject(obj.objectsBehindMe[i], depth);
		}
        foreach(WalkerScript walker in obj.walkersBehindMe) {
            depth = VisitObject(walker, depth);
        }
		obj.SetSpriteDepth(depth / 10f);
		return depth - 1;
	}

    public static int VisitObject(WalkerScript walker, int depth) {
        if (walker.visited) return depth;
        walker.visited = true;
        foreach(GridObjectRendererScript renderer in walker.objectsBehindMe) {
            depth = VisitObject(renderer, depth);
        }
        walker.SetSpriteDepth(depth / 10f);
        return depth - 1;
    }



    public void SetSpriteDepth(float d) {
		Transform t = spriteObject.transform;
		t.position = new Vector3(t.position.x, t.position.y, d);
	}
}
