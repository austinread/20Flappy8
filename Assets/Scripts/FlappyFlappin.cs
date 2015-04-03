using UnityEngine;
using System.Collections;

//Handles bird behavior
//I may have named this class while drunk.  I'm not changing it.
public class FlappyFlappin : MonoBehaviour {

	public Vector2 speed = new Vector2();
	private Vector2 startingPos;	//So it can return there to restart the game

	void Start()
	{
		startingPos = transform.position;
	}
	
	void FixedUpdate(){
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		if (inputX != 0 || inputY != 0){
			GetComponent<Rigidbody2D>().velocity = speed;
		}
	}

	public Vector2 StartingPos{
		get{return startingPos;}
		set{startingPos = value;}
	}
}
