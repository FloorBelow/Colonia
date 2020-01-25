using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoPanelScript : MonoBehaviour {

	public GameObject textBox;
	public GameObject needsPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	public void SetData(BuildingScript building) {
		string buildingInfo = string.Format("{0}\nX: {1} Y: {2}", building.buildingName, building.x, building.y);

		//Resource Storage
		ResourceStorageScript resourceStorageScript = building.gameObject.GetComponent<ResourceStorageScript>();
		if (resourceStorageScript != null) {
			if (resourceStorageScript.resources.Count == 0) {
				buildingInfo = "No resources in current building";
			} else {
				buildingInfo = building.buildingName + "\n";
				foreach (ResourceScript resource in resourceStorageScript.resources) {
					buildingInfo += resource.GetName() + ": " + resource.count + "\n";
				}

			}
		
		}

		//Needs
		BuildingHouseScript houseScript = building.gameObject.GetComponent<BuildingHouseScript>();
		if(houseScript != null) {
			//needsPanel.SetActive(true);
			//needsPanel.GetComponent<NeedsPanelScript>().SetPips(houseScript);
		} else {
			needsPanel.SetActive(false);
		}

		textBox.GetComponent<Text>().text = buildingInfo;
	}
}
