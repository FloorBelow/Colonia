using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonSetResolutionScript : MonoBehaviour
{

	public int x; public int y;
    // Start is called before the first frame update
    void Start() {
		ScriptCamera scriptCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScriptCamera>();
		Image overlay = GameObject.Find("Overlay").GetComponent<Image>();
		UnityAction action = delegate { SetResolutionStarter(x, y, scriptCamera, overlay); };
		gameObject.GetComponent<Button>().onClick.AddListener(action);
    }

	public void SetResolutionStarter(int x, int y, ScriptCamera scriptCamera, Image overlay) {
		StartCoroutine(SetResolution(x, y, scriptCamera, overlay));
	}

	IEnumerator SetResolution(int x, int y, ScriptCamera scriptCamera, Image overlay) {
		overlay.color = Color.black;
		yield return null; yield return null;
		if ((x == 1920) && (y == 1080)) { Screen.SetResolution(x, y, FullScreenMode.FullScreenWindow); } else { Screen.SetResolution(x, y, FullScreenMode.Windowed); }
		yield return null; yield return null;
		scriptCamera.UpdateCamSize();
		overlay.color = Color.clear;
	}
}
