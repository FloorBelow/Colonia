using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingPanelHouseCounterScript : MonoBehaviour
{
	public GameObject iconHappy;
	public GameObject iconUnhappy;
	public GameObject barHappiness;
	public GameObject ringFood;
	public GameObject iconFood;
	public GameObject iconWater;
	public GameObject iconReligious;

	public NeedData needWater;
	public NeedData needReligious;

	public void SetData(BuildingHouseScript house) {
		//Happiness
		if (house.needsMet) iconUnhappy.SetActive(false); else iconHappy.SetActive(false);
		barHappiness.GetComponent<Image>().fillAmount = house.happiness / 100f;
		//Food
		if (house.food.resource != null) {
			Image image = ringFood.GetComponent<Image>();
			image.color = Color.white; image.fillAmount = house.food.timer / house.foodConsumptionTime;
			image = iconFood.GetComponent<Image>();
			image.sprite = house.food.resource.icon; image.color = house.food.resource.color;
		}
		//Needs
		if (house.GetNeedCount(needWater) > 0) iconWater.GetComponent<Image>().color = needWater.color;
		if (house.GetNeedCount(needReligious) > 0) iconReligious.GetComponent<Image>().color = needReligious.color;
	}

}
