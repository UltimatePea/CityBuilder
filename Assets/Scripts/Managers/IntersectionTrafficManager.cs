using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTrafficManager : MonoBehaviour
{


	private List<Intersection> allIntersections = new List<Intersection>();
	private List<IntersectionControl> allControls = new List<IntersectionControl>();

	public void addNewIntersection(Intersection intersection)
	{
		allIntersections.Add(intersection);
		allControls.Add(new IntersectionControl(intersection));
	}

	public IntersectionControl intersectionControlForIntersection(Intersection intersection)
	{
		return allControls[allIntersections.IndexOf(intersection)];
	}

}
