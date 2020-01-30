using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerScript : MonoBehaviour
{
    // Start is called before the first frame update

	//Note that walkers have to be child of 3dworld?

    public float speed;
    public MapScript map;
    public string jobDescription;
	public Sprite sprite;
	GameObject spriteObject;
	ResourceStorageScript storage;

    public Queue<Job> jobs;
    public Job currentJob;

    public void Init(MapScript map){
        this.map = map;
        jobs = new Queue<Job>();
		storage = gameObject.GetComponent<ResourceStorageScript>();
		storage.Init();
		spriteObject = new GameObject();
		SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
		spriteObject.transform.position = GetSpritePos(transform.localPosition);
		spriteRenderer.sprite = sprite;

		currentJob = new JobIdle(this); currentJob.OnBeginJob();

		//transform.position = new Vector3(transform.position.x, transform.position.y + .25f, -.1f);
	}

	public void SetActive(bool b) {
		spriteObject.SetActive(b); gameObject.GetComponentInChildren<MeshRenderer>().enabled = b;
	}

	private void OnDestroy() {
		Destroy(spriteObject);
	}

	float GetBuildingZ() {
		return map.GetTile((int)transform.localPosition.x * -1, (int)transform.localPosition.z * -1).building.GetComponent<GridObjectRendererScript>().spriteObject.transform.position.z;
	}

	float GetBuildingZ(Vector3 v) {
		return map.GetTile((int)v.x * -1, (int)v.z * -1).building.GetComponent<GridObjectRendererScript>().spriteObject.transform.position.z;
	}

	float GetBuildingZ(int i) {
		return map.GetTile(i % map.sizeX, i / map.sizeX).building.GetComponent<GridObjectRendererScript>().spriteObject.transform.position.z;
	}

	public void SetSpritePos(Vector3 v) {
		spriteObject.transform.position = v;
		if(storage.slots[0] != null) {
			storage.slots[0].GetComponent<ObjectRendererScript>().spriteObject.transform.position = new Vector3(v.x, v.y, v.z + 0.1f);
		}
		
	}

	//This is converting 3dpos to spritepos so negative x and z, switched y and z
	static Vector3 GetSpritePos(float x, float y, float z) {
		return new Vector3((x - z) * -.5f, (x + z) * -.25f + y * .6125f, (x + z) * -.05f - 0.5f);
	}

	static Vector3 GetSpritePos(Vector3 v) {
		return new Vector3((v.x - v.z) * -.5f, (v.x + v.z) * -.25f + v.y * .6125f, (v.x + v.z) * -.05f - 0.5f);
	}

	static Vector3 GetSpritePos(Vector3 v, float z) {
		return new Vector3((v.x - v.z) * -.5f, (v.x + v.z) * -.25f + v.y * .6125f, z - 0.1f);
	}

	public void AddJob(Job job){
        jobs.Enqueue(job);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentJob.GetType() == typeof(JobIdle) && jobs.Count != 0){
			SetActive(true);
            currentJob = jobs.Dequeue();
            currentJob.OnBeginJob();
        }
        if (currentJob.OnUpdate()) {
            if (jobs.Count == 0) currentJob = new JobIdle(this);
            else {
                currentJob = jobs.Dequeue();
                currentJob.OnBeginJob();
            }
        }
    }

    public abstract class Job {
        protected WalkerScript walker;
        public abstract void OnBeginJob();
        public abstract bool OnUpdate();
    }

    public class JobIdle : Job {

        public JobIdle(WalkerScript walker){
            this.walker = walker;
        }

        public override void OnBeginJob()
        {
            walker.jobDescription = "Idle";
			walker.SetActive(false);
        }
        public override bool OnUpdate()
        {
            return false;
        }
    }

    public class JobWalk : Job {
        int[] tiles;
        Vector3 nextPosition;
		float z; float nextZ;
		int i;
		int sizeX;
        //float oldZ;
        public JobWalk(WalkerScript walker, int[] tiles) {
            this.walker = walker; this.tiles = tiles; i = 0;
        }

        public override void OnBeginJob() {
			sizeX = walker.map.sizeX;
			z = walker.GetBuildingZ(tiles[0]);
			nextZ = walker.GetBuildingZ(tiles[0]);
			walker.transform.localPosition = new Vector3((tiles[0] % sizeX + tiles[1] % sizeX) / -2f -.5f, 0, (tiles[0] / sizeX + tiles[1] / sizeX) / -2f -.5f);
			//walker.spriteObject.transform.position = GetSpritePos(walker.transform.localPosition, Mathf.Min(z, nextZ));
			//Debug.Log("Start pos = " + walker.transform.localPosition.ToString());

            //oldZ = walker.transform.position.z;
            nextPosition = GetNextPosition();
            walker.jobDescription = "Moving";
        }

        public override bool OnUpdate()
        {
            Vector3 newPos = Vector3.MoveTowards(walker.transform.localPosition, nextPosition, walker.speed * Time.deltaTime);
            walker.transform.localPosition = newPos;
			walker.SetSpritePos(GetSpritePos(walker.transform.localPosition, Mathf.Min(z, nextZ)));
			//walker.spriteObject.transform.position = GetSpritePos(walker.transform.localPosition, Mathf.Min(z, nextZ));
			if (newPos == nextPosition) {
				if (i + 1 == tiles.Length) {
					return true;
				}
                nextPosition = GetNextPosition();
            }
            return false;
        }

		Vector3 GetNextPosition() {
			i++;
			z = nextZ;
			if (i == tiles.Length - 1) {
				nextZ = walker.GetBuildingZ(tiles[i - 1]);
				return new Vector3((tiles[i - 1] % sizeX + tiles[i] % sizeX) / -2f - .5f, 0, (tiles[i - 1] / sizeX + tiles[i] / sizeX) / -2f - .5f);
			}
			nextZ = walker.GetBuildingZ(tiles[i]);
			return new Vector3(tiles[i] % sizeX * -1 - .5f, 0, tiles[i] / sizeX * -1 - .5f);
		}

        Vector3 GetPosition() {
            int x = tiles[i] % sizeX;
            int y = tiles[i] / sizeX;
			return new Vector3(x * -1 -.5f, 0, y * -1 -.5f);
            //Depth
            //float newZ = walker.region.tiles[x, y].building.GetComponent<GridObjectRendererScript>().spriteObject.transform.position.z - 0.1f;
            //walker.transform.position = new Vector3(walker.transform.position.x, walker.transform.position.y, Mathf.Min(oldZ, newZ));
            //oldZ = newZ;
            //return new Vector3((x - y) * 0.5f, (x + y) * .25f + .25f, walker.transform.position.z);
        }
    }

	/*   public class JobWalk : Job {
        int[] tiles;
        Vector3 nextPosition;
        int i;
        float oldZ;
        public JobWalk(WalkerScript walker, int[] tiles) {
            this.walker = walker; this.tiles = tiles; i = 0;
        }

        public override void OnBeginJob() {
            oldZ = walker.transform.position.z;
            nextPosition = GetPosition();
            walker.jobDescription = "Moving";
        }

        public override bool OnUpdate()
        {
            Vector3 newPos = Vector3.MoveTowards(walker.transform.position, nextPosition, walker.speed * Time.deltaTime);
            walker.transform.position = newPos;
            if(newPos == nextPosition) {
                if(i + 1 == tiles.Length) {
                    return true;
                }
                i++;
                nextPosition = GetPosition();
            }
            return false;
        }

        Vector3 GetPosition() {
            int x = tiles[i] % walker.region.sizeX;
            int y = tiles[i] / walker.region.sizeX;
            //Depth
            float newZ = walker.region.tiles[x, y].building.GetComponent<GridObjectRendererScript>().spriteObject.transform.position.z - 0.1f;
            walker.transform.position = new Vector3(walker.transform.position.x, walker.transform.position.y, Mathf.Min(oldZ, newZ));
            oldZ = newZ;
            return new Vector3((x - y) * 0.5f, (x + y) * .25f + .25f, walker.transform.position.z);
        }
    }
	*/

    public class JobLoadResource : Job {
        ResourceStorageScript storage;
        ResourceStorageScript otherStorage;
        ResourceData resource;
        float timer;
        int count;
        public JobLoadResource(WalkerScript walker, ResourceStorageScript otherStorage, ResourceData resource, int count, float time){
            this.walker = walker; this.otherStorage = otherStorage; this.resource = resource; this.count = count; timer = time;
        }

        public override void OnBeginJob() {
            walker.jobDescription = "Loading resource";
            storage = walker.gameObject.GetComponent<ResourceStorageScript>();
            otherStorage.TransferResource(resource, count, storage);
            //Debug.Log("Took " + count.ToString() + " " + name + " from destination.");
        }

        public override bool OnUpdate()
        {
			return true;
			/*
            timer -= Time.deltaTime;
            if (timer < 0)
            {  
                return true;
            }
            return false;
			*/
        }
    }

    public class JobUnloadResource : Job
    {
        ResourceStorageScript storage;
        ResourceStorageScript otherStorage;
        ResourceData resource;
        float timer;
        int count;
        public JobUnloadResource(WalkerScript walker, ResourceStorageScript otherStorage, ResourceData resource, int count, float time)
        {
            this.walker = walker; this.otherStorage = otherStorage; this.resource = resource; this.count = count; timer = time;
        }

        public override void OnBeginJob()
        {
            walker.jobDescription = "Unloading resource";
            storage = walker.gameObject.GetComponent<ResourceStorageScript>();
            storage.TransferResource(resource, count, otherStorage);
            //Debug.Log("Placed " + count.ToString() + " " + name + " in destination.");
        }

        public override bool OnUpdate()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                return true;
            }
            return false;
        }
    }

	public class JobAddPopulation : Job {
		BuildingHouseScript house;
		public JobAddPopulation(WalkerScript walker, BuildingHouseScript house) {
			this.walker = walker; this.house = house;
		}

		public override void OnBeginJob() {
			house.AddPopulation(1);
			house.occupantsArriving = false;
			Destroy(walker.gameObject);
		}
		public override bool OnUpdate() {
			return false;
		}
	}
}
