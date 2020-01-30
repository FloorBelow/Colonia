using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMarketplaceScript : BuildingJobScript
{

	public float timer;
	public ResourceStorageScript storage;
	public ResourceData[] resourcesToDistribute;
	BuildingScript building;
    // Start is called before the first frame update

	public override void Init() {
		storage = gameObject.GetComponent<ResourceStorageScript>();
		building = gameObject.GetComponent<BuildingScript>();
		timer = 1;
	}

	// Update is called once per frame
	void Update()
    {
		if(isActive) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				foreach (ResourceData resource in resourcesToDistribute) Process(resource);
				timer = 1;
			}
		}
    }

	void Process(ResourceData resource) {
		//Debug.Log(resourceName + ": " + storage.GetCount(resourceName).ToString());
		if (storage.GetCount(resource) > 0) {
			int min = System.Int16.MaxValue;
			foreach(BuildingHouseScript house in building.map.houses) {
				int i = house.gameObject.GetComponent<ResourceStorageScript>().GetCount(resource);
				if (i < min) min = i;
			}
			foreach (BuildingHouseScript house in building.map.houses) {
				if (house.gameObject.GetComponent<ResourceStorageScript>().GetCount(resource) == min) {
					//Debug.Log("Distributing Grain");
					storage.TransferResource(resource, 1, house.gameObject.GetComponent<ResourceStorageScript>());
					break;
				}
			}
		}
	}
}
