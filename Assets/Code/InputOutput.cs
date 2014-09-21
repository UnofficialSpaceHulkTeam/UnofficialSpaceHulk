﻿/* 
 * The InputOutput class handles graphic representation of the map and input from the GUI and mouse clicks
 * Created by Alisdair Robertson 9/9/2014
 * Version 21-9-14.0
 * */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputOutput : MonoBehaviour {

	public GameObject FloorPiecePrefab; //Added 9/9/2014 Alisdair
	public GameObject SpaceMarinePrefab, GenestealerPrefab, BlipPrefab, OpenDoorPrefab, ClosedDoorPrefab, BlipDeploymentPiecePrefab; //Added 11/9/2014 Alisdair
	public Map mapClass; //Added 11/9/2014 Alisdair
	public Game gameClass; //Added 11/9/2014 Alisdair 

	GameObject selectedUnit; //Added by Alisdair 11/9/14

	//UI and Button References added 14/9/14 by Alisdair
	public GameObject UICanvas;
	GameObject btnAttackGO, btnShootGO, btnMoveGO, btnToggleDoorGO, btnOverwatchGO, btnRevealGO;
	Button btnAttack, btnShoot, btnMove, btnToggleDoor, btnOverwatch, btnReveal;

	//UI text field references 14 Sept 14 by Alisdair
	GameObject unitAPText, playerCPText;

	//Facing Selection canvas Added by Alisdair 17/9/2014
	public GameObject facingSelectionCanvas;
	GameObject currentFacingSelectionCanvas;

	public void instantiateUI(){ //Method Added by Alisdair Robertson 11/9/14
		/*
		 * This method creates the UI and then links the buttons to the code here
		 */ 

		Instantiate (UICanvas);//Instantiate the canvas

		//Assign the buttons (and make then not interactable)
		btnAttackGO = GameObject.Find ("BtnAttack");
		btnAttack = btnAttackGO.GetComponent<Button>();
		btnAttack.onClick.AddListener(() => {btnAttackClicked();}); //Assigning the call through code Alisdair 14/9/14 http://answers.unity3d.com/questions/777818/46-ui-calling-function-on-button-click-via-script.html
		btnAttack.interactable = false;

		btnShootGO = GameObject.Find ("BtnShoot");
		btnShoot = btnShootGO.GetComponent<Button>();
		btnShoot.onClick.AddListener(() => {btnShootClicked();});
		btnShoot.interactable = false;

		btnMoveGO = GameObject.Find ("BtnMove");
		btnMove = btnMoveGO.GetComponent<Button>();
		btnMove.onClick.AddListener(() => {btnMoveClicked();});
		btnMove.interactable = false;

		btnToggleDoorGO = GameObject.Find ("BtnToggleDoor");
		btnToggleDoor = btnToggleDoorGO.GetComponent<Button>();
		btnToggleDoor.onClick.AddListener(() => {btnToggleDoorClicked();});
		btnToggleDoor.interactable = false;;

		btnOverwatchGO = GameObject.Find ("BtnOverwatch");
		btnOverwatch = btnOverwatchGO.GetComponent<Button>();
		btnOverwatch.onClick.AddListener(() => {btnOverwatchClicked();});
		btnOverwatch.interactable = false;

		btnRevealGO = GameObject.Find ("BtnReveal");
		btnReveal = btnRevealGO.GetComponent<Button>();
		btnReveal.onClick.AddListener(() => {btnRevealClicked();});
		btnReveal.interactable = false;


		//assign the text elements
		unitAPText = GameObject.Find("APText");
		playerCPText = GameObject.Find ("CPText");

	}

	public void generateMap () { //Method Added by Alisdair Robertson 9/9/2014

		Debug.Log("Generating Map"); 

		/*
		 * The First part of this method is generating the visual map
		 */

		for (int i = 0 ; i < mapClass.map.Length ; i++){//Iterate through map list
			Square square = mapClass.map[i]; //extract square object
			Vector2 positionV2 = square.position; //get position

			//Converting Vector2 to Vector3
			//Vector2 y = Vector3 z (North/South)
			//Vector2 x = Vector3 x (East/West)
			//Vector3 y is vertical (leave at constant value)
			int xPos = (int) positionV2.x;
			int zPos = (int) positionV2.y;
			float baseYPos = -0.5f;

			GameObject floorPiece = (GameObject) Instantiate(FloorPiecePrefab, new Vector3(xPos, (baseYPos), zPos), Quaternion.identity); //Create the game object in the scene
			square.model = floorPiece; //Pass reference to the gameobject back to the square

			//Added Alisdair 11/9/2014 Theses are for passing the unit reference back to the square (if needed)
			GameObject doorPiece;
			GameObject unit;

			//Go on to create units or doors - 11/9/14 Alisdair
			//if the square has a unit or door create that unit or door on it
			if (square.isOccupied){

				//Switch to assign facing
				Quaternion facing;
				switch (square.occupant.facing){
					case Game.Facing.North:
						facing = Quaternion.identity;
						break;

					case Game.Facing.East:
						facing = Quaternion.Euler(0,90,0);
						break;

					case Game.Facing.South:
						facing = Quaternion.Euler(0,180,0);
						break;

					case Game.Facing.West:
						facing = Quaternion.Euler(0,270,0);
						break;

					default:
						facing = Quaternion.identity;
						Debug.LogError("Unable to determine direction for " + square.occupant.unitType + " @ xPos: " + xPos + " zPos: " + zPos + ". set to Quaternion.identity. Refer Alisdair");
						break;

				}

				//Switch to place units
				switch (square.occupant.unitType){
				
					case Game.EntityType.Blip:
						unit = (GameObject) Instantiate(BlipPrefab, new Vector3(xPos, (baseYPos + 1), zPos), facing); //Create the blip object above the floor object
						square.occupant.gameObject = unit; //Pass reference to the gameobject back to the square
						break;
	
					case Game.EntityType.Door:
						doorPiece = (GameObject) Instantiate(ClosedDoorPrefab, new Vector3(xPos, (baseYPos + 1), zPos), Quaternion.identity); //Create the closed door object above the floor object
						square.door.gameObject = doorPiece; //Pass reference to the gameobject back to the square
						break;
				
					case Game.EntityType.GS:
						unit = (GameObject) Instantiate(GenestealerPrefab, new Vector3(xPos, (baseYPos + 0.5f), zPos), facing); //Create the blip object above the floor object
						square.occupant.gameObject = unit; //Pass reference to the gameobject back to the square
						break;

					case Game.EntityType.SM:
						unit = (GameObject) Instantiate(SpaceMarinePrefab, new Vector3(xPos, (baseYPos + 0.5f), zPos), facing); //Create the blip object above the floor object
						square.occupant.gameObject = unit; //Pass reference to the gameobject back to the square
						break;
				}
			}
					
			//if the square has a door and it's open create it
			if (square.hasDoor && square.doorIsOpen){
				doorPiece = (GameObject) Instantiate(OpenDoorPrefab, new Vector3(xPos, (baseYPos + 0.55f), zPos), Quaternion.identity); //Create the open door object above the floor object
				square.door.gameObject = doorPiece; //Pass reference to the gameobject back to the square
			}

		}

		/*
		 * The second part of the generateMap method deals with generating the deployment points (these are not real parts of the map)
		 * Added By Alisdair 11/9/2014
		 */
			for (int i = 0 ; i < mapClass.otherAreas.Length; i++){//Iterate through map list
				DeploymentArea depArea = mapClass.otherAreas[i]; //extract square object
				Vector2 adjPos = depArea.adjacentPosition; //get position of adjecent piece
				
				//Converting Vector2 to Vector3
				//Vector2 y = Vector3 z (North/South)
				//Vector2 x = Vector3 x (East/West)
				//Vector3 y is vertical (leave at constant value)
				int xPos = (int) adjPos.x;
				int zPos = (int) adjPos.y;
				float baseYPos = -0.5f;

				//determine the position of the deployment area based on the facing 
				switch (depArea.relativePosition){

					case Game.Facing.North:
						zPos--;
						break;

					case Game.Facing.East:
						xPos--;
						break;

					case Game.Facing.South:
						zPos++;
						break;

					case Game.Facing.West:
						xPos++;
						break;
					default:
						Debug.LogError("No valid relative position assigned to deployment piece adjacent to xPos: " + xPos + " zPos: " + zPos);
						break;
				}

				Quaternion depAreaFacing = Quaternion.Euler(0,0,0);
				
				GameObject deploymentPiece = (GameObject) Instantiate(BlipDeploymentPiecePrefab, new Vector3(xPos, baseYPos, zPos), depAreaFacing); //Create the game object in the scene
			}


	}

	public void showActionSequence(Action[] actions){
		Debug.LogError("showActionSequence method INCOMPLETE. Refer Alisdair.");
	}

	public void selectUnit (GameObject unit, Game.ActionType[] actions){ //Filled by Alisdair 11/9/2014
		Debug.Log("Selecting Unit: " + unit.ToString());
		/*
		 * Set the display to be appropriate to the selection of this unit, as well as showing/enabling the buttons for the action types.
		 */

		//deselect any previously selected units (if there are any)
		if (selectedUnit != null) {
			deselect ();
		}

		//assign the variable to the new unit
		selectedUnit = unit;

		//colour the selectedUnit unit
		selectedUnit.renderer.material.color = Color.cyan;

		//update the GUI actions
		updateGUIActions(actions);
	}

	public void deselect(){ //Filled by Alisdair 11/9/2014
		Debug.Log("Deselecting selected units...");
		/*
		 * This method removes the mesh renderer tint on the selected unit
		 */

		//set the render colour on the selected object back to nothing (if there is a selected unit)
		//Must change this to a tint later, rather than a full material colour
		if (selectedUnit != null) {
			selectedUnit.renderer.material.color = Color.white;

			selectedUnit = null;

			//set the gui to show no actions
			updateGUIActions();
		} 
		else {
			Debug.LogWarning ("There is not a unit selected.");
		}

	}

	public void showDeployment(Unit[] units, Vector2[] positions){
		Debug.LogError("ShowDeployment method INCOMPLETE. Refer Alisdair.");
	}

	public void placeUnit(Unit unit){
		Debug.LogError("placeUnit method INCOMPLETE. Refer Alisdair.");
	}

	public void removeUnit(Vector2 position){
		Debug.LogError("removeUnit method INCOMPLETE. Refer Alisdair.");
	}

	public void resetMap(){

		//Added removing old gameobjects to this method - Alisdair 19-9-2014

		Debug.Log ("Resetting Map - Removing GameObjects");

		for (int i = 0; i < mapClass.map.Length; i++) {

			Square square = (Square) mapClass.map.GetValue(i);

			try{
				Destroy(square.occupant.gameObject);
			}
			catch (UnityException ex)
			{
				Debug.Log("Exception - no occupant at Position: " + square.position);
			}

			try{
				Destroy (square.door.gameObject);
			}
			catch (UnityException ex)
			{
				Debug.Log("Exception - no door at Position: " + square.position);
			}

			try{
				Destroy(square.model);
			}
			catch (UnityException ex)
			{
				Debug.Log("Exception - no model at Position: " + square.position);
			}
		}

		Debug.Log("Resetting Map - Calling generateMap()");
		//Alisdair 11/Sept/2014
		generateMap (); //Rerun generate map so that it matches the map class again
	}

	void updateGUIActions(){
		/*
		 * This method disables all the action buttons on the GUI
		 */ 
		Debug.Log("Disabling All UI Action Buttons");

		//make the buttons uninteractable
		btnAttack.interactable = false;
		btnShoot.interactable = false;
		btnMove.interactable = false;
		btnToggleDoor.interactable = false;;
		btnOverwatch.interactable = false;
		btnReveal.interactable = false;
	}

	void updateGUIActions(Game.ActionType[] actions){
		/*
		 * This method is for updating the GUI command buttons to reflect the commands that are allowed for the specific unit
		 */
		Debug.Log("Enabling requested UI Action Buttons:");

		//disable all the buttons
		updateGUIActions ();

		//loop throguh all the objects in the list and enable the required buttons
		Debug.Log ("Button action list is of Length: " + actions.Length);
		for (int i = 0; i < actions.Length; i++) {
			Debug.Log ("Testing Action " + i + ": " + actions[i]);

			switch (actions[i]){
			case Game.ActionType.Attack:
				btnAttack.interactable = true;
				Debug.Log("Enabling Attack Button");
				break;
			case Game.ActionType.Move:
				btnMove.interactable = true;
				Debug.Log("Enabling Move Button");
				break;
			case Game.ActionType.Overwatch:
				btnOverwatch.interactable = true;
				Debug.Log("Enabling Overwatch Button");
				break;
			case Game.ActionType.Reveal:
				btnReveal.interactable = true;
				Debug.Log("Enabling Reveal Button");
				break;
			case Game.ActionType.Shoot:
				btnShoot.interactable = true;
				Debug.Log("Enabling Shoot Button");
				break;
			case Game.ActionType.ToggleDoor:
				btnToggleDoor.interactable = true;
				Debug.Log("Enabling ToggleDoor Button");
				break;
			default:
				Debug.LogError("There was not an ActionType at position: " + i + " in the actions Array. updateGUIActions(Game.ActionType[] actions)");
				break;
			}
		}
	}

	public void btnAttackClicked(){ //Added By Alisdair 14/9/14
		/*
		 * This method needs to pass the button click back to the Game class so that action can be taken
		 */ 
		Debug.LogWarning ("Attack Button Clicked, this method is INCOMPLETE. Refer Alisdair");
	}

	public void btnMoveClicked(){ //Added By Alisdair 14/9/14
		/*
		 * This method needs to pass the button click back to the Game class so that action can be taken
		 */ 
		Debug.LogWarning ("Move Button Clicked, this method is INCOMPLETE. Refer Alisdair");
	}

	public void btnOverwatchClicked(){ //Added By Alisdair 14/9/14
		/*
		 * This method needs to pass the button click back to the Game class so that action can be taken
		 */ 
		Debug.LogWarning ("Overwatch Button Clicked, this method is INCOMPLETE. Refer Alisdair");
	}

	public void btnRevealClicked(){ //Added By Alisdair 14/9/14
		/*
		 * This method needs to pass the button click back to the Game class so that action can be taken
		 */ 
		Debug.LogWarning ("Reveal Button Clicked, this method is INCOMPLETE. Refer Alisdair");
	}

	public void btnShootClicked(){ //Added By Alisdair 14/9/14
		/*
		 * This method needs to pass the button click back to the Game class so that action can be taken
		 */ 
		Debug.LogWarning ("Shoot Button Clicked, this method is INCOMPLETE. Refer Alisdair");
	}

	public void btnToggleDoorClicked(){ //Added By Alisdair 14/9/14
		/*
		 * This method needs to pass the button click back to the Game class so that action can be taken
		 */ 
		Debug.LogWarning ("ToggleDoor Button Clicked, this method is INCOMPLETE. Refer Alisdair");
	}

	//METHODS ADDED 17-9-14

	//Method to create a facing selection canvas (at a specified position)
	// - Need to add new gameobject decleration to the top of this class for the button canvas
	// Needs to assign methods to call to the buttons
	public void instantiateFacingSelection(Vector2 position){
		Debug.Log ("Creating facing selection buttons @: " + position + " -Alisdair");

		//Create the canvas at the position
		currentFacingSelectionCanvas = (GameObject) Instantiate (facingSelectionCanvas, makePosition(position, 2), Quaternion.Euler (90, 0, 0));

		//Assign methods to the buttons
		Button btnNorth = GameObject.Find ("BtnNorth").GetComponent<Button>();
		btnNorth.onClick.AddListener(() => {btnFaceNorth();});
		Button btnEast = GameObject.Find ("BtnEast").GetComponent<Button>();
		btnEast.onClick.AddListener(() => {btnFaceEast();});
		Button btnSouth = GameObject.Find ("BtnSouth").GetComponent<Button>();
		btnSouth.onClick.AddListener(() => {btnFaceSouth();});
		Button btnWest = GameObject.Find ("BtnWest").GetComponent<Button>();
		btnWest.onClick.AddListener(() => {btnFaceWest();});
	}

	//Methods for buttons
	//- each button needs to call a method on the selection/input class to tell it what button has been clicked
	// - the selection canvas that is the parent of the button then needs to be destroyed.
	public void btnFaceNorth(){
		Debug.LogWarning("BtnFaceNorth Clicked, no method assigned to call (ASK ALISDAIR AND RORY. Destroying canvas");
		Destroy(currentFacingSelectionCanvas);
	}
	public void btnFaceEast(){
		Debug.LogWarning("BtnFaceEast Clicked, no method assigned to call (ASK ALISDAIR AND RORY. Destroying canvas");
		Destroy(currentFacingSelectionCanvas);

	}
	public void btnFaceSouth(){
		Debug.LogWarning("BtnFaceSouth Clicked, no method assigned to call (ASK ALISDAIR AND RORY. Destroying canvas");
		Destroy(currentFacingSelectionCanvas);
		
	}
	public void btnFaceWest(){
		Debug.LogWarning("BtnFaceWest Clicked, no method assigned to call (ASK ALISDAIR AND RORY. Destroying canvas");
		Destroy(currentFacingSelectionCanvas);
		
	}

	//Method to convert vector 2 to vector 3
	Vector3 makePosition(Vector2 position, float elevation){
		int xPos = (int) position.x;
		int zPos = (int) position.y;
		Vector3 v3 = new Vector3 (xPos, elevation, zPos); 
		return v3;

	}
	
}