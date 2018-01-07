using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCustomizationBuilder : MonoBehaviour
{

	public IntersectionBuilder intersectionBuilder;

	public GameObject stopSignPrefab;

	public GameObject buildCustomization(GameObject baseObj, Intersection intersection)
	{
		// build stop sign
		switch (intersection.customization.trafficControlStyle)
		{
				case IntersectionCustomization.TrafficControlStyle.ALL_WAY_STOP_SIGN:
				if (shouldBuildAllWayStopSign(intersection))
				{
					return buildAllWayStopSign(baseObj, intersection);
				}

					break;
		}
		
		// nothing gets built we return the base obj
		return baseObj;

	}

	private bool shouldBuildAllWayStopSign(Intersection intersection)
	{
		switch (intersectionBuilder.GetIntersectionType(intersection))
		{
			case IntersectionBuilder.IntersectionType.NONE:
			case IntersectionBuilder.IntersectionType.ONE_WAY:
				return false;
			case IntersectionBuilder.IntersectionType.MULTI_WAY:
				return true;
			case IntersectionBuilder.IntersectionType.TWO_WAY:
				switch (intersectionBuilder.GetTwoWayIntersectionType(intersection))
				{
					case IntersectionBuilder.TwoWayIntersectionType.SKEW:
					case IntersectionBuilder.TwoWayIntersectionType.TRANSITION:
						return true;
					case IntersectionBuilder.TwoWayIntersectionType.SMOOTH:
						return false;
				}

				break;
		}

		throw new InvalidOperationException();
	}

	private GameObject buildAllWayStopSign(GameObject baseObj, Intersection intersection)
	{
		foreach (Road road in intersection.getConnectedRoads())
		{
			// get the coordinate of stop sign for that road

			// left and right are regarding the viewpoint from the intersection, although we want our stop sign on 
			// the right, we need to pass in Left since stop sign is on the left of the road if you look at the road
			// from the intersection
			Vector3 position = intersectionBuilder.
				coordinateForRoadAtIntersection(intersection, road, IntersectionBuilder.RightOrLeft.LEFT);
			
			position = position + intersection.position;
			
			// get the orientation
			Quaternion orientation = IntersectionMath.getAngle(intersection, road);
			
			GameObject stopSign = Instantiate(stopSignPrefab, position, orientation);

			stopSign.transform.parent = baseObj.transform;

		}
		return baseObj;
	}
}
