using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{

	public GameObject roadPrefab;

	public GameObject BuildRoad (Road road)
	{

		Vector3 start = road.fromIntersection.position;
		Vector3 end = road.toIntersection.position;

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
//		obj.transform.parent = road.fromIntersection.GetGameObject ().transform;


		MeshFilter objMeshFilter = obj.GetComponent<MeshFilter> ();
		objMeshFilter.mesh = mesh;	

		return obj;
	}

	public GameObject UpdateRoad (Road road, GameObject obj)
	{
		Destroy (obj);
		return BuildRoad (road);
	}


}