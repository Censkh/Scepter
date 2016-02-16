using UnityEngine;
using System.Collections.Generic;

public class BlockModelBuilder
{

	public static BlockModelBuilder current = new BlockModelBuilder();

	public BlockModelBuilder()
	{
		current = this;
	}

	public void BuildMesh(BlockModel model, BlockModel.ModelData data)
	{
		model.StartCoroutine(DoBuild(model, data));
	}

	IEnumerator<object> DoBuild(BlockModel model, BlockModel.ModelData data)
	{
		Mesh mesh = new Mesh();
		mesh.name = model.name + "_Model";
		MeshFilter filter = model.gameObject.GetComponent<MeshFilter>();
		filter.sharedMesh = mesh;
		MeshCollider collider = model.gameObject.GetComponent<MeshCollider>();
		if (collider == null)
		{
			collider = model.gameObject.AddComponent<MeshCollider>();
		}
		collider.convex = model.IsConvex();
		mesh.Clear();
		mesh.triangles = data.tris.ToArray();
		BoolMap blockData = model.GetBlockData();
		data.width = blockData.Width;
		data.height = blockData.Height;
		data.depth = blockData.Depth;
		data.offset = new Vector3(data.width / 2, data.height / 2, data.depth / 2);
		for (int x = 0; x < data.width; x++)
		{
			for (int y = 0; y < data.height; y++)
			{
				for (int z = 0; z < data.depth; z++)
				{
					if (model.IsSolid(blockData, x, y, z,true))
					{
					Vector3 start = new Vector3(x, y, z);
						if (model.IsCustom(x, y, z))
						{
							model.CustomBuild(data, start + data.modelOffset);
							continue;
						}
						Vector3 offset1, offset2;
						if (!model.IsSolid(blockData, x, y - 1, z))
						{
							offset1 = Vector3.left;
							offset2 = Vector3.back;
							model.BuildFace(data, start + data.modelOffset + Vector3.right, offset1, offset2);
							model.ApplyUV(start, offset1, offset2);
						}
						if (!model.IsSolid(blockData, x, y + 1, z))
						{
							offset1 = Vector3.right;
							offset2 = Vector3.back;
							model.BuildFace(data, start + data.modelOffset + Vector3.up, offset1, offset2);
							model.ApplyUV(start, offset1, offset2);
						}
						if (!model.IsSolid(blockData, x - 1, y, z))
						{
							offset1 = Vector3.up;
							offset2 = Vector3.back;
							model.BuildFace(data, start + data.modelOffset, offset1, offset2);
							model.ApplyUV(start, offset1, offset2);
						}
						if (!model.IsSolid(blockData, x + 1, y, z))
						{
							offset1 = Vector3.down;
							offset2 = Vector3.back;
							model.BuildFace(data, start + data.modelOffset + Vector3.right + Vector3.up, offset1, offset2);
							model.ApplyUV(start, offset1, offset2);
						}

						/*if (!IsSolid(data, x, y, z + 1))
						{
							offset1 = Vector3.right;
							offset2 = Vector3.up;
							BuildFace(start, offset1, offset2);
						}*/

						if (!model.IsSolid(blockData, x, y, z - 1))
						{
							offset1 = Vector3.left;
							offset2 = Vector3.up;
							model.BuildFace(data, start + data.modelOffset + Vector3.right + Vector3.back, offset1, offset2);
							model.ApplyUV(start, offset1, offset2);
						}
					}

				}
			}
		}
		mesh.vertices = data.verts.ToArray();
		mesh.triangles = data.tris.ToArray();
		mesh.uv = data.uv.ToArray();
		collider.sharedMesh = null;
		collider.sharedMesh = mesh;
		collider.isTrigger = !model.HasCollision();
		data.verts.Clear();
		data.uv.Clear();
		data.tris.Clear();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		model.buildDone = true;
		mesh.Optimize();
		yield return null;
	}

}
