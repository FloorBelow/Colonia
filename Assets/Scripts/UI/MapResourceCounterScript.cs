using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapResourceCounterScript : MonoBehaviour
{
	TMPro.TextMeshProUGUI text;
	public ResourceData resource;
	int count;

	void Start() {
		text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
	}

	void Update() {
		int newCount = GameManagerScript.m.activeMap.resourceCounts[resource];
		if (newCount == count) return;
		count = newCount;
		text.text = count.ToString();
	}

	public void UpdateCount() {
		gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = GameManagerScript.m.activeMap.resourceCounts[resource].ToString();
	}
}
