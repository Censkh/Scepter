#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldInspector : Editor
{
	public override void OnInspectorGUI()
	{
		
		World world = (World) target;

		DrawDefaultInspector();

		if (GUILayout.Button("Open Level Editor"))
		{
			LevelEditorWindow.ShowWindow();
		}
		if (GUILayout.Button("Build"))
		{
			world.Build();
		}
	}
}

#endif