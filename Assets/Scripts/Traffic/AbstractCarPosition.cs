
public class AbstractCarPosition
{
    public Road referenceRoad;
    public Intersection referenceIntersection;// the intersection to count offset from, also determines the direction of travel
    public float offset;
    public int laneNumber; // the lane with inner most at 0 

    public AbstractCarPosition(Road referenceRoad, Intersection referenceIntersection, float offset, int laneNumber)
    {
        this.referenceRoad = referenceRoad;
        this.referenceIntersection = referenceIntersection;
        this.offset = offset;
        this.laneNumber = laneNumber;
    }
}
