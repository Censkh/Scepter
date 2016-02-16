using UnityEngine;

using System.Collections;

[System.Serializable]
public class Chunk
{

	public static int CHUNK_WIDTH = 16;
	public static int CHUNK_HEIGHT = 16;
	public static int CHUNK_DEPTH = 4;

	[HideInInspector]
	[SerializeField]
	private TileMap tileMap = new TileMap(CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_DEPTH);
	[HideInInspector]
	[SerializeField]
	private BoolMap solidMap = new BoolMap(CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_DEPTH);

	[SerializeField]
	[HideInInspector]
	private int x;
	[SerializeField]
	[HideInInspector]
	private int y;

	private bool dirty;
	public bool Dirty
	{
		get { return dirty; }
		set { this.dirty = value; }
	}

	private GameObject chunkGameObject;
	public GameObject ChunkGameObject
	{
		get
		{
			if (chunkGameObject == null)
			{
				GameObject obj = GameObject.Find("Chunk(" + x + "," + y + ")");
				if (obj != null)
				{
					return obj;
				}
				CreateGameObject();
			}
			return chunkGameObject;
		}
	}
	public ChunkModel ChunkModel
	{
		get { return ChunkGameObject.GetComponent<ChunkModel>(); }
	}

	public Transform transform { get { return ChunkGameObject.transform; } }

	public Chunk(int x, int y)
	{
		this.x = x;
		this.y = y;
		TestGen();
	}

	void TestGen()
	{
		for (int x = 0; x < CHUNK_WIDTH; x++)
		{
			for (int y = 0; y < CHUNK_HEIGHT; y++)
			{
				byte id = Random.Range(0, 4) == 0 ? (byte)1 : (byte)0;
				SetTileId(id, x, y, 2);
				SetTileId(2, x, y, 3);
			}
		}
	}

	public int GetX()
	{
		return x;
	}

	public void SetActive(bool enabled)
	{
		if (IsActive() != enabled) ChunkGameObject.SetActive(enabled);
	}

	public bool IsActive()
	{
		return ChunkGameObject.activeSelf;
	}

	public int GetY()
	{
		return y;
	}

	public void Dispose()
	{
		if (chunkGameObject != null)
		{
			Object.DestroyImmediate(chunkGameObject);
		}
	}

	public BoolMap GetSolidMap()
	{
		return solidMap;
	}

	public void SetTileId(byte id, int x, int y, int z)
	{
		if (!IsTileValid(x, y, z))
		{
			return;
		}
		dirty = true;
		tileMap.Set(x, y, z, id);
		solidMap.Set(x, y, z, IsTileSolid(id));
	}

	public bool IsTileSolid(int x, int y, int z)
	{
		return IsTileSolid(GetTileId(x, y, z));
	}

	public Tile GetTile(int x, int y, int z)
	{
		return GetTile(GetTileId(x, y, z));
	}

	public bool IsTileSolid(byte id)
	{
		return GetTile(id).solid;
	}

	public Tile GetTile(byte id)
	{
		return TileManager.Current.GetTile(id);
	}

	public bool IsTileValid(int x, int y, int z)
	{
		return !(x < 0 || y < 0 || z < 0 || x >= CHUNK_WIDTH || y >= CHUNK_HEIGHT || z >= CHUNK_DEPTH);
	}

	public byte GetTileId(int x, int y, int z)
	{
		if (!IsTileValid(x, y, z))
		{
			return 0;
		}
		return tileMap.Get(x, y, z);
	}

	public void CreateGameObject()
	{
		chunkGameObject = new GameObject();
		chunkGameObject.name = "Chunk(" + x + "," + y + ")";
		chunkGameObject.tag = "Chunk";
		chunkGameObject.transform.parent = World.Current.transform;
		chunkGameObject.transform.position = new Vector3(CHUNK_WIDTH * x * Tile.TILE_WIDTH, CHUNK_HEIGHT * y * Tile.TILE_HEIGHT);
		chunkGameObject.AddComponent<MeshRenderer>();
		chunkGameObject.AddComponent<MeshFilter>();
		chunkGameObject.GetComponent<Renderer>().sharedMaterial = World.Current.tileAtlas;
		chunkGameObject.isStatic = true;
		chunkGameObject.AddComponent<ChunkModel>();
		ChunkModel.chunkX = x;
		ChunkModel.chunkY = y;
	}

}
