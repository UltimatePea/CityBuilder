using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class IntersectionBuilder : MonoBehaviour
{

	public GameObject fourWayIntersectionPrefab;
	public GameObject oneWayIntersectionPrefab;


	// this method will not change intersectionObject and returns an updated game object
	// intersectionObject must be the one returned by a previous call to BuildIntersection
	public GameObject UpdateIntersection (Intersection intersection, GameObject intersectionObject)
	{
		Destroy (intersectionObject);
		return BuildIntersection (intersection);
	}


	// build a single intersection, returns the created game object
	public GameObject BuildIntersection (Intersection intersection)
	{
		Road[] roads = intersection.getConnectedRoads ();
		// no intersection if no roads
		if (roads.Length == 0) {
//			return Instantiate (oneWayIntersectionPrefab, intersection.position, Quaternion.identity);
			return new GameObject ("intersection");
		} else 

		// this is one way intersection
		if (roads.Length == 1) {
			return buildOneWayIntersection (intersection, roads [0]);
		} else {
			return buildMultiwayIntersection (intersection, roads);
		}
		
	}

	private GameObject buildOneWayIntersection (Intersection intersection, Road connectedRoad)
	{
		float roadEndWidth = 1f;
		float roadEndLength = roadEndWidth / 2;

		Mesh mesh = new Mesh ();

		/**
		 * Vertices are arranged along the diagonal shaped
		 */
		Vector3[] vertices = {
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 0) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 0) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 1) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 1) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 2) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 2) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 3) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 3) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 4) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 4) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 5) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 5) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 6) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 6) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 7) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 7) * roadEndWidth / 2), 
			Vector3.zero, 
			new Vector3 (Mathf.Cos (Mathf.PI / 2 - Mathf.PI / 8 * 8) * roadEndLength, 0, Mathf.Sin (Mathf.PI / 2 - Mathf.PI / 8 * 8) * roadEndWidth / 2), 
		};
		mesh.vertices = vertices;


		/**
		 * UV:
		 * 0 2 4 6 8 10 12 14 16
		 *
		 *  1 3 5 7 9 11 13 15 
		 * 
		 */

		Vector2[] uv = {
			new Vector2 (1f / 8 * 0, 1f / 1 * 0), // 1
			new Vector2 (1f / 8 * 0 + 1f / 8 / 2, 1f / 1 * 1), // 1
			new Vector2 (1f / 8 * 1, 1f / 1 * 0), // 2 
			new Vector2 (1f / 8 * 1 + 1f / 8 / 2, 1f / 1 * 1), // 2 
			new Vector2 (1f / 8 * 2, 1f / 1 * 0), // 3 
			new Vector2 (1f / 8 * 2 + 1f / 8 / 2, 1f / 1 * 1), // 3 
			new Vector2 (1f / 8 * 3, 1f / 1 * 0), // 4
			new Vector2 (1f / 8 * 3 + 1f / 8 / 2, 1f / 1 * 1), // 4
			new Vector2 (1f / 8 * 4, 1f / 1 * 0), // 5
			new Vector2 (1f / 8 * 4 + 1f / 8 / 2, 1f / 1 * 1), // 5
			new Vector2 (1f / 8 * 5, 1f / 1 * 0), // 6
			new Vector2 (1f / 8 * 5 + 1f / 8 / 2, 1f / 1 * 1), // 6
			new Vector2 (1f / 8 * 6, 1f / 1 * 0), // 7
			new Vector2 (1f / 8 * 6 + 1f / 8 / 2, 1f / 1 * 1), // 7
			new Vector2 (1f / 8 * 7, 1f / 1 * 0), // 8
			new Vector2 (1f / 8 * 7 + 1f / 8 / 2, 1f / 1 * 1), // 8
			new Vector2 (1f / 8 * 8, 1f / 1 * 0), // 9
		};
		mesh.uv = uv;

		int[] triangles = new int[8 * 3];
		for (int i = 0; i < 8; i++) {
			triangles [i * 3 + 0] = 2 * i + 1;
			triangles [i * 3 + 1] = 2 * i; 
			triangles [i * 3 + 2] = 2 * i + 2;
		}
		// triangles start from zero
		mesh.triangles = triangles;

		Vector3[] normals = new Vector3[vertices.Length];
		for (int i = 0; i < normals.Length; i++) {
			normals [i] = Vector3.up; 
		}
		mesh.normals = normals;


		/* create object */

		Vector3 position;
		Quaternion orientation;

		// figure out position and orientation
		switch (connectedRoad.typeOfIntersection (intersection)) {
		case Road.IntersectionClass.FROM:
			position = connectedRoad.fromIntersection.position;
			orientation = Quaternion.FromToRotation (Vector3.right, 
				connectedRoad.fromIntersection.position - connectedRoad.toIntersection.position);
			break;
		case Road.IntersectionClass.TO:
			position = connectedRoad.toIntersection.position;
			orientation = Quaternion.FromToRotation (Vector3.right,
				connectedRoad.toIntersection.position - connectedRoad.fromIntersection.position);
			break;
		case Road.IntersectionClass.NONE:
		default:
			// this shouldnever happen
			Debug.LogError ("Intersection are not connected to road, but road connected to intersection");
			position = Vector3.zero;
			orientation = Quaternion.identity;
			break;
		}


		// move up a little bit
		position = position + new Vector3 (0, 0.01f, 0);

		GameObject intersectionObj = Instantiate (oneWayIntersectionPrefab, position, orientation);
		MeshFilter objMeshFilter = intersectionObj.GetComponent<MeshFilter> ();
		objMeshFilter.mesh = mesh;

		return intersectionObj;
	}

	private GameObject buildMultiwayIntersection (Intersection intersection, Road[] connectedRoads)
	{
		// TODO

		return null;
	}
}
