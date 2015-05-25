using UnityEngine;
using System.Collections;

//Handles behavior of pipes and ground, as well as the invisible core trigger between pipes
public class ScrollingMovement : MonoBehaviour {

	private Vector2 speed = new Vector2();
	public Vector2 direction = new Vector2();
	private Vector2 movement;
	private Vector2 startingPos;	//So it can return there when the game restarts
	
	private GameObject background;
	private GameObject controller;
	private SpriteRenderer backgroundSprite;
	private BoxCollider2D myCollider;

	private float myWidth;
	private float backgroundWidth;
	private float myX;
	private float myY;
	private float backgroundX;

	void Start(){


		startingPos = transform.position;

		background = GameObject.Find("FlappyBackground");
		backgroundSprite = background.gameObject.GetComponent<SpriteRenderer>();
		myCollider = gameObject.GetComponent<BoxCollider2D>();
		controller = GameObject.FindGameObjectWithTag("GameController");
	}

	void Update () {
		movement = new Vector2 (speed.x * direction.x, speed.y * direction.y);

		myWidth = myCollider.bounds.size.x;
		backgroundWidth = backgroundSprite.sprite.bounds.size.x;
		myX = gameObject.transform.position.x;
		myY = gameObject.transform.position.y;
		backgroundX = background.transform.position.x;
	}

	void FixedUpdate(){
		GetComponent<Rigidbody2D>().velocity = movement;

		//Moves ground back to origin, destroys pipes
		if (myX < (backgroundX - myWidth)){
			if (gameObject.tag == "Ground"){
				gameObject.transform.position = new Vector3(backgroundWidth, myY, transform.position.z);
			}
			else if (gameObject.tag == "Pipe" || gameObject.tag == "ScoreTrigger"){
				Destroy(gameObject);
			}
			else{
				Debug.Log("You put this script somewhere it doesn't belong: " + gameObject.name +  ".  Get your shit together");
			}
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Bird" && gameObject.tag != "ScoreTrigger" && controller.gameObject.GetComponent<GameController>().CollisionBuffer == false){
			controller.gameObject.GetComponent<GameController>().LoseGame();
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.tag == "Bird" && gameObject.tag == "ScoreTrigger"){
			controller.gameObject.GetComponent<GameController>().ScoreFlappy++;
		}
	}


	public Vector2 Speed{
		get{return speed;}
		set{speed = value;}
	}

	public Vector2 StartingPos{
		get{return startingPos;}
		set{startingPos = value;}
	}
}