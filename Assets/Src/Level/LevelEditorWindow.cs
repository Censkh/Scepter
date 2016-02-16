#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class LevelEditorWindow : EditorWindow
{

	const string Name = "Level Editor";
	bool generationToggle, meshToggle, chunkToggle, tileToggle, paletteToggle,toolToggle;
	bool tileAbs;
	int generationWidth = 0, generationHeight = 0;
	int chunkX = 0, chunkY = 0;
	byte tileId = 0;
	GameObject worldObject;
	Chunk chunk
	{
		get { return world.GetChunk(chunkX, chunkY); }
	}
	World world
	{
		get
		{
			if (worldObject == null) return null;
			return worldObject.GetComponent<World>();
		}
	}
	Vector3 tilePos
	{
		get { return TileTool.Current.tilePos;}
		set { TileTool.Current.tilePos = value;}
	}

	[MenuItem("Tools/Show Level Editor")]
	[MenuItem("Window/Level Editor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<LevelEditorWindow>();
	}

	void OnGUI()
	{
		GUILayout.Label("World", EditorStyles.boldLabel);
		worldObject = (GameObject)EditorGUILayout.ObjectField("Current World Object", worldObject, typeof(GameObject), true);
		if (!IsWorldValid())
		{
			GUILayout.Label("Current world object is invalid, one must be found before editing can begin.", EditorStyles.miniLabel);
			GUILayout.Space(2);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Create World Object"))
			{
				worldObject = World.Create();
			}
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.BeginVertical();
			if (world.Chunks != null)
			{
				GUILayout.Label("Chunk Count: " + world.Chunks.Count, EditorStyles.label);
			}
			world.tileAtlas = (Material)EditorGUILayout.ObjectField("Tile Atlas", world.tileAtlas, typeof(Material), true);
			world.fileLocation = EditorGUILayout.TextField("File Location", world.fileLocation);
			meshToggle = EditorGUILayout.BeginToggleGroup("Mesh", meshToggle);
			if (meshToggle)
			{
				if (GUILayout.Button("Build World Mesh"))
				{
					world.Build();
				}
				if (GUILayout.Button("Clear World Mesh"))
				{
					world.CleanupMesh();
				}
			}
			EditorGUILayout.EndToggleGroup();
			paletteToggle = EditorGUILayout.BeginToggleGroup("Palette", paletteToggle);
			if (paletteToggle)
			{
				TileTool.Current.ShowPaletteMenu();
			}
			EditorGUILayout.EndToggleGroup();
			chunkToggle = EditorGUILayout.BeginToggleGroup("Chunk", chunkToggle);
			if (chunkToggle)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Current Chunk", chunk == null ? "Null" : chunk + "(" + chunk.GetX() + "," + chunk.GetY() + ")", EditorStyles.label);
				EditorGUILayout.EndHorizontal();
				if (chunk.Dirty)
				{
					EditorGUILayout.HelpBox("Chunk mesh is dirty and needs to be built.", MessageType.Warning, true);
				}
				int x = EditorGUILayout.IntField("Chunk X", chunkX);
				int y = EditorGUILayout.IntField("Chunk Y", chunkY);
				if (x != chunkX || y != chunkY)
				{
					chunkX = x;
					chunkY = y;
				}
				GUI.enabled = Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<ChunkModel>() != null;
				if (GUILayout.Button("From Selection"))
				{
					chunkX = chunk.GetX();
					chunkY = chunk.GetY();
				}
				GUI.enabled = chunk != null && chunk.ChunkGameObject != null;
				if (GUILayout.Button("Select Game Object"))
				{
					Selection.activeGameObject = chunk.ChunkGameObject;
				}
				if (GUILayout.Button("Build Mesh"))
				{
					chunk.ChunkModel.BuildMesh();
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndToggleGroup();
			tileToggle = EditorGUILayout.BeginToggleGroup("Tile", tileToggle);
			if (tileToggle)
			{
				TileMenu();
			}
			EditorGUILayout.EndToggleGroup();
			generationToggle = EditorGUILayout.BeginToggleGroup("Generation", generationToggle);
			if (generationToggle)
			{
				GUILayout.BeginVertical();
				generationWidth = EditorGUILayout.IntField("Chunk Width", generationWidth);
				generationHeight = EditorGUILayout.IntField("Chunk Height", generationHeight);
				GUILayout.EndVertical();
				if (GUILayout.Button("Generate World"))
				{
					world.Generate(generationWidth, generationHeight);
				}
			}
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.EndVertical();
		}
		toolToggle = EditorGUILayout.BeginToggleGroup("Tool", toolToggle);
		if (toolToggle)
		{
			TileTool.Current.ShowMenu();
		}
		EditorGUILayout.EndToggleGroup();
	}

	bool IsWorldValid()
	{
		if (worldObject == null) { worldObject = GameObject.FindGameObjectWithTag(World.NAME); return worldObject != null; }
		if (world == null)
			return false;
		if (world.GetComponent<World>() == null) return false;
		return true;
	}

	void TileMenu()
	{
		bool enabled = chunk != null;
		GUI.enabled = enabled;
		EditorGUILayout.LabelField("Current Tile", "Tile(" + tilePos.x + "," + tilePos.y + "," + tilePos.z + ")");
		if (enabled) tileId = world.GetTileId((int)tilePos.x, (int)tilePos.y, (int)tilePos.z); else tileId = 0;
		byte id = tileId;
		id = (byte)TileEditorWindow.TilePopup("Tile Info", id);
		id = (byte)EditorGUILayout.IntField("Tile Id", id);
		if (GUILayout.Button("Apply Palette"))
		{
			id = TileTool.Current.paletteId;
		}
		if (id != tileId)
		{
			if (tileAbs)
				World.Current.SetTileId(id, (int)tilePos.x, (int)tilePos.y, (int)tilePos.z);
			else if (enabled)
			{
				chunk.SetTileId(id, (int)tilePos.x % Chunk.CHUNK_WIDTH, (int)tilePos.y % Chunk.CHUNK_HEIGHT, (int)tilePos.z);
			}
			chunk.ChunkModel.BuildMesh();
		}
		Vector2 offset = new Vector2(tileAbs ? 0 : (tilePos.x) - (tilePos.x % Chunk.CHUNK_WIDTH), tileAbs ? 0 : (tilePos.y) - (tilePos.y % Chunk.CHUNK_HEIGHT));
		Vector3 tile = new Vector3(tilePos.x - offset.x, tilePos.y - offset.y, tilePos.z);
		tile = EditorGUILayout.Vector3Field("Tile Pos", tile);
		tileAbs = EditorGUILayout.Toggle("Absolute Pos", tileAbs);
		if (tileAbs)
		{
			tile.Set(tile.x + offset.x, tile.y + offset.y, tile.z);
		}
		else
		{
			tile.Set((chunkX * Chunk.CHUNK_WIDTH) + tile.x, (chunkY * Chunk.CHUNK_HEIGHT) + tile.y, tile.z);
		}
		int tz = (int)tile.z;
		tz = EditorGUILayout.Popup("Tile Depth", tz, Tile.DEPTH_NAMES);
		if (tz != (int)tile.z)
		{
			tile.Set(tile.x, tile.y, tz);
		}
		if (tile.x != tilePos.x || tile.y != tilePos.y || tile.z != tilePos.z)
		{
			tilePos.Set(tile.x, tile.y, tile.z);
		}
		GUI.enabled = true;
	}

}

#endif