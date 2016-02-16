#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpriteModel),true)]
public class BlockModelInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		BlockModel model = (BlockModel)target;
		if (GUILayout.Button("Build Mesh"))
		{
			model.BuildMesh();
		}
	}
}

#endif