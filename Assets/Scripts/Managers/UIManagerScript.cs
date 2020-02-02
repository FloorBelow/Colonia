using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManagerScript : MonoBehaviour {
	public GameObject buildingInfoPanel;
	public GameObject regionPanel;

	//tooltip
	public GameObject buildingButtonPanel;
	public GameObject buildingButtonTooltip;
	public GameObject buildingTooltipResourcePrefab;
	public List<GameObject> buildingButtonTooltipChildren;

	public void SetBuildingButtonTooltip(BuildingScript building, float posY) {
		foreach(GameObject o in buildingButtonTooltipChildren) DestroyImmediate(o); buildingButtonTooltipChildren.Clear();
		RectTransform rect = buildingButtonTooltip.GetComponent<RectTransform>();
		rect.position = new Vector3(rect.position.x, posY, rect.transform.position.z);
		for(int i = 0; i < building.resources.Length; i++) {
			GameObject counter = Instantiate(buildingTooltipResourcePrefab, buildingButtonTooltip.transform);
			buildingButtonTooltipChildren.Add(counter);
			Image image = counter.GetComponentInChildren<Image>();
			image.sprite = building.resources[i].iconShort;
			image.color = building.resources[i].color;
			counter.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = building.resourceCounts[i].ToString();
		}
	}

	public void SetBuildingButtonTooltipActive(bool b) { buildingButtonTooltip.SetActive(b); }

	public void SetBuildingButtonPanelActive(bool b) { buildingButtonPanel.SetActive(b); }


	public void SetRegionPanel(RegionScript region) {
		regionPanel.SetActive(true);
		regionPanel.GetComponent<RegionPanelScript>().SetData(region);
		SetBuildingButtonPanelActive(true);
	}

	public void UpdateRegionPanel() { regionPanel.GetComponent<RegionPanelScript>().UpdatePanel(); }

	public void HideRegionPanel() { regionPanel.SetActive(false); }

	public void SetInfoBox(GameObject selectedObject) {
		if (selectedObject == null) {
			buildingInfoPanel.SetActive(false);
		} else {
			BuildingScript buildingScript = selectedObject.GetComponent<BuildingScript>();
			if (buildingScript != null) {
				buildingInfoPanel.GetComponent<BuildingPanelScript>().SetData(selectedObject);
				buildingInfoPanel.SetActive(true);
			} else {
				Debug.LogError("Selected object has no building script");
			}
		}
	}
}
