
using UnityEngine;

public class NavigationManager : MonoBehaviour
{

    // finds which direction to head to at the intersection
    // note that car.referenceIntersection is different from the parameter intersection
    // car's referenceRoad connects the above two intersection
    // car is at the parameter intersection
    public Road targetWayForAbstractCarAtIntersection(AbstractCar car, Intersection intersection)
    {
        int pickedIndex = Random.Range(0, intersection.getConnectedRoads().Length);
        Debug.LogFormat("Picked {0} out of {1} ", pickedIndex, intersection.getConnectedRoads().Length);
        return intersection.getConnectedRoads()[pickedIndex];
        // TODO: Use a realistic navigation algorithm

    }
    
}
