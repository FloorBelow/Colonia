using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScript : InitScript {

    //Consts
    public ResourceData data;
    //SpriteRenderer spriteRenderer;
	ObjectRendererScript objectRenderer;

	//Variables
	public int count;

	void Start() {
		
	}

    public string GetName() {
        return data.resourceName;
    }

	public override void Init() {
		//spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        //spriteRenderer.sprite = sprites[0];
        //spriteRenderer.color = data.color;
	}

    public void Init(ResourceData data, Vector3 pos) {
        this.data = data;
		gameObject.name = data.resourceName;

		//Renderer shit
		objectRenderer = gameObject.AddComponent<ObjectRendererScript>();
		objectRenderer.SetData(data.sprites, data.models);
		objectRenderer.CreateRenderers(new GameObject(data.resourceName + " Sprite"), gameObject, transform.parent.GetComponent<GridObjectRendererScript>());
		//This is okay as long as we only ever create resources as a child of buildings
		//So like, it isn't okay
		objectRenderer.SetSpriteColor(data.color);
		objectRenderer.SetPosition(pos.x, pos.y, pos.z);

		Init();
    }

    public void AddCount(int countToAdd) {
		SetCount(count + countToAdd);
	}

    public void RemoveCount(int countToRemove)
    {
        SetCount(count - countToRemove);
    }

    public void SetCount(int aCount) {
		count = aCount;
        if(count != 0)
		UpdateSprite();
	}

	public void UpdateSprite() {
		//gameObject.GetComponent<SpriteRenderer>().sprite = sprites[Mathf.FloorToInt(count / stackSize * sprites.Length)];
		objectRenderer.SetIndexNormalised((float)(count - 1) / data.stackSize);
		//spriteRenderer.sprite = sprites[Mathf.FloorToInt((count - 1) * sprites.Length / data.stackSize) % sprites.Length];
	}

}