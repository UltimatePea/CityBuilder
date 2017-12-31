using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class IntersectionBuilder : MonoBehaviour
{

	public GameObject fourWayIntersectionPrefab;
	public GameObject oneWayIntersectionPrefab;


	// build a single intersection, returns the created game object
	public GameObject BuildIntersection (Intersection intersection)
	{
		Road[] roads = intersection.getConnectedRoads ();
		// no intersection if no roads
		if (roads.Length == 0) {
			return Instantiate(oneWayIntersectionPrefab, )
		}
		
	}

}
