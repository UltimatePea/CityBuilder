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
			connectedRoads.ForEach (rd => rd.UpdateGameObject (this));
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



}