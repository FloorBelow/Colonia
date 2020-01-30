using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResourcePinsScript : MonoBehaviour
{
	public GameObject popupPanel;
	public GameObject resourceCounterPrefab;
	public GameObject resourceCounterTogglePrefab;
	public ResourceData[] defaultPinnedResources;

	void Start() {
		Button b = GetComponent<Button>();
		b.onClick.AddListener(delegate { TogglePopupPanel(); });
		InitPopupPanel();
	}

	void InitPopupPanel() {
		foreach (ResourceData resource in GameManagerScript.m.resourceTypeSet.objects) {
			GameObject counter = Instantiate(resourceCounterPrefab, transform);
			Image counterImage = counter.transform.GetChild(0).GetComponent<Image>();
			counterImage.sprite = resource.iconShort;
			counterImage.color = resource.color;
			counter.transform.GetChild(1).GetComponent<MapResourceCounterScript>().resource = resource;



			GameObject toggle = Instantiate(resourceCounterTogglePrefab, popupPanel.transform);
			Image toggleImage = toggle.transform.GetChild(0).GetComponent<Image>();
			toggleImage.sprite = resource.iconShort;
			toggleImage.color = resource.color;
			toggle.transform.GetChild(1).GetComponent<MapResourceCounterScript>().resource = resource;
			Toggle t = toggle.transform.GetChild(2).GetComponent<Toggle>();
			t.onValueChanged.AddListener(delegate { Toggle(counter); });
			if (System.Array.IndexOf(defaultPinnedResources, resource) != -1) t.SetIsOnWithoutNotify(true);
			else counter.SetActive(false);
		}
	}

	void Toggle(GameObject o) {
		o.SetActive(!o.activeSelf);
		o.GetComponentInChildren<MapResourceCounterScript>().UpdateCount();
	}

	void SetResourcePins() {

	}

	void TogglePopupPanel() {
		popupPanel.SetActive(!popupPanel.activeSelf);
		foreach (MapResourceCounterScript m in popupPanel.GetComponentsInChildren<MapResourceCounterScript>()) m.UpdateCount();
	}
}
