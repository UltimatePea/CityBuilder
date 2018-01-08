using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AbstractCar
{
    public AbstractCar(AbstractCarPosition position, GameObject carGameObject, IntersectionTrafficMath trafficMath)
    {
        this.position = position;
        this.carGameObject = carGameObject;
        this.trafficMath = trafficMath;
    }

    public AbstractCarPosition position;
    public GameObject carGameObject;
    private IntersectionTrafficMath trafficMath;

    public void updateCarPosition()
    {
        
       // update the carGameObject's coordinate and orientation to be the correct one

        
        /**
         *
         * from                                                     to
         * -------------------------------------------------------------
         *       ^
         *       v  roadWidth / number of lanes
         * ============================================================
         *  | 
         *  v  << perpendicular
         * -------------------------------------------------------------
         *
         *
         * Note : this assumes yellow line is at the center we may need to change
         *
         * 
         */
        Vector3 roadFrom = position.referenceIntersection.position;
        Vector3 roadTo = position.referenceRoad.otherIntersection(position.referenceIntersection).position;

        Vector3 facingDirection = roadTo - roadFrom;

        Vector3 perpendicular = Quaternion.EulerAngles(0, 90, 0) * facingDirection.normalized;

        int numberOfLanes = position.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(position.referenceIntersection);
        float roadWidth = position.referenceRoad.GetRoadWidthWithReferenceIntersection(position.referenceIntersection);

        float laneWidth = roadWidth / numberOfLanes;

        // lane number starts at zero
        Vector3 perpendicularOffset = perpendicular * (laneWidth * ((float) position.laneNumber + 1f / 2));

        Vector3 horizontalOffset = facingDirection.normalized * position.offset;

        carGameObject.transform.position = roadFrom + horizontalOffset + perpendicularOffset;
        carGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.right, facingDirection);



    }

    // placeholder method to move the car foward
    // TODO: Delete this
    public void moveForwardMathOnly(float amount)
    {
        if (position.offset < trafficMath.stoppingDistanceForCurrentDrive(position))
        {
            position.offset += amount;
        }
        else
        {
            position.offset = 0;
            // turn around
            // TODO: CHange to handle the car to the intersection
            position.referenceIntersection = position.referenceRoad.otherIntersection(position.referenceIntersection);

        }
        
    }
}

