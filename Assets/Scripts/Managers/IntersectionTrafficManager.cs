using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTrafficManager : MonoBehaviour
{


	private List<Intersection> allIntersections = new List<Intersection>();
	private List<Road> allRoads = new List<Road>();
	private List<IntersectionTrafficControl> allIntersectionControls = new List<IntersectionTrafficControl>();
	private List<RoadTrafficControl> allRoadControls = new List<RoadTrafficControl>();

	public void addNewIntersection(Intersection intersection)
	{
		allIntersections.Add(intersection);
		allIntersectionControls.Add(new IntersectionTrafficControl(intersection));
	}

	public IntersectionTrafficControl intersectionControlForIntersection(Intersection intersection)
	{
		return allIntersectionControls[allIntersections.IndexOf(intersection)];
	}

	public void addNewRoad(Road road)
	{
		allRoads.Add(road);
		allRoadControls.Add(new RoadTrafficControl(road));
	}

	public RoadTrafficControl roadControlForRoad(Road road)
	{
		return allRoadControls[allRoads.IndexOf(road)];
	}
}
