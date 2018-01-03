using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;




public class IntersectionBuilder : MonoBehaviour
{
	private float MATH_3D_COMPENSATION = 100f;
	// compensation for math3d line intersection algorithm
	public GameObject fourWayIntersectionPrefab;
	public GameObject oneWayIntersectionPrefab;


	// this method will not change intersectionObject and returns an updated game object
	// intersectionObject must be the one returned by a previous call to BuildIntersection
	public GameObject UpdateIntersection (Intersection intersection, GameObject intersectionObject)
	{
		Destroy (intersectionObject);
		GameObject newIntersection = BuildIntersection (intersection);
		return newIntersection;
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
		} else if (roads.Length == 2) {
			return buildTwoWayIntersection (intersection, roads [0], roads [1]);
		} else {
			return buildMultiwayIntersection (intersection, roads);
		}
		
	}

	private GameObject buildOneWayIntersection (Intersection intersection, Road connectedRoad)
	{




		float roadEndWidth = connectedRoad.GetRoadWidth ();
		float roadEndLength = roadEndWidth / 2;

		Vector3 outVec = getOutgoingVector (intersection, connectedRoad).normalized * roadEndWidth / 2;
		Vector3 rightVec = Quaternion.Euler (0, 90, 0) * outVec;
		Vector3 leftVec = Quaternion.Euler (0, -90, 0) * outVec;


		GameObject rightObj = buildOuter (rightVec, -outVec, 4, intersection.position, intersection.customization);
		GameObject leftObj = buildOuter (leftVec, -outVec, 4, intersection.position, intersection.customization);

		return createParentIntersectionWithChildIntersections ("Road End Wrap", intersection.customization, rightObj, leftObj);


	}

	private GameObject buildTwoWayIntersection (Intersection intersection, Road road1, Road road2)
	{
		
		Vector3 vec1 = getOutgoingVector (intersection, road1).normalized;
		Vector3 vec2 = getOutgoingVector (intersection, road2).normalized;
		// create a four way intersection instead of a two way intersection if the angle is less than 90
		if (Vector3.Angle (vec1, vec2) < 90) {
			Vector3[] vecs = { vec1, vec2, -vec1, -vec2 };
			float[] roadWidths = { road1.GetRoadWidth (), road2.GetRoadWidth (), road1.GetRoadWidth (), road2.GetRoadWidth () };
			// zip vecs and roadWidths into abstract road infos
			AbstractWayInfo[] infos = AbstractWayInfo.InfosWithDirectionsAndWidths (vecs, roadWidths);
			return buildMultiwayIntersectionWithVectors (intersection, infos);
		}

		//force a four way intersection if two roads are of different width
		if (Mathf.Approximately (road1.GetRoadWidth (), road2.GetRoadWidth ()) == false) {
			// create a halfway rotation
			Quaternion rotation = Quaternion.Euler (Quaternion.FromToRotation (vec1, vec2).eulerAngles / 2);
			// this is the virtual directional vector
			Vector3 newVec = rotation * vec1;
			// this is the virtual road width, set it to be the average of the two
			float newWidth = road1.GetRoadWidth () + road2.GetRoadWidth () / 2;

			Vector3[] vecs = { vec1, vec2, newVec, -newVec };
			float[] roadWidths = { road1.GetRoadWidth (), road2.GetRoadWidth (), newWidth, newWidth };

			// zip vecs and roadWidths into abstract road infos
			AbstractWayInfo[] infos = AbstractWayInfo.InfosWithDirectionsAndWidths (vecs, roadWidths);
			return buildMultiwayIntersectionWithVectors (intersection, infos);


		}

		// only when angle > 90

		float roadWidth1 = road1.GetRoadWidth ();
		float roadWidth2 = road2.GetRoadWidth ();
		Vector3 vec1Norm = Quaternion.Euler (new Vector3 (0, 90, 0)) * vec1 * roadWidth1 / 2;
		Vector3 vec2Norm = Quaternion.Euler (new Vector3 (0, -90, 0)) * vec2 * roadWidth2 / 2;
		// check the intersection, if there's its inner, if there isn't, its outer
		Vector3 refPoint;
		GameObject inner;
		GameObject outer;
		if (Math3d.LineLineIntersection (out refPoint, vec1Norm, vec1 * MATH_3D_COMPENSATION, vec2Norm, vec2 * MATH_3D_COMPENSATION)) {
			// we intersected, its inner
			inner = buildInner (vec1Norm, vec2Norm, refPoint, intersection.position, intersection.customization);
			outer = buildOuter (-vec1Norm, -vec2Norm, 4, intersection.position, intersection.customization);
		} else {
			// we are outer 
			outer = buildOuter (vec1Norm, vec2Norm, 4, intersection.position, intersection.customization);
			// get the real intersection
			// we need to * a const since the Math scirpt only considers line to be that long
			if (Math3d.LineLineIntersection (out refPoint, -vec1Norm, vec1 * MATH_3D_COMPENSATION, -vec2Norm, vec2 * MATH_3D_COMPENSATION) == false) {
				Debug.LogError ("Error creating intersection, Lines do not intersect");
				// BUG: TODO: FIXIT
				// BEHAVIOR: two lines intersect on the upper right corner, with less than 45 deg angle between them
				/**	               
				 *                 vvvvv Error Here
				 * ------------------
				 *                +
				 *             +
				 *         +
				 *      +
				 *   +
				 * 
				 */
			}
			inner = buildInner (-vec1Norm, -vec2Norm, refPoint, intersection.position, intersection.customization);

		}
		// add rigidbody to enable selection
		return createParentIntersectionWithChildIntersections ("Two way intersection", intersection.customization, inner, outer);
	}

	private GameObject createParentIntersectionWithChildIntersections (string name, IntersectionCustomization customization, params GameObject[] childs)
	{
		GameObject obj = new GameObject (name, typeof(Rigidbody));
		obj.tag = GlobalTags.Intersection;
		obj.layer = customization.isTemporary ? GlobalLayers.IgnoreRaycast : GlobalLayers.Default;
		foreach (GameObject c in childs) {

			c.transform.parent = obj.transform;
			c.tag = GlobalTags.Untagged;
			c.layer = customization.isTemporary ? GlobalLayers.IgnoreRaycast : GlobalLayers.Default;
		}
		// Kinematic to prevent gravity
		obj.GetComponent<Rigidbody> ().isKinematic = true;
		return obj;
		
	}

	// this builds a quadrilateral shaped mesh, with four points
	/**
	 *     r
	 * 
	 * 
	 * 
	 * f       t
	 * 
	 *     z
	 * 
	 * r -- refpoint
	 * f -- frompoint
	 * t -- topoint
	 * z -- Vector3.zero
	 */
	private GameObject buildInner (Vector3 fromPoint, Vector3 toPoint, Vector3 refPoint, Vector3 position, IntersectionCustomization customization)
	{
		// swap from to ponit if necessary
		if (Vector3.Cross (fromPoint, toPoint).y < 0) {
			Vector3 temp = fromPoint;
			fromPoint = toPoint;
			toPoint = temp;
		}

		Mesh mesh = new Mesh ();
		Vector3[] vertices = {
			refPoint, 
			fromPoint,
			toPoint, 
			Vector3.zero
		};
		int[] triangles = {
			0, 2, 3,
			3, 1, 0
		};
		Vector3[] normals = {
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		return createGameObjectWithPrefabPositionAndMesh (oneWayIntersectionPrefab, position, mesh, customization);
		
	}

	// smoothness : number of triangles to create
	private GameObject buildOuter (Vector3 fromPoint, Vector3 toPoint, int smoothness, Vector3 position, IntersectionCustomization customization)
	{
		// swap from to ponit if necessary
		if (Vector3.Cross (fromPoint, toPoint).y < 0) {
			Vector3 temp = fromPoint;
			fromPoint = toPoint;
			toPoint = temp;
		}

		// Outer is mimicking to one way road end, 
		// TODO delete one way road end, use this method

		// DEBUG HINT: 
		/**
		 *  e.g. s = 3
		 * 
		 * 0  2   4
		 * |  /  +
		 * | / +
		 * |/+------ 6
		 * 1 3 5
		 * 
		 * Triangles : 0 2 1, 2 4 3, 4 6 5
         * 
         * 
         * UV:
         * 0 2 4 6
		 * 
		 *  1 3 5
		 * 
		 * 
		 */

		Mesh mesh = new Mesh ();

		Vector3[] vertices = new Vector3[2 * smoothness + 1];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector3[] normals = new Vector3[vertices.Length];
		for (int i = 0; i < smoothness + 1; i++) {
			vertices [2 * i] = Vector3.Slerp (fromPoint, toPoint, (float)i / smoothness);
			uv [2 * i] = Vector2.Lerp (new Vector2 (0, 0), new Vector2 (1, 0), (float)i / smoothness);
			normals [2 * i] = Vector3.up;
			if (i != smoothness) {
				vertices [2 * i + 1] = Vector3.zero;
				uv [2 * i + 1] = Vector2.Lerp (new Vector2 (0, 1), new Vector2 (1, 1), (float)i / smoothness)
				+ new Vector2 (1f, 0) / smoothness / 2;
				normals [2 * i + 1] = Vector3.up;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.normals = normals;

		int[] triangles = new int[smoothness * 3];
		for (int i = 0; i < smoothness; i++) {
			triangles [3 * i] = 2 * i;
			triangles [3 * i + 1] = 2 * i + 2;
			triangles [3 * i + 2] = 2 * i + 1;
		}
		mesh.triangles = triangles;



		GameObject sectorRoad = createGameObjectWithPrefabPositionAndMesh (oneWayIntersectionPrefab, position, mesh, customization);
		// the outer is lower than normal intersection, same as roads
		sectorRoad.transform.Translate (new Vector3 (0, -0.01f, 0));
		return sectorRoad;
	}

	private GameObject buildMultiwayIntersection (Intersection intersection, Road[] connectedRoads)
	{
		// convert roads to vectos
		Vector3[] vectors = connectedRoads.Select (rd => getOutgoingVector (intersection, rd)).ToArray ();
		float[] widths = connectedRoads.Select (rd => rd.GetRoadWidth ()).ToArray ();


		// handle when connectedRoads.Length == 3  << build a four-way intersection instead
		if (connectedRoads.Length == 3) {
			// find the vector whose direction is of greatest difference to other two

			Vector3 maxVector = vectors [0];
			float maxRoadWidth = widths [0];
			float maxAngles = 0; 
			for (int i = 0; i < 3; i++) {
				float accum = 0;
				for (int j = 0; j < 3; j++) {
					float zeroTo180Angle = Vector3.Angle (vectors [i], vectors [j]);
					float distance = zeroTo180Angle > 90 ? 180 - zeroTo180Angle : zeroTo180Angle;
					accum += distance;
				}
				if (accum > maxAngles) {
					maxVector = vectors [i];
					maxRoadWidth = widths [i];
					maxAngles = accum;
				}
				
			}

			Vector3[] resultingVectors = { vectors [0], vectors [1], vectors [2], -maxVector };
			float[] resultingWidths = { widths [0], widths [1], widths [2], maxRoadWidth };

			var abstractWayInfos = AbstractWayInfo.InfosWithDirectionsAndWidths (resultingVectors, resultingWidths);
			return buildMultiwayIntersectionWithVectors (intersection, abstractWayInfos);
			
		}

		AbstractWayInfo[] infos = AbstractWayInfo.InfosWithDirectionsAndWidths (vectors, widths);
		return buildMultiwayIntersectionWithVectors (intersection, infos);
	}

	private class AbstractWayInfo
	{
		public Vector3 direction;
		public float roadWidth;

		public AbstractWayInfo (Vector3 direction, float roadWidth)
		{
			this.direction = direction;
			this.roadWidth = roadWidth;
		}

		static public AbstractWayInfo[] InfosWithDirectionsAndWidths (Vector3[] vecs, float[] roadWidths)
		{
			var numElems = vecs.Length <= roadWidths.Length ? vecs.Length : roadWidths.Length;
			AbstractWayInfo[] infos = new AbstractWayInfo[numElems];

			for (int i = 0; i < numElems; i++) { 
				infos [i] = new AbstractWayInfo (vecs [i], roadWidths [i]);
			}
			return infos;
		}


	}


	private GameObject buildMultiwayIntersectionWithVectors (Intersection intersection, AbstractWayInfo[] outgoingWays)
	{
		// sort vectors according to orientations first
		AbstractWayInfo[] sorted = outgoingWays
			.OrderBy (way => Quaternion.FromToRotation (Vector3.right, way.direction).eulerAngles.y)
			.ToArray ();
		Mesh mesh = new Mesh ();

		// get a list of intersection vertices
		Vector3[] vertices = new Vector3[sorted.Length + 2];
		for (int i = 0; i < sorted.Length; i++) {
			AbstractWayInfo rd1 = sorted [i];
			AbstractWayInfo rd2 = sorted [(i + 1) % sorted.Length];
			// get intersection of rd1's right edge with rd2's left edge
			Vector3 vec1 = rd1.direction.normalized;
			Vector3 vec2 = rd2.direction.normalized;
			Vector3 vec1Norm = Quaternion.Euler (new Vector3 (0, 90, 0)) * vec1 * rd1.roadWidth / 2;
			Vector3 vec2Norm = Quaternion.Euler (new Vector3 (0, -90, 0)) * vec2 * rd2.roadWidth / 2;

			// TODO : 
			bool res = Math3d.LineLineIntersection (out vertices [i], vec1Norm, vec1 * MATH_3D_COMPENSATION, vec2Norm, vec2 * MATH_3D_COMPENSATION);
			if (res == false) {
				res = Math3d.LineLineIntersection (out vertices [i], vec1Norm, -vec1 * MATH_3D_COMPENSATION, vec2Norm, -vec2 * MATH_3D_COMPENSATION);
				if (res == false) {
					Debug.LogWarning ("Error when creating intersection: Line intersection does not exist");
				}
			}
			Debug.LogFormat ("vec1 = {2}, vec2 = {3}, Vertex ({0}) : {1}", i, vertices [i], vec1, vec2);
		}
		vertices [sorted.Length] = vertices [0]; // the second last vertex is the first (for UV)
		vertices [sorted.Length + 1] = Vector3.zero; // the last vertex is the intersection center
		Debug.LogFormat ("Vertex ({0}) : {1}", sorted.Length, vertices [sorted.Length]);
		mesh.vertices = vertices;

		int[] triangles = new int[outgoingWays.Length * 3];
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



		var intersectionObj = createGameObjectWithPrefabPositionAndMesh (fourWayIntersectionPrefab, intersection.position, mesh, intersection.customization);


		return intersectionObj;

	}


	// this method will elevate intersection
	private GameObject createGameObjectWithPrefabPositionAndMesh (GameObject prefab, Vector3 position, Mesh mesh, IntersectionCustomization customization)
	{
		// move up a little bit
		// TODO Fix magic numbers
		Vector3 elevatedPos = position + new Vector3 (0, 0.02f, 0);

		GameObject intersectionObj = Instantiate (prefab, elevatedPos, Quaternion.identity);
		MeshFilter objMeshFilter = intersectionObj.GetComponent<MeshFilter> ();
		objMeshFilter.mesh = mesh;
		MeshCollider objMeshCollider = intersectionObj.GetComponent<MeshCollider> ();
		objMeshCollider.sharedMesh = mesh;
		intersectionObj.layer = customization.isTemporary ? GlobalLayers.IgnoreRaycast : GlobalLayers.Default;
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

