using UnityEngine;

using System.Collections;

public class ChunkModel : BlockModel
{

	class TileModel : SpriteModel
	{
		public Texture2D texture;

		public override Texture2D GetMainTexture()
		{
			return texture;
		}

		public override Vector3 GetScale()
		{
			Vector3 scale = base.GetScale();
			scale.Set(scale.x, scale.y, Tile.TILE_DEPTH);
			return scale;
		}
	}


	[HideInInspector]
	public int chunkX;
	[HideInInspector]
	public int chunkY;

	public Chunk Chunk
	{
		get { return World.Current.GetChunk(chunkX, chunkY); }
	}

	void Start()
	{
	}

	void Update()
	{

	}

	public override bool IsCentered()
	{
		return false;
	}

	public override bool IsConvex()
	{
		return false;
	}

	public override bool HasCollision()
	{
		return true;
	}

	public override void ApplyUV(Vector3 start, Vector3 offset1, Vector3 offset2)
	{
		float offset = 0.005f;
		byte id = Chunk.GetTileId(Mathf.CeilToInt(start.x), Mathf.CeilToInt(start.y), Mathf.CeilToInt(start.z));
		Tile tile = TileManager.Current.GetTile(id);
		Texture texture = World.Current.tileAtlas.mainTexture;
		int x = (int)tile.texturePos.x;
		int y = (int)tile.texturePos.y;
		float w = texture.width / Tile.PIXEL_WIDTH;
		float h = texture.height / Tile.PIXEL_HEIGHT;
		y = (int)h - y;
		data.uv.Add(new Vector2((x + 1) / w, (y - 1) / h) + new Vector2(-offset, offset));
		data.uv.Add(new Vector2(x / w, (y - 1) / h) + new Vector2(offset, offset));
		data.uv.Add(new Vector2((x + 1) / w, y / h) + new Vector2(-offset, -offset));
		data.uv.Add(new Vector2(x / w, y / h) + new Vector2(offset, -offset));
	}

	public override void BuildMesh()
	{
		Chunk.Dirty = false;
		base.BuildMesh();
	}

	public override Vector3 GetScale()
	{
		return Tile.TILE_SIZE;
	}

	public override BoolMap GetBlockData()
	{
		return Chunk.GetSolidMap();
	}

	public override void CustomBuild(BlockModel.ModelData data, Vector3 start)
	{
		Mesh mesh = null;
		Tile tile = Chunk.GetTile((int)start.x, (int)start.y, (int)start.z);
		if (tile.mesh == null)
		{
			GameObject obj = new GameObject();
			MeshFilter filter = obj.AddComponent<MeshFilter>();
			TileModel model = obj.AddComponent<TileModel>();
			model.textureOffset = new Vector2(tile.texturePos.x * Tile.PIXEL_WIDTH, (3 - tile.texturePos.y) * Tile.PIXEL_HEIGHT);
			model.textureSize = new Vector2(Tile.PIXEL_WIDTH, Tile.PIXEL_HEIGHT);
			model.texture = (Texture2D)World.Current.tileAtlas.mainTexture;
			model.BuildMesh();
			mesh = tile.mesh = filter.sharedMesh;
			DestroyImmediate(model);
			DestroyImmediate(filter);
			DestroyImmediate(obj);
			Debug.Log("Tile Mesh built for " + tile.Info);
		}
		else
		{
			mesh = tile.mesh;
		}
		int startIndex = data.verts.Count;
		Vector3 offset = new Vector3((start.x+3)*Tile.TILE_WIDTH,(start.y+2)*Tile.TILE_HEIGHT,start.z*Tile.TILE_DEPTH);
		foreach (Vector2 v in mesh.uv)
		{
			data.uv.Add(v);
		}
		foreach (int v in mesh.triangles)
		{
			data.tris.Add(v + startIndex);
		}
		foreach (Vector3 v in mesh.vertices)
		{
			data.verts.Add(v + offset);
		}
	}

	public override bool IsCustom(int x, int y, int z)
	{
		return Chunk.GetTile(x, y, z).specialMesh;
	}

	public override bool IsSolid(BoolMap data, int x, int y, int z, bool customIsSolid)
	{
		if (Chunk.IsTileValid(x, y, z))
		{
			return base.IsSolid(data, x,y,z,customIsSolid);
		
		}
		byte id = World.Current.GetTileId(new Vector3((Chunk.GetX() * Chunk.CHUNK_WIDTH) + x, (Chunk.GetY() * Chunk.CHUNK_HEIGHT) + y,z));
		Tile tile = TileManager.Current.GetTile(id);
		if (tile.specialMesh)
		{
			return customIsSolid;
		}
		return Chunk.IsTileSolid(id);
	}

}
