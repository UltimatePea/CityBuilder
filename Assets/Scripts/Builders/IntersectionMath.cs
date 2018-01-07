using UnityEngine;

public class IntersectionMath
{
    public static Vector3 getOutgoingVector(Intersection intersection, Road road)
    {
        Vector3 vec = Vector3.zero;
        switch (road.typeOfIntersection(intersection))
        {
            case Road.IntersectionClass.FROM:
                vec = road.toIntersection.position - road.fromIntersection.position;
                break;
            case Road.IntersectionClass.TO:
                vec = road.fromIntersection.position - road.toIntersection.position;
                break;
        }

        return vec;
    }

    public static Quaternion getAngle(Intersection intersection, Road road)
    {
        Quaternion angle = Quaternion.identity;
        switch (road.typeOfIntersection(intersection))
        {
            case Road.IntersectionClass.FROM:
                angle = Quaternion.FromToRotation(Vector3.right,
                    road.toIntersection.position - road.fromIntersection.position);
                break;
            case Road.IntersectionClass.TO:
                angle = Quaternion.FromToRotation(Vector3.right,
                    road.fromIntersection.position - road.toIntersection.position);
                break;
        }

        return angle;
    }
}