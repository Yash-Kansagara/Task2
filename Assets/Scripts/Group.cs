using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
	group of connected blocks which shares same material [mat]
*/
public class Group {
	
		public List<Block> blocks;
		public Material mat;
		public string name;
		public Group ()
		{
			blocks = new List<Block>();
		}
		
		// add block to this group
		public void AddBlock(Block b){
			b.group = this;
			b.rend.sharedMaterial = mat;
			blocks.Add (b);
		}

}
