using UnityEngine;
using System.Collections;

public class GoalHoop : MonoBehaviour {

	// Use this for initialization
	private void Start () 
	{
		var RR = 1.046f;
		var RC = new Vector3 (0.088f, 2.874f, 1.109f);

		for (var i=0; i<360; i+=30) {
			var rad = (i*Mathf.PI)/180.0f;
			var sc = gameObject.AddComponent<SphereCollider>();
			sc.center = new Vector3 (RC.x + RR*Mathf.Sin(rad), RC.y, RC.z+ RR*Mathf.Cos(rad));
			sc.radius = 0.042f;
		}

	}
}
