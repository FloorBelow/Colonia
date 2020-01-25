using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCreatorScript : BuildingJobScript {

	public float resourceTime;
	public string resourceName;
	public ResourceData resource;
	ResourceStorageScript storageScript;
	float timer;

	public override void Init() {
		if (resource == null) resource = GetResource();
		storageScript = gameObject.GetComponent<ResourceStorageScript>();
		timer = resourceTime;
	}

	public ResourceData GetResource() { return GameManagerScript.m.resources[resourceName]; }

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

	IEnumerator ResourceTimer() {
        yield return new WaitForSeconds(resourceTime);

        StartCoroutine("ResourceTimer");
    }

    public void AddResources(int count) {
		storageScript.AddResource(resource, count);
	}


}
