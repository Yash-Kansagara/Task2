using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	represents a single map of 16X16 blocks
*/

public class Map : MonoBehaviour {

	// blocks
	public Block[][] blocks;

	// 16X16 int to describe color of same indexed block
	public int[,] typeMap;

	// colors used in Map, can be changed from MapEditor
	public Color[] colors;

	// Materials used initialy
	public List<Material> materials;

	public void Start(){
		
		// find and assign blocks from scene to this map
		blocks = new Block[16][];
		for (int i = 0; i < 16; i++) {
			blocks [i] = new Block[16];
		}
		Block[] b = GameObject.FindObjectsOfType<Block>();
		for (int i = 0; i < b.Length; i++) {
			blocks [b [i].y] [b [i].x] = b[i];
		}
	}

	// Never Used
	// To generate Maps at runtime from map data
//	public void GenerateMap(){
//		GameManager gm = GameObject.FindObjectOfType<GameManager> ();
//		if (gm == null) {
//			return;
//		}
//
//		if (gm.maps == null) {
//			gm.maps = new Map[1];
//		}
//
//		//Transform MapParent = new GameObject ("MapParent").transform;
//		Map m = this;
//		//gm.maps [0] = m;
//
//		// create Blocks and assign Basic things
//		m.blocks = new Block[16][];
//
//		// load a single blockPrefab
//		GameObject blockPrefab = Resources.Load<GameObject> ("Block");
//
//
//
//		// create and position blocks
//		for (int i = 0; i < 16; i++) {
//			m.blocks [i] = new Block[16];
//			for (int j = 0; j < 16; j++) {
//				Block b = Instantiate<GameObject> (blockPrefab).GetComponent<Block> ();
//				m.blocks [i] [j] = b;
//				b.x = i;
//				b.y = j;
//				b.trans.position = new Vector3 (i, j, 10f);
//				b.trans.parent = transform;
//			}
//		}
//
//		// link all the blocks
//		for (int i = 0; i < 16; i++) {
//			for (int j = 0; j < 16; j++) {
//				if (i != 0)
//					m.blocks [i] [j].Left = m.blocks [i - 1] [j];
//				if (i != 15)
//					m.blocks [i] [j].Right = m.blocks [i + 1] [j];
//				if (j != 0)
//					m.blocks [i] [j].Down = m.blocks [i] [j - 1];
//				if (j != 15)
//					m.blocks [i] [j].Up = m.blocks [i] [j + 1];
//			}
//		}
//	
//
//		for (int i = 0; i < 16; i++) {
//			for (int j = 0; j < 16; j++) {
//				blocks [i] [j].rend.sharedMaterial = materials [typeMap [i, j]];
//			}
//		}
//	}
}
