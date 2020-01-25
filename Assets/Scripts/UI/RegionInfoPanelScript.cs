using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoPanelScript : MonoBehaviour {
	public GameObject needsPanel;
	public GameObject populationCount;
	public GameObject districtName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetData(MapScript region) {

		//needsPanel.GetComponent<NeedsPanelScript>().SetPips((DistrictResidentialScript) region.district);
		populationCount.GetComponent<Text>().text =  region.population.ToString();
		districtName.GetComponent<Text>().text = region.districtName;
	}
}
