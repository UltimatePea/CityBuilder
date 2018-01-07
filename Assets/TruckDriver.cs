using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckDriver : MonoBehaviour
{

	public Material redLightOnMat, redLightMediumMat, redLightOffMat, yellowLightOnMat, 
		yellowLightOffMat, whiteLightOnMat, whiteLightOffMat;
	public GameObject[] leftTurningSignalObjs, rightTurningSignalObjs, breakSignalObjs, headLightObjs, tailLightObjs;
	public GameObject leftFrontWheel, leftRearWhell, rightFrontWheel, rightRearWheel;

	// TODO : ADD ngiht mode -- change red light off to dim light
	private bool isNight = false;

	void Start()
	{
		// initalize materials
		setGameObjectsMaterials(leftTurningSignalObjs, yellowLightOffMat);
		setGameObjectsMaterials(rightTurningSignalObjs, yellowLightOffMat);
		setGameObjectsMaterials(breakSignalObjs, redLightOffMat);
		// TODO: fix repeated setting of materials
		setGameObjectsMaterials(tailLightObjs, redLightOffMat);
		setGameObjectsMaterials(headLightObjs, whiteLightOffMat);
		
		
	}

	public void turnOnLeftTurningSignal()
	{
		StartCoroutine(turningSignal(leftTurningSignalObjs));
	}

	public void turnOffLeftTurningSignal()
	{
		StopCoroutine(turningSignal(leftTurningSignalObjs));
		setGameObjectsMaterials(leftTurningSignalObjs, yellowLightOffMat);
	}
	public void turnOnRighjtTurningSignal()
	{
		StartCoroutine(turningSignal(rightTurningSignalObjs));
	}

	public void turnOffRightTurningSignal()
	{
		StopCoroutine(turningSignal(rightTurningSignalObjs));
		setGameObjectsMaterials(rightTurningSignalObjs, yellowLightOffMat);
	}

	public void startBreak()
	{
		setGameObjectsMaterials(breakSignalObjs, redLightOnMat);
	}

	public void stopBreak()
	{
		setGameObjectsMaterials(breakSignalObjs, redLightOffMat);
	}

	// turning signals on or off
	private IEnumerator turningSignal(GameObject[] lights)
	{
		while (true)
		{
			setGameObjectsMaterials(lights, yellowLightOnMat);
			yield return new WaitForSeconds(0.5f);
			setGameObjectsMaterials(lights, yellowLightOffMat);
			yield return new WaitForSeconds(0.5f);
		}
	}

	// set a group of game objects to the material
	private void setGameObjectsMaterials(GameObject[] objects, Material material)
	{
		foreach (GameObject obj in objects)
		{
			MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
			renderer.material = material;

		}
	}
	
}
