using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegionLevelRequirement : ScriptableObject
{
    public int population;
    public int food;
    public int goods;
    public int sanitation;
    public int religion;
    public int entertainment;
    public NeedData[] needs;
}
