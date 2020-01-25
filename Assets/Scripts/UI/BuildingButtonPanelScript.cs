using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildingButtonPanelScript : MonoBehaviour
{
    public GameObject BuildingButtonPrefab;
	public GameObject[] buttons;
    void Start()
    {
		buttons = new GameObject[GameManagerScript.m.buildingSet.objects.Count];
		for(int i = 0; i < buttons.Length; i++) {
			BuildingScript buildingScript = GameManagerScript.m.buildingSet.objects[i].GetComponent<BuildingScript>();
			UnityAction action = delegate { GameManagerScript.m.SetCurrentBuildingPrefab(buildingScript.buildingName); };
			GameObject button = Instantiate(BuildingButtonPrefab, transform);
			button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buildingScript.buildingName;
			if (buildingScript.icon != null) {
				Image img = button.transform.Find("Icon").GetComponent<Image>();
				img.sprite = buildingScript.icon.sprite;
				img.color = buildingScript.icon.color.color;
			}
			button.GetComponent<Button>().onClick.AddListener(action);
			buttons[i] = button;
			//Hover tooltip
			EventTrigger trigger = button.AddComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener((data) => { GameManagerScript.m.uiManager.SetBuildingButtonTooltip(buildingScript, button.GetComponent<RectTransform>().position.y); });
			trigger.triggers.Add(entry);
			button.SetActive(false);
		}
    }
}
