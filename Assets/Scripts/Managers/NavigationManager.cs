
using UnityEngine;

public class NavigationManager : MonoBehaviour
{

    // finds which direction to head to at the intersection
    // note that car.referenceIntersection is different from the parameter intersection
    // car's referenceRoad connects the above two intersection
    // car is at the parameter intersection
    public Road targetWayForAbstractCarAtIntersection(AbstractCar car, Intersection intersection)
    {
        return intersection.getConnectedRoads()[Random.Range(0, intersection.getConnectedRoads().Length - 1)];
        // TODO: Use a realistic navigation algorithm

    }
    
}
