using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileMap
{

	[Serializable]
	class TileArrayArray
	{
		[SerializeField]
		public TileArray[] array;

		public TileArrayArray(int length)
		{
			array = new TileArray[length];
		}
	}

	[Serializable]
	class TileArray
	{
		[SerializeField]
		public byte[] array;

		public TileArray(int length)
		{
			array = new byte[length];
		}
	}

	[SerializeField]
	private TileArrayArray[] map = null;

	[SerializeField]
	private int width;
	[SerializeField]
	private int height;
	[SerializeField]
	private int depth;

	public int Width
	{
		get { return width; }
	}

	public int Height
	{
		get { return height; }
	}

	public int Depth
	{
		get { return depth; }
	}

	public TileMap(int width, int height, int depth)
	{
		this.width = width;
		this.height = height;
		this.depth = depth;
		map = new TileArrayArray[width];
	}

	public byte Get(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth)
		{
			return 0;
		}
		return GetZArray(x, y)[z];
	}

	public void Set(int x, int y, int z, byte value)
	{
		if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth)
		{
			return;
		}
		GetZArray(x, y)[z] = value;
	}

	byte[] GetZArray(int x, int y)
	{
		if (map[x] == null)
		{
			map[x] = new TileArrayArray(height);
		}
		if (((TileArrayArray)map[x]).array[y] == null)
		{
			((TileArrayArray)map[x]).array[y] = new TileArray(depth);
		}
		return ((TileArray)((TileArrayArray)map[x]).array[y]).array;
	}
}