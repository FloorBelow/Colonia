using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
	bool isActive;
	public GameObject pausePanel;

	private void Start() {
		isActive = false;
		pausePanel.SetActive(false);
	}

	public void ToggleActive() {
		isActive = !isActive;
		pausePanel.SetActive(isActive);
	}

	public void Exit() { Application.Quit(); }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape)) ToggleActive();
    }
}