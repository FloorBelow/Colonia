using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegionType : ScriptableObject
{
	public string regionName;
	public RegionScript.REGION_TYPE type;
	public Sprite icon;
	public ColorData iconColor;
	public ColorData outlineColor;
	public GameObject[] buildingUnlocks;
	public RegionType upgrade;
	public RegionLevelRequirement levelUpRequirement;
}
