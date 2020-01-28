using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public int stackSize;
    public Sprite[] sprites;
	public Mesh[] models;
    public Color color = Color.white; //probably remove this later once there's enough art to go around
	//also need to add 3d models later
	public Sprite icon;
	public Sprite iconShort;
	public enum Type { General, Food, Goods };
	public Type type;
}
