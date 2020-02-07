using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHouseScript : InitScript {
    [Header("Base Data")]
    public int capacity;
	public float foodConsumptionTime;
	public float goodsConsumptionTime;
	[Header("Instance Variables")]
	public int happiness;
	public float happinessUpdateTime;
	float happinessTimer;
	public bool needsMet;

	public int population;
	public NeedData[] needData;
	public Dictionary<NeedData, BuildingNeedProviderScript[]> needs;
    public bool occupantsArriving;
	ResourceStorageScript storage;
	public ResourceUse food;
	public ResourceUse goods;

	public struct ResourceUse {
		public ResourceData resource;
		public float timer;
	}


	public override void Init() {
		storage = gameObject.GetComponent<ResourceStorageScript>();
		needs = new Dictionary<NeedData, BuildingNeedProviderScript[]>();
		foreach (NeedData n in needData) needs[n] = new BuildingNeedProviderScript[8];
		happiness = 80;
		occupantsArriving = false;
		food = new ResourceUse();
		goods = new ResourceUse();
	}
	private void Update() {
		happinessTimer -= Time.deltaTime;
		if(happinessTimer <= 0) {
			needsMet = UpdateHappiness();
			happinessTimer = happinessUpdateTime;
		}

		if(food.timer <= 0) {
			food.resource = null;
			foreach(ResourceData resource in storage.totals.Keys) {
				if(resource.type == ResourceData.Type.Food) {
					if(storage.totals[resource] > 0) {
						storage.RemoveResource(resource, 1);
						food.resource = resource;
						food.timer = foodConsumptionTime;
						break;
					}
				}
			}
		}
		if (goods.timer <= 0) {
			goods.resource = null;
			foreach (ResourceData resource in storage.totals.Keys) {
				if (resource.type == ResourceData.Type.Goods) {
					if (storage.totals[resource] > 0) {
						storage.RemoveResource(resource, 1);
						goods.resource = resource;
						goods.timer = goodsConsumptionTime;
						break;
					}
				}
			}
		}
		food.timer -= Time.deltaTime;
		goods.timer -= Time.deltaTime;
	}

	bool UpdateHappiness() {
		bool needsMet = (food.resource != null);
		foreach(NeedData need in needData) {
			if (GetNeedCount(need) <= 0) needsMet = false;
		}
		if (needsMet) happiness = System.Math.Min(100, happiness + 1); else happiness = System.Math.Max(0, happiness - 1);
		return needsMet;
	}

	public void AddPopulation(int count) {
        if (population + count > capacity) {
            population = capacity;
            Debug.LogError("Adding more population than fits to house");
        }
        population += count;
    }

	public int GetNeedCount(NeedData n) {
		int v = 0;
		for (int i = 0; i < 8; i++) {
			if (needs[n][i] != null) v++;
		}
		return v;
	}

	public void AddNeed(BuildingNeedProviderScript n) {
		for (int i = 0; i < 8; i++) {
			if (needs[n.need][i] == n) return;
		}
		for (int i = 0; i < 8; i++) {
			if (needs[n.need][i] == null) needs[n.need][i] = n;
		}
	}

	public void RemoveNeed(BuildingNeedProviderScript n) {
		for (int i = 0; i < 8; i++) {
			if (needs[n.need][i] == n) needs[n.need][i] = null;
		}
	}

	public int GetVacancy() {
		if (occupantsArriving) return 0;
		return capacity - population;
	}


}
