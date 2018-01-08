using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrafficControl
{
	private Road referenceRoad;
    private IntersectionTrafficMath trafficMath;

	private List<AbstractCar> cars = new List<AbstractCar>();

	public RoadTrafficControl (Road road, 
        IntersectionTrafficMath trafficMath)
	{
		this.referenceRoad = road;
		this.trafficMath = trafficMath;
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
			float stoppingDistance = trafficMath.stoppingDistanceForCurrentDrive(car.position);
			if (car.position.offset < stoppingDistance) 
			{
				car.position.offset += Time.deltaTime * 5;
				//we should not exceed the stopping distance
				if (car.position.offset > stoppingDistance)
				{
					car.position.offset = stoppingDistance;
				}
			}

			car.updateCarPosition();
		}
		
	}
}
