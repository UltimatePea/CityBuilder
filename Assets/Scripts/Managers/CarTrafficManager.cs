using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrafficManager : MonoBehaviour
{

	public float carSpeed;

	private List<AbstractCar> cars = new List<AbstractCar> ();

	public void AddCar (AbstractCar car)
	{
		Debug.Assert (!cars.Contains (car));
        
		cars.Add (car);
	}

	public void Update ()
	{
		// drive the car
		foreach (AbstractCar car in cars) {
			car.moveForwardMathOnly (carSpeed * Time.deltaTime);
			car.updateCarPosition ();
		}
	}

}
