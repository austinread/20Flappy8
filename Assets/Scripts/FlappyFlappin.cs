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
		//Keyclicks More precise than axes when looking for a single click
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
			GetComponent<Rigidbody2D>().velocity = speed;
		}
	}

	public Vector2 StartingPos{
		get{return startingPos;}
		set{startingPos = value;}
	}
}
