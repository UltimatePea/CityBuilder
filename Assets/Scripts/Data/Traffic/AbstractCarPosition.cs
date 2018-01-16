
/** This class is the same as AbstractBuildingPosition except that this one has lane number */
public class AbstractCarPosition
{
    /** The road the car is on */
    public Road referenceRoad;
    /** the intersection to count offset from, also determines the direction of travel */
    public Intersection referenceIntersection;
    /** the distance between the car and the referenceIntersection */
    public float offset;
    /**  this number marks which lane the car is in. counting from leftmost lane (which is at zero) */
    public int laneNumber; // the lane with inner most at 0 

    public AbstractCarPosition(Road referenceRoad, Intersection referenceIntersection, float offset, int laneNumber)
    {
        this.referenceRoad = referenceRoad;
        this.referenceIntersection = referenceIntersection;
        this.offset = offset;
        this.laneNumber = laneNumber;
    }
}
