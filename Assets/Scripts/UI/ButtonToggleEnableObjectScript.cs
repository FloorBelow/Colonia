using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonToggleEnableObjectScript : MonoBehaviour
{

	public GameObject toggleObject;
	bool enabled;
    // Start is called before the first frame update
    void Start()
    {
		enabled = toggleObject.activeSelf;
		UnityAction action = delegate { Toggle(); };
		gameObject.GetComponent<Button>().onClick.AddListener(action);
    }

    void Toggle() {
		enabled = !enabled;
		toggleObject.SetActive(enabled);
	}
}
