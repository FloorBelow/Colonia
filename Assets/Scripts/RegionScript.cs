using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionScript : MonoBehaviour
{

	public enum REGION_TYPE {
		Wilderness = 0,
		Rural = 1,
		Urban = 2,
		Forum = 3
	}

	public MapScript map;

	public RegionType regionType;
	public RegionPerk[] perks;
	public Color color;
	public int[,] tiles;

	GameObjectSetData.ObjectList buildingsSet;
	public int[] allowedBuildings;

	public HashSet<BuildingScript> buildings;
	public HashSet<BuildingHouseScript> houses;

	private void Start() {
		houses = new HashSet<BuildingHouseScript>();
		buildingsSet = GameManagerScript.m.buildingSet.objects;
		allowedBuildings = new int[buildingsSet.Length];
	}

	public void AddBuilding(BuildingScript building) {
		if (buildings == null) buildings = new HashSet<BuildingScript>();
		buildings.Add(building);
		if(regionType.type == REGION_TYPE.Urban) {
			BuildingHouseScript house = building.GetComponent<BuildingHouseScript>();
			if (house != null) houses.Add(house);
		}
	}

	public void RemoveBuilding(BuildingScript building) {
		buildings.Remove(building);
		if (regionType.type == REGION_TYPE.Urban) {
			BuildingHouseScript house = building.GetComponent<BuildingHouseScript>();
			if (house != null) houses.Remove(house);
		}
	}

	public int GetPopulation() {
		int p = 0;
		foreach(BuildingHouseScript h in houses) {
			p += h.population;
		}
		return p;
	}

	public int[] GetLevelUp() {
		int[] values = new int[8];
		int index = 1;
		bool satisfied = true;
		if (regionType.levelUpRequirement.food > 0) {
			values[index] = GetHouseFood();
			satisfied = values[index] >= regionType.levelUpRequirement.population;
			index++;
		}
		for(int i = 0; i < regionType.levelUpRequirement.needs.Length; i++) {
			values[index] = GetHouseNeed(regionType.levelUpRequirement.needs[i]);
			if(satisfied) satisfied = values[index] >= regionType.levelUpRequirement.population;
			index++;
		}
		values[0] = satisfied ? 1 : 0;
		System.Array.Resize(ref values, index);
		return values;
	}

	int GetHouseFood() {
		int i = 0;
		foreach(BuildingHouseScript house in houses) {
			if (house.food.resource != null) i += house.population;
		}
		return i;
	}
	int GetHouseNeed(NeedData n) {
		int i = 0;
		foreach (BuildingHouseScript house in houses) {
			if (house.GetNeedCount(n) > 0) i += house.population;
		}
		return i;
	}

	public void Upgrade() {
		if (regionType.upgrade == null) {
			Debug.Log("Maximum level reached!");
			return;
		}
		regionType = regionType.upgrade;
		UpdateAllowedBuildings();
	}

	public void UpdateAllowedBuildings() {
		//Reset List
		for (int i = 0; i < allowedBuildings.Length; i++) allowedBuildings[i] = 0;
		for(int i = 0; i < regionType.buildingUnlocks.Length; i++) {
			allowedBuildings[buildingsSet.IndexOf(regionType.buildingUnlocks[i])] = 1;
		}	


		for(int i = 0; i < perks.Length; i++) {
			bool usePerk = false;
			if (perks[i].requiredTypes.Length == 0) usePerk = true; else {
				for(int j = 0; j < perks[i].requiredTypes.Length; j++) {
					if(regionType.type == perks[i].requiredTypes[j]) { usePerk = true; break; }
				}
			}
			if(usePerk) for(int j = 0; j < perks[i].buildingUnlocks.Length; j++) {
				allowedBuildings[buildingsSet.IndexOf(perks[i].buildingUnlocks[j])] = 1;
			}
		}
	}

	private void OnDrawGizmosSelected() {
		Vector3[] positions = map.GetRegionOutlinePositions(this, 0);
		for(int i = 0; i < positions.Length - 1; i++) {
			Gizmos.DrawLine(positions[i], positions[i + 1]);
		}
	}
}
