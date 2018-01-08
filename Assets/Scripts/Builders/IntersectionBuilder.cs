using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;




public partial class IntersectionBuilder : MonoBehaviour
{
	// compensation for math3d line intersection algorithm
	public GameObject fourWayIntersectionPrefab;
	public GameObject oneWayIntersectionPrefab;

	public IntersectionCustomizationBuilder customizationBuilder;
	
	public enum IntersectionType { NONE, ONE_WAY, TWO_WAY, MULTI_WAY}

	public IntersectionType GetIntersectionType(Intersection intersection)
	{
		Road[] roads = intersection.getConnectedRoads ();
		if (roads.Length == 0)
		{
			return IntersectionType.NONE;
		} else if (roads.Length == 1)
		{
			return IntersectionType.ONE_WAY;
		} else if (roads.Length == 2)
		{
			return IntersectionType.TWO_WAY;
		} else
		{
			return IntersectionType.MULTI_WAY;
		}
	}
	
	/************************
	 * Coordinate Calculation
	 **********************************/


	
	/**
	 * Returns the coordinate for the left edge and right edge of the road.
	 *
	 *
	 * The behavior is undefined when road is not attached to the intersection.
	 * TODO: Throw an exception if this is the case (exp one way)
	 *
	 *
	 * Right or left is regarding the direction going out from intersection
	 * 
	 *
	 * rtlt: Right : the point on the right of the outgoing vector
	 * 		 Left : the point on the left of the outgoing vector
	 * 		Note: This direction is the reverse of what traditionally known as left or right
	 * 
	 */
	public enum RightOrLeft { RIGHT, LEFT }
	public Vector3 coordinateForRoadAtIntersection (Intersection intersection, Road road, RightOrLeft rtlt)
	{
		Road[] roads = intersection.getConnectedRoads ();
		// no intersection if no roads
		switch (GetIntersectionType(intersection))
		{
			case IntersectionType.NONE:
				return Vector3.zero;
			case IntersectionType.ONE_WAY:
				return CoordinateForOneWayIntersection(intersection, road, rtlt);
			case IntersectionType.TWO_WAY:
				return CoordinateForTwoWayIntersection(intersection, road, rtlt);
			case IntersectionType.MULTI_WAY:
				return CoordinateForMultiwayIntersection(intersection, road, rtlt);
		}
		Debug.LogError("Semantics Error, please check");

		return Vector3.zero;
	}

	private Vector3 CoordinateForOneWayIntersection(Intersection intersection, Road connectedRoad , RightOrLeft rtlt)
	{
		Vector3 outVec = getOutgoingVectorForOneWayIntersection(intersection, connectedRoad);
		Vector3 rightVec = Quaternion.Euler (0, 90, 0) * outVec;
		Vector3 leftVec = Quaternion.Euler (0, -90, 0) * outVec;
		switch (rtlt)
		{
				case RightOrLeft.LEFT:
					return leftVec;
				case RightOrLeft.RIGHT:
					return rightVec;
		}

		Debug.LogError("Semantics Error, please check");
		return Vector3.zero;
	}

	private Vector3 CoordinateForTwoWayIntersection(Intersection intersection, Road connectedRoad, RightOrLeft rtlt)
	{
		switch (GetTwoWayIntersectionType(intersection))
		{
		case TwoWayIntersectionType.SKEW:
		case TwoWayIntersectionType.TRANSITION:
			AbstractWayInfo[] infos = AbstractWayInfosForTwoWayIntersection(intersection, intersection.getConnectedRoads()[0],
				intersection.getConnectedRoads()[1]);
			return CoordinateForRoadWithAbstractRoadInfos(intersection, connectedRoad, rtlt, infos);
		case TwoWayIntersectionType.SMOOTH:
			// basically it is the same as one way

			return CoordinateForOneWayIntersection(intersection, connectedRoad, rtlt);
		}

		Debug.LogError("Semantics Error, please check");
		return Vector3.zero;
	}

	private Vector3 CoordinateForMultiwayIntersection(Intersection intersection, Road connectedRoad, RightOrLeft rtlt)
	{
		AbstractWayInfo[] infos = AbstractWayInfosForMultiwayIntersection(intersection, intersection.getConnectedRoads());
		return CoordinateForRoadWithAbstractRoadInfos(intersection, connectedRoad, rtlt, infos);
	}

