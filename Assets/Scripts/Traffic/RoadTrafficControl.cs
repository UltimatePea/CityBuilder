using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrafficControl
{
	private Road referenceRoad;
    private IntersectionTrafficMath trafficMath;
    private CarTrafficManager trafficManager;
	private List<AbstractCar> cars = new List<AbstractCar>();

	public RoadTrafficControl (Road road, 
        IntersectionTrafficMath trafficMath, CarTrafficManager trafficManager)
	{
		this.referenceRoad = road;
		this.trafficMath = trafficMath;
		this.trafficManager = trafficManager;
	}

	public void AddCar(AbstractCar car)
	{
		cars.Add(car);
	}

	// this method should be called once per frame to update car's positions
	public void tick()
	{
		List<AbstractCar> carsToRemove = new List<AbstractCar>();
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

					moveCarToIntersection(car);
					carsToRemove.Add(car);
				}
			}

			car.updateCarPosition();
		}
//TODO: Find a bettwe way to remove, (this is the workaroud for list traversal modification
		cars.RemoveAll(c => carsToRemove.Contains(c));

	}

	private void moveCarToIntersection(AbstractCar car)
	{
		Intersection toIntersection = car.position.referenceRoad.otherIntersection(car.position.referenceIntersection);
		IntersectionTrafficControl intersectionControl = trafficManager.intersectionControlForIntersection(toIntersection);
		intersectionControl.AddCar(car);
		

	}
}
