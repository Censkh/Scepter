using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoolMap
{

	[Serializable]
	class BoolArrayArray
	{
		[SerializeField]
		public BoolArray[] array;

		public BoolArrayArray(int length)
		{
			array = new BoolArray[length];
		}
	}

	[Serializable]
	class BoolArray
	{
		[SerializeField]
		public bool[] array;

		public BoolArray(int length)
		{
			array = new bool[length];
		}
	}

	[SerializeField]
	private BoolArrayArray[] map = null;

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

	public BoolMap(int width, int height, int depth)
	{
		this.width = width;
		this.height = height;
		this.depth = depth;
		map = new BoolArrayArray[width];
	}

	public bool Get(int x, int y, int z)
	{
		if (x<0||y<0||z<0||x>=width||y>=height||z>=depth) return false;
		return (bool)GetZArray(x, y)[z];
	}

	public void Set(int x, int y, int z, bool value)
	{
		if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth) return;
		GetZArray(x, y)[z] = value;
	}

	bool[] GetZArray(int x, int y)
	{
		if (map[x] == null)
		{
			map[x] = new BoolArrayArray(height);
		}
		if (((BoolArrayArray)map[x]).array[y] == null)
		{
			((BoolArrayArray)map[x]).array[y] = new BoolArray(depth);
		}
		return ((BoolArray)((BoolArrayArray)map[x]).array[y]).array;
	}
}