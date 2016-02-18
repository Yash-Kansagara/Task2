using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {
	
	public Camera cam;

	// reference to map object
	public Map map;
	// # of player moves
	int moves;

	// to display status
	public TextMesh movesText;
	public TextMesh gameMessege;

	// number of blocks destroyed
	int blocksRemoved = 0;

	// user input status
	public bool inputEnable;

	IEnumerator Start () {
		// reset moves
		moves = 0;
		// delay to ensure blocks are creates in Map script
		yield return 0;
		// create groups which share material to highlight them
		GenerateGroups ();
		// enable the input
		inputEnable = true;
	}

	// to track active focus and reset last focused group
	Group lastActive;
	Color lastColor;


	void Update () {
		
		if(inputEnable){
			// tracking mouse position over blocks to highlight a group

			Vector3 pos = Input.mousePosition;
			pos = cam.ScreenToWorldPoint (pos);
			
			int x = (int)(pos.x );
			int y = (int)(pos.y);

			// if pointer is outside of map
			if(pos.x < 0 || pos.y < 0 || x > 15 || y > 15){
				if(lastActive != null)
					lastActive.mat.color = lastColor; 
				lastActive = null;
				return;
			}

			// if mouse is on same group
			if (map.blocks [y] [x] && map.blocks [y] [x].group == lastActive) {
				#if UNITY_EDITOR
				UnityEditor.Selection.activeTransform = map.blocks [y] [x].trans;
				#endif
			// if mouse is on removed block
			} else if (map.blocks [y] [x] == null) {
				if (lastActive != null) {
					lastActive.mat.color = lastColor;
					lastActive = null;
				}
			// if mouse swithces from current group to other group
			}else {
				
				if (lastActive != null) {
				
					lastActive.mat.color = lastColor;
				}

				if(map.blocks[y][x]){

					#if UNITY_EDITOR
					UnityEditor.Selection.activeTransform = map.blocks [y] [x].trans;
					#endif

					// if group has more than 2 members highlight it
					if (map.blocks [y] [x].group.blocks.Count > 2) {
						lastColor = map.blocks [y] [x].group.mat.color;
						map.blocks [y] [x].group.mat.color = Color.white;
						lastActive = map.blocks [y] [x].group;
					} else {
						lastActive = null;
					}
				}
			}
		}

		// if clicked on a group
		if(inputEnable && Input.GetMouseButtonDown(0)){
			if (lastActive == null)
				return;

			moves++;
			movesText.text = moves.ToString ();

			// disable all blocks in that group and remove reference from map
			for (int i = 0; i < lastActive.blocks.Count; i++) {
				Block b = lastActive.blocks [i];
				b.gameObject.SetActive (false);
				blocksRemoved++;
				if (b != null)
					map.blocks [b.y] [b.x] = null;
			}
			lastActive = null;

			// if there is only one group and all the blocks are removed, Game complete !
			if (groups.Count == 1 && blocksRemoved == 256) {
				gameMessege.text = "Game Complete !";
				inputEnable = false;
				return;

			// if any of the group has more than 2 blocks, we can still play
			} else {
				bool noMovesLeft = true;
				for (int i = 0; i < groups.Count; i++) {
					if (groups [i].blocks.Count > 2)
						noMovesLeft = false;
				}

				if (noMovesLeft) {
					gameMessege.text = "No Moves Left";
					inputEnable = false;
				}
			}

			// fill the space created by removing blocks
			StartCoroutine (moveDown());
		}


	}


	/*
		movement is spread over several frames which reduces too much computation in single frame
	*/
	IEnumerator moveDown(){
		// disable input
		inputEnable = false;
			
			
			bool needToMove = false;
			// row loop
			for (int i = 0; i < 16; i++) {
				needToMove = false;
				// column loop
				for (int j = 0; j < 16; j++) {

					// if block is null shift all above blocks down
					if(map.blocks[i][j] == null){
						
						// above blocks loop
						for (int k = i+1; k < 16; k++) {
							
							// update map and block data
							if (map.blocks [k] [j] != null) {
								needToMove = true;	
								map.blocks [k] [j].y--;
								map.blocks [k-1] [j] = map.blocks [k] [j];
								map.blocks [k] [j] = null;
								map.blocks [k-1] [j].UpdatePositionFromIndex ();
								//yield return new WaitForSeconds(0.05f);
							}
						}
					}

				}
				// wait for a frame before moving to next column
				if (needToMove) {
					yield return 0;
					// stay on same row because row is updated
					i--;
				}
			}

		// Move towards center if there is an empty column
		yield return StartCoroutine (moveSideWays());

		// create new groups after movement finishes
		GenerateGroups ();

		// enable user input
		inputEnable = true;
	}

	// movement spread over several frames, similar to vertical movement
	IEnumerator moveSideWays(){
		
		inputEnable = false;

		bool needToMoveLeft = false;
		bool needToMoveRight = false;

		// left half and right half row loop
		for (int jLeft = 7, jRight = 8; jLeft >= 0 ; jLeft--, jRight++) {
			needToMoveLeft = false;
			// Left column loop
			for (int i = 0; i < 16; i++) {

				// check if whole column is empty
				bool columnEmpty = true;
				for (int y = 0; y < 16; y++) {
					if (map.blocks [y] [jLeft] != null)
						columnEmpty = false;
				}

				// if yes move whole left half to right
				if(columnEmpty){
					for (int c = 0; c < 16; c++)
					for (int k = jLeft-1; k >= 0; k--) {
						if (map.blocks [c] [k] != null) {
							needToMoveLeft = true;	
							map.blocks [c] [k].x++;
							map.blocks [c] [k+1] = map.blocks [c] [k];
							map.blocks [c] [k] = null;
							map.blocks [c] [k+1].UpdatePositionFromIndex ();
							//yield return new WaitForSeconds(0.05f);
						}
					}
				}
			}

			// single frame delay
			if (needToMoveLeft) {
				yield return 0;
				jLeft++;
			}

			// right half loop condition
			if (jRight >= 16)
				continue;

			needToMoveRight = false;
			// right half column loop
			for (int i = 0; i < 16; i++) {


				// check if whole column is empty
				bool columnEmpty = true;
				for (int y = 0; y < 16; y++) {
					if (map.blocks [y] [jRight] != null)
						columnEmpty = false;
				}

				// if we can move left, move left!
				if(columnEmpty){
					for (int c = 0; c < 16; c++)
					for (int k = jRight+1; k < 16; k++) {
						//						Debug.Log ("K:"+k+" j2:"+j2);
						if (map.blocks [c] [k] != null) {
							needToMoveRight = true;	
							map.blocks [c] [k].x--;
							map.blocks [c] [k-1] = map.blocks [c] [k];
							map.blocks [c] [k] = null;
							map.blocks [c] [k-1].UpdatePositionFromIndex ();
							//yield return new WaitForSeconds(0.05f);
						}
					}
				}
			}

			// single frame delay
			if(needToMoveRight){
				yield return 0;
				jRight--;
			}
		}

	}

	// not used
	void populateList(Block b, List<Block> list){
		if (b.isGrouped)
			return;
		//Debug.Log ("Adding" + b.x + "," + b.y);
		list.Add (b);
		b.isGrouped = true;
		if(b.x > 0 && b.colorType == b.Left.colorType){
			populateList (b.Left, list);
		}
		if(b.x < 15 && b.colorType == b.Right.colorType){
			populateList (b.Right, list);
		}
		if(b.y > 0 && b.Down.colorType == b.colorType){
			populateList (b.Down, list);
		}
		if(b.y < 15 && b.Up.colorType == b.colorType){
			populateList (b.Up, list);
		}
	}

	// add blocks recursively to group
	void populateGroup(Block b, Group g){
		if (b == null || b.isGrouped)
			return;
		
		g.AddBlock (b);
		b.isGrouped = true;
		if(b.x > 0 && b.Left && b.colorType == b.Left.colorType){
			populateGroup (b.Left, g);
		}
		if(b.x < 15 && b.Right && b.colorType == b.Right.colorType){
			populateGroup (b.Right, g);
		}
		if(b.y > 0 && b.Down && b.Down.colorType == b.colorType){
			populateGroup (b.Down, g);
		}
		if(b.y < 15 && b.Up &&  b.Up.colorType == b.colorType){
			populateGroup (b.Up, g);
		}
	}

	List<Group> groups;

	// scanes the map and creates groups
	void GenerateGroups(){

		// re-link blocks after update and before generating groups
		for (int i = 0; i < 16; i++) {
			for (int j = 0; j < 16; j++) {
				if (map.blocks [i] [j] == null)
					continue;
				map.blocks [i] [j].isGrouped = false;
				if (i != 0)
					map.blocks [i] [j].Left = map.blocks [i - 1] [j];
				if (i != 15)
					map.blocks [i] [j].Right = map.blocks [i + 1] [j];
				if (j != 0)
					map.blocks [i] [j].Down = map.blocks [i] [j - 1];
				if (j != 15)
					map.blocks [i] [j].Up = map.blocks [i] [j + 1];

			}
		}

		groups = new List<Group> ();
		for (int i = 0; i < 16; i++) {
			for (int j = 0; j < 16; j++) {
				if (map.blocks [i] [j] == null || map.blocks [i] [j].isGrouped) {
					continue;
				} else {
					// create a group
					Group g = new Group ();
					// name to debug
					g.name = "g"+(groups.Count + 1).ToString();
					groups.Add (g);
					// shared material between group blocks
					Material m = new Material (Shader.Find("Unlit/Color"));
					m.color = map.colors[map.blocks[i][j].colorType];
					g.mat = m;
					// create group recursively, similer to flood fill
					populateGroup (map.blocks[i][j],g);
				}

			}
		}
	}
}
