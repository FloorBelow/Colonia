using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameStateScript : MonoBehaviour {
	abstract public void EnterState();
	abstract public void ExitState();
	abstract public void LeftClick(bool onUI);
	abstract public void RightClick(bool onUI);
}