	private Vector3 CoordinateForRoadWithAbstractRoadInfos(Intersection intersection, Road connectedRoad, RightOrLeft rtlt,
		AbstractWayInfo[] infos)
	{
		infos = AbstractWayInfo.sortAbstractWayInfos(infos);

		// convert the road to an abstract way info
		AbstractWayInfo info = new AbstractWayInfo(IntersectionMath.getOutgoingVector(intersection, connectedRoad).normalized,
			connectedRoad.GetRoadWidth());
		// TODO : CHECK FOR nonexistent index
		int index = 0;
		for (int i = 0; i < infos.Length; i++)
		{
			if (infos[i] == info)
			{
				index = i;
			}
		}

		Vector3[] vertices = MeshVerticesForAbstractWayInfos(infos);

		switch (rtlt)
		{
			case RightOrLeft.LEFT:
				return vertices[index - 1 < 0 ? infos.Length - 1 : index - 1];
			case RightOrLeft.RIGHT:
				return vertices[index];
		}
		Debug.LogError("Semantics Error, please check");

		return Vector3.zero;
	}


	/**********************************************
	 * Intersection Building
	 ************************************************/

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
		GameObject baseObj =  BuildBaseTerrain(intersection);
		GameObject obj2 = customizationBuilder.buildCustomization(baseObj, intersection);
		return obj2;
	}

	private GameObject BuildBaseTerrain(Intersection intersection)
	{
		Road[] roads = intersection.getConnectedRoads();
		// no intersection if no roads
		switch (GetIntersectionType(intersection))
		{
			case IntersectionType.NONE:
				return new GameObject("intersection");
			case IntersectionType.ONE_WAY:
				return buildOneWayIntersection(intersection, roads[0]);
			case IntersectionType.TWO_WAY:
				return buildTwoWayIntersection(intersection, roads[0], roads[1]);
			case IntersectionType.MULTI_WAY:
				return buildMultiwayIntersection(intersection, roads);
		}

		Debug.LogError("Semantics Error, please check");

		return null;
	}

	private GameObject buildOneWayIntersection (Intersection intersection, Road connectedRoad)
	{


		/**
		 *
		 * ================================= ^                   
		 *                                   | << rightVec        
		 *                                   |                    
		 *                     vv outVec     |             
		 * --------------------- <----------  -------->  - outVec
		 *                                   |                    
		 *                                   |                    
		 *                                   | << leftVec         
		 * ================================= v                  
		 *
		 *
		 * 
		 */
		Vector3 outVec = getOutgoingVectorForOneWayIntersection(intersection, connectedRoad);
		Vector3 rightVec = CoordinateForOneWayIntersection(intersection, connectedRoad, RightOrLeft.RIGHT);
		Vector3 leftVec = CoordinateForOneWayIntersection(intersection, connectedRoad, RightOrLeft.LEFT);


		GameObject rightObj = buildOuter (rightVec, -outVec, 4, intersection.position, intersection.customization);
		GameObject leftObj = buildOuter (leftVec, -outVec, 4, intersection.position, intersection.customization);

		return createParentIntersectionWithChildIntersections ("Road End Wrap", intersection.customization, rightObj, leftObj);


	}

	private Vector3 getOutgoingVectorForOneWayIntersection(Intersection intersection, Road connectedRoad)
	{
		float roadEndWidth = connectedRoad.GetRoadWidth ();
		float roadEndLength = roadEndWidth / 2;
		return IntersectionMath.getOutgoingVector (intersection, connectedRoad).normalized * roadEndWidth / 2;
	}
	
	public enum TwoWayIntersectionType { SKEW /* Sharop turn */ , SMOOTH /* Change road width */, 
		TRANSITION /* just a curvature */}

	public TwoWayIntersectionType GetTwoWayIntersectionType(Intersection intersection)
	{
		Road road1 = intersection.getConnectedRoads()[0];
		Road road2 = intersection.getConnectedRoads()[1];
		
		Vector3 vec1 = IntersectionMath.getOutgoingVector(intersection, road1).normalized;
		Vector3 vec2 = IntersectionMath.getOutgoingVector(intersection, road2).normalized;
		// create a four way intersection instead of a two way intersection if the angle is less than 90
		if (Vector3.Angle(vec1, vec2) < 90)
		{
			return TwoWayIntersectionType.SKEW;
		}
		else //force a four way intersection if two roads are of different width
		if (Mathf.Approximately(road1.GetRoadWidth(), road2.GetRoadWidth()) == false)
		{
			return TwoWayIntersectionType.TRANSITION;
		}
		else
		{
			return TwoWayIntersectionType.SMOOTH;
		}
	}


	private AbstractWayInfo[] AbstractWayInfosForTwoWayIntersection(Intersection intersection, Road road1, Road road2)
	{
		Vector3 vec1 = IntersectionMath.getOutgoingVector(intersection, road1).normalized;
		Vector3 vec2 = IntersectionMath.getOutgoingVector(intersection, road2).normalized;
		// create a four way intersection instead of a two way intersection if the angle is less than 90
		Vector3[] vecs = null;
		float[] roadWidths = null;
		switch (GetTwoWayIntersectionType(intersection))
		{
			case TwoWayIntersectionType.SKEW:

			
				vecs = new[] {vec1, vec2, -vec1, -vec2};
				roadWidths = new[] {road1.GetRoadWidth(), road2.GetRoadWidth(), road1.GetRoadWidth(), road2.GetRoadWidth()};
				break;

			case TwoWayIntersectionType.TRANSITION:
				// create a halfway rotation
				Quaternion rotation = Quaternion.Euler(Quaternion.FromToRotation(vec1, vec2).eulerAngles / 2);
				// this is the virtual directional vector
				Vector3 newVec = rotation * vec1;
				// this is the virtual road width, set it to be the average of the two
				float newWidth = road1.GetRoadWidth() + road2.GetRoadWidth() / 2;

				vecs = new[] {vec1, vec2, newVec, -newVec};
				roadWidths = new[] {road1.GetRoadWidth(), road2.GetRoadWidth(), newWidth, newWidth};
				break;
			case TwoWayIntersectionType.SMOOTH:
				Debug.LogError("Code Error, this method is to be called without smooth transition");

				break;

		}
		// zip vecs and roadWidths into abstract road infos
        AbstractWayInfo[] infos = AbstractWayInfo.InfosWithDirectionsAndWidths(vecs, roadWidths);
        return infos;
	}


	private GameObject buildTwoWayIntersection(Intersection intersection, Road road1, Road road2)
	{

		// create a four way intersection instead of a two way intersection if the angle is less than 90
		switch (GetTwoWayIntersectionType(intersection))
		{
			case TwoWayIntersectionType.SKEW:
			case TwoWayIntersectionType.TRANSITION:
				AbstractWayInfo[] infos = AbstractWayInfosForTwoWayIntersection(intersection, road1, road2);
                return buildMultiwayIntersectionWithVectors(intersection, infos);
			case TwoWayIntersectionType.SMOOTH:
                // only when angle > 90
                Vector3 vec1 = IntersectionMath.getOutgoingVector(intersection, road1).normalized;
                Vector3 vec2 = IntersectionMath.getOutgoingVector(intersection, road2).normalized;
                float roadWidth1 = road1.GetRoadWidth();
                float roadWidth2 = road2.GetRoadWidth();
                Vector3 vec1Norm = Quaternion.Euler(new Vector3(0, 90, 0)) * vec1 * roadWidth1 / 2;
                Vector3 vec2Norm = Quaternion.Euler(new Vector3(0, -90, 0)) * vec2 * roadWidth2 / 2;
                // check the intersection, if there's its inner, if there isn't, its outer
                Vector3 refPoint;
                GameObject inner;
                GameObject outer;
                if (Math3d.LineLineIntersection(out refPoint, vec1Norm, vec1 , vec2Norm,
                    vec2 ))
                {
                    // we intersected, its inner
                    inner = buildInner(vec1Norm, vec2Norm, refPoint, intersection.position, intersection.customization);
                    outer = buildOuter(-vec1Norm, -vec2Norm, 4, intersection.position, intersection.customization);
                }
                else
                {
                    // we are outer 
                    outer = buildOuter(vec1Norm, vec2Norm, 4, intersection.position, intersection.customization);
                    // get the real intersection
                    // we need to * a const since the Math scirpt only considers line to be that long
                   

                    inner = buildInner(-vec1Norm, -vec2Norm, refPoint, intersection.position, intersection.customization);

                }

                // add rigidbody to enable selection
                return createParentIntersectionWithChildIntersections("Two way intersection", intersection.customization, inner,
                    outer);
		}

		Debug.LogError("Semantics Error, please check");
		return null;
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
		var infos = AbstractWayInfosForMultiwayIntersection(intersection, connectedRoads);

		return buildMultiwayIntersectionWithVectors (intersection, infos);
	}

	private AbstractWayInfo[] AbstractWayInfosForMultiwayIntersection(Intersection intersection, Road[] connectedRoads)
	{
// convert roads to vectos
		Vector3[] vectors = connectedRoads.Select(rd => IntersectionMath.getOutgoingVector(intersection, rd)).ToArray();
		float[] widths = connectedRoads.Select(rd => rd.GetRoadWidth()).ToArray();

		AbstractWayInfo[] infos;
		// handle when connectedRoads.Length == 3  << build a four-way intersection instead
		if (connectedRoads.Length == 3)
		{
			// find the vector whose direction is of greatest difference to other two

			Vector3 maxVector = vectors[0];
			float maxRoadWidth = widths[0];
			float maxAngles = 0;
			for (int i = 0; i < 3; i++)
			{
				float accum = 0;
				for (int j = 0; j < 3; j++)
				{
					float zeroTo180Angle = Vector3.Angle(vectors[i], vectors[j]);
					float distance = zeroTo180Angle > 90 ? 180 - zeroTo180Angle : zeroTo180Angle;
					accum += distance;
				}

				if (accum > maxAngles)
				{
					maxVector = vectors[i];
					maxRoadWidth = widths[i];
					maxAngles = accum;
				}
			}

			Vector3[] resultingVectors = {vectors[0], vectors[1], vectors[2], -maxVector};
			float[] resultingWidths = {widths[0], widths[1], widths[2], maxRoadWidth};

			infos = AbstractWayInfo.InfosWithDirectionsAndWidths(resultingVectors, resultingWidths);
		}
		else
		{
			infos = AbstractWayInfo.InfosWithDirectionsAndWidths(vectors, widths);
		}

		return infos;
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
		public static AbstractWayInfo[] sortAbstractWayInfos(AbstractWayInfo[] outgoingWays)
		{
			return outgoingWays
				.OrderBy (way => Quaternion.FromToRotation (Vector3.right, way.direction).eulerAngles.y)
				.ToArray ();
		}

		public static bool operator ==(AbstractWayInfo info1, AbstractWayInfo info2)
		{
			return info1.direction.normalized == info2.direction.normalized
			       && Mathf.Approximately(info1.roadWidth, info2.roadWidth);
		}

		public static bool operator !=(AbstractWayInfo info1, AbstractWayInfo info2)
		{
			return !(info1 == info2);
		}
	}


	private GameObject buildMultiwayIntersectionWithVectors (Intersection intersection, AbstractWayInfo[] outgoingWays)
	{
		// sort vectors according to orientations first
		AbstractWayInfo[] sorted = AbstractWayInfo.sortAbstractWayInfos(outgoingWays);
		Mesh mesh = new Mesh ();

		var vertices = MeshVerticesForAbstractWayInfos(sorted);
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

	

	/**
	 * Assume the input vertices are sorted, 
	 */
	private Vector3[] MeshVerticesForAbstractWayInfos(AbstractWayInfo[] sorted)
	{
// get a list of intersection vertices
		Vector3[] vertices = new Vector3[sorted.Length + 2];
		for (int i = 0; i < sorted.Length; i++)
		{
			AbstractWayInfo rd1 = sorted[i];
			AbstractWayInfo rd2 = sorted[(i + 1) % sorted.Length];
			vertices[i] = CoordinateOfIntersectionBetweenTwoWays(rd1, rd2);
			//Debug.LogFormat("vec1 = {2}, vec2 = {3}, Vertex ({0}) : {1}", i, vertices[i], rd1.direction, rd2.direction);
		}

		vertices[sorted.Length] = vertices[0]; // the second last vertex is the first (for UV)
		vertices[sorted.Length + 1] = Vector3.zero; // the last vertex is the intersection center
		//Debug.LogFormat("Vertex ({0}) : {1}", sorted.Length, vertices[sorted.Length]);
		return vertices;
	}

	// get intersection of rd1's right edge with rd2's left edge
	private Vector3 CoordinateOfIntersectionBetweenTwoWays(AbstractWayInfo rd1, AbstractWayInfo rd2)
	{

		Vector3 coordinateRes;
		
		Vector3 vec1 = rd1.direction.normalized;
		Vector3 vec2 = rd2.direction.normalized;
		Vector3 vec1Norm = Quaternion.Euler(new Vector3(0, 90, 0)) * vec1 * rd1.roadWidth / 2;
		Vector3 vec2Norm = Quaternion.Euler(new Vector3(0, -90, 0)) * vec2 * rd2.roadWidth / 2;

		bool res = Math3d.LineLineIntersection(out coordinateRes, vec1Norm, vec1 , vec2Norm, vec2 );

		return coordinateRes;

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
		//TODO : Change to Sphere Collider instead of Mesh Collider
		MeshCollider objMeshCollider = intersectionObj.GetComponent<MeshCollider> ();
		objMeshCollider.sharedMesh = mesh;
		intersectionObj.layer = customization.isTemporary ? GlobalLayers.IgnoreRaycast : GlobalLayers.Default;
		return intersectionObj;
	}

	// TODO Combine below two methods
}

