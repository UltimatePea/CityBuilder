using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{

	public GameObject roadPrefab;
	public GameObject roadEndPrefab;


	private Vector3 startPosition;

	private GameObject tempRoad;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {

			// record start position on mouse press
			this.startPosition = getMousePositionOnPlane ();

		} else if (Input.GetMouseButton (0) || Input.GetMouseButtonUp (0)) {

			// create and destroy tempRoad on mouse drag and mouse release
			Vector3 endPosition = getMousePositionOnPlane ();
			if (tempRoad != null) {
				Destroy (tempRoad);
			}
			CreateRoad (startPosition, endPosition);
		}

		// temp road becomes permanent when the user releases the mouse
		if (Input.GetMouseButtonUp (0)) {
			this.tempRoad = null;
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

	void CreateRoad (Vector3 start, Vector3 end)
	{

		float roadLength = Vector3.Distance (start, end);
		Quaternion rotation = Quaternion.FromToRotation (Vector3.right, end - start);


		Mesh mesh = new Mesh ();

		float width = 1f;
		Vector3[] verticies = {
			new Vector3 (0, 0, 0 - width / 2),
			new Vector3 (roadLength, 0, 0 - width / 2),
			new Vector3 (roadLength, 0, 1 - width / 2),
			new Vector3 (0, 0, 1 - width / 2)
		};

		mesh.vertices = verticies;

		int[] triangles = {
			1, 0, 2,
			2, 0, 3
		};
		mesh.triangles = triangles;

		Vector2[] uv = {

			new Vector2 (0, 0),
			new Vector2 (roadLength, 0),
			new Vector2 (roadLength, 1),
			new Vector2 (0, 1)
			
		};
		mesh.uv = uv;

		Vector3[] normals = {
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};

		mesh.normals = normals;





		Vector3 roadPosition = start + new Vector3 (0, 0.01f, 0);
		GameObject obj = Instantiate (roadPrefab, roadPosition, rotation);


		MeshFilter objMeshFilter = obj.GetComponent<MeshFilter> ();
		objMeshFilter.mesh = mesh;

		tempRoad = obj;

		CreateRoadEnds (start, end, obj);

	}

	void CreateRoadEnds (Vector3 start, Vector3 end, GameObject road)
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


		// create right End


		GameObject rightRoadEnd = Instantiate (roadEndPrefab, 
			                          end + new Vector3 (0, 0.01f, 0),  // position
			                          Quaternion.FromToRotation (Vector3.right, end - start));
		MeshFilter rightRoadEndMeshFilter = rightRoadEnd.GetComponent<MeshFilter> ();
		rightRoadEndMeshFilter.mesh = mesh;

		rightRoadEnd.transform.parent = road.transform;


		// create left End

		GameObject leftRoadEnd = Instantiate (roadEndPrefab, 
			                         start + new Vector3 (0, 0.01f, 0),
			                         Quaternion.FromToRotation (Vector3.right, start - end));
		MeshFilter leftRoadEndMeshFilter = leftRoadEnd.GetComponent<MeshFilter> ();
		leftRoadEndMeshFilter.mesh = mesh;
		leftRoadEnd.transform.parent = road.transform;

	}
}
