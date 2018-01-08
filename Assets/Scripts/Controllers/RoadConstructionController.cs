using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadConstructionController : MonoBehaviour
{

	public IntersectionManager intersecManager;
	public float roadWidth;

	private Intersection startIntersection;


	void SetRoadWidth(){
		if(Input.GetKeyDown(KeyCode.Alpha1)){ roadWidth = 1; } else 
		if(Input.GetKeyDown(KeyCode.Alpha2)){ roadWidth = 2; } else 
		if(Input.GetKeyDown(KeyCode.Alpha3)){ roadWidth = 3; } else 
		if(Input.GetKeyDown(KeyCode.Alpha4)){ roadWidth = 4; } else 
		if(Input.GetKeyDown(KeyCode.Alpha5)){ roadWidth = 5; }
		
	}

	void Update ()
	{
		SetRoadWidth();
		GameObject obj;
		Vector3 position = MouseCollisionDetection.getMousePositionOnPlane (out obj);
		if (Input.GetMouseButtonDown (0)) {

			Debug.Log (obj.tag);
			// record start position on mouse press

			if (obj.tag == GlobalTags.Ground) {
				this.startIntersection = intersecManager.createIntersection (position);
			} else if (obj.tag == GlobalTags.Intersection) {
				this.startIntersection = intersecManager.intersectionForGameObject (obj);
			} else {
				Debug.LogWarningFormat ("Unexpected ray hit on {0} with tag {1}", obj, obj.tag);
			}

		} else if (Input.GetMouseButton (0)) {

//			// create and destroy tempRoad on mouse drag and mouse release
			if (obj.tag == GlobalTags.Ground) {
				if (!intersecManager.temporaryIntersectionExists ()) {
					// no temporary intersection exists
					intersecManager.removeTemporaryRoadIfThereIsAny ();
					// create temporary if there is not any
					Intersection temporary = intersecManager.createTemporaryIntersection (position);
					// if the intersection is temporary, road could be non-temporary
					if (this.intersecManager.canConnectTwoIntersections (temporary, this.startIntersection, roadWidth)) {
						// conditional check to prevent road length to be zero( thus a physics error)
						// would not be a problem if we switched to sphere
						this.intersecManager.createTemporaryConnection (temporary, this.startIntersection, roadWidth);
					} else {
						// we could not create connection here, remove the temporary
						this.intersecManager.removeTemporaryIntersectionIfThereIsAny ();
					}
				} else {
					// some temporary intersection exists
					// otherwise move the current temporary intersection to the new mouse position
					this.intersecManager.updateTemporaryIntersectionPosition (position);
				}
			} else if (obj.tag == GlobalTags.Intersection) {
				// We've moused on an intersection
				Intersection intersectionOnHit = intersecManager.intersectionForGameObject (obj);

				// check if there's a temporary intersection
				if (!intersecManager.temporaryIntersectionExists ()) {
					// there's no temporary intersection, and we've clicked on an existing intersection
					if (this.intersecManager.canConnectTwoIntersections (intersectionOnHit, this.startIntersection, roadWidth)) {
						this.intersecManager.createTemporaryConnection (intersectionOnHit, 
							this.startIntersection, roadWidth);
					} else {
						// TODO: Get failure reason
					}
				} else {
					// there is a temporary intersection, check to see if the we've hit a temporary intersection
					// TODO: We should not hit a temporary intersection with Raycast
					if (intersecManager.isTemporaryIntersectionPresent (intersectionOnHit)) {
						// otherwise move the current temporary intersection to the new mouse position
						this.intersecManager.updateTemporaryIntersectionPosition (position);
						Debug.Assert (false);// we should not hit a temporary intersection with Raycast
					} else {
						// we've on a intersection, and the intersection is not the temporary intersection
						// we remove all temporary intersection
						this.intersecManager.removeTemporaryIntersectionIfThereIsAny ();
						// and we create the temporary connection
						this.intersecManager.createTemporaryConnection (this.startIntersection, intersectionOnHit, roadWidth);
					}
				}
			}
		}

		// temp road becomes permanent when the user releases the mouse
		if (Input.GetMouseButtonUp (0)) {
			Debug.Log (obj.tag);
			Intersection endIntersection = null;
			if (obj.tag == GlobalTags.Ground) {
				endIntersection = intersecManager.createIntersection (position);
			} else if (obj.tag == GlobalTags.Intersection) {
				endIntersection = intersecManager.intersectionForGameObject (obj);
			}
			//Debug.LogFormat ("start = {0}, end = {1}", this.startIntersection, endIntersection);
			intersecManager.removeTemporaryRoadIfThereIsAny ();
			intersecManager.removeTemporaryIntersectionIfThereIsAny ();
			this.intersecManager.connectTwoIntersections (this.startIntersection, endIntersection, roadWidth);
			this.startIntersection = null;
		}


	}



}
