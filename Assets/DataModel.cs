using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Roads are attached to the from intersection */
public class Road
{
	public Intersection fromIntersection;
	public Intersection toIntersection;

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
			return IntersectionClass.NONE;
		}
	}
}

public class Intersection
{
	private ArrayList connectedRoads;
	private Vector3 _position;

	public Vector3 position {
		get { return _position; }
		set {
			_position = value;
			// TODO update all roads mesh and this's mesh;
			this.gameObject = builder.UpdateIntersection (this, gameObject);
		}
	}

	private GameObject gameObject;
	// the game object we own

	// reference to the global builder
	private IntersectionBuilder builder;

	public Intersection (Vector3 position)
	{
		this.connectedRoads = new ArrayList ();
		this.position = position;

		this.builder = GameObject.FindWithTag ("Builder").GetComponent<IntersectionBuilder> ();
		// TODO Performance optimizaiton: this always build an empty game object
		this.gameObject = this.builder.BuildIntersection (this);

	}

	public Road[] getConnectedRoads ()
	{
		return this.connectedRoads.ToArray () as Road[];
	}
}

public class DataModel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
