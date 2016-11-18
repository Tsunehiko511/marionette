using UnityEngine;
using System.Collections;

public class MoveSkyCamera : MonoBehaviour {
	Camera camera;
	public float speed;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
		speed = 1f;
	}
	
	// Update is called once per frame
	void Update () {
    float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");
		// this.transform.position	+= new Vector3(speed*x, 0, speed*z);
		// this.transform.position	+= new Vector3(z*speed, 0, -1*speed*x);
		this.transform.position	+= new Vector3(x*speed, 0, speed*z);
	}


	public void SetSkyCamera(){
		camera.enabled = !camera.enabled;
	}
}
