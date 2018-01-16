using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public class BuildingManager : MonoBehaviour
{


	private List<AbstractBuilding> buildings = new List<AbstractBuilding>();
	
	public void addBuilding(AbstractBuilding building)
	{
		this.buildings.Add(building);
	}

		
}
