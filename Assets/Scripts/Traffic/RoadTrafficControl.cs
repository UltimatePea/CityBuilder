using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrafficControl
{
	private Road referenceRoad;

	private List<AbstractCar> cars = new List<AbstractCar>();

	public RoadTrafficControl (Road road)
	{
		this.referenceRoad = road;
	}

	public void AddCar(AbstractCar car)
	{
		cars.Add(car);
	}

	// this method should be called once per frame to update car's positions
	public void tick()
	{
		foreach (AbstractCar car in cars)
		{
			car.moveForwardMathOnly(Time.deltaTime * 5);
			car.updateCarPosition();
		}
		
	}
}
