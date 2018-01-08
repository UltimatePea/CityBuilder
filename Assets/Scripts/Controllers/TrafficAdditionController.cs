using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficAdditionController : MonoBehaviour
{
	public  GameObject carPrefab;

	public CarTrafficManager carTrafficManager;
	public IntersectionManager intersectionManager;

	public IntersectionTrafficMath intersectionTrafficMath;

	public int laneNumber;


	void Update()
	{
		GameObject obj;
		Vector3 position = MouseCollisionDetection.getMousePositionOnPlane(out obj);

		laneNumberControl();
		
		if (Input.GetMouseButtonDown(0))
		{
			// if we click on a plane, add a car to it;
			if (obj.tag == GlobalTags.Intersection)
			{
				Intersection intersection = intersectionManager.intersectionForGameObject(obj);
				Road road = intersection.getConnectedRoads()[0];
				AbstractCarPosition abstractPosition = new AbstractCarPosition(road, intersection, 0, laneNumber);
				AbstractCar car = new AbstractCar(abstractPosition, Instantiate(carPrefab, intersection.position, Quaternion.identity));
				carTrafficManager.AddCar(car);
				
				
			}
		}

	}

	private void laneNumberControl()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			laneNumber = 0;
		} else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			laneNumber = 1;
		}else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			laneNumber = 2;
		}
	}
}
