using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateRegionScript : GameStateScript {

	MapScript region;
	bool placingBuilding;

	//Input Functions
	public override void EnterState() {
	}

	public override void ExitState() {
	}

	public override void LeftClick(bool onUI) {
		if (onUI) {
			Debug.Log("Left Click on UI");
		} else {
			Debug.Log("Left Click in Region view");
		}
	}

	 public override void RightClick(bool onUI) {
		if (onUI) {
			Debug.Log("Right Click on UI");
		} else {
			Debug.Log("Right Click in Region view");
		}
	}



	void Update () {
		
	}
}