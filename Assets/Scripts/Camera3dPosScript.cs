using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3dPosScript : MonoBehaviour
{
	public GameObject spriteCamObject;
	Camera spriteCam;
    public float ratio = 1.73205257917f;
    public float offset = 98f;

	private void Start() {
		spriteCam = spriteCamObject.GetComponent<Camera>();
	}

	void LateUpdate()
    {
        transform.localPosition = new Vector3(spriteCamObject.transform.position.x, spriteCamObject.transform.position.y, offset + transform.localPosition.y * ratio);
    }
}
