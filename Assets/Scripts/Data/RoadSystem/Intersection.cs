using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Intersection
{
	private List<Road> connectedRoads;
	private Vector3 _position;

	public IntersectionCustomization customization;

	public Vector3 position {
		get { return _position; }
		set {
			_position = value;
			// TODO update all roads mesh and this's mesh;
			connectedRoads.ForEach (rd => rd.UpdateGameObjectAndOtherIntersection (this));
			this.gameObject = builder.UpdateIntersection (this, gameObject);
		}
	}

	// the game object we own
	private GameObject gameObject;

	// reference to the global builder
	private IntersectionBuilder builder;

	public Intersection (Vector3 position, IntersectionCustomization customization)
	{
		this.customization = customization;
		this.connectedRoads = new List<Road> ();
		_position = position;

		this.builder = GameObject.FindWithTag (GlobalTags.Builder).GetComponent<IntersectionBuilder> ();
		// TODO Performance optimizaiton: this always build an empty game object
		this.gameObject = this.builder.BuildIntersection (this);


	}

	// this method updates the game object only, without resetting the roads it connects to
	public void UpdateGameObject ()
	{
		this.gameObject = builder.UpdateIntersection (this, gameObject);
	}

	public void UpdateGameObjectAndAllConnectedRoadsExcept(Road road)
	{
		this.gameObject = builder.UpdateIntersection (this, gameObject);
		foreach (Road rd in this.getConnectedRoads())
		{
			if (rd != road)
			{
				rd.UpdateGameObject();
			}
				
		}
		
	}

	public GameObject GetGameObject ()
	{
		return gameObject;
	}

	public Road[] getConnectedRoads ()
	{
		Road[] rds = (Road[])this.connectedRoads.ToArray ();
		return rds;
	}

	public void connectToRoad (Road road)
	{
		if (this.connectedRoads.Contains (road)) {
			Debug.LogWarning ("Repeated Road connection to intersection");
		}
		this.connectedRoads.Add (road);
		this.gameObject = this.builder.UpdateIntersection (this, this.gameObject);
	}

	public void DeleteRoad (Road road)
	{
		if (!this.connectedRoads.Contains (road)) {
			Debug.LogWarning ("Attemp to delete non-existent road");
		}
		this.connectedRoads.Remove (road);
		UpdateGameObject (); // we definitely changed appearance upon road deletion
	}

	/** This only checks for One Step **/
	public bool IsDirectlyConnectedTo (Intersection intersection)
	{
		foreach (Road rd in this.connectedRoads) {
			// return true if any road's other end
			if (rd.otherIntersection (this) == intersection) {
				return true;
			}
		}
		return false;
	}

	// this method does nothing if argument is the same as receiver
	public Road connectToIntersection (Intersection intersection, RoadConfiguration config)
	{

		// TODO : Fix this design
		return new Road (this, intersection, config);
	}

	public void DeleteIntersection ()
	{
		this.connectedRoads.ForEach (rd => rd.DeleteRoad ());
		GameObject.Destroy (this.gameObject);

	}

	public void RemoveConnectionTo (Intersection intersection)
	{
		foreach (Road road in connectedRoads) {
			switch (road.typeOfIntersection (this)) {
			case Road.IntersectionClass.FROM:
				if (intersection == road.toIntersection) {
					road.DeleteRoad ();
				}
				break;
			case Road.IntersectionClass.TO:
				if (intersection == road.fromIntersection) {
					road.DeleteRoad ();
				}
				break;
			}
		}
	}
	
	
	/** Convinience Delegation method **/
	// TODO: Change all usage to use these methods instead of IntersectionBuilder's

	public enum IntersectionGraphicsType { NONE, ONE_WAY_WRAP, TWO_WAY_TRANSITION, TWO_WAY_SKEW, TWO_WAY_SMOOTH, MULTI_WAY}

	public IntersectionGraphicsType GetGraphicsType()
	{
		switch (builder.GetIntersectionType(this))
		{
			case IntersectionBuilder.IntersectionType.NONE:
				return IntersectionGraphicsType.NONE;
			case IntersectionBuilder.IntersectionType.ONE_WAY:
				return IntersectionGraphicsType.ONE_WAY_WRAP;
			case IntersectionBuilder.IntersectionType.TWO_WAY:
				switch (builder.GetTwoWayIntersectionType(this))
				{
					case IntersectionBuilder.TwoWayIntersectionType.SKEW:
						return IntersectionGraphicsType.TWO_WAY_SKEW;
					case IntersectionBuilder.TwoWayIntersectionType.SMOOTH:
						return IntersectionGraphicsType.TWO_WAY_SMOOTH;
					case IntersectionBuilder.TwoWayIntersectionType.TRANSITION:
						return IntersectionGraphicsType.TWO_WAY_TRANSITION;
					default:
						throw new ArgumentOutOfRangeException();
				}
				break;
			case IntersectionBuilder.IntersectionType.MULTI_WAY:
				return IntersectionGraphicsType.MULTI_WAY;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public Vector3 ccoordinateForRoadEnds(Road road, IntersectionBuilder.RightOrLeft ltrt)
	{
		return builder.coordinateForRoadAtIntersection(this, road, ltrt);
	}


}