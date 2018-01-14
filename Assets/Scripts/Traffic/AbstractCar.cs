using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AbstractCar
{
    public AbstractCar(AbstractCarPosition position, GameObject carGameObject)
    {
        this.position = position;
        this.carGameObject = carGameObject;
        this.carControl = carGameObject.GetComponent<TruckControl>();
        this.speed = 0f;
    }

    public AbstractCarPosition position;
    public GameObject carGameObject;
    
    
    private TruckControl carControl;
    
    
    private float speed; // the current speed of the car
    
    /* speed related */
    public float GetSpeed()
    {
        Debug.LogFormat("Speed = {0}", speed);
        return speed;
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount * Time.deltaTime;
        carControl.stopBreak();
    }
    
    public void DecreaseSpeed(float amount)
    {
        speed -= amount * Time.deltaTime;
        
        // no negative speed
        if (speed < 0)
        {
            speed = 0;
            
        }
        carControl.startBreak();
    }

    public void SmartAdjustSpeedAccordingToStoppingDistance(float stoppingDistance, float maxAcceleration, float maxSpeed = Mathf.Infinity)
    {
//        float safeDistance = stoppingDistance - GetSpeed();// subtract 1s from stopping distance
        float safeDistance = stoppingDistance;// experiment to see if this would solve the blinking break, fixes the blinking issue on straight roads
        // check if we can stop at safe distance at maxDecceleration
        // s = 1/2 a t^2                       vvv acceleration             vvv time
        float stopDistaceAtFullBreak = 1 / 2f * maxAcceleration * Mathf.Pow(speed / maxAcceleration, 2f);
        if (stopDistaceAtFullBreak < safeDistance)
        {
            // we still have safe distance, we accelerate
            if (speed + maxAcceleration < maxSpeed)
            {
                // after full accelerate, we're still well below maxSpeed, we accelerate at full torque
                IncreaseSpeed(maxAcceleration);
            } else if (speed < maxSpeed)
            {
                // we are reaching maxSpeed, increase to maxSpeed
                IncreaseSpeed(maxSpeed - speed);
            }
            else
            {
                // we keep the speed, no action
            }
            
        }
        else
        {
            // we are no longer safe, stopping distance is greater than safe distance
            // full break
            DecreaseSpeed(maxAcceleration);
            
        }
    }
    

    /* Game Object related */
    public void updateCarPosition()
    {
        
       // update the carGameObject's coordinate and orientation to be the correct one


        Vector3 objPosition;
        Quaternion objRotation;
        CalculatePositionAndOrientation(out objPosition, out objRotation);

        carGameObject.transform.position = objPosition;
        carGameObject.transform.rotation = objRotation;



    }

    public void CalculatePositionAndOrientation(out Vector3 objPosition, out Quaternion objRotation)
    {
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

        int numberOfLanes =
            position.referenceRoad.GetNumberOfLanesInDirectionWithReferenceIntersection(position.referenceIntersection);
        float roadWidth = position.referenceRoad.GetRoadWidthWithReferenceIntersection(position.referenceIntersection);

        float laneWidth = roadWidth / numberOfLanes;

        // lane number starts at zero
        Vector3 perpendicularOffset = perpendicular * (laneWidth * ((float) position.laneNumber + 1f / 2));

        Vector3 horizontalOffset = facingDirection.normalized * position.offset;

        objPosition = roadFrom + horizontalOffset + perpendicularOffset;
        objRotation = Quaternion.FromToRotation(Vector3.right, facingDirection);
    }
}

