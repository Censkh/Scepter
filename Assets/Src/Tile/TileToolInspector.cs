#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileTool))]
class TileToolInspector : Editor
{

	public void OnSceneGUI()
	{
		if (Event.current != null)
			((TileTool)target).HandleEvent(Event.current);
	}

}

#endif