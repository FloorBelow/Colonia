using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionHouseCounterPanelScript : MonoBehaviour
{
	public Image populationImage;
	public TMPro.TextMeshProUGUI populationCounter;
	public Button upgradeButton;
	public GameObject[] needCounters;

	public void UpdateHouseCounter(RegionScript region) {
		int[] values = region.GetLevelUp();
		int pop = region.regionType.levelUpRequirement.population;
		populationCounter.text = region.GetPopulation().ToString();
		upgradeButton.interactable = GameManagerScript.m.debugMode ? true : values[0] == 1;
		for(int i = 0; i < 5; i++) {
			if (values[i + 1] == -1) {
				needCounters[i].SetActive(false);
			} else {
				needCounters[i].SetActive(true);
				//needCounters[i].transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = UtilityScript.IntToRoman(;
				needCounters[i].transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0}/{1}", values[i + 1], pop);
				needCounters[i].transform.GetChild(1).GetComponent<Image>().fillAmount = values[i + 1] / (float)pop;
			}
			//	Image icon = needCounters[i].transform.GetChild(2).GetComponent<Image>();
			//	icon.sprite = region.regionType.levelUpRequirement.needs[i - 2].icon;
			//	icon.color = region.regionType.levelUpRequirement.needs[i - 2].color;
			//}
		}
	}
}
