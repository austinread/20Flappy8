using UnityEngine;
using System.Collections;

//Handles overarching game behavior:  Score, starting, losing, etc.
public class GameController : MonoBehaviour {
	public Vector2 gameSpeed;
	public float birdGravity;

	private GameObject[] ground;
	private GameObject[] pipes;
	private bool paused;

	GameObject bird;

	void Start(){
		paused = true;
		bird = GameObject.FindGameObjectWithTag("Bird");
		ground = GameObject.FindGameObjectsWithTag("Ground");
	}

	public void StartGame(){
		paused = false;
		bird.GetComponent<Rigidbody2D>().gravityScale = birdGravity;

		foreach (GameObject block in ground){
			block.GetComponent<ScrollingMovement>().Speed = gameSpeed;
		}

		//TODO: Pipe generation goes here
	}

	public void LoseGame(){
		paused = true;
		bird.GetComponent<Rigidbody2D>().gravityScale = (float)0.0;
		bird.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		bird.transform.position = bird.GetComponent<FlappyFlappin>().StartingPos;

		foreach (GameObject block in ground){
			block.GetComponent<ScrollingMovement>().Speed = Vector2.zero;
			block.transform.position = block.GetComponent<ScrollingMovement>().StartingPos;
		}

		pipes = GameObject.FindGameObjectsWithTag("Pipe");
		foreach (GameObject pipe in pipes){
			Destroy(pipe);
		}
	}

	void Update(){
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		if ((inputX != 0 || inputY != 0) && paused){
			StartGame();
		}
	}
}
