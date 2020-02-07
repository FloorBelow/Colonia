using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maybe for child objects. Use for resources
public class ObjectRendererScript : MonoBehaviour
{

	//Honestly it wouldn't be hard to change this to all objects just by using floats
	//What do we do about animated sprites, tho?
	//public GridObjectRendererData data;
	//public int x; public int y; public int z;
	GridObjectRendererScript parentRenderer; //We need this for sorting order on z since we're not sorting stuff not on a grid. Maybe we should tho
	//Btw we should make sure this is initialized first, obv.
	float x; float y; float z;
	public Sprite[] sprites;
	public Mesh[] models;
	public GameObject spriteObject; //Public for sorting sprites, should be moved here? static method on this[]
	protected GameObject modelObject;
	//public bool flip;
	//public List<GridObjectRendererScript> objectsBehindMe;

	public void CreateRenderers(GameObject spriteObject, GameObject modelObject, GridObjectRendererScript parentRenderer = null) {
		this.parentRenderer = parentRenderer;
		//2d
		this.spriteObject = spriteObject; this.modelObject = modelObject;
		SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprites[0];
		if(parentRenderer != null) spriteObject.transform.SetParent(parentRenderer.spriteObject.transform, false);
		//3d
		modelObject.layer = 8;
		if (parentRenderer != null) modelObject.transform.SetParent(parentRenderer.modelObject.transform, false);
		MeshFilter meshFilter = modelObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = models[0];
		MeshRenderer meshRenderer = modelObject.AddComponent<MeshRenderer>();
		meshRenderer.material = UtilityScript.data.modelMat;
	}
	public void SetData(Sprite[] sprites, Mesh[] models) {
		if (sprites.Length != models.Length) Debug.LogError("Sprites and models not same size!");
		this.sprites = sprites; this.models = models;
	}

	//0-1 indexed?
	public void SetIndexNormalised(float f) {
		int i = Mathf.FloorToInt(sprites.Length * f);
		//Debug.Log(System.String.Format("f: {0}, i: {1}, sprites: {2}", f, i, sprites.Length));
		spriteObject.GetComponent<SpriteRenderer>().sprite = sprites[i];
		modelObject.GetComponent<MeshFilter>().sharedMesh = models[i];
	}

	public void SetSpriteColor(Color color) {
		spriteObject.GetComponent<SpriteRenderer>().color = color;
	}

	private void OnDestroy() {
		if (spriteObject != gameObject) Destroy(spriteObject);
		if (modelObject != gameObject) Destroy(modelObject);
	}

	public virtual void SetPosition(float x, float y, float z = 0) {
		this.x = x; this.y = y; this.z = z;
		spriteObject.transform.localPosition = new Vector3((x - z) * -.5f, (x + z) * -.25f + y * .6125f,  (x + z) *-.01f - 0.08f);
		modelObject.transform.localPosition = new Vector3(x, y, z);
	}

	/*
	public void Flip() {
		if (flip) {
			spriteObject.GetComponent<SpriteRenderer>().sprite = data.spriteNorm;
			modelObject.transform.localRotation = Quaternion.identity;
			modelObject.transform.localPosition += new Vector3(data.sizeY, 0, 0);

		} else {
			spriteObject.GetComponent<SpriteRenderer>().sprite = data.spriteFlip;
			modelObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
			modelObject.transform.localPosition += new Vector3(data.sizeY * -1, 0, 0);
		}
		flip = !flip;
	}
	

	public void CheckOverlaps(GridObjectRendererScript[] objects) {
		if (objectsBehindMe == null) objectsBehindMe = new List<GridObjectRendererScript>();
		for (int i = 0; i < objects.Length - 1; i++) {
			if (objects[i] != this) {
				int overlap = CheckOverlap(objects[i]);
				if (overlap == 1) {
					//Debug.Log("New building is in front of building " + (i + 1));
					objectsBehindMe.Add(objects[i]);
				} else if (overlap == 2) {
					//Debug.Log("New building is behind building " + (i + 1));
					objects[i].objectsBehindMe.Add(this);
				} else {
					//Debug.Log("New building does not overlap with building " + (i + 1));
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

	*/
}
