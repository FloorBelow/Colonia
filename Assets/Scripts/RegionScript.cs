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

	public int[] allowedBuildings;

	public HashSet<BuildingScript> buildings;
	public HashSet<BuildingHouseScript> houses;

	private void Start() {
		houses = new HashSet<BuildingHouseScript>();
		allowedBuildings = new int[UtilityScript.data.buildings.Length];
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
		int[] values = new int[6];
		bool satisfied = true;
		values[1] = GetHouseFood();
		values[2] = GetHouseGoods();
		values[3] = GetHouseNeed(UtilityScript.data.needSanitation, regionType.levelUpRequirement.sanitation);
		values[4] = GetHouseNeed(UtilityScript.data.needReligon, regionType.levelUpRequirement.religion);
		values[5] = GetHouseNeed(UtilityScript.data.needEntertainment, regionType.levelUpRequirement.entertainment);
		for(int i = 1; i < 6; i++) {
			if (values[i] != -1 && values[i] < regionType.levelUpRequirement.population) satisfied = false;
		}
		values[0] = satisfied ? 1 : 0;
		return values;
	}

	int GetHouseFood() {
		if (regionType.levelUpRequirement.food == 0) return -1;
		int i = 0;
		foreach(BuildingHouseScript house in houses) {
			if (house.food.resource != null) i += house.population;
		}
		return i;
	}

	int GetHouseGoods() {
		if (regionType.levelUpRequirement.goods == 0) return -1;
		int i = 0;
		foreach (BuildingHouseScript house in houses) {
			if (house.goods.resource != null) i += house.population;
		}
		return i;
	}

	int GetHouseNeed(NeedData n, int requirement) {
		if (requirement == 0) return -1;
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

		//Unlocks from region type
		RegionType region = regionType;
		while(region != null) {
			for (int i = 0; i < region.buildingUnlocks.Length; i++) {
				allowedBuildings[System.Array.IndexOf(UtilityScript.data.buildings, region.buildingUnlocks[i])] = 1;
			}
			region = region.parent;
		}

		//Unlocks from perks
		for(int i = 0; i < perks.Length; i++) {
			bool usePerk = false;
			if (perks[i].requiredTypes.Length == 0) usePerk = true; else {
				for(int j = 0; j < perks[i].requiredTypes.Length; j++) {
					if(regionType.type == perks[i].requiredTypes[j]) { usePerk = true; break; }
				}
			}
			if(usePerk) for(int j = 0; j < perks[i].buildingUnlocks.Length; j++) {
				allowedBuildings[System.Array.IndexOf(UtilityScript.data.buildings, perks[i].buildingUnlocks[j])] = 1;
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
