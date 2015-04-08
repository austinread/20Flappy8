using UnityEngine;
using System.Collections;

//Handles overarching game behavior:  Score, starting, losing, etc.
public class GameController : MonoBehaviour {
	public Vector2 gameSpeed;	//These are public for easy tweaking
	public float birdGravity;
	public float pipeInterval;	//How often they appear
	public float pipeSpacing;	//In between the top and bottom pipes
	public GameObject pipePrefabDown;
	public GameObject pipePrefabUp;

	private float timer;	//pipes

	private GameObject[] ground;
	private GameObject[] pipes;
	private bool paused;

	GameObject bird;

	void Start(){
		paused = true;
		timer = pipeInterval;
		bird = GameObject.FindGameObjectWithTag("Bird");
		ground = GameObject.FindGameObjectsWithTag("Ground");
	}

	public void StartGame(){
		paused = false;
		bird.GetComponent<Rigidbody2D>().gravityScale = birdGravity;

		foreach (GameObject block in ground){
			block.GetComponent<ScrollingMovement>().Speed = gameSpeed;
		}
	}

	public void LoseGame(){
		paused = true;
		timer = pipeInterval;
		bird.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
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

	//At globally defined interval, spawns a new set of pipes at a slightly random location
	void SpawnPipes(){
		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = pipeInterval;

			float randomOffset = Random.Range(-3.0f, 3.0f);
			Vector2 newPipeLocation = new Vector2(8, -10 + randomOffset);	//BEWARE, HERE BE HARDCODED (X, Y) VALUES BY A LAZY ASS DEVELOPER
			GameObject newPipeDown = (GameObject)Instantiate(pipePrefabDown, newPipeLocation, Quaternion.identity);
			GameObject newPipeUp = (GameObject)Instantiate(pipePrefabUp, new Vector2(newPipeLocation.x, newPipeLocation.y + pipeSpacing), Quaternion.identity);
			newPipeDown.GetComponent<ScrollingMovement>().Speed = gameSpeed;
			newPipeUp.GetComponent<ScrollingMovement>().Speed = gameSpeed;
		}
	}

	void Update(){
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		if ((inputX != 0 || inputY != 0) && paused){
			StartGame();
		}

		if (!paused){
			SpawnPipes();
		}

	}
}
