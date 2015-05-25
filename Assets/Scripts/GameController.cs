using UnityEngine;
using System.Collections;

//Handles overarching game behavior:  Score, starting, losing, tile tracking
using System.Collections.Generic;


public class GameController : MonoBehaviour {
	public Vector2 gameSpeed;	//These are public for easy tweaking
	public float birdGravity;
	public float pipeInterval;	//How often they appear
	public float pipeSpacing;	//Space in between the top and bottom pipes
	public GameObject pipePrefab;
	public GameObject scoreTriggerPrefab;
	public Sprite pipeSpriteUp;
	public Sprite pipeSpriteDown;

	private float timer;	//pipes

	private GameObject[] ground;
	private GameObject[] pipes;
	private GameObject[] scoreTriggers;
	private GameObject[] tiles;

	//This is already tracked in each tile object, but working with this list for movememnt calculations minimizes finding and calling the right Tile every time
	private List<int> tileValues;

	private bool paused;
	private int scoreFlappy;
	private int score2048;
	private int scoreTotal;
	private int scoreLast;
	private int scoreHigh;

	//The LoseGame method was getting called multiple times, screwing the score up.  This is utilized to prevent that.
	private bool collisionBuffer;

	GameObject bird;

	void Start(){
		scoreFlappy = 0;
		score2048 = 0;
		scoreTotal = 0;
		scoreLast = 0;
		scoreHigh = 0;
		paused = true;
		timer = pipeInterval;
		bird = GameObject.FindGameObjectWithTag("Bird");
		ground = GameObject.FindGameObjectsWithTag("Ground");

		//setup initial tile values
		tiles = GameObject.FindGameObjectsWithTag("Tile");
		foreach(GameObject tile in tiles){
			tile.GetComponent<TileController>().NumValue = 0;
		}
		tileValues = new List<int>();
		for (int i=0; i < 16; i++){
			tileValues.Add(0);
		}

		GenerateRandomBlock(-1);
		GenerateRandomBlock(-1);
	}

	void Update(){
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		if ((inputX != 0 || inputY != 0) && paused){
			StartGame();
		}

		//Keyclicks More precise than axes when looking for a single click
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
			ShiftTiles("Right");
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
			ShiftTiles("Left");
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
			ShiftTiles("Up");
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
			ShiftTiles("Down");
		}
		
