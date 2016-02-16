using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class World : MonoBehaviour
{

	public const string NAME = "World";

	public static World Current
	{
		get
		{
			return GameObject.FindGameObjectWithTag("World").GetComponent<World>();
		}
	}

	[HideInInspector]
	[SerializeField]
	private ChunkMap chunkMap = new ChunkMap();

	public List<Chunk> Chunks
	{
		get
		{
			return chunkMap.Chunks;
		}
	}

	public Material tileAtlas;
	public string fileLocation = "Data/Level/test.xml";

	public void Build()
	{
		CleanupMesh();
		foreach (Chunk chunk in chunkMap.Chunks)
		{
			chunk.ChunkModel.BuildMesh();
		}
	}

	public void Generate(int width, int height)
	{
		for (int x = -Mathf.FloorToInt(width / 2); x < Mathf.CeilToInt(width / 2) + (width % 2); x++)
		{
			for (int y = -Mathf.FloorToInt(height / 2); y < Mathf.CeilToInt(height / 2) + (height % 2); y++)
			{
				Chunk chunk = GetChunk(x, y);
				if (chunk == null)
				{
					chunk = CreateChunk(x, y);
				}
			}
		}
	}

	public void Clear()
	{
		chunkMap.Clear();
	}

	public void CleanupMesh()
	{
		GameObject[] oldChunks = GameObject.FindGameObjectsWithTag("Chunk");
		foreach (GameObject oldChunk in oldChunks)
		{
			DestroyImmediate(oldChunk);
		}
	}

	void Update()
	{

	}

	public static GameObject Create()
	{
		GameObject worldObject = new GameObject();
		worldObject.tag = NAME;
		worldObject.name = NAME;
		return worldObject;
	}

	public bool IsColliding(Bounds bounds, Vector3 worldPos, out Vector3 tileWorldPos)
	{
		Vector3 size = bounds.size;
		Vector3 topLeft = worldPos - bounds.extents;
		topLeft = new Vector3(topLeft.x, topLeft.y, worldPos.z);
		int tileWidth = Mathf.CeilToInt((size.x) / Tile.TILE_WIDTH) + 1;
		int tileHeight = Mathf.CeilToInt((size.y) / Tile.TILE_HEIGHT) + 1;
		for (int x = 0; x < tileWidth; x++)
		{
			for (int y = 0; y < tileHeight; y++)
			{
				Vector3 checkBox = new Vector3((x) * Tile.TILE_WIDTH, (y) * Tile.TILE_HEIGHT, 0);
				if (checkBox.x > size.x)
				{
					checkBox.x = size.x;
				}
				if (checkBox.y > size.y)
				{
					checkBox.y = size.y;
				}
				if (IsTileSolid(topLeft + checkBox))
				{
					Chunk chunk = null;
					Vector3 tilePos;
					GetTilePosAtLocation(topLeft + checkBox,out chunk, out tilePos);
					tileWorldPos = new Vector3(((tilePos.x) + (chunk.GetX() * Chunk.CHUNK_WIDTH)) * Tile.TILE_WIDTH, ((tilePos.y) + (chunk.GetY() * Chunk.CHUNK_HEIGHT)) * Tile.TILE_HEIGHT, tilePos.z * Tile.TILE_DEPTH);
					return true;
				}
			}
		}
		tileWorldPos = Vector3.zero;
		return false;
	}

	public bool IsColliding(Bounds bounds, Vector3 worldPos)
	{
		Vector3 tileWorldPos = Vector3.zero;
		return IsColliding(bounds, worldPos, out tileWorldPos);
	}

	public bool IsColliding(Bounds bounds, Vector2 worldPos,int layer)
	{
		Vector3 tileWorldPos = Vector3.zero;
		return IsColliding(bounds, worldPos, layer, out tileWorldPos);
	}

	public bool IsColliding(Bounds bounds, Vector2 worldPos, int layer,out Vector3 tileWorldPos)
	{
		return IsColliding(bounds, new Vector3(worldPos.x, worldPos.y, layer * Tile.TILE_DEPTH),out tileWorldPos);
	}

	public void GetTilePosAtLocation(Vector3 worldPos, out Chunk chunk, out Vector3 tilePos)
	{
		chunk = GetChunkAtLocation(new Vector2(worldPos.x, worldPos.y));
		if (chunk == null)
		{
			tilePos = Vector3.zero;
			return;
		}
		Vector3 tileSpace = ToTileSpace(worldPos);
		tilePos = new Vector3(tileSpace.x - (chunk.GetX() * Chunk.CHUNK_WIDTH), tileSpace.y - (chunk.GetY() * Chunk.CHUNK_HEIGHT), tileSpace.z);
	}

	public bool IsTileSolid(Vector3 worldPos)
	{
		Vector3 tilePos;
		Chunk chunk;
		GetTilePosAtLocation(worldPos, out chunk, out tilePos);
		if (chunk == null) return false;
		bool solid = chunk.IsTileSolid((int)tilePos.x, (int)tilePos.y, (int)tilePos.z);
		return solid;
	}

	public bool IsTileSolid(Vector2 worldPos, int layer)
	{
		return IsTileSolid(new Vector3(worldPos.x, worldPos.y, layer * Tile.TILE_DEPTH));
	}

	public Chunk GetChunkAtLocation(Vector3 worldPos)
	{
		Vector3 tileSpace = ToTileSpace(worldPos);
		Chunk chunk = GetChunk(Mathf.FloorToInt(tileSpace.x / Chunk.CHUNK_WIDTH), Mathf.FloorToInt(tileSpace.y / Chunk.CHUNK_HEIGHT));
		return chunk;
	}

	public Vector3 ToTileSpace(Vector3 worldPos)
	{
		return new Vector3(Mathf.FloorToInt(worldPos.x / Tile.TILE_WIDTH), Mathf.FloorToInt(worldPos.y / Tile.TILE_HEIGHT), Mathf.FloorToInt(worldPos.z / Tile.TILE_DEPTH));
	}

	public void SetChunk(int x, int y, Chunk chunk)
	{
		chunkMap.SetChunk(x, y, chunk);
	}

	public Chunk CreateChunk(int x, int y)
	{
		Chunk chunk = GetChunk(x, y);
		if (chunk != null) return chunk;
		chunk = new Chunk(x, y);
		SetChunk(x, y, chunk);
		chunkMap.Chunks.Add(chunk);
		return chunk;
	}

	public Chunk GetChunk(int x, int y)
	{
		return chunkMap.GetChunk(x, y);
	}



	public Chunk SetTileId(byte id, int x, int y, int z)
	{
		Chunk chunk = GetChunkFromTile(x, y);
		if (chunk != null)
		{

			int tx = Mathf.Abs(x) % (Chunk.CHUNK_WIDTH);
			int ty = Mathf.Abs(y) % (Chunk.CHUNK_HEIGHT);
			if (x < 0 && tx!=0)
			{
				tx = Chunk.CHUNK_WIDTH - tx;
			}
			if (y < 0 && ty!=0)
			{
				ty = Chunk.CHUNK_HEIGHT - ty;
			}
			chunk.SetTileId(id,tx, ty, z);
		}
		return chunk;
	}

	public byte GetTileId(int x, int y, int z)
	{
		Chunk chunk = GetChunkFromTile(x, y);
		if (chunk != null)
		{
			int tx = Mathf.Abs(x) % Chunk.CHUNK_WIDTH;
			int ty = Mathf.Abs(y) % Chunk.CHUNK_HEIGHT;
			if (x < 0 && tx != 0)
			{
				tx = Chunk.CHUNK_WIDTH - tx;
			}
			if (y < 0 && ty != 0)
			{
				ty= Chunk.CHUNK_HEIGHT - ty;
			}
			return chunk.GetTileId(tx,ty,z);
		}
		else
		{
			return 0;
		}
	}

	public Chunk GetChunkFromTile(int x, int y)
	{
		int cx = Mathf.FloorToInt((float)(x) / (float)Chunk.CHUNK_WIDTH);
		int cy = Mathf.FloorToInt((float)(y) / (float)Chunk.CHUNK_HEIGHT);
		return GetChunk(cx, cy);
	}

	public byte GetTileId(Vector3 tilePos)
	{
		return GetTileId((int)tilePos.x, (int)tilePos.y, (int)tilePos.z);
	}

}
