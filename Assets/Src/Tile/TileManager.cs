#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

[Serializable]
public class TileManager
{

	public static XmlSerializer serializer = new XmlSerializer(typeof(TileManager));

	private static TileManager current;
	public static TileManager Current
	{
		get
		{
			if (current == null)
			{
				string path = TileSession.Current.currentTileFilePath;
				if (path != null)
				{
					FileStream stream = null;
					try
					{
						stream = new FileStream(Application.dataPath + "/" + path, FileMode.OpenOrCreate);
						current = (TileManager)serializer.Deserialize(stream);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
						current = new TileManager();
					}
					finally
					{
						if (stream != null) stream.Close();
					}
				}
			}
			return current;
		}
		set { current = value; TileSession.Current.currentTileFilePath = value.location; }
	}

	[SerializeField]
	public string location = "Data/Tile/test.xml";

	public string name = "Untitled";

	[SerializeField]
	public List<Tile> tiles = new List<Tile>();

	public Tile GetTile(byte id)
	{
		if (id < 0 || id >= tiles.Count) return null;
		return tiles[id];
	}

	public void Save()
	{
		File.WriteAllText(Application.dataPath + "/" + location, string.Empty);
		FileStream stream  = new FileStream(Application.dataPath + "/" + location, FileMode.OpenOrCreate);
		serializer.Serialize(stream, this);
		stream.Close();
	}

}

#endif