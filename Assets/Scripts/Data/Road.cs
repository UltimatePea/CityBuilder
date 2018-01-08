using System;
using UnityEngine;



/* Roads are attached to the from intersection */
public class Road
{
	public Intersection fromIntersection;
	public Intersection toIntersection;

	private RoadBuilder builder;
	private GameObject gameObject;

	private float roadWidth;

	public float GetRoadWidth ()
	{
		return roadWidth;
	}

	public Road (Intersection fromIntersection, Intersection toIntersection, RoadConfiguration config)
	{
		this.fromIntersection = fromIntersection;
		this.toIntersection = toIntersection;
		this.roadWidth = config.roadWidth;

		fromIntersection.connectToRoad (this);
		toIntersection.connectToRoad (this);

		this.builder = GameObject.FindWithTag (GlobalTags.Builder).GetComponent<RoadBuilder> ();
		this.gameObject = this.builder.BuildRoad (this);


	}

	public void UpdateGameObject()
	{
		this.gameObject = this.builder.UpdateRoad (this, this.gameObject);
	}

	public void UpdateGameObjectAndOtherIntersection (Intersection callingIntersection)
	{
		this.gameObject = this.builder.UpdateRoad (this, this.gameObject);
		switch (typeOfIntersection (callingIntersection)) {
		case IntersectionClass.FROM:
			this.toIntersection.UpdateGameObjectAndAllConnectedRoadsExcept(this);
			break;
		case IntersectionClass.TO:
			this.fromIntersection.UpdateGameObjectAndAllConnectedRoadsExcept(this);
			break;
		case IntersectionClass.NONE:
			break;
		}
	}

	public enum IntersectionClass
	{
		FROM,
		TO,
		NONE
		/* Intersection ot part of this road */}

	;

	public IntersectionClass typeOfIntersection (Intersection intersection)
	{
		if (fromIntersection == intersection) {
			return IntersectionClass.FROM;
		} else if (toIntersection == intersection) {
			return IntersectionClass.TO;
		} else {
			Debug.LogError ("Invalid Intersection Query");
			return IntersectionClass.NONE;
		}
	}

	public void DeleteRoad ()
	{
		GameObject.Destroy (this.gameObject);
		this.fromIntersection.DeleteRoad (this);
		this.toIntersection.DeleteRoad (this);
		
	}

	public Intersection otherIntersection (Intersection intersection)
	{
		switch (typeOfIntersection (intersection)) {
		case IntersectionClass.FROM:
			return toIntersection;
		case IntersectionClass.TO:
			return fromIntersection;
		}
		return null;
	}

	public int GetNumberOfLanesInDirectionWithReferenceIntersection(Intersection positionReferenceIntersection)
	{
		// TODO : Allow variadic lanes
		return 2;
	}

	public float GetRoadWidthWithReferenceIntersection(Intersection positionReferenceIntersection)
	{
		// TODO: allow different road width for different direction
		return roadWidth / 2;
	}

// return the distance between from and to intersections
	public float GetRoadLength()
	{
		return (fromIntersection.position - toIntersection.position).magnitude;
	}
}
