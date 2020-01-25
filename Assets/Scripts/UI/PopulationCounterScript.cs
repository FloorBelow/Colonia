using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationCounterScript : MonoBehaviour
{
	TMPro.TextMeshProUGUI text;
    void Start()
    {
		text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
		text.text = System.String.Format("{0}/{1}", GameManagerScript.m.activeMap.population, GameManagerScript.m.activeMap.workersRequired) ;
    }
}
