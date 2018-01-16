using System;
using UnityEngine;
using Object = UnityEngine.Object;


/* Roads are attached to the from intersection */
public class Road
{
	public Intersection fromIntersection;
	public Intersection toIntersection;

	private RoadBuilder builder;
	private GameObject gameObject;

	private RoadConfiguration config;


	public float GetRoadWidth ()
	{
		return config.roadWidth;
	}

	public Road (Intersection fromIntersection, Intersection toIntersection, RoadConfiguration config)
	{
		this.fromIntersection = fromIntersection;
		this.toIntersection = toIntersection;
		this.config = config;

		fromIntersection.connectToRoad (this);
		toIntersection.connectToRoad (this);

		this.builder = GameObject.FindWithTag (GlobalTags.Builder).GetComponent<RoadBuilder> ();
		this.gameObject = this.builder.BuildRoad (this);
		if (config.isTemporary)
		{
			this.gameObject.layer = GlobalLayers.IgnoreRaycast;
			this.gameObject.tag = GlobalTags.Untagged;
		}

	}

	public void UpdateGameObject()
	{
		this.gameObject = this.builder.UpdateRoad (this, this.gameObject);
		// we don't want physics on temporary roads
		if (this.config.isTemporary)
		{
			this.gameObject.layer = GlobalLayers.IgnoreRaycast;
			this.gameObject.tag = GlobalTags.Untagged;
		}
	}

	public void UpdateGameObjectAndOtherIntersection (Intersection callingIntersection)
	{
		UpdateGameObject();
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
		return config.roadWidth / 2;
	}

// return the distance between from and to intersections
	public float GetRoadLength()
	{
		return (fromIntersection.position - toIntersection.position).magnitude;
	}

    /**
     * Returns the game object associated with this road
     */
	public GameObject GetGameObject()
	{
		return gameObject;
	}

	
}
