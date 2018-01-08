using  UnityEngine;
public class MouseCollisionDetection
{
    public static Vector3 getMousePositionOnPlane (out GameObject tag)
    {

        // Drag initiated
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        RaycastHit hitInfo = new RaycastHit ();
        if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, ~(1 << GlobalLayers.IgnoreRaycast))) {
            Vector3 point = hitInfo.point;
            tag = hitInfo.transform.gameObject;
            return point;
        }
        tag = null;

        // TODO :: HANDLE LOGIC WHEN NOT HIT
        return Vector3.zero;
    } 
}
