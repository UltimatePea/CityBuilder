using UnityEngine;
using System;
using System.Collections.Generic;


public class IntersectionManager : MonoBehaviour
{
	
	/* dependency */
	public CarTrafficManager trafficManager;

	/* temporary */
	private List<Intersection> temporaryIntersections = new List<Intersection> ();
	private Road tempRoad;

	// temporary intersection cannot be retrieved via game object
	public Intersection createTemporaryIntersection (Vector3 position)
	{
		Intersection temp = new  Intersection (position, new IntersectionCustomization (isTemporary:true, 
			trafficControlStyle:IntersectionCustomization.TrafficControlStyle.ALL_WAY_STOP_SIGN));
		temporaryIntersections.Add (temp);
		return temp;
	}
	// true : there's a temporary intersection
	public bool temporaryIntersectionExists ()
	{
		return temporaryIntersections.Count != 0;
	}

	public bool isTemporaryIntersectionPresent (Intersection intersec)
	{
		return temporaryIntersections.Contains (intersec);
	}

	public void updateTemporaryIntersectionPosition (Vector3 position)
	{
		temporaryIntersections.ForEach (inter => inter.position = position);
	}

	public void createTemporaryConnection (Intersection fromIntersection, Intersection toIntersection, float roadWidth)
	{
		Debug.Assert (canConnectTwoIntersections (fromIntersection, toIntersection, roadWidth));
		Road road =  fromIntersection.connectToIntersection (toIntersection, new RoadConfiguration (roadWidth, isTemporary:true));
		
		tempRoad = road;
	}

	public void removeTemporaryRoadIfThereIsAny ()
	{
		if (tempRoad != null) {
			tempRoad.DeleteRoad ();
			tempRoad = null;
		}
	}

	public void removeTemporaryIntersectionIfThereIsAny ()
	{
		// since temporary does not count towards all intersections
		temporaryIntersections.ForEach (i => i.DeleteIntersection ());
		temporaryIntersections.Clear ();
	}


	public Intersection createIntersection (Vector3 position)
	{
		Intersection intersec = new Intersection (position, new IntersectionCustomization (isTemporary: false, 
			trafficControlStyle: IntersectionCustomization.TrafficControlStyle.ALL_WAY_STOP_SIGN));
		allInstances.Add (intersec);
		trafficManager.addNewIntersection(intersec);
		return intersec;
	}

	public bool canConnectTwoIntersections (Intersection fromIntersection, Intersection toIntersection, float roadWidth)
	{
		// TODO : Add failure reason
		if (fromIntersection == toIntersection) {
			// no self loop
			return false;
		} else if (fromIntersection.IsDirectlyConnectedTo (toIntersection)) {
			// connection already exits
			return false;
		} else if (fromIntersection.position == toIntersection.position) {
			// no overlapping interesections
			return false;
		}
		return true;
	}

	public Road connectTwoIntersections (Intersection fromIntersection, Intersection toIntersection, float roadWidth)
	{
		Debug.Assert (canConnectTwoIntersections (fromIntersection, toIntersection, roadWidth));
		Road road =  fromIntersection.connectToIntersection (toIntersection, new RoadConfiguration (roadWidth, isTemporary:false));
		trafficManager.addNewRoad(road);
		return road;
	}

	/* Mapping */
	private List<Intersection> allInstances = new List<Intersection> ();


	public Intersection intersectionForGameObject (GameObject gameObject)
	{
		foreach (Intersection inter in allInstances) {
			if (inter.GetGameObject () == gameObject) {
				return inter;
			}
		}
		Debug.LogError("Unable to find intersection for game object");
		return null;
	}


	/**
	 * Return the road for corresponding to a given gameobject. The road should be one previously returned
	 * by `connectTwoIntersection`.
	 */
	public Road roadForGameObject(GameObject gameObj)
	{
		foreach (Intersection inter in allInstances) {
			foreach (Road rd in inter.getConnectedRoads())
			{
                if (rd.GetGameObject () == gameObj) {
                    return rd;
                }
			}
		}
		Debug.LogError("Unable to find road for game object");
		return null;	
	}
}

