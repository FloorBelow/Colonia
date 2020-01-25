using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionPanelScript : MonoBehaviour
{
	public GameObject buildingButtonPanel;
	BuildingButtonPanelScript buildingButtons;


	public GameObject namePanel;
	public GameObject perkPanel;
	public GameObject purchasePanel;
	public GameObject houseCounterPanel;

	RegionScript region;

	Image[] perkImages;
    // Start is called before the first frame update
    void Init()
    {
		perkImages = perkPanel.GetComponentsInChildren<Image>();
		buildingButtons = buildingButtonPanel.GetComponent<BuildingButtonPanelScript>();
    }

	public void SetData(RegionScript region) {
		this.region = region;
		if (perkImages == null) Init();
		//Title
		Image image = namePanel.GetComponentInChildren<Image>();
		image.sprite = region.regionType.icon; image.color = region.regionType.iconColor;
		namePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = region.regionType.regionName;
		//Perks
		if (region.perks.Length == 0) perkPanel.SetActive(false); else {
			perkPanel.SetActive(true);
			for (int i = 0; i < perkImages.Length; i++) {
				if (i < region.perks.Length) {
					perkImages[i].gameObject.SetActive(true);
					perkImages[i].sprite = region.perks[i].icon;
					perkImages[i].color = region.perks[i].iconColor;
				} else perkImages[i].gameObject.SetActive(false);
			}
		}

		//Purchase panel
		purchasePanel.SetActive(region.regionType.type == RegionScript.REGION_TYPE.Wilderness);

		//Building Buttons
		for(int i = 0; i < region.allowedBuildings.Length; i++) {
			if (region.allowedBuildings[i] == 1) buildingButtons.buttons[i].SetActive(true); else buildingButtons.buttons[i].SetActive(false);
		}

		//House Panel
		if (region.regionType.type == RegionScript.REGION_TYPE.Urban) {
			houseCounterPanel.SetActive(true); houseCounterPanel.GetComponent<RegionHouseCounterPanelScript>().UpdateHouseCounter(region);
		} else houseCounterPanel.SetActive(false);
	}

	public void UpdatePanel() {
		if (region.regionType.type == RegionScript.REGION_TYPE.Urban) houseCounterPanel.GetComponent<RegionHouseCounterPanelScript>().UpdateHouseCounter(region);
	}
}
