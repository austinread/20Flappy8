using UnityEngine;
using System.Collections;

public class FlappyFlappin : MonoBehaviour {

	public Vector2 speed = new Vector2();
	private Rigidbody2D body;

	void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}
	
	void Update () {

	}
	
	void FixedUpdate(){
		float inputX = Input.GetAxis ("Horizontal");	//Updown Input
		float inputY = Input.GetAxis ("Vertical");		//leftright input
		if (inputX != 0 || inputY != 0){
			body.velocity = speed;
			//body.AddForce(speed);
		}
	}
}
