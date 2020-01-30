using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingResourceFetcherScript : BuildingJobScript
{
    //[Header("Base Data")]
	public ResourceData resourceToFetch;
    public int minToFetch;
    public GameObject fetcherPrefab;
	[SerializeField]
	Keyword[] fetchKeywords;
    [Header("Instance Variables")]
    public WalkerScript fetcher;

    //public State state;
    BuildingScript buildingScript;
    ResourceStorageScript storage;
    BuildingScript otherBuildingScript;
    ResourceStorageScript otherStorage;

    void Start()
    {
        //state = State.Searching;
    }

	private void OnDestroy() {
		Destroy(fetcher.gameObject);
	}

	public override void Init() {
		buildingScript = gameObject.GetComponent<BuildingScript>();
		storage = gameObject.GetComponent<ResourceStorageScript>();
		//fetcher = Instantiate(fetcherPrefab, GetComponent<GridObjectRendererScript>().spriteObject.transform).GetComponent<WalkerScript>();
		fetcher = Instantiate(fetcherPrefab, GameManagerScript.m.modelPivot).GetComponent<WalkerScript>();
		fetcher.Init(buildingScript.map);
		//fetcher.gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if (fetcher.currentJob.GetType() == typeof(WalkerScript.JobIdle)) {
			if(isActive) {
				List<int[]> paths = new List<int[]>();
				List<ResourceStorageScript> otherStores = new List<ResourceStorageScript>();
				foreach (ResourceStorageScript otherStorage in buildingScript.map.stores) {
					if (otherStorage != null && otherStorage.gameObject != gameObject && otherStorage.gameObject.HasKeyword(fetchKeywords) && storage.GetSpaceFor(resourceToFetch) >= minToFetch) {
						if (otherStorage.GetCount(resourceToFetch) >= minToFetch) {
							otherBuildingScript = otherStorage.GetComponent<BuildingScript>();
							int[] path = PathFindScript.Pathfind(buildingScript.map, buildingScript.x, buildingScript.y, otherBuildingScript.x, otherBuildingScript.y, true);
							if (path != null) {
								paths.Add(path);
								otherStores.Add(otherStorage);
							}
						}
					}
				}
				if(paths.Count != 0) {
					int minIndex = 0;
					int minLength = System.Int32.MaxValue;
					for (int i = 0; i < paths.Count; i++) {
						if (paths[i].Length < minLength) { minIndex = i; minLength = paths[i].Length; }
					}

					fetcher.AddJob(new WalkerScript.JobWalk(fetcher, paths[minIndex]));
					fetcher.AddJob(new WalkerScript.JobLoadResource(fetcher, otherStores[minIndex], resourceToFetch, minToFetch, 1f));
					int[] backPath = new int[paths[minIndex].Length]; System.Array.Copy(paths[minIndex], backPath, paths[minIndex].Length); System.Array.Reverse(backPath);
					fetcher.AddJob(new WalkerScript.JobWalk(fetcher, backPath));
					fetcher.AddJob(new WalkerScript.JobUnloadResource(fetcher, storage, resourceToFetch, minToFetch, 1f));
				}

			} else {
				//if (fetcher.gameObject.activeSelf) fetcher.gameObject.SetActive(false);
			}
        }
    }
}
/*
if (!buildingScript.active) return;
switch(state) {
    case State.Searching:
        foreach(GameObject building in buildingScript.region.buildings) {
            otherBuildingScript = building.GetComponent<BuildingScript>();
            otherStorage = building.GetComponent<BuildingResourceStorageScript>();
            if (otherStorage != null && otherStorage.gameObject != gameObject) {
                foreach (ResourceScript resource in otherStorage.resources) {
                    if(resource.resourceName == resourceToFetch && resource.count >= minToFetch) {
                        path = PathFindScript.Pathfind(buildingScript.region, buildingScript.x, buildingScript.y, otherBuildingScript.x, otherBuildingScript.y);
                        if(path != null) {
                            fetcher = Instantiate(fetcherPrefab, transform).GetComponent<WalkerScript>();
                            fetcher.Init(buildingScript.region);
                            fetcher.AddJob(new WalkerScript.JobWalk(fetcher, path));
                            fetcher.AddJob(new WalkerScript.JobLoadResource(fetcher, otherStorage, resourceToFetch, minToFetch, 1f));
                            int[] pathBack = new int[path.Length]; System.Array.Copy(path, pathBack, path.Length); System.Array.Reverse(pathBack);
                            fetcher.AddJob(new WalkerScript.JobWalk(fetcher, pathBack));
                            fetcher.AddJob(new WalkerScript.JobUnloadResource(fetcher, otherStorage, resourceToFetch, minToFetch, 1f));
                            pathfindObject = PathFindScript.DrawPath(buildingScript.region, gameObject, path);
                            timer = (float)path.Length / speed;
                            state = State.Fetching;
                            goto End; //Multi-track breaking
                        }
                    }
                }
            }
        }
        break;
    case State.Fetching:
        timer -= Time.deltaTime;
        if(timer < 0) {
            timer = 1;
            Destroy(pathfindObject);
            state = State.Loading;
            //Debug.Log("Loading...");
        }
        break;
    case State.Loading:
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            stackInTransit = otherStorage.PopResource(resourceToFetch, minToFetch, gameObject);
            Debug.Log("Took " + minToFetch.ToString() + " " + resourceToFetch + " from destination.");
            path = PathFindScript.Pathfind(buildingScript.region, otherBuildingScript.x, otherBuildingScript.y, buildingScript.x, buildingScript.y);
            if (path != null)
            {
                pathfindObject = PathFindScript.DrawPath(buildingScript.region, gameObject, path);
                timer = (float)path.Length / speed;
                state = State.Returning;
            }
        }
        break;
    case State.Returning:
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 1;
            Destroy(pathfindObject);
            state = State.Unloading;
            //Debug.Log("Unloading...");
        }
        break;
    case State.Unloading:
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            //Debug.Log("Deposited " + stackInTransit.GetComponent<ResourceScript>().count.ToString() + " " + resourceToFetch + ".");
            storage.addResource(stackInTransit, stackInTransit.GetComponent<ResourceScript>().count);
            Destroy(stackInTransit);
            otherBuildingScript = null;
            otherStorage = null;
            path = null;
            state = State.Searching;
        }
        break;

}
*/

