#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

public class TileSession : Session
{

	[MenuItem("Tools/Session/Reload Tile Session")]
	public static void ReloadSession()
	{
		current = null;
	}


	private static TileSession current;
	public static TileSession Current
	{
		get
		{
			if (current == null)
			{
				current = new TileSession(Application.dataPath + "/Data/TileSession.cfg");
			}
			return current;
		}
		set { current = value; }
	}

	private PropertyFile file;

	public string currentTileFilePath
	{
		get
		{
			return file.GetProperty("currentTileFilePath");
		}
		set
		{
			file.SetProperty("currentTilefilePath",value);
			file.Save();
		}
	}

	public TileSession(string path)
	{
		file = new PropertyFile(path);
		file.Load();
	}

}

#endif