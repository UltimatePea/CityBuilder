using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour
{

	public enum GameMode
	{
		NOT_DEFINED,
		ROAD_CONSTRUCTION,
		ROAD_ADJUSTMENT,
		TRAFFIC_ADDITION,
		BUILDING_ADDITION
	}

	;



	public GameMode defaultGameMode;

	private GameMode currentGameMode;
	private MonoBehaviour currentActiveController;



	public GameMode GetCurrentGameMode ()
	{
		return currentGameMode;
	}

	// Use this for initialization
	void Start ()
	{

		setGameMode (defaultGameMode);

		
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKeyDown (KeyCode.C)) {
			setGameMode (GameMode.ROAD_CONSTRUCTION);
		} else if (Input.GetKeyDown (KeyCode.A)) {
			setGameMode (GameMode.ROAD_ADJUSTMENT);
		} else if (Input.GetKeyDown(KeyCode.Q))
		{
			setGameMode(GameMode.TRAFFIC_ADDITION);
		} else if (Input.GetKeyDown(KeyCode.W))
		{
			setGameMode(GameMode.BUILDING_ADDITION);
		}
	}

	void setGameMode (GameMode gameMode)
	{

		// do nothing if the modes are equal
		if (currentGameMode == gameMode) {
			return;
		}


		Debug.LogFormat ("Game mode set to {0}", gameMode);

		// disable current mode if there's any
		if (this.currentActiveController) {
			this.currentActiveController.enabled = false;
		}


		// set property
		this.currentGameMode = gameMode;

		// set active controller
		switch (gameMode) {
		case GameMode.ROAD_ADJUSTMENT:
			this.currentActiveController = GetComponent<RoadAdjustmentController> ();
			break;
		case GameMode.ROAD_CONSTRUCTION:
			this.currentActiveController = GetComponent<RoadConstructionController> ();
			break;
		case GameMode.TRAFFIC_ADDITION:
			this.currentActiveController = GetComponent<TrafficAdditionController>();
			break;
		case GameMode.BUILDING_ADDITION:
			this.currentActiveController = GetComponent<BuildingAdditionController>();
			break;
		}

		// enable current component
		this.currentActiveController.enabled = true;
	}

}
