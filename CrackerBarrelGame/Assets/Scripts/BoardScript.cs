using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BoardScript : MonoBehaviour {

	//Rows of gameobjects for game logic 
	//Row0 is the top, row 4 is the bottom row 
	//The more left an object in the row is, the lower the index (leftmost is inedex 0)
	public GameObject[] row0 = new GameObject[1];
	public GameObject[] row1 = new GameObject[2];
	public GameObject[] row2 = new GameObject[3];
	public GameObject[] row3 = new GameObject[4];
	public GameObject[] row4 = new GameObject[5];
	//arraylist to hold all of the rows
	private List<GameObject[]> board = new List<GameObject[]>();

	//playGame() variables 
	//Must be public 
	public GameObject previousSelectedPeg; 
	public GameObject selectedPeg;
	private PegScript selectedPegScript; 
	//code only works when these variables are public, don't know why
	public List<GameObject> validMoves;
	public List<GameObject> previousValidMoves;

	//checkSelected() variables 
	private PegScript currentPegScript;
	private PegScript previousPegScript;
	private GameObject currentPeg;
	private GameObject previousPeg;

	//checkValidMoves() variables
	private PegScript checkPegScript; 
	private GameObject middlePeg;
	private PegScript middlePegScript;
	private int col;
	int row;

	//removePeg() variables
	private int startRow;
	private int startCol;
	private int endRow;
	private int endCol; 
	private int averageRow;
	private int averageCol;
	private GameObject pegToRemove;
	private PegScript removeScript;

	//setValidMoves() variables
	private PegScript setMoveScript;

	//checkForMoves() variables
	//Must be public, again, don't know why 
	public List<GameObject> moveList; 
	private PegScript pegScript;

	//checkForWin() variables
	private int pegNum;
	private PegScript winPegScript; 

	//setPegNum() variables 
	public Text pegsText;
	private int pegsLeft; 

	//UI variables 
	public Text gameEndText;
	public Button resetButton; 
	private Button button; 


	// Use this for initialization
	void Start (){
		//initialize board 
		board.Add (row0);
		board.Add (row1);
		board.Add (row2);
		board.Add (row3);
		board.Add (row4);

		//initialized UI
		pegsText.text = "14";
		gameEndText.enabled = false; 
		button = resetButton.GetComponent<Button> (); 
		button.onClick.AddListener (TaskOnClick);
	}
	
	// Update is called once per frame
	void Update () {

		//playGame ();

		if (Input.GetMouseButtonDown (0)) {
			playGame ();
			setPegNum ();
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			Application.Quit ();
		}

	}
	//play game
	void playGame()
	{
		//Check for the selected peg 
		previousSelectedPeg = selectedPeg;
		selectedPeg = checkSelectedPeg();
		//get a list of all the possible valid locations 
		if (selectedPeg != null) {
			//check if contains a peg 
			selectedPegScript = selectedPeg.GetComponent<PegScript>();
			if (selectedPegScript.getHasPeg () == true) {
				//Debug.Log ("selection is a peg");

				if (previousValidMoves != null) {
					previousValidMoves = validMoves;
					clearPreviousMoves (previousValidMoves);
				}
				validMoves = checkValidMoves (selectedPeg);
				setValidMoves (validMoves);

			}
			//remove peg and set current peg to this one
			else if (selectedPegScript.getHasPeg() == false){
				//Debug.Log ("selection is a hole");
				selectedPegScript.setHasPeg (true);
				selectedPegScript.setIsSelected (true);
				selectedPegScript.setPegSelected ();
				removePeg (selectedPeg, previousSelectedPeg);
				//then show the currently available moves 
				previousValidMoves = validMoves;
				clearPreviousMoves(previousValidMoves);
				validMoves = checkValidMoves (selectedPeg);
				setValidMoves (validMoves);
			}


			if (checkForWin ()) {
				gameEndText.enabled = true; 
				gameEndText.text = "You won!";
			}
			if (!checkForMoves ()) {
				gameEndText.enabled = true; 
				gameEndText.text = "You Lost";
			}

		}

		//check for if the player clicked on one of the valid locations 

		//move the peg and delete the one in between
	}

	//checks all of the game objects to search for isSelected = true
	//restricts selection to one peg only 
	GameObject checkSelectedPeg()
	{
		for (int i = 0; i < board.Count; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				currentPeg = board [i] [j];
				currentPegScript = board [i] [j].GetComponent<PegScript> ();
				//checks if the selection is a peg 
				if (currentPegScript.getIsSelected() == true && currentPegScript.getHasPeg() == true) {
					if (currentPeg.Equals(previousPeg)) {
						//continues the loop to check for a new peg 
						continue;
					} else {
						//set previous peg to not selected 
						if (previousPeg != null) {
							previousPegScript = previousPeg.GetComponent<PegScript>();
							previousPegScript.setIsSelected (false);
							previousPegScript.setPeg();
						}
						currentPegScript.setPegSelected ();
						previousPeg = currentPeg;
						return currentPeg;
					}
				}

				//checks if the selection is an empty space
				else if (currentPegScript.getIsSelected() == true && currentPegScript.getHasPeg() == false){
					//if its a valid move 
					if (validMoves != null) {
						if (validMoves.Contains (currentPeg)) {
							//set the previous peg to empty 
							if (previousPeg != null) {
								previousPegScript = previousPeg.GetComponent<PegScript> ();
								previousPegScript.setIsSelected (false);
								previousPegScript.setHasPeg (false);
								previousPegScript.setEmpty ();
							}
							currentPegScript.setPeg ();
							previousPeg = currentPeg;
							return currentPeg;
						}
						//if its not a valid move, remove the isSelected return the seleted peg 
						else {
							currentPegScript.setIsSelected (false);
							return selectedPeg;
						}
					}

				}

			}
		}
		//always return the current peg in case of errant clicks
		return selectedPeg;
	}

	//Checks for valid moves based on the selected peg, returns a list of the valid gameobjects 
	List<GameObject> checkValidMoves(GameObject peg)
	{
		int.TryParse(peg.name.Substring (3, 1), out row);
		int.TryParse(peg.name.Substring(5, 1), out col);

		//valid moves would be anything that are two spots away from it, as long as it is still within the bounds of the board
		List<GameObject> locationList = new List<GameObject>(); 
		//position 0 - row
		//position 1 - col
		int[] validLocation = new int[2]; 
		//start looking for 2 rows above, then check current row, then check two rows below 
		for (int i = row - 2; i <= row + 2; i+=2) {
			//make sure i is still in the bounds of the board 
			if (i < 0) {
				continue; 
			}
			if (i >= board.Count) {
				continue;
			}
			validLocation [0] = i;
			for (int j = col - 2; j <= col + 2; j += 2) {
				//make sure j is still in the bounds of the board
				if (j < 0) {
					continue; 
				}
				if (j >= board [i].Length) {
					continue; 
				}
				validLocation [1] = j;

				//checks and makes sure you're not trying to access the current location 
				if (validLocation [0] == row && validLocation [1] == col) {
					continue; 
				}

				//check and see if the spot is empty 
				checkPegScript = board[i][j].GetComponent<PegScript>();
				if (checkPegScript.getHasPeg() == false) {
					//ROW4_0 breaks the algrorithm -> Check for this one edge case 
					if (peg.name.Equals("Row4_0") && i == 2 && j == 2) {
						continue;
					}
					//if there is a peg in between them then it is a valid move 
					middlePeg = getPegInbetween(peg, board[i][j]);
					middlePegScript = middlePeg.GetComponent<PegScript> ();
					if (middlePegScript.getHasPeg () == true){
						locationList.Add (board [i] [j]);
					}
				}
			}
		}
		return locationList;
	}

	//sets selected for everything in the checkValid List 
	void setValidMoves(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++) {
			setMoveScript = list [i].GetComponent<PegScript> ();
			setMoveScript.setBoardSelected ();
		}
	}

	//clears previous valid moves when a new selection has been made 
	void clearPreviousMoves(List<GameObject> moves)
	{
		PegScript script; 
		for (int i = 0; i < moves.Count; i++) {
			if (moves [i].Equals (selectedPeg)) {
				//skip the current peg 
				continue;
			}
			script = moves [i].GetComponent<PegScript> ();
			script.setEmpty ();
		}
	}

	//clears a peg in between the previous one and the new one 
	void removePeg(GameObject startPeg, GameObject endPeg)
	{
		pegToRemove = getPegInbetween (startPeg, endPeg);
		//remove peg 
		removeScript = pegToRemove.GetComponent<PegScript>();
		removeScript.setHasPeg (false);
		removeScript.setEmpty ();
	}

	GameObject getPegInbetween(GameObject startPeg, GameObject endPeg)
	{
		int.TryParse(startPeg.name.Substring (3, 1), out startRow);
		int.TryParse (startPeg.name.Substring (5, 1), out startCol);
		int.TryParse (endPeg.name.Substring (3, 1), out endRow);
		int.TryParse (endPeg.name.Substring (5, 1), out endCol);

		//find the peg inbetween the two 
		averageRow = (startRow + endRow) / 2;
		averageCol = (startCol + endCol) / 2;

		return board [averageRow] [averageCol];
	}

	//checks to see if there are any valid moves yet -> helps determine if its the end of the game or not 
	bool checkForMoves()
	{
		for (int i = 0; i < board.Count; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				pegScript = board [i] [j].GetComponent<PegScript> ();
				if (pegScript.hasPeg == true) {
					moveList = checkValidMoves (board [i] [j]);
					if (moveList.Count > 0) {
						return true; 
					}
					moveList.Clear ();
				}
			}
		}
		return false; 
	}

	//check for a win condition of one peg left 
	bool checkForWin()
	{
		pegNum = 0;
		for (int i = 0; i < board.Count; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				winPegScript = board [i] [j].GetComponent<PegScript> ();
				if (winPegScript.getHasPeg () == true) {
					pegNum++;
				}
				if (pegNum > 1) {
					return false; 
				}
			}
		}
		return true;
	}

	//set the number of pegs left in the canvas 
	void setPegNum()
	{
		pegsLeft = 0;
		for (int i = 0; i < board.Count; i++) {
			for (int j = 0; j < board [i].Length; j++) {
				winPegScript = board [i] [j].GetComponent<PegScript> ();
				if (winPegScript.getHasPeg () == true) {
					pegsLeft++;
				}
			}
		}

		pegsText.text = pegsLeft.ToString();
	}

	//Button click to reset the level 
	void TaskOnClick()
	{
		SceneManager.LoadScene (0);
	}
}
