using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{

	public GameObject roadPrefab;
	public IntersectionBuilder intersectionBuilder;// we need to query intersection builder for coordinate information

	public GameObject BuildRoad (Road road)
	{

		Vector3 start = road.fromIntersection.position;
		Vector3 end = road.toIntersection.position;
		/**
		 * FROM LEFT 3 ------------------------- TO RIGHT 2 
		 *
		 * start                                      end
		 * 
		 * FROM RIGHT 0 -------------------------- TO LEFT 1
		 *
		 *
		 * 0
		 * 
		 */



		Mesh mesh = new Mesh ();

		float width = road.GetRoadWidth ();
		Vector3[] verticies = {
			intersectionBuilder.coordinateForRoadAtIntersection(road.fromIntersection, road, IntersectionBuilder.RightOrLeft.RIGHT),
			intersectionBuilder.coordinateForRoadAtIntersection(road.toIntersection, road, IntersectionBuilder.RightOrLeft.LEFT) + end - start,
			intersectionBuilder.coordinateForRoadAtIntersection(road.toIntersection, road, IntersectionBuilder.RightOrLeft.RIGHT) + end - start,
			intersectionBuilder.coordinateForRoadAtIntersection(road.fromIntersection, road, IntersectionBuilder.RightOrLeft.LEFT),
		};

		mesh.vertices = verticies;

		int[] triangles = {
			1, 0, 2,
			2, 0, 3
		};
		mesh.triangles = triangles;

		float distance = Vector3.Distance(verticies[0], verticies[1]);
		Vector2[] uv = {

			new Vector2 (0, 0),
			new Vector2 (distance, 0),
			new Vector2 (Vector3.Dot(verticies[2] - verticies[0], verticies[1] - verticies[0]) / distance, 1),
			new Vector2 (Vector3.Dot(verticies[3] - verticies[0], verticies[1] - verticies[0]) / distance, 1)

			
		};
		//Debug.LogFormat("UV = {0}{1}{2}{3}", uv[0], uv[1], uv[2], uv[3]);
		mesh.uv = uv;

		Vector3[] normals = {
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};

		mesh.normals = normals;





		Vector3 roadPosition = start + new Vector3 (0, 0.01f, 0);
		GameObject obj = Instantiate (roadPrefab, roadPosition, Quaternion.identity);
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