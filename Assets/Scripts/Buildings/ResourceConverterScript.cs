using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceConverterScript : BuildingJobScript {

	public float resourceTime;
    public ResourceData input;
	public ResourceData output;
	ResourceStorageScript storageScript;

	public override void Init() {
		storageScript = gameObject.GetComponent<ResourceStorageScript>();
	}

	void Start () {
		StartCoroutine("ResourceTimer");
	}

    IEnumerator ResourceTimer() {
        while (storageScript.GetCount(input) < 1) yield return null;
		float timer = resourceTime;
		while(timer > 0) {
			if (isActive) timer -= Time.deltaTime;
			yield return null;
		}
        //yield return new WaitForSeconds(resourceTime);
        while (storageScript.GetCount(input) < 1) yield return null;
        ConvertResources(1);
        StartCoroutine("ResourceTimer");
    }

    public void ConvertResources(int count) {
        storageScript.RemoveResource(input, count);
		storageScript.AddResource(output, count);
	}

}