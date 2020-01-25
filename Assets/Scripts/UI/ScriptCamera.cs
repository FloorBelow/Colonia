using UnityEngine;
using System.Collections;

public class ScriptCamera : MonoBehaviour {

	public float speedMult;
	float speed;
	public float scaling;

	Camera cameraVar;
    Camera modelCam;
    public GameObject modelCamObject;
    public GameObject quad;
	// Use this for initialization
	void Start () {
		speed = speedMult / 32f;
		cameraVar = gameObject.GetComponent<Camera>();
        if(!GameManagerScript.m.mobile) modelCam = modelCamObject.GetComponent<Camera>();
		else {
			modelCamObject.SetActive(false);
			quad.SetActive(false);
			GameManagerScript.m.threedeeworld.GetChild(0).gameObject.SetActive(false);
			GameManagerScript.m.threedeeworld.GetChild(1).gameObject.SetActive(false);
		}
        UpdateCamSize();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W)){
			transform.Translate(0,speed,0);
		}

		if (Input.GetKey(KeyCode.S)){
			transform.Translate(0,-speed,0);
		}

		if (Input.GetKey(KeyCode.A)){
			transform.Translate(-speed,0,0);
		}

		if (Input.GetKey(KeyCode.D)){
			transform.Translate(speed,0,0);
		}
		if (Input.GetKeyDown (KeyCode.RightBracket)) {
            if (scaling >= 1)
            {
                scaling++;

            }
            else
            {
                scaling = scaling * 2;
            }
            UpdateCamSize();
		}

		if (Input.GetKeyDown (KeyCode.LeftBracket)) {
			if (scaling > 1){
				scaling--;

            }
            else
            {
                scaling = scaling / 2;
            }
            UpdateCamSize();
        }
	}

    public void UpdateCamSize() {
        float size = Screen.height / (256f * scaling);
        cameraVar.orthographicSize = size;
		if(!GameManagerScript.m.mobile) {
			modelCam.orthographicSize = size;
			quad.transform.localScale = new Vector3(size * 32f / 9f, size * 2, 1);
		}
    }
}
