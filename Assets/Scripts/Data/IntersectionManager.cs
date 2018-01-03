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
		Intersection temp = new  Intersection (position, new IntersectionCustomization (true));
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

	public void createTemporaryConnection (Intersection intersection1, Intersection intersection2)
	{
		Road rd = connectTwoIntersections (intersection1, intersection2);
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
		Intersection intersec = new Intersection (position, new IntersectionCustomization (false));
		allInstances.Add (intersec);
		return intersec;
	}

	public bool canConnectTwoIntersections (Intersection fromIntersection, Intersection toIntersection)
	{
		// TODO : Add failure reason
		if (fromIntersection == toIntersection) {
			return false;
		} else if (fromIntersection.IsDirectlyConnectedTo (toIntersection)) {
			return false;
		}
		return true;
	}

	public Road connectTwoIntersections (Intersection fromIntersection, Intersection toIntersection)
	{
		Debug.Assert (canConnectTwoIntersections (fromIntersection, toIntersection));
		return fromIntersection.connectToIntersection (toIntersection);
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

