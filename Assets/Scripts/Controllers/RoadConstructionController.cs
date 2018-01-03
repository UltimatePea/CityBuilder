using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadConstructionController : MonoBehaviour
{

	public IntersectionManager intersecManager;

	private Intersection startIntersection;



	void Update ()
	{
		GameObject obj;
		Vector3 position = getMousePositionOnPlane (out obj);
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
					this.intersecManager.connectTwoIntersections (temporary, this.startIntersection);
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
					if (this.intersecManager.canConnectTwoIntersections (intersectionOnHit, this.startIntersection)) {
						this.intersecManager.createTemporaryConnection (intersectionOnHit, 
							this.startIntersection);
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
						this.intersecManager.createTemporaryConnection (this.startIntersection, intersectionOnHit);
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
			Debug.LogFormat ("start = {0}, end = {1}", this.startIntersection, endIntersection);
			intersecManager.removeTemporaryRoadIfThereIsAny ();
			intersecManager.removeTemporaryIntersectionIfThereIsAny ();
			this.intersecManager.connectTwoIntersections (this.startIntersection, endIntersection);
			this.startIntersection = null;
		}


	}

	private Vector3 getMousePositionOnPlane (out GameObject tag)
	{

		// Drag initiated
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hitInfo = new RaycastHit ();
		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, ~(1 << GlobalLayers.IgnoreRaycast))) {
			Vector3 point = hitInfo.point;
			tag = hitInfo.transform.gameObject;
			return point;
		}
		tag = null;

		// TODO :: HANDLE LOGIC WHEN NOT HIT
		return Vector3.zero;
	}

}
