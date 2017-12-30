using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{

	public Material matRoad;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
				Vector3 point = hitInfo.point;
				CreateRoad (point);
			}
		}

	}

	void CreateRoad (Vector3 position)
	{


		Mesh mesh = new Mesh ();

		Vector3[] verticies = {
			new Vector3 (0, 0, 0),
			new Vector3 (1, 0, 0),
			new Vector3 (1, 0, 1),
			new Vector3 (0, 0, 1)
		};

		mesh.vertices = verticies;

		int[] triangles = {
			1, 0, 2,
			2, 0, 3
		};
		mesh.triangles = triangles;




		GameObject obj = new GameObject ("road", typeof(MeshFilter), typeof(MeshRenderer));
		obj.transform.position = new Vector3 (0, 0.01f, 0);
		obj.transform.Translate (position);

		MeshFilter objMeshFilter = obj.GetComponent<MeshFilter> ();
		objMeshFilter.mesh = mesh;
		MeshRenderer objMeshRenderer = obj.GetComponent<MeshRenderer> ();
		objMeshRenderer.material = matRoad;
		objMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;



	}

}
