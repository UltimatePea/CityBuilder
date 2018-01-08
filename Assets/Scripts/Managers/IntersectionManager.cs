using UnityEngine;
using System;
using System.Collections.Generic;


public class IntersectionManager : MonoBehaviour
{

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

	public void createTemporaryConnection (Intersection intersection1, Intersection intersection2, float roadWidth)
	{
		Road rd = connectTwoIntersections (intersection1, intersection2, roadWidth);
		tempRoad = rd;
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
		return fromIntersection.connectToIntersection (toIntersection, new RoadConfiguration (roadWidth));
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
		return null;
	}



}

