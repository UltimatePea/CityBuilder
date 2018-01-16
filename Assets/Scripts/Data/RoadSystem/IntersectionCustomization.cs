using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCustomization
{

	public bool isTemporary;

	
	public enum TrafficControlStyle { ALL_WAY_STOP_SIGN }

	public TrafficControlStyle trafficControlStyle;

	public IntersectionCustomization(bool isTemporary, TrafficControlStyle trafficControlStyle)
	{
		this.isTemporary = isTemporary;
		this.trafficControlStyle = trafficControlStyle;
	}
}
