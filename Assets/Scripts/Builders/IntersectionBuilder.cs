using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;




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
			if (roads.Length == 1 /* temp */ || roads.Length == 2 /* temp */) {
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

		MeshCollider objMeshCollider = intersectionObj.GetComponent<MeshCollider> ();
		objMeshCollider.sharedMesh = mesh;

		return intersectionObj;
	}


	private GameObject buildMultiwayIntersection (Intersection intersection, Road[] connectedRoads)
	{
		// get an angle of all roads
		Road[] sorted = connectedRoads.OrderBy (rd => getAngle (intersection, rd).eulerAngles.y).ToArray ();

		Mesh mesh = new Mesh ();

		// get a list of intersection vertices
		Vector3[] vertices = new Vector3[sorted.Length + 2];
		float roadWidth = 1f;
		for (int i = 0; i < sorted.Length; i++) {
			Road rd1 = sorted [i];
			Road rd2 = sorted [(i + 1) % sorted.Length];
			// get intersection of rd1's right edge with rd2's left edge
			Vector3 vec1 = getOutgoingVector (intersection, rd1).normalized;
			Vector3 vec2 = getOutgoingVector (intersection, rd2).normalized;
			Vector3 vec1Norm = Quaternion.Euler (new Vector3 (0, 90, 0)) * vec1 * roadWidth / 2;
			Vector3 vec2Norm = Quaternion.Euler (new Vector3 (0, -90, 0)) * vec2 * roadWidth / 2;

			bool res = Math3d.ClosestPointsOnTwoLines (out vertices [i], out vertices [i], vec1Norm, vec1, vec2Norm, vec2);
			if (res == false) {
				Debug.LogWarning ("Error when creating intersection: Line intersection does not exist");
			}
			Debug.LogFormat ("vec1 = {2}, vec2 = {3}, Vertex ({0}) : {1}", i, vertices [i], vec1, vec2);
		}
		vertices [sorted.Length] = vertices [0]; // the second last vertex is the first (for UV)
		vertices [sorted.Length + 1] = Vector3.zero; // the last vertex is the intersection center
		Debug.LogFormat ("Vertex ({0}) : {1}", sorted.Length, vertices [sorted.Length]);
		mesh.vertices = vertices;

		int[] triangles = new int[connectedRoads.Length * 3];
		// TODO : Figure out how sortBy works, thus affecting the orientation of the plane
		for (int i = 0; i < sorted.Length; i++) {
			triangles [i * 3 + 2] = i;
			triangles [i * 3 + 1] = sorted.Length + 1;
			triangles [i * 3 + 0] = i + 1;
		}
		mesh.triangles = triangles;

		Vector2[] uv = new Vector2[vertices.Length];
		for (int i = 0; i < sorted.Length + 1; i++) {
			switch (i % 4) {
			case 0:
				uv [i] = new Vector2 (0, 0);
				break;
			case 1:
				uv [i] = new Vector2 (0, 1);
				break;
			case 2:
				uv [i] = new Vector2 (1, 1);
				break;
			case 3:
				uv [i] = new Vector2 (1, 0);
				break;
			}
		}
		uv [sorted.Length + 1] = new Vector2 (0.5f, 0.5f);
		mesh.uv = uv;

		Vector3[] normals = new Vector3[uv.Length];
		for (int i = 0; i < normals.Length; i++) {
			normals [i] = Vector3.up;
		}
		mesh.normals = normals;


		// move up a little bit
		// TODO Fix magic numbers
		Vector3 position = intersection.position + new Vector3 (0, 0.02f, 0);

		GameObject intersectionObj = Instantiate (fourWayIntersectionPrefab, position, Quaternion.identity);
		MeshFilter objMeshFilter = intersectionObj.GetComponent<MeshFilter> ();
		objMeshFilter.mesh = mesh;

		MeshCollider objMeshCollider = intersectionObj.GetComponent<MeshCollider> ();
		objMeshCollider.sharedMesh = mesh;

		return intersectionObj;
	}

	// TODO Combine below two methods
	Vector3 getOutgoingVector (Intersection intersection, Road road)
	{
		Vector3 vec = Vector3.zero;
		switch (road.typeOfIntersection (intersection)) {
		case Road.IntersectionClass.FROM:
			vec = road.toIntersection.position - road.fromIntersection.position;
			break;
		case Road.IntersectionClass.TO:
			vec = road.fromIntersection.position - road.toIntersection.position;
			break;
		}
	
		return vec;
		
	}

	Quaternion getAngle (Intersection intersection, Road road)
	{
		Quaternion angle = Quaternion.identity;
		switch (road.typeOfIntersection (intersection)) {
		case Road.IntersectionClass.FROM:
			angle = Quaternion.FromToRotation (Vector3.right, road.toIntersection.position - road.fromIntersection.position);
			break;
		case Road.IntersectionClass.TO:
			angle = Quaternion.FromToRotation (Vector3.right, road.fromIntersection.position - road.toIntersection.position);
			break;
		}
	
		return angle;
		
	}
}
