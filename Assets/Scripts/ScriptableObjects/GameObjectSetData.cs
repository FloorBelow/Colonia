using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

[CreateAssetMenu]
public class GameObjectSetData : ScriptableObject
{
	[SerializeField, Reorderable(elementNameProperty = "name")]
	public ObjectList objects;

	[System.Serializable]
	public class ObjectList : ReorderableArray<GameObject> { }

}
