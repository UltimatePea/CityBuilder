using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Camera mainCamera;
	public CarTrafficManager trafficManager;

	/** The distance that camera moves back from car */
	public float backDistance;
	/** The distance taht camera moves up from car */
	public float upDisatnce;
	
	
	private int index = -1;
	private Vector3 initialCameraPosition;
	private Quaternion initialCameraRotation;
	private Transform initialCameraParent;

	// Use this for initialization
	void Start ()
	{

		initialCameraPosition = mainCamera.transform.position;
		initialCameraRotation = mainCamera.transform.rotation;
		initialCameraParent = mainCamera.transform.parent;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.E))
		{
			var cars = trafficManager.GetAllCars();
			if (cars.Count == 0)
			{
				Debug.LogWarning("Attempt to switch to car view when there is no car visible");
				return;
			}
			
			// reset the index to zero if index is out of range
			index = (index + 1) % cars.Count;

			switchCameraToCar(cars[index]);


		} else if (Input.GetKeyDown(KeyCode.R))
		{
			// reset camera to initial transform
			mainCamera.transform.position = initialCameraPosition;
			mainCamera.transform.rotation = initialCameraRotation;
			mainCamera.transform.parent = initialCameraParent;

		}
	}

	private void switchCameraToCar(AbstractCar abstractCar)
	{
		// offset the camera to the correct position
		Vector3 position = abstractCar.carGameObject.transform.position
		                   - backDistance * abstractCar.carGameObject.transform.right
		                   + upDisatnce * abstractCar.carGameObject.transform.up;
		mainCamera.transform.position = position;
		
		// let the camera look at the car
		mainCamera.transform.LookAt(abstractCar.carGameObject.transform.position);
		// TODO: use lerp tracking
		mainCamera.transform.parent = abstractCar.carGameObject.transform;

	}
}