		if (!paused){
			SpawnPipes();
		}
		
	}

	//Sets things in motion
	public void StartGame(){
		collisionBuffer = false;
		paused = false;
		bird.GetComponent<Rigidbody2D>().gravityScale = birdGravity;

		foreach (GameObject block in ground){
			block.GetComponent<ScrollingMovement>().Speed = gameSpeed;
		}
	}

	//Resets game and pauses, awaiting button press
	public void LoseGame(){
		collisionBuffer = true;
		scoreLast = scoreTotal;
		if (scoreTotal > scoreHigh){
			scoreHigh = scoreTotal;
		}
		score2048 = 0;
		scoreFlappy = 0;
		scoreTotal = 0;

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

		scoreTriggers = GameObject.FindGameObjectsWithTag("ScoreTrigger");
		foreach (GameObject trigger in scoreTriggers){
			Destroy(trigger);
		}

		for (int i = 0; i < 16; i++){
			tileValues[i] = 0;
			GetTileByPosition(i).NumValue = tileValues[i];
		}

		GenerateRandomBlock(-1);
		GenerateRandomBlock(-1);
	}

	//At globally defined interval, spawns a new set of pipes at a slightly random location
	private void SpawnPipes(){
		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = pipeInterval;

			float randomOffset = Random.Range(-2.75f, 2.75f);
			Vector2 newPipeLocation = new Vector2(8, -10 + randomOffset);	//BEWARE, HERE BE HARDCODED (X, Y) VALUES BY A LAZY ASS DEVELOPER
			GameObject newPipeDown = (GameObject)Instantiate(pipePrefab, newPipeLocation, Quaternion.identity);
			GameObject newScoreTrigger = (GameObject)Instantiate(scoreTriggerPrefab, new Vector2(newPipeLocation.x, newPipeLocation.y + pipeSpacing/2), Quaternion.identity);
			GameObject newPipeUp = (GameObject)Instantiate(pipePrefab, new Vector2(newPipeLocation.x, newPipeLocation.y + pipeSpacing), Quaternion.identity);
			newPipeDown.GetComponent<ScrollingMovement>().Speed = gameSpeed;
			newPipeDown.GetComponent<SpriteRenderer>().sprite = pipeSpriteDown;
			newScoreTrigger.GetComponent<ScrollingMovement>().Speed = gameSpeed;
			newPipeUp.GetComponent<ScrollingMovement>().Speed = gameSpeed;
			newPipeUp.GetComponent<SpriteRenderer>().sprite = pipeSpriteUp;
		}
	}

	//Moves the tiles according to input
	private void ShiftTiles(string direction){
		bool moved = false;	//Ensures that new blocks aren't added unless some have moved
		int[] workingRowValues;
		int modifier;	//determines which direction to check for empty squares

		List<bool> mergings = new List<bool>();	//List exists to prevent multiple mergings in a single move
		for (int i=0; i < 16; i++){
			mergings.Add(false);
		}

		//first, establish values depending on which direction we're going
		switch (direction){
		case "Right":
			workingRowValues = new int[]{2, 6, 10, 14};
			modifier = 1;
			break;
		case "Left":
			workingRowValues = new int[]{1, 5, 9, 13};
			modifier = -1;
			break;
		case "Up":
			workingRowValues = new int[]{4, 5, 6, 7};
			modifier = -4;
			break;
		case "Down":
			workingRowValues = new int[]{8, 9, 10, 11};
			modifier = 4;
			break;
		default:
			workingRowValues = new int[]{0};
			modifier = 0;
			Debug.Log("Invalid direction to shift tiles: " + direction + ".  Dumbass.");
			break;
		}

		//Second, loop through every square on the board on calculate where it is free to move to, then do so
		for (int workingRow = 1; workingRow <= 3; workingRow++){
			for (int block = 0; block < workingRowValues.Length; block++){
				if (tileValues[workingRowValues[block]] != 0){
					int target = -1;
					bool merge = false;
					for (int rowToCheck = 1; rowToCheck <= workingRow; rowToCheck++){
						int next = workingRowValues[block] + (modifier * rowToCheck);
						if (tileValues[next] == 0){
							target = next;
						}
						else if (tileValues[next] == tileValues[workingRowValues[block]] && mergings[next] == false){
							target = next;
							merge = true;
						}
						else{
							break;
						}
					}
					if (target != -1){
						if (!merge){
							tileValues[target] = tileValues[workingRowValues[block]];
							tileValues[workingRowValues[block]] = 0;
							moved = true;
						}
						else{
							int mergedValue = tileValues[workingRowValues[block]] * 2;
							tileValues[target] = mergedValue;
							mergings[target] = true;	//Prevents double mergings in a single turn
							tileValues[workingRowValues[block]] = 0;
							moved = true;
							score2048 += mergedValue;
						}
					}
				}
			}

			for (int i = 0; i < workingRowValues.Length; i++){
				workingRowValues[i] -= modifier;
			}
		}

		//finally, update tile object values, generate a random new block, and check for a game over
		for (int i = 0; i < 16; i++){
			GetTileByPosition(i).NumValue = tileValues[i];
		}

		if (moved)
			GenerateRandomBlock(-1);

		if (tileValues.IndexOf(0) == -1 && collisionBuffer == false){
			LoseGame();
		}
	}
	
	//picks a free space and adds a new block
	//if parameter is -1, space is random, otherwise the space matches the parameter (for testing purposes)
	private void GenerateRandomBlock(int debug){
		List<int> freeSpaces = new List<int>();
		for (int i = 0; i < 16; i++){
			if (tileValues[i] == 0){
				freeSpaces.Add(i);
			}
		}
		int space;
		if (debug == -1)
			space = Random.Range(0, freeSpaces.Count);
		else
			space = debug;

		int newVal;
		if (Random.Range(0.0f, 1.0f) < .9)
			newVal = 2;
		else
			newVal = 4;
		tileValues[freeSpaces[space]] = newVal;
		GetTileByPosition(freeSpaces[space]).NumValue = newVal;
	}

	//Returns the Tile script for the tile at the specified position
	private TileController GetTileByPosition(int pos){
		foreach (GameObject tile in tiles){
			if (tile.GetComponent<TileController>().Position == pos){
				return tile.GetComponent<TileController>();
			}
		}
		Debug.Log("Invalid tile requested: " + pos + ".  Idiot.");
		return null;
	}
	
	public int ScoreFlappy{
		get{return scoreFlappy;}
		set{scoreFlappy = value;}
	}

	public bool CollisionBuffer{
		get{return collisionBuffer;}
		set {collisionBuffer = value;}
	}

	void OnGUI(){
		scoreTotal = score2048 * scoreFlappy;
		GUI.Box(new Rect(10, 10, 150, 60), score2048 + " * " + scoreFlappy + " = " + scoreTotal + "\n" +
		        "Last Game: " + scoreLast + "\n" +
		        "High Score " + scoreHigh);

	}
}
