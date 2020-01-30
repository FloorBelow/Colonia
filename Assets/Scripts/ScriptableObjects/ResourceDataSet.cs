using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

[CreateAssetMenu]
public class ResourceDataSet : ScriptableObject
{
	[SerializeField, Reorderable(elementNameProperty = "name")]
	public ResourceList objects;

	[System.Serializable]
	public class ResourceList : ReorderableArray<ResourceData> { }

}
