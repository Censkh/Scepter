#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class TileEditorWindow : EditorWindow
{

	public static XmlSerializer serializer
	{
		get { return TileManager.serializer; }
	}
	const string Name = "Tile Editor";

	string loadLocation = "Data/Tile/test.xml";
	Vector2 tilesScroll;
	byte currentTileId;
	Tile[] searchTiles = null;
	string searchString = "";
	bool changes = false;

	[MenuItem("Tools/Show Tile Editor")]
	[MenuItem("Window/Tile Editor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<TileEditorWindow>();
	}

	void OnGUI()
	{
		changes = false;
		if (!title.Equals(Name)) title = Name;
		GUILayout.Label("Tile Manager", EditorStyles.boldLabel);
		string name = EditorGUILayout.TextField("Name", TileManager.Current == null ? "Null" : TileManager.Current.name);
		if (TileManager.Current != null)
		{
			if (!name.Equals(TileManager.Current.name))
			{
				TileManager.Current.name = name;
				changes = true;
			}
			string location = EditorGUILayout.TextField("Location", TileManager.Current.location);
			if (!location.Equals(TileManager.Current.location))
			{
				TileManager.Current.location = location;
				changes = true;
			}
		}
		if (GUILayout.Button("New Tile Manager"))
		{
			TileManager.Current = new TileManager();
		}
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Load Tile Manager"))
		{
			FileStream stream = new FileStream(Application.dataPath + "/" + loadLocation, FileMode.Open);
			if (!stream.CanRead) Debug.Log("Cant read file.");
			else
			{
				TileManager.Current = (TileManager)serializer.Deserialize(stream);
				TileManager.Current.location = loadLocation;
			}
			stream.Close();
		}
		loadLocation = EditorGUILayout.TextField(loadLocation);
		EditorGUILayout.EndHorizontal();
		if (TileManager.Current != null)
		{
			GUILayout.Label("Tiles", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Tile Count", "" + TileManager.Current.tiles.Count);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Tile"))
			{
				Tile tile;
				TileManager.Current.tiles.Add(tile = new Tile());
				tile.id = (byte)(TileManager.Current.tiles.Count - 1);
				changes = true;
			}
			if (GUILayout.Button("Remove Tile"))
			{
				TileManager.Current.tiles.RemoveAt(TileManager.Current.tiles.Count - 1);
				changes = true;
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Label("Current Tile", EditorStyles.boldLabel);
			byte id = currentTileId;
			id = (byte)TilePopup("Tile Info", id);
			id = (byte)EditorGUILayout.IntField("Tile Id", id);
			if (currentTileId != id)
			{
				if (TileManager.Current.GetTile(id) != null)
					currentTileId = id;
			}
			Tile t = TileManager.Current.GetTile(currentTileId);
			if (t == null)
			{
				currentTileId = 0;
				t = TileManager.Current.GetTile(currentTileId);
			}
			ShowTileEditor(t);
			GUILayout.Label("Search", EditorStyles.boldLabel);
			string search = searchString;
			GUILayout.BeginHorizontal();
			search = EditorGUILayout.TextField(search);
			if (GUILayout.Button("Clear"))
			{
				search = "";
			}
			GUILayout.EndHorizontal();
			if (!search.Equals(searchString) || searchTiles == null)
			{
				searchString = search;
				searchTiles = Search(searchString);
			}
			EditorGUILayout.Space();
			foreach (Tile searchTile in searchTiles)
			{
				if (GUILayout.Button(searchTile.Info))
				{
					currentTileId = searchTile.id;
				}
			}
		}
		if (changes)
		{
			TileManager.Current.Save();
		}
	}

	void ShowTileEditor(Tile tile)
	{
		if (tile == null) return;
		string name = tile.name;
		name = EditorGUILayout.TextField("Tile Name", name);
		if (!name.Equals(tile.name))
		{
			tile.name = name;
			changes = true;
		}
		Vector2 texturePos = new Vector2(tile.texturePos.x, tile.texturePos.y);
		texturePos = EditorGUILayout.Vector2Field("Texture Position", texturePos);
		if (!texturePos.Equals(tile.texturePos))
		{
			tile.texturePos = texturePos;
			changes = true;
		}
		Texture texture = World.Current.tileAtlas.mainTexture;
		float x = texturePos.x / (texture.width/Tile.PIXEL_WIDTH);
		float y = ((texture.height / Tile.PIXEL_HEIGHT) - texturePos.y - 1) / (texture.height / Tile.PIXEL_HEIGHT);
		GUILayout.BeginArea(new Rect(0,0,80f,80f),EditorStyles.textArea);
		GUI.DrawTextureWithTexCoords(new Rect(0,0, 64f, 64f), texture, new Rect(x, y, 1f/(texture.width/Tile.PIXEL_WIDTH), 1f/(texture.height/Tile.PIXEL_HEIGHT)));
		GUILayout.EndArea();
		bool solid = tile.solid;
		solid = EditorGUILayout.Toggle("Solid", solid);
		if (solid != tile.solid)
		{
			tile.solid = solid;
			changes = true;
		}
		bool specialMesh = tile.specialMesh;
		specialMesh = EditorGUILayout.Toggle("Special Mesh", specialMesh);
		if (specialMesh != tile.specialMesh)
		{
			tile.specialMesh = specialMesh;
			changes = true;
		}
		GUI.enabled = specialMesh;
		if (GUILayout.Button("Rebuild Special Mesh"))
		{
			tile.mesh = null;
		}
		GUI.enabled = true;
	}


	static Tile[] Search(string search)
	{
		List<Tile> tiles = new List<Tile>();
		foreach (Tile tile in TileManager.Current.tiles)
		{
			if (tile.name == null)
			{
				if (search == null || search.Length == 0)
				{
					tiles.Add(tile);
				}
				continue;
			}
			if (search == null || tile.name.ToLower().Contains(search.ToLower()))
			{
				tiles.Add(tile);
			}
		}
		return tiles.ToArray();
	}

	public static int TilePopup(string label, int id)
	{
		string[] names = new string[TileManager.Current.tiles.Count];
		int i = 0;
		foreach (Tile t in TileManager.Current.tiles)
		{
			names[i] = t.Info;
			i++;
		}
		return EditorGUILayout.Popup(label, id, names);
	}

}

#endif