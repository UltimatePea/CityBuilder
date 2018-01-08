using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficAdditionController : MonoBehaviour
{
	public  GameObject carPrefab;

	public CarTrafficManager carTrafficManager;
	public IntersectionManager intersectionManager;


	void Update()
	{
		GameObject obj;
		Vector3 position = MouseCollisionDetection.getMousePositionOnPlane(out obj);
		
		if (Input.GetMouseButtonDown(0))
		{
			// if we click on a plane, add a car to it;
			if (obj.tag == GlobalTags.Intersection)
			{
				Intersection intersection = intersectionManager.intersectionForGameObject(obj);
				Road road = intersection.getConnectedRoads()[0];
				AbstractCarPosition abstractPosition = new AbstractCarPosition(road, intersection, 0, 0);
				AbstractCar car = new AbstractCar(abstractPosition, Instantiate(carPrefab, intersection.position, Quaternion.identity));
				carTrafficManager.AddCar(car);
				
			}
		}

	}
}
