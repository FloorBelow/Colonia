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
		foreach(GameObject building in area.GetBuildingsInRange()) {
			if (building.HasComponent<BuildingHouseScript>()) building.GetComponent<BuildingHouseScript>().AddNeed(this);
		}
	}

	private void OnDestroy() {
		foreach (BuildingHouseScript house in building.map.houses) {
			house.RemoveNeed(this);
		}
	}
}
