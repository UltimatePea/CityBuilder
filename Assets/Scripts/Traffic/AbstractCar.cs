using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AbstractCar
{
    public AbstractCar(AbstractCarPosition position, GameObject carGameObject)
    {
        this.position = position;
        this.carGameObject = carGameObject;
    }

    public AbstractCarPosition position;
    public GameObject carGameObject;

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

        Vector3 perpendicular = Quaternion.Euler(0, 90, 0) * facingDirection.normalized;

        int numberOfLanes = position.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(position.referenceIntersection);
        float roadWidth = position.referenceRoad.GetRoadWidthWithReferenceIntersection(position.referenceIntersection);

        float laneWidth = roadWidth / numberOfLanes;

        // lane number starts at zero
        Vector3 perpendicularOffset = perpendicular * (laneWidth * ((float) position.laneNumber + 1f / 2));

        Vector3 horizontalOffset = facingDirection.normalized * position.offset;

        carGameObject.transform.position = roadFrom + horizontalOffset + perpendicularOffset;
        carGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.right, facingDirection);



    }

    
}

