#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

class TileTool : MonoBehaviour
{

	public const string NAME = "TileTool";
	public bool paint;
	public Vector3 tilePos = Vector3.zero;
	public int depth;
	public byte paletteId;
	public Tile palette
	{
		get { return TileManager.Current.GetTile(paletteId); }
	}
	public int size = 1;

	public void ShowPaletteMenu()
	{
		byte id = paletteId;
		id = (byte)TileEditorWindow.TilePopup("Palette", id);
		id = (byte)EditorGUILayout.IntField("Palatte Id", id);
		if (id != paletteId)
		{
			paletteId = id;
		}
	}

	public static GameObject CurrentObject
	{
		get
		{
			GameObject obj = GameObject.FindGameObjectWithTag(NAME);
			if (obj == null)
			{
				obj = Create();
			}
			return obj;
		}
	}
	public static TileTool Current
	{
		get
		{
			return CurrentObject.GetComponent<TileTool>();
		}
	}
	public bool ToolActive
	{
		get { return Selection.activeGameObject != null && Selection.activeGameObject.Equals(CurrentObject); }
		set
		{
			if (value)
			{
				Selection.activeGameObject = CurrentObject;
			}
			else
			{
				Selection.activeGameObject = null;
			}
		}
	}

	public static GameObject Create()
	{
		GameObject obj = new GameObject();
		obj.tag = NAME;
		obj.name = NAME;
		obj.AddComponent<TileTool>();
		return obj;
	}

	void OnDrawGizmos()
	{
		Vector3 size = new Vector3(Tile.TILE_WIDTH, Tile.TILE_HEIGHT, Tile.TILE_DEPTH);
		size *= this.size + (this.size - 1);
		Gizmos.DrawWireCube(transform.position, size);
	}

	void OnSceneGUI()
	{
		Event e = Event.current;
		Debug.Log(e);
	}

	Chunk TileSelected(int x, int y)
	{
		if (paint)
		{
			return World.Current.SetTileId(paletteId, x, y, depth);

		}
		return World.Current.GetChunkFromTile(x, y);
	}

	public void HandleEvent(Event e)
	{
		if (e.type == EventType.layout)
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
		if (ToolActive)
		{
			if (e.type == EventType.mouseDown || e.type == EventType.mouseDrag)
				if (e.button == 0)
				{
					SceneView sceneView = SceneView.currentDrawingSceneView;
					Vector2 mousePos = e.mousePosition;
					mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
					Vector3 position = sceneView.camera.ScreenPointToRay(mousePos).origin;
					position.Set(position.x, position.y, Tile.DEPTH_CHARACTER * Tile.TILE_DEPTH);
					Vector3 startPos = World.Current.ToTileSpace(position);
					HashSet<Chunk> chunks = new HashSet<Chunk>();
					for (int x = (int)startPos.x - size + 1; x < startPos.x + size; x++)
					{
						for (int y = (int)startPos.y - size + 1; y < startPos.y + size; y++)
						{
							Chunk chunk = TileSelected(x, y);
							if (chunk != null) chunks.Add(chunk);
						}
					}
					foreach (Chunk chunk in chunks)
					{
						chunk.ChunkModel.BuildMesh();
					}
					this.tilePos.Set(startPos.x, startPos.y, depth);
					startPos.Scale(Tile.TILE_SIZE);
					transform.position = startPos + (Tile.TILE_SIZE / 2) - new Vector3(0, 0, Tile.TILE_DEPTH);
					e.Use();
				}
		}
	}

	public void ShowMenu()
	{
		if (GUILayout.Button("Select Tool"))
		{
			Selection.activeGameObject = TileTool.CurrentObject;
		}
		ToolActive = GUILayout.Toggle(ToolActive, "Tool Active", GUI.skin.button);
		paint = GUILayout.Toggle(paint, "Paint", GUI.skin.button);
		int tz = (int)depth;
		tz = EditorGUILayout.Popup("Tile Depth", tz, Tile.DEPTH_NAMES);
		if (tz != depth)
		{
			depth = tz;
		}
		size = EditorGUILayout.IntField("Brush Size", size);
		GUILayout.Space(5);
		ShowPaletteMenu();
	}

}

#endif