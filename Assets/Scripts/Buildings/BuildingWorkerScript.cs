using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWorkerScript : MonoBehaviour
{
	public int workersRequired;
	public int workers;

	public int AddWorkers(int i) {
		if(i >= workersRequired) {
			workers = workersRequired;
			i -= workers;
			foreach (BuildingJobScript job in gameObject.GetComponents<BuildingJobScript>()) {
				job.isActive = true;
			}
			return i;
		} else {
			workers = 0;
			foreach (BuildingJobScript job in gameObject.GetComponents<BuildingJobScript>()) {
				job.isActive = false;
			}
		}
		return i;
	}
}
