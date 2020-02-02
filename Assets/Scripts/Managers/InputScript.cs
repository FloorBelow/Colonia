using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputScript : MonoBehaviour {

    public GameManagerScript gameManager;
    public Vector3 mousePos;
    public int mouseTileX;
    public int mouseTileY;
	public bool isMouseOverUI;

	public delegate void OnMouseTileChangeDelegate(int x, int y);
	public static event OnMouseTileChangeDelegate OnMouseTileChange;


    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
		int newMouseTileX = Mathf.FloorToInt(mousePos.y * 2 + mousePos.x);
		int newMouseTileY = Mathf.FloorToInt(mousePos.y * 2 - mousePos.x);
		if ((newMouseTileX != mouseTileX || newMouseTileY != mouseTileY) && OnMouseTileChange != null) OnMouseTileChange(newMouseTileX, newMouseTileY);
		mouseTileX = newMouseTileX;
		mouseTileY = newMouseTileY;
		isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
		if(!isMouseOverUI) {
			if (Input.GetMouseButtonDown(0)) {
				gameManager.LeftClick();
			} else if (Input.GetMouseButtonDown(1)) {
				gameManager.RightClick();
			} else if (Input.GetMouseButtonDown(2)) {
				gameManager.MiddleClick();
			}
		}

		/*
		if (Input.GetMouseButtonDown(2)) {
			if (gameManager.placeBuilding == 1) {
				gameManager.RotateBuildingGhost();
			}
		}

		
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			if(gameManager.gameState == 0) {
				//gameManager.CreateBuildingGhost();
			} else if(gameManager.gameState == 1) {
				gameManager.PlaceBuilding(mouseTileX, mouseTileY);
			}
        }
		


		//Left click:
		//Clicking on UI should disable building placement?
		//Clicking while in build mode places building
		//Clicking while out of build mode selects building(/Walker?)

		if (Input.GetMouseButtonDown(0) && gameManager.placeBuilding == 1) {
			if (EventSystem.current.IsPointerOverGameObject()) {
				
				gameManager.RemoveBuildingGhost();
			} else {
				gameManager.PlaceBuilding(mouseTileX, mouseTileY);
			}
		}
		

		if (Input.GetMouseButtonDown(1)) {
			if (gameManager.placeBuilding == 1) {
				gameManager.RemoveBuildingGhost();
			} else {
				gameManager.RemoveBuilding(mouseTileX,mouseTileY);
			}
		}
		*/


	}
}
