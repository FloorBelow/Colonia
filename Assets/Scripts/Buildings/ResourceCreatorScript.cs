using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCreatorScript : BuildingJobScript {

	public float resourceTime;
	public ResourceData resource;
	ResourceStorageScript storageScript;
	float timer;

	public override void Init() {
		storageScript = gameObject.GetComponent<ResourceStorageScript>();
		timer = resourceTime;
	}

	public float GetCompletionPercentage() { return (resourceTime - timer) / resourceTime; }

	private void Update() {
		if(isActive) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				AddResources(1);
				timer = resourceTime;
			}
		}
	}

    public void AddResources(int count) {
		storageScript.AddResource(resource, count);
	}


}
