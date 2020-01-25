using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelScript : MonoBehaviour
{
	public GameObject buildingIconGameObject;
	Image buildingIcon;

	public GameObject resourceCounterPrefab;
	public GameObject workerCounterPrefab;
	public GameObject resourceCreatorCounterPrefab;

	//Houses
	public GameObject houseCounterPrefab;

	public GameObject happinessCounterPrefab;
	public GameObject needCounterPrefab;
	public GameObject foodCounterPrefab;
	List<GameObject> dynamicChildren;

	private void Start() {
		dynamicChildren = new List<GameObject>();
		buildingIcon = buildingIconGameObject.GetComponent<Image>();
	}

	public void SetData(GameObject building) {
		foreach (GameObject o in dynamicChildren) Destroy(o); dynamicChildren.Clear(); //Need to do some kind of pooling later, seperate SetData and UpdateData
		BuildingScript buildingScript = building.GetComponent<BuildingScript>();
		gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buildingScript.buildingName;
		buildingIcon.sprite = buildingScript.icon.sprite;
		buildingIcon.color = buildingScript.icon.color.color;

		BuildingHouseScript house = building.GetComponent<BuildingHouseScript>();
		if (house != null) {
			CreateHouseCounter(house);
			//CreateHappinessCounter(house);
			//CreateFoodCounter(house);
			//foreach (NeedData data in house.needData) CreateNeedsCounter(data, house.needs[data]);
		}

		BuildingWorkerScript workerScript = building.GetComponent<BuildingWorkerScript>();
		if (workerScript != null) CreateWorkerCounter(workerScript);

		ResourceCreatorScript resourceCreator = building.GetComponent<ResourceCreatorScript>();
		if (resourceCreator != null) CreateResourceCreatorCounter(resourceCreator);

		ResourceStorageScript storage = building.GetComponent<ResourceStorageScript>();
		if(storage != null) {
			foreach (ResourceData resource in storage.totals.Keys) {
				CreateResourceCounter(resource, storage.totals[resource]);
			}
		}


	}

	void CreateHouseCounter(BuildingHouseScript house) {
		GameObject counter = Instantiate(houseCounterPrefab, transform);
		dynamicChildren.Add(counter);
		counter.GetComponent<UIBuildingPanelHouseCounterScript>().SetData(house);
	}

	void CreateHappinessCounter(BuildingHouseScript house) {
		GameObject counter = Instantiate(happinessCounterPrefab, transform);
		dynamicChildren.Add(counter);
		if (house.needsMet) counter.transform.GetChild(1).gameObject.SetActive(false); else counter.transform.GetChild(0).gameObject.SetActive(false);
		counter.transform.GetChild(3).GetComponent<Image>().fillAmount = house.happiness/100f;
	}

	void CreateFoodCounter(BuildingHouseScript house) {
		GameObject counter = Instantiate(foodCounterPrefab, transform);
		dynamicChildren.Add(counter);
		if(house.food.resource != null) {
			Image image = counter.transform.GetChild(1).GetComponent<Image>();
			image.color = Color.white; image.fillAmount = house.food.timer / house.foodConsumptionTime;
			image = counter.transform.GetChild(2).GetComponent<Image>();
			image.sprite = house.food.resource.icon; image.color = house.food.resource.color;
		}
	}

	void CreateNeedsCounter(NeedData data, int count) {
		GameObject counter = Instantiate(needCounterPrefab, transform);
		dynamicChildren.Add(counter);
		for(int i = 0; i <= count; i++) counter.transform.GetChild(i).GetComponent<Image>().color = data.color.color;
	}

	void CreateResourceCounter(ResourceData resource, int count) {
		GameObject counter = Instantiate(resourceCounterPrefab, transform);
		dynamicChildren.Add(counter);
		counter.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = resource.resourceName + ": " + count.ToString();
		counter.GetComponentInChildren<Image>().sprite = resource.icon;
		counter.GetComponentInChildren<Image>().color = resource.color;
	}

	void CreateWorkerCounter(BuildingWorkerScript workerScript) {
		GameObject counter = Instantiate(workerCounterPrefab, transform);
		dynamicChildren.Add(counter);
		counter.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = workerScript.workers.ToString() + "/" + workerScript.workersRequired.ToString();
	}

	void CreateResourceCreatorCounter(ResourceCreatorScript creator) {
		GameObject counter = Instantiate(resourceCreatorCounterPrefab, transform);
		dynamicChildren.Add(counter);
		float c = creator.GetCompletionPercentage();
		counter.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = System.String.Format("{0:0%} Complete", c);
		counter.transform.GetChild(0).GetComponent<Image>().fillAmount = c;
		counter.transform.GetChild(1).GetComponent<Image>().sprite = creator.resource.icon;
		counter.transform.GetChild(1).GetComponent<Image>().color = creator.resource.color;
	}
}
