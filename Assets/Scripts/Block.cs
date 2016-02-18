using UnityEngine;
using System.Collections;


public class Block : MonoBehaviour {

	// position on map
	public int x, y;

	public int colorType;

	// Left, Right, Up, Down Blocks
	public Block Left, Right, Up, Down;

	// if its connected to 3 or more blocks
	public bool isGrouped;
	public int groupIndex;
	public Group group;

	// Renderer to change material
	public Renderer rend;

	// Transform to animate
	public Transform trans;


	public void UpdatePositionFromIndex(){
		trans.localPosition = new Vector3 (x,y,10f);
	}
}
