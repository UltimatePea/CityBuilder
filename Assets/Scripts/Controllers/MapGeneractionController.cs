using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneractionController : MonoBehaviour
{
	public IntersectionManager intersecManager;
	public CarTrafficManager CarTrafficManager;
	public bool shouldStartOnPlay;

	// Use this for initialization
	void Start () {

		// procedurally generate a basic intersection structure to test traffic
		if (shouldStartOnPlay)
		{

			float gridSz = 10f;
			/**
			 *
			 * 			^ z
			 * 			|
			 *
			 * 
			 * 1 ------ 2 --------- 3
			 *          |           |
			 *          |           |
			 *          |           |
			 *          |           |
			 *          4-----------5----------6      -> x
			 *  								  
			 * 
			 */

			var inter1 = intersecManager.createIntersection(new Vector3(-1f, 0f, 1f)*gridSz);
			var inter2 = intersecManager.createIntersection(new Vector3(0f, 0f, 1f)*gridSz);
			var inter3 = intersecManager.createIntersection(new Vector3(1f, 0f, 1f)*gridSz);
			var inter4 = intersecManager.createIntersection(new Vector3(0f, 0f, 0f)*gridSz);
			var inter5 = intersecManager.createIntersection(new Vector3(1f, 0f, 0f)*gridSz);
			var inter6 = intersecManager.createIntersection(new Vector3(2f, 0f, 0f)*gridSz);
			Road rd1 = intersecManager.connectTwoIntersections(inter1, inter2, 4f);
			Road rd2 =  intersecManager.connectTwoIntersections(inter2, inter3, 4f);
			Road rd3 = intersecManager.connectTwoIntersections(inter2, inter4, 4f);
			Road rd4 = intersecManager.connectTwoIntersections(inter3, inter5, 4f);
			Road rd5 = intersecManager.connectTwoIntersections(inter4, inter5, 4f);
			Road rd6 = intersecManager.connectTwoIntersections(inter5, inter6, 4f);
			
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
