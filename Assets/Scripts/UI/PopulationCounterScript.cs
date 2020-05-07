using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationCounterScript : MonoBehaviour
{
	TMPro.TextMeshProUGUI text;
    int pop;
    int work;
    void Start()
    {
		text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int newPop = GameManagerScript.m.activeMap.workersRequired;
        int newWork = GameManagerScript.m.activeMap.workersRequired;
        if(newPop != pop || newWork != work) {
            pop = newPop; work = newWork;
            text.text = $"{pop}/{work}";
        }
        
    }
}
