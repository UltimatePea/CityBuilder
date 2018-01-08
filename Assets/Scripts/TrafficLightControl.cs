using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightControl : MonoBehaviour
{


	public MeshRenderer redLightMeshRenderer;
	public MeshRenderer yellowLightMeshRenderer;
	public MeshRenderer greenLightMeshRenderer;

	public Material redLightOnMaterial;
	public Material yellowLightOnMaterial;
	public Material greenLightOnMaterial;
	public Material redLightOffMaterial;
	public Material yellowLightOffMaterial;
	public Material greenLightOffMaterial;

	void Start ()
	{

		StartCoroutine (runLight ());
	}

	void Update ()
	{
		
	}

	IEnumerator runLight ()
	{
		while (true) {
			redLightMeshRenderer.material = redLightOnMaterial;
			yellowLightMeshRenderer.material = yellowLightOffMaterial;
			greenLightMeshRenderer.material = greenLightOffMaterial;
			yield return new WaitForSeconds (4);
			redLightMeshRenderer.material = redLightOffMaterial;
			yellowLightMeshRenderer.material = yellowLightOffMaterial;
			greenLightMeshRenderer.material = greenLightOnMaterial;
			yield return new WaitForSeconds (2);
			redLightMeshRenderer.material = redLightOffMaterial;
			yellowLightMeshRenderer.material = yellowLightOnMaterial;
			greenLightMeshRenderer.material = greenLightOffMaterial;
			yield return new WaitForSeconds (1);
		}
		
	}
}
