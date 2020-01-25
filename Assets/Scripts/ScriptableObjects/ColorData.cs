using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ColorData : ScriptableObject
{
	public Color color;

	public static implicit operator Color(ColorData c) { return c.color; }
}
