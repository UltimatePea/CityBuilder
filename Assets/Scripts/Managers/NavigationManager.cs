
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NavigationManager : MonoBehaviour
{

    // finds which direction to head to at the intersection
    // note that car.referenceIntersection is different from the parameter intersection
    // car's referenceRoad connects the above two intersection
    // car is at the parameter intersection
    public Road targetWayForAbstractCarAtIntersection(AbstractCar car, Intersection intersection)
    {
        switch (intersection.GetGraphicsType())
        {
            case Intersection.IntersectionGraphicsType.NONE:
                return null;
            case Intersection.IntersectionGraphicsType.ONE_WAY_WRAP:
                return intersection.getConnectedRoads()[0];
            case Intersection.IntersectionGraphicsType.TWO_WAY_SKEW:
            case Intersection.IntersectionGraphicsType.TWO_WAY_TRANSITION:
            case Intersection.IntersectionGraphicsType.TWO_WAY_SMOOTH:
                // for two way, we always return the other way
                return intersection.getConnectedRoads()[0] == car.position.referenceRoad
                    ? intersection.getConnectedRoads()[1]
                    : intersection.getConnectedRoads()[0];
            case Intersection.IntersectionGraphicsType.MULTI_WAY:
                // we return random for now
                int pickedIndex = 0;
                do
                {
                    pickedIndex = Random.Range(0, intersection.getConnectedRoads().Length);
                } while (intersection.getConnectedRoads()[pickedIndex] == car.position.referenceRoad);

                return intersection.getConnectedRoads()[pickedIndex];
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
    
}
