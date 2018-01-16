using UnityEngine;

public class IntersectionTrafficMath : MonoBehaviour
{
    public IntersectionBuilder builder;


    // returns the maximum value of AbstractCarPosition.offset so that the car is not rushing into the intersection
    public float stoppingDistanceForCurrentDrive(AbstractCarPosition carPosition)
    {
        /**
         *
         *  toIntersection                                     ref intersection
         *
         *  toLeftEdge
         *  \===================================================|   ^
         *   \|<          STOPPING DISTANCE                   > |  width(refIntersection)
         *    \ ------------------------------------------------|   x
         *     \                                                |  width(toIntersection)
         *      \===============================================|   v
         *     toRightEdge
         *
         *
         * 
         */

        Intersection toIntersection = carPosition.referenceRoad.otherIntersection(carPosition.referenceIntersection);
        Vector3 toLeftEdge = builder.coordinateForRoadAtIntersection(toIntersection, carPosition.referenceRoad, 
                                  IntersectionBuilder.RightOrLeft.LEFT);
        Vector3 toRightEdge = builder.coordinateForRoadAtIntersection(toIntersection, carPosition.referenceRoad,
                                  IntersectionBuilder.RightOrLeft.RIGHT);
        
        // get the proportion
        int totalNumberOfLanes =
            carPosition.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(toIntersection)
            + carPosition.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(carPosition
                .referenceIntersection);

        Vector3 stoppingLocation = Vector3.Lerp(toLeftEdge, toRightEdge,
            1f / 2 - (1f / 2 + carPosition.laneNumber) / totalNumberOfLanes) * (1f/2);
        
        // project the stopping location onto the outgoing vector

        Vector3 stoppingCompensation = Vector3.Project(stoppingLocation,
            IntersectionMath.getOutgoingVector(toIntersection, carPosition.referenceRoad));

        float stoppingDistance = carPosition.referenceRoad.GetRoadLength() - stoppingCompensation.magnitude;

        return stoppingDistance;
    }

    public float startingDistanceForCurrentDrive(AbstractCarPosition carPosition)
    {
        
        /**
         *
         *  refIntersection                                     toIntersection
         *
         *  fromLeftEdge
         *  \===================================================|   ^
         *   \                                                  |  width(toIntersection)
         *    \ ------------------------------------------------|   x
         * <  >\                                                |  width(refIntersection)
         *  |   \===============================================|   v
         *  |  fromRightEdge
         *  |
         *  +--> startingDistance
         * 
         */

        Intersection toIntersection = carPosition.referenceRoad.otherIntersection(carPosition.referenceIntersection);
        Vector3 fromLeftEdge = builder.coordinateForRoadAtIntersection(carPosition.referenceIntersection, carPosition.referenceRoad, 
                                  IntersectionBuilder.RightOrLeft.LEFT);
        Vector3 fromRightEdge = builder.coordinateForRoadAtIntersection(carPosition.referenceIntersection, carPosition.referenceRoad,
                                  IntersectionBuilder.RightOrLeft.RIGHT);
        
        // get the proportion
        int totalNumberOfLanes =
            carPosition.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(toIntersection)
            + carPosition.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(carPosition
                .referenceIntersection);

        Vector3 startingLocation = Vector3.Lerp(fromLeftEdge, fromRightEdge,
            1f / 2 + (1f / 2 + carPosition.laneNumber) / totalNumberOfLanes) * (1f/2);
        
        // project the stopping location onto the outgoing vector

        Vector3 startignCompensation = Vector3.Project(startingLocation,
            IntersectionMath.getOutgoingVector(carPosition.referenceIntersection, carPosition.referenceRoad));

        float startingDistance =  startignCompensation.magnitude;

        return startingDistance;
    }
}
