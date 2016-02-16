using UnityEngine;
using System;

[Serializable]
public class Tile
{


	public readonly static string[] DEPTH_NAMES = new string[]{
		"Sky",
		"Foreground",
		"Character",
		"Background",
		"Vista"
	};
	public const int DEPTH_CHARACTER = 2;
	public const int DEPTH_BACKGROUND = 3;

	public const float TILE_WIDTH = 0.4f;
	public const float TILE_HEIGHT = TILE_WIDTH;
	public const float TILE_DEPTH = 0.3f;
	public const int PIXEL_WIDTH = 16;
	public const int PIXEL_HEIGHT = 16;
	public static Vector3 TILE_SIZE = new Vector3(TILE_WIDTH,TILE_HEIGHT,TILE_DEPTH);

	public byte id;
	public Vector2 texturePos;
	public string name = "NewTile";
	public bool solid;
	public bool specialMesh;

	[NonSerialized]
	public Mesh mesh;

	public string Info
	{
		get { return name + " (" + id + ")";}
	}

	public static string GetDepthName(int depth)
	{
		return DEPTH_NAMES[depth];
	}

	public static int GetDepthId(string name)
	{
		int i = 0;
		foreach (string depthName in DEPTH_NAMES)
		{
			if (depthName.Equals(name))
			{
				return i;
			}
			i++;
		}
		return -1;
	}
}

