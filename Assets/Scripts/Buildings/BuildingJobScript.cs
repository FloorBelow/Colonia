using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingJobScript : InitScript
{
	public bool isActive = false;
	public virtual void SetActive() {
		isActive = true;
	}
}
