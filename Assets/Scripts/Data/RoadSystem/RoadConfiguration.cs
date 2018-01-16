using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadConfiguration
{

	public float roadWidth;
	public bool isTemporary;

	public RoadConfiguration (float roadWidth, bool isTemporary)
	{
		this.roadWidth = roadWidth;
		this.isTemporary = isTemporary;
	}
}
