using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection{
	Left, Right, Up, Down
}

public class InputManager : MonoBehaviour {

	private GameManager gm;


	void Awake(){
		gm = GetComponent<GameManager> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gm.State == GameState.Playing) {
			if (Input.GetKey (KeyCode.LeftArrow)) {
				gm.Move (MoveDirection.Left);
			} else if (Input.GetKey (KeyCode.RightArrow)) {
				gm.Move (MoveDirection.Right);
			} else if (Input.GetKey (KeyCode.UpArrow)) {
				gm.Move (MoveDirection.Up);
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				gm.Move (MoveDirection.Down);
			}

		}
	}
}
