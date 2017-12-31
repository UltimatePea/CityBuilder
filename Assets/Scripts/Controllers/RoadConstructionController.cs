using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadConstructionController : MonoBehaviour
{


	private Intersection startIntersection;
	private Intersection endIntersection;

	void Update ()
	{
		GameObject obj;
		Vector3 position = getMousePositionOnPlane (out obj);
		if (Input.GetMouseButtonDown (0)) {

			Debug.Log (obj.tag);
			// record start position on mouse press

			if (obj.tag == "Ground") {
				this.startIntersection = new Intersection (position);
			} else if (obj.tag == "Intersection") {
				this.startIntersection = Intersection.IntersectionForGameObject (obj);
			}

		} else if (Input.GetMouseButton (0) || Input.GetMouseButtonUp (0)) {

//			// create and destroy tempRoad on mouse drag and mouse release
//			if (obj.tag == "Plane") {
//				if (this.endIntersection == null) {
//					this.endIntersection = new Intersection (position);
//					this.startIntersection.connectToIntersection (this.endIntersection);
//				} else {
//					if (this.endIntersection.getConnectedRoads ().Length == 1) {
//						this.endIntersection.position = position;
//					} else {
//						this.startIntersection.RemoveConnectionTo (this.endIntersection);
//						this.endIntersection = new Intersection (position);
//						this.startIntersection.connectToIntersection (this.endIntersection);
//					}
//				}
//			} else if (obj.tag == "Intersection") {
//				if (this.endIntersection == null) {
//					this.endIntersection = Intersection.IntersectionForGameObject (obj);
//					this.startIntersection.connectToIntersection (this.endIntersection);
//				} else {
//					if (this.endIntersection.getConnectedRoads ().Length == 1) {
//						// we raycasted onto our own road
//						if(this.endIntersection.getConnectedRoads()[0].otherIntersection(this.endIntersection) == this.startIntersection){
//							this.endIntersection
//							
//						}
//						
//					}
//				}
//			}
		}

		// temp road becomes permanent when the user releases the mouse
		if (Input.GetMouseButtonUp (0)) {
			Debug.Log (obj.tag);
			if (obj.tag == "Ground") {
				this.endIntersection = new Intersection (position);
			} else if (obj.tag == "Intersection") {
				this.endIntersection = Intersection.IntersectionForGameObject (obj);
			}
			this.startIntersection.connectToIntersection (this.endIntersection);
			this.startIntersection = null;
			this.endIntersection = null;
		}


	}

	private Vector3 getMousePositionOnPlane (out GameObject tag)
	{

		// Drag initiated
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hitInfo = new RaycastHit ();
		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			Vector3 point = hitInfo.point;
			tag = hitInfo.collider.gameObject;
			return point;
		}
		tag = null;

		// TODO :: HANDLE LOGIC WHEN NOT HIT
		return Vector3.zero;
	}

}
