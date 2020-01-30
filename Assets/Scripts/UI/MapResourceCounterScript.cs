using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapResourceCounterScript : MonoBehaviour
{
	TMPro.TextMeshProUGUI text;
	public ResourceData resource;
	void Start() {
		text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
	}

	void Update() {
		text.text = GameManagerScript.m.activeMap.resourceCounts[resource].ToString();
	}

	public void UpdateCount() {
		gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = GameManagerScript.m.activeMap.resourceCounts[resource].ToString();
	}
}
