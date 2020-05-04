using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingNeedProviderScript : InitScript, PeriodicUpdate
{
	public NeedData need;
	BuildingScript building;
	BuildingAreaDisplayScript area;

	bool PeriodicUpdate.IsPeriodicUpdateEnabled() {
		return building.IsActive();
	}
	void PeriodicUpdate.PeriodicUpdate() {
		AddNeeds();
	}

	public override void Init() {
		building = gameObject.GetComponent<BuildingScript>();
		area = gameObject.GetComponent<BuildingAreaDisplayScript>();
		building.map.updateQueue.Enqueue(this);
	}

	public void AddNeeds() {
		HashSet<GameObject> buildings = area.GetBuildingsInRange(); //TODO should just make a IsInRange method and apply on all houses in map
		for (int i = 0; i < building.map.houses.Count; i++) {
			if (buildings.Contains(building.map.houses[i].gameObject)) building.map.houses[i].AddNeed(this);
		}
		/*
		foreach(GameObject building in area.GetBuildingsInRange()) {
			BuildingHouseScript house = building.GetComponent<BuildingHouseScript>(); if (house != null) house.AddNeed(this);
		}
		*/
	}

	private void OnDestroy() {
		foreach (BuildingHouseScript house in building.map.houses) {
			house.RemoveNeed(this);
		}
	}
}
