using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunrotatescript : MonoBehaviour
{

    public float rotation = 0;
    public float rotationSpeed = 3;
    public float sunAngle = 30;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        rotation = (rotation + rotationSpeed * Time.deltaTime) % 360;
        transform.localRotation = Quaternion.Euler(sunAngle, rotation, 0);
    }
}
