using UnityEngine;
using System.Collections.Generic;

public abstract class BlockModel : MonoBehaviour
{

	public struct ModelData
	{
		public List<Vector3> verts;
		public List<int> tris;
		public List<Vector2> uv;
		public int width;
		public int height;
		public int depth;
		public Vector3 offset;
		public Vector3 modelOffset;

		public ModelData Init()
		{
			verts = new List<Vector3>();
			tris = new List<int>();
			uv = new List<Vector2>();
			width = 0;
			height = 0;
			depth = 0;
			offset = Vector3.zero;
			modelOffset = Vector3.zero;
			return this;
		}
	}

	public bool buildDone;

	public ModelData data = new ModelData().Init();

	public abstract BoolMap GetBlockData();

    public virtual bool HasCollision()
    {
        return true;
    }

    public abstract Vector3 GetScale();

    public abstract bool IsConvex();

    public abstract bool IsCentered();

	public virtual bool IsCustom(int x, int y, int z)
	{
		return false;
	}

	public virtual void CustomBuild(ModelData data, Vector3 start)
	{

	}

    public virtual void BuildMesh()
    {
        BlockModelBuilder.current.BuildMesh(this,data);
    }

    public abstract void ApplyUV(Vector3 start, Vector3 offset1, Vector3 offset2);

    public void BuildFace(ModelData data, Vector3 start, Vector3 offset1, Vector3 offset2)
    {
        int index = data.verts.Count;

        Vector3 scale = GetScale();
        start.Scale(scale);
        offset1.Scale(scale);
        offset2.Scale(scale);

        if (IsCentered())
        {
			Vector3 o = data.offset;
            o.Scale(scale);
            start -= o;
        }

		data.verts.Add(start);
		data.verts.Add(start + offset1);
		data.verts.Add(start + offset2);
		data.verts.Add(start + offset1 + offset2);

		data.tris.Add(index + 0);
		data.tris.Add(index + 1);
		data.tris.Add(index + 2);
		data.tris.Add(index + 3);
		data.tris.Add(index + 2);
		data.tris.Add(index + 1);
    }

	public virtual bool IsSolid(BoolMap data, int x, int y, int z,bool customIsSolid)
    {
		if (IsCustom(x,y,z)) return customIsSolid;
		if (x < 0 || y < 0 || z < 0 || x >= data.Width || y >= data.Height || z >= data.Depth)
        {
            return false;
        }
        return data.Get(x,y,z);
    }

	public bool IsSolid(BoolMap data, int x, int y, int z)
	{
		return IsSolid(data,x,y,z,false);
	}

}
