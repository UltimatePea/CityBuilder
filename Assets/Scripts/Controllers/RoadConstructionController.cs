using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadConstructionController : MonoBehaviour
{


	private Intersection startIntersection;
	private Intersection endIntersection;

	private GameObject temp;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {

			// record start position on mouse press
			this.startIntersection = new Intersection (getMousePositionOnPlane ());

		} else if (Input.GetMouseButton (0) || Input.GetMouseButtonUp (0)) {

			// create and destroy tempRoad on mouse drag and mouse release
			if (this.endIntersection == null) {
				this.endIntersection = new Intersection (getMousePositionOnPlane ());
			} else {
				this.endIntersection.position = getMousePositionOnPlane ();
			}
		}

		// temp road becomes permanent when the user releases the mouse
		if (Input.GetMouseButtonUp (0)) {
			new Road (this.startIntersection, this.endIntersection);
			this.startIntersection = null;
			this.endIntersection = null;
		}


	}

	private Vector3 getMousePositionOnPlane ()
	{

		// Drag initiated
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hitInfo = new RaycastHit ();
		if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			Vector3 point = hitInfo.point;
			return point;
		}

		// TODO :: HANDLE LOGIC WHEN NOT HIT
		return Vector3.zero;
	}

}
