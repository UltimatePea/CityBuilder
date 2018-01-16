using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAdditionController : MonoBehaviour
{


	/** we require intersection manager for collision testing */
	public IntersectionManager intersectionManager;
	public BuildingManager buildingManager;
	public GameObject buildingPrefab;
	
	// Update is called once per frame
	void Update () {
		GameObject obj;
		Vector3 position = MouseCollisionDetection.getMousePositionOnPlane(out obj);
		
		if (Input.GetMouseButtonDown(0))
		{
			// if we click on a plane, add a car to it;
			if (obj.tag == GlobalTags.Road)
			{
				Road road = intersectionManager.roadForGameObject(obj);
				
				// try to construct a Abstract Builidng Object
				
				// we need to get the reference intersection
				// determine the side of the road by euler angles.
				Quaternion angleQuaternion = Quaternion.FromToRotation(
					road.toIntersection.position - road.fromIntersection.position, 
					position - road.fromIntersection.position);
				;
				Intersection referenceIntersection =
					angleQuaternion.eulerAngles.y < 180 ? road.fromIntersection : road.toIntersection;
				
				// determine the offset by projection
				Vector3 locationVector = Vector3.Project(position - referenceIntersection.position,
					IntersectionMath.getOutgoingVector(referenceIntersection, road));
				float offset = locationVector.magnitude;
				
				// figure out the position of the gameObject
				Quaternion rotateRight = Quaternion.Euler(0, 90, 0);
				Vector3 offsetFromLocationVector = road.GetRoadWidthWithReferenceIntersection(referenceIntersection) 
				                                   * (rotateRight * locationVector.normalized);

				Vector3 gameObjectPosition = referenceIntersection.position + locationVector + offsetFromLocationVector;
				
				// figure out the correct orientation
				Quaternion gameObjectOrientation = Quaternion.FromToRotation(Vector3.right, Quaternion.Euler(0, -90, 0) * locationVector);

				// create correct gameobject
				GameObject newGameObject = Instantiate(buildingPrefab, gameObjectPosition, gameObjectOrientation);
				
				// create abstract building
				AbstractBuildingPosition buildingPos = new AbstractBuildingPosition(road, referenceIntersection, offset);
				AbstractBuilding building = new AbstractBuilding(buildingPos, newGameObject);
				buildingManager.addBuilding(building);

			}
		}	
	}
}
