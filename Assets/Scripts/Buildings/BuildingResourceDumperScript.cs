using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingResourceDumperScript : BuildingJobScript, PeriodicUpdate {
	//[Header("Base Data")]
	public string resourceToDump;
	ResourceData resource;
	public int minToDump;
	public GameObject walkerPrefab;

	[SerializeField]
	Keyword[] dumpKeywords;

	//[Header("Instance Variables")]
	WalkerScript dumper;
	BuildingScript buildingScript;
	ResourceStorageScript storage;

	void Start() {
	}

	private void OnDestroy() {
		Destroy(dumper.gameObject);
	}

	bool PeriodicUpdate.IsPeriodicUpdateEnabled() {
		return true;
	}

	void PeriodicUpdate.PeriodicUpdate() {
		 if(isActive && storage.GetCount(resource) >= minToDump && dumper.currentJob.GetType() == typeof(WalkerScript.JobIdle)) {
			List<int[]> paths = new List<int[]>();
			List<ResourceStorageScript> otherStores = new List<ResourceStorageScript>();
			foreach (GameObject building in buildingScript.map.buildings) {
				ResourceStorageScript otherStorage = building.GetComponent<ResourceStorageScript>();
				if (otherStorage != null && otherStorage.gameObject != gameObject && building.HasKeyword(dumpKeywords) && otherStorage.GetSpaceFor(resource) >= minToDump) {
					BuildingScript otherBuildingScript = building.GetComponent<BuildingScript>();
					int[] path = PathFindScript.Pathfind(buildingScript.map, buildingScript.x, buildingScript.y, otherBuildingScript.x, otherBuildingScript.y, true);
					if (path != null) {
						paths.Add(path);
						otherStores.Add(otherStorage);
					}
				}
			}
			if (paths.Count != 0) {
				int minIndex = 0;
				int minLength = System.Int32.MaxValue;
				for (int i = 0; i < paths.Count; i++) {
					if (paths[i].Length < minLength) { minIndex = i; minLength = paths[i].Length; }
				}

				dumper.AddJob(new WalkerScript.JobLoadResource(dumper, storage, resourceToDump, minToDump, 1f));
				dumper.AddJob(new WalkerScript.JobWalk(dumper, paths[minIndex]));
				dumper.AddJob(new WalkerScript.JobUnloadResource(dumper, otherStores[minIndex], resourceToDump, minToDump, 1f));
				int[] backPath = new int[paths[minIndex].Length]; System.Array.Copy(paths[minIndex], backPath, paths[minIndex].Length); System.Array.Reverse(backPath);
				dumper.AddJob(new WalkerScript.JobWalk(dumper, backPath));
				dumper.AddJob(new WalkerScript.JobIdle(dumper));
			}
		}
	}


	public override void Init() {
		buildingScript = gameObject.GetComponent<BuildingScript>();
		storage = gameObject.GetComponent<ResourceStorageScript>();
		resource = GameManagerScript.m.resources[resourceToDump];
		//dumper = Instantiate(walkerPrefab, GetComponent<GridObjectRendererScript>().spriteObject.transform).GetComponent<WalkerScript>();
		dumper = Instantiate(walkerPrefab, GameManagerScript.m.threedeeworld).GetComponent<WalkerScript>();
		dumper.Init(buildingScript.map);
		buildingScript.map.updateQueue.Enqueue(this);
	}

    // Update is called once per frame
}
