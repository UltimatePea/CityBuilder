using UnityEngine;


public class AbstractBuilding
{


    public AbstractBuildingPosition position;
    public GameObject gameObject;

    public AbstractBuilding(AbstractBuildingPosition position, GameObject gameObject)
    {
        this.position = position;
        this.gameObject = gameObject;
    }
}

/** This class is the same as `AbstractCarPosition` except that this class does not have lane nubmer; */
public class AbstractBuildingPosition
{
    /** the road that this building is attached to */
    public Road referenceRoad;
    /** the intersection to count offset from */
    public Intersection referenceIntersection;// the intersection to count offset from, also determines the direction of travel
    /** the distance between the building and the reference intersection */
    public float offset;

    public AbstractBuildingPosition(Road referenceRoad, Intersection referenceIntersection, float offset)
    {
        this.referenceRoad = referenceRoad;
        this.referenceIntersection = referenceIntersection;
        this.offset = offset;
    }
}
