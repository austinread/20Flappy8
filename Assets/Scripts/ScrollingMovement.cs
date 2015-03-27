using UnityEngine;
using System.Collections;

public class ScrollingMovement : MonoBehaviour {

	public Vector2 speed = new Vector2();
	public Vector2 direction = new Vector2();
	private Vector2 movement;

	private GameObject background;
	
	SpriteRenderer backgroundSprite;
	SpriteRenderer mySprite;

	float myWidth;
	float backgroundWidth;
	float myX;
	float myY;
	float backgroundX;

	void Start(){
		background = GameObject.Find("FlappyBackground");
		backgroundSprite = background.gameObject.GetComponent<SpriteRenderer>();
		mySprite = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update () {
		//Sets the movement to speed and direction
		movement = new Vector2 (speed.x * direction.x, speed.y * direction.y);

		myWidth = mySprite.sprite.bounds.size.x;
		backgroundWidth = backgroundSprite.sprite.bounds.size.x;
		myX = gameObject.transform.position.x;
		myY = gameObject.transform.position.y;
		backgroundX = background.transform.position.x;
	}

	void FixedUpdate(){
		GetComponent<Rigidbody2D>().velocity = movement;

		//Moves the object back to origin
		if (myX < (backgroundX - myWidth)){
			gameObject.transform.position = new Vector2(backgroundWidth, myY);
		}
	}
}