using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegionPerk : ScriptableObject
{
	public string perkName;
	public Sprite icon;
	public ColorData iconColor;
	public GameObject[] buildingUnlocks;
	public RegionScript.REGION_TYPE[] requiredTypes;
}
