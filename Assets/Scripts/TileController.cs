using UnityEngine;
using System.Collections;

//Controls 2048 tile properties
public class TileController : MonoBehaviour {
	public int position;	//1-16, top to bottom, left to right
	public Sprite sprite0;
	public Sprite sprite2;
	public Sprite sprite4;
	public Sprite sprite8;
	public Sprite sprite16;
	public Sprite sprite32;
	public Sprite sprite64;
	public Sprite sprite128;
	public Sprite sprite256;
	public Sprite sprite512;
	public Sprite sprite1024;
	public Sprite sprite2048;

	private int numValue;
	private SpriteRenderer mySprite;

	public int Position{
		get{return position;}
		set{position = value;}
	}

	public int NumValue{
		get{return numValue;}
		set{
			numValue = value;
			mySprite = GetComponent<SpriteRenderer>();
			switch(numValue){
			case 0:
				mySprite.sprite = sprite0;
				break;
			case 2:
				mySprite.sprite = sprite2;
				break;
			case 4:
				mySprite.sprite = sprite4;
				break;
			case 8:
				mySprite.sprite = sprite8;
				break;
			case 16:
				mySprite.sprite = sprite16;
				break;
			case 32:
				mySprite.sprite = sprite32;
				break;
			case 64:
				mySprite.sprite = sprite64;
				break;
			case 128:
				mySprite.sprite = sprite128;
				break;
			case 256:
				mySprite.sprite = sprite256;
				break;
			case 512:
				mySprite.sprite = sprite512;
				break;
			case 1024:
				mySprite.sprite = sprite1024;
				break;
			case 2048:
				mySprite.sprite = sprite2048;
				break;
			default:
				Debug.Log("Unrecognized tile value: " + value + ".  Do Better");
				break;
			}
		}
	}
}
