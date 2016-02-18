using UnityEngine;
using UnityEditor;
using System.Collections;


public class MapEditor : EditorWindow {


	[MenuItem("Tools/Genrate16x16Map")]
	public static void GenerateMap(){
		GameManager gm = GameObject.FindObjectOfType<GameManager> ();
		if (gm == null) {
			return;
		}

		if (gm.maps == null) {
			gm.maps = new Map[1];
		}

		Transform MapParent = new GameObject ("MapParent").transform;
		Map m = MapParent.gameObject.AddComponent<Map> ();
		gm.maps [0] = m;

		// create Blocks and assign Basic things
		m.blocks = new Block[16][];

		// load a single blockPrefab
		GameObject blockPrefab = Resources.Load<GameObject> ("Block");



		// create and position blocks
		for (int i = 0; i < 16; i++) {
			m.blocks [i] = new Block[16];
			for (int j = 0; j < 16; j++) {
				Block b = Instantiate<GameObject> (blockPrefab).GetComponent<Block> ();
				m.blocks [i] [j] = b;
				b.x = i;
				b.y = j;
				b.trans.position = new Vector3 (i, j, 10f);
				b.trans.parent = MapParent.transform;
			}
		}

		// link all the blocks
		for (int i = 0; i < 16; i++) {
			for (int j = 0; j < 16; j++) {
				if (i != 0)
					m.blocks [i] [j].Left = m.blocks [i - 1] [j];
				if (i != 15)
					m.blocks [i] [j].Right = m.blocks [i + 1] [j];
				if (j != 0)
					m.blocks [i] [j].Down = m.blocks [i] [j - 1];
				if (j != 15)
					m.blocks [i] [j].Up = m.blocks [i] [j + 1];
			}
		}
	}

	public static Map edit_map;

	[MenuItem("Tools/MapEditor")]
	public static void OpenEditor(){
		MapEditor mEditor = EditorWindow.GetWindow<MapEditor> (true, "Map Editor", true);
		GameManager gm = GameObject.FindObjectOfType<GameManager> ();
		edit_map = GameObject.FindObjectOfType<Map>();

		if (edit_map.typeMap == null) {
			edit_map.typeMap = new int[16,16];
		}

		edit_map.blocks = new Block[16][];
		for (int i = 0; i < 16; i++) {
			edit_map.blocks [i] = new Block[16];
		}
		Block[] b = GameObject.FindObjectsOfType<Block>();
		Debug.Log (b.Length+"Blocks found");
		for (int i = 0; i < b.Length; i++) {
			edit_map.blocks [b [i].y] [b [i].x] = b[i];

			edit_map.typeMap [b [i].x, b [i].y] = b [i].colorType;
		}
		mEditor.Show ();
	}

	static int mapBeingEdited;

	void OnGUI(){
		EditorGUILayout.BeginHorizontal ();
		// can be editable later
		EditorGUILayout.IntField ("Map Index",0);
		if(GUILayout.Button("LoadMap")){
			// load the map
			edit_map = GameObject.FindObjectOfType<Map>();
		}
		EditorGUILayout.EndHorizontal ();
		// material Fields

		for (int i = 15;i >= 0; i--) {
			EditorGUILayout.BeginHorizontal ();
			for (int j = 0; j < 16; j++) {
				edit_map.typeMap [j, i] = EditorGUILayout.IntField (edit_map.typeMap [j, i],GUILayout.Width(20));
				//edit_map.savedTypeMap [i * 16 + j] = edit_map.typeMap [j, i];
				edit_map.blocks [i] [j].rend.sharedMaterial = edit_map.materials [Mathf.Clamp(edit_map.typeMap [j, i],0,edit_map.materials.Count)];
				edit_map.blocks [i] [j].colorType = edit_map.typeMap [j, i];
			}
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.BeginHorizontal ();
		for (int i = 0; i < edit_map.colors.Length; i++) {
			
			edit_map.colors [i] = EditorGUILayout.ColorField (edit_map.colors [i]);
			edit_map.materials [i].color = edit_map.colors [i];
		}
		EditorGUILayout.EndHorizontal ();





	}
}
