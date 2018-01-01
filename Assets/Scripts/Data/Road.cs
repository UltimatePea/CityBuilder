using System;
using UnityEngine;



/* Roads are attached to the from intersection */
public class Road
{
	public Intersection fromIntersection;
	public Intersection toIntersection;

	private RoadBuilder builder;
	private GameObject gameObject;

	public Road (Intersection fromIntersection, Intersection toIntersection)
	{
		this.fromIntersection = fromIntersection;
		this.toIntersection = toIntersection;

		fromIntersection.connectToRoad (this);
		toIntersection.connectToRoad (this);

		this.builder = GameObject.FindWithTag (GlobalTags.Builder).GetComponent<RoadBuilder> ();
		this.gameObject = this.builder.BuildRoad (this);


	}

	public void UpdateGameObject (Intersection callingIntersection)
	{
		this.gameObject = this.builder.UpdateRoad (this, this.gameObject);
		switch (typeOfIntersection (callingIntersection)) {
		case IntersectionClass.FROM:
			this.toIntersection.UpdateGameObject ();
			break;
		case IntersectionClass.TO:
			this.fromIntersection.UpdateGameObject ();
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
}
