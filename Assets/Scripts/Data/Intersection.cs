using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Intersection
{
	private List<Road> connectedRoads;
	private Vector3 _position;

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

	public Intersection (Vector3 position)
	{
		this.connectedRoads = new List<Road> ();
		_position = position;

		this.builder = GameObject.FindWithTag ("Builder").GetComponent<IntersectionBuilder> ();
		// TODO Performance optimizaiton: this always build an empty game object
		this.gameObject = this.builder.BuildIntersection (this);

		allInstances.Add (this);

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
	}

	public void connectToIntersection (Intersection intersection)
	{
		new Road (this, intersection);
	}

	public void DeleteIntersection ()
	{
		this.connectedRoads.ForEach (rd => rd.DeleteRoad ());
		GameObject.Destroy (this.gameObject);
		allInstances.Remove (this);
		
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

	private static List<Intersection> allInstances;

	static Intersection ()
	{
		allInstances = new List<Intersection> ();
	}

	public static Intersection IntersectionForGameObject (GameObject gameObject)
	{
		foreach (Intersection inter in allInstances) {
			if (inter.gameObject == gameObject) {
				return inter;
			}
		}
		return null;
	}

}