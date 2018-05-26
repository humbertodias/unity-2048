using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.ComponentModel;


public enum GameState{
	Playing,
	GameOver,
	WaitingForMoveToEnd
}

public class GameManager : MonoBehaviour {


	// NEW AFTER ADD DELAYS
	public GameState State;

	[Range(0,2f)]
	public float delay = 0.8f;

	private bool moveMade;
	private bool[] lineMoveComplete;// = new bool[4]{true,true,true,true};
	// NEW AFTER ADD DELAYS


	public int RowsCount = 4;

	public GameObject YouWonText;
	public GameObject GameOverText;
	public Text GameOverScoreText;
	public GameObject GameOverPanel;

	public GameObject GameField;
	public Tile tilePrefab;


	public Tile[,] AllTiles;// = new Tile[4, 4];
	private List<Tile[]> columns = new List <Tile[]>();
	private List<Tile[]> rows = new List <Tile[]>();


	public List<Tile> EmptyTiles = new List<Tile>();

	public Dropdown dropDownLevel;
	private int MaxMatch;
	private int[] levels = new int[]{ 2048, 4096, 8192, 16384, 32768, 65536 };


	public AudioClip audioWon;
	public AudioClip audioGameOver;
	public AudioClip audioMoveTile;
	public AudioClip audioMergeTile;
	public AudioClip musicBackground;

	// Use this for initialization
	void Start () {
		Board ();

		Generate ();
		Generate ();
	}

	void Reset(){
		columns.Clear();
		rows.Clear();
		EmptyTiles.Clear();
		ScoreTracker.Instance.Score = 0;
		DestroyAllTiles ();

		Start ();
	}

	void DestroyAllTiles(){
		Tile [] allChildrenTiles = GameField.GetComponentsInChildren<Tile> ();
		foreach (Tile tile in allChildrenTiles) {
			Destroy (tile.gameObject);
		}
	}

	void  Board(){

		lineMoveComplete = new bool[RowsCount];
		for(int i=0; i< lineMoveComplete.Length; i++){
			lineMoveComplete [i] = true;
		}
		AllTiles = new Tile[RowsCount, RowsCount];


		for (int i = 0; i < RowsCount; i++) {
			for (int j = 0; j < RowsCount; j++) {
				Tile tile = Instantiate (tilePrefab, transform.position, Quaternion.identity, GameField.transform) as Tile;
				tile.name = "Tile(" + i + "," + j + ")";
				tile.Number = 0;

				AllTiles [i, j] = tile;
				EmptyTiles.Add (tile);
			}
		}


		// rows
		{
			int i, j;
			List<Tile> lRows = new List<Tile> ();
			for (i = 0; i < RowsCount; ) {
				for (j = 0; j < RowsCount; j++) {
					lRows.Add (AllTiles [i, j]);
				}
				i++;
				rows.Add (lRows.ToArray ());
				lRows.Clear ();
			}
		}

		// Cols
		{
			int i, j;
			List<Tile> lCols = new List<Tile>();
			for (i = 0; i < RowsCount; i++) {
				for (j = 0; j < RowsCount; j++) {
					lCols.Add (AllTiles [j, i]);
				}
				columns.Add (lCols.ToArray ());
				lCols.Clear ();
			}
		}

	}

	private void YouWon(){
		AudioManager.Instance.PlaySFX (audioWon);
		GameOverText.SetActive ( false );
		YouWonText.SetActive( true );
		GameOverScoreText.text = ScoreTracker.Instance.Score.ToString ();
		GameOverPanel.SetActive (true);
	}

	private void GameOver(){
		AudioManager.Instance.PlaySFX (audioGameOver);
		GameOverScoreText.text = ScoreTracker.Instance.Score.ToString ();
		GameOverPanel.SetActive (true);
	}

	bool CanMove (){
		if (EmptyTiles.Count > 0) {
			return true;
		} else {
			
			// check columns x rows
			for (int i = 0; i < columns.Count; i++) {
				for (int j = 0; j < rows.Count-1; j++) {
					if (AllTiles [j, i].Number == AllTiles [j + 1, i].Number) {
						return true;
					}
				}
			}

			// check rows y columns
			for (int i = 0; i < rows.Count; i++) {
				for (int j = 0; j < columns.Count-1; j++) {
					if (AllTiles [i, j].Number == AllTiles [i, j+1].Number) {
						return true;
					}
				}
			}

		}
		return false;
	}

	public void NewGameHandler(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	bool MakeOneMoveOnDownIndex(Tile[] LineOfTiles){
		for (int i = 0; i < LineOfTiles.Length - 1; i++) {

			// MERGE BLOCK
			if(LineOfTiles[i].Number == 0 && LineOfTiles[i+1].Number != 0){
				LineOfTiles [i].Number = LineOfTiles [i + 1].Number;
				LineOfTiles [i+1].Number = 0;
				return true;
			}
			// MERGE BLOCK

			if (LineOfTiles [i].Number != 0 && 
			    LineOfTiles [i].Number == LineOfTiles [i + 1].Number &&
			    LineOfTiles [i].mergeThisReturn == false && LineOfTiles [i+1].mergeThisReturn == false) {
				LineOfTiles [i].Number *= 2;
				LineOfTiles [i+1].Number = 0;
				LineOfTiles [i].mergeThisReturn = true;

				Merge (LineOfTiles, i);

				if (LineOfTiles [i].Number == MaxMatch) {
					YouWon ();
				}

				return true;
			}

		}

		return false;
	}

	bool MakeOneMoveOnUpIndex(Tile[] LineOfTiles){
		for (int i = LineOfTiles.Length-1; i > 0; i--) {

			// MERGE BLOCK
			if(LineOfTiles[i].Number == 0 && LineOfTiles[i-1].Number != 0){
				LineOfTiles [i].Number = LineOfTiles [i - 1].Number;
				LineOfTiles [i-1].Number = 0;
				return true;
			}
			// MERGE BLOCK

			if (LineOfTiles [i].Number != 0 && 
				LineOfTiles [i].Number == LineOfTiles [i - 1].Number &&
				LineOfTiles [i].mergeThisReturn == false && LineOfTiles [i-1].mergeThisReturn == false) {
				LineOfTiles [i].Number *= 2;
				LineOfTiles [i-1].Number = 0;
				LineOfTiles [i].mergeThisReturn = true;

				Merge (LineOfTiles, i);

				if (LineOfTiles [i].Number == MaxMatch) {
					YouWon ();
				}

				return true;
			}

		}
		return false;
	}

	private void Merge(Tile[] LineOfTiles, int index){
		AudioManager.Instance.PlaySFX (audioMergeTile);
		LineOfTiles [index].PlayMergedAnimation ();
		ScoreTracker.Instance.Score += LineOfTiles [index].Number;
	}

	private void UpdateEmptyTiles(){

		EmptyTiles.Clear ();

		foreach (Tile t in AllTiles) {
			if (t.Number == 0) {
				EmptyTiles.Add (t);
			}
		}

	}


	private void ResetMergeFlags(){
		foreach (Tile t in AllTiles) {
			t.mergeThisReturn = false;
		}
	}


	void Generate(){
		if (EmptyTiles.Count > 0) {
			int indexForNewNumber = Random.Range (0, EmptyTiles.Count);

			int randomNumber = Random.Range (0, 10);
			if (randomNumber == 0)
				EmptyTiles [indexForNewNumber].Number = 4;
			else
				EmptyTiles [indexForNewNumber].Number = 2;

			EmptyTiles [indexForNewNumber].PlayAppearAnimation ();

			EmptyTiles.RemoveAt (indexForNewNumber);
		}
	}

	public void Move(MoveDirection md){
		Debug.Log (md.ToString () + " move.");
		moveMade = false;

		ResetMergeFlags ();

		AudioManager.Instance.PlaySFX (audioMoveTile);

		if (delay > 0) {
			StartCoroutine (MoveCoroutine (md));
		} else {
			for (int i = 0; i < rows.Count; i++) {
				switch (md) {
				case MoveDirection.Down:
					while (MakeOneMoveOnUpIndex (columns [i])) {
						moveMade = true;
					}
					break;
				case MoveDirection.Left:
					while (MakeOneMoveOnDownIndex (rows [i])) {
						moveMade = true;
					}
					break;
				case MoveDirection.Right:
					while (MakeOneMoveOnUpIndex (rows [i])) {
						moveMade = true;
					}
					break;
				case MoveDirection.Up:
					while (MakeOneMoveOnDownIndex (columns [i])) {
						moveMade = true;
					}
					break;
				}
			}
		}

		AfterMoveMade ();
	}

	private void AfterMoveMade(){
		if (moveMade) {

			UpdateEmptyTiles ();
			Generate ();

			if (!CanMove ()) {
				GameOver ();
			}

		}
	}

	IEnumerator MoveOneLineUpIndexCoroutine(Tile[] line, int index){
	
		lineMoveComplete [index] = false;
		while (MakeOneMoveOnUpIndex (line)) {
			moveMade = true;
			yield return new WaitForSeconds (delay);
		}
		lineMoveComplete [index] = true;

	}

	IEnumerator MoveOneLineDownIndexCoroutine(Tile[] line, int index){

		lineMoveComplete [index] = false;
		while (MakeOneMoveOnDownIndex (line)) {
			moveMade = true;
			yield return new WaitForSeconds (delay);
		}
		lineMoveComplete [index] = true;

	}


	IEnumerator MoveCoroutine(MoveDirection md){

		State = GameState.WaitingForMoveToEnd;

		switch (md) {
		case MoveDirection.Down:
			for(int i=0; i<columns.Count; i++) 
				StartCoroutine(MoveOneLineUpIndexCoroutine (columns [i], i));
			break;
		case MoveDirection.Left:
			for(int i=0; i<rows.Count; i++) 
				StartCoroutine(MoveOneLineDownIndexCoroutine (rows [i], i));
			break;
		case MoveDirection.Right:
			for(int i=0; i<rows.Count; i++) 
				StartCoroutine(MoveOneLineUpIndexCoroutine (rows [i], i ));
			break;
		case MoveDirection.Up:
			for(int i=0; i<columns.Count; i++) 
				StartCoroutine(MoveOneLineDownIndexCoroutine (columns [i], i));
			break;
		}

		bool AllLineMoveComplete = false;
		while (!AllLineMoveComplete) {
			AllLineMoveComplete = true;
			for (int i = 0; i < lineMoveComplete.Length; i++) {
				if (lineMoveComplete [i] == false)
					AllLineMoveComplete = false;
			}
				
			yield return null;
		}


		AfterMoveMade ();

		State = GameState.Playing;
		StopAllCoroutines ();

	}


	public void SoundOnOff(){
		AudioManager.Instance.MuteSoundToggle ();
	}

	public void MusicOnOff(){
		AudioManager.Instance.MuteMusicToggle ();
	}


	public void ChangeLevel(int indexLevel){
		Debug.Log ("ChangeLevel: index[" + indexLevel + "]=" + levels[indexLevel] );
		MaxMatch = levels[indexLevel];
		Reset ();
	}

}
