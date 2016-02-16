using UnityEngine;
using System.Collections.Generic;

public class SpriteModel : BlockModel
{

	private float scale = 0.025f;
	public Vector2 textureOffset = Vector2.zero;
	public Vector2 textureSize = Vector2.zero;

	void Start()
	{
	}

	public override void ApplyUV(Vector3 start, Vector3 offset1, Vector3 offset2)
	{
		Texture texture = GetMainTexture();
		start = new Vector3(start.x+textureOffset.x,start.y+textureOffset.y,start.z);
		float offset = 0.003f;
		data.uv.Add(new Vector2((start.x+1) / texture.width, start.y / texture.height) + new Vector2(-offset,offset));
		data.uv.Add(new Vector2(start.x / texture.width, start.y / texture.height) + new Vector2(offset, offset));
		data.uv.Add(new Vector2((start.x + 1) / texture.width, (start.y + 1) / texture.height) + new Vector2(-offset, -offset));
		data.uv.Add(new Vector2(start.x / texture.width, (start.y + 1) / texture.height) + new Vector2(offset, -offset));
	}

	public override bool IsCentered()
	{
		return true;
	}

	public override BoolMap GetBlockData()
	{
		Texture2D texture = GetMainTexture();
		BoolMap data = new BoolMap(texture.width, texture.height, 1);
		for (int x = 0; x < (textureSize.x>0 ? textureSize.x : texture.width); x++)
		{
			for (int y = 0; y < (textureSize.y > 0 ? textureSize.y : texture.height); y++)
			{
				if (IsPixelSolid(x, y))
				{
					data.Set(x, y, 0, true);
				}
			}
		}
		return data;
	}

	void Update()
	{

	}

	public override bool IsConvex()
	{
		return true;
	}

	Color GetPixel(int x, int y)
	{
		Texture2D texture = GetMainTexture();
		if (texture == null || x >= texture.width || y >= texture.height || x < 0 || y < 0)
		{
			return Color.clear;
		}
		return texture.GetPixel(x + (int)textureOffset.x, y + (int)textureOffset.y);
	}

	public override Vector3 GetScale()
	{
		return new Vector3(scale, scale, 0.05f
						   );
	}

	public bool IsPixelSolid(int x, int y)
	{
		return IsPixelSolid(GetPixel(x, y));
	}

	public bool IsPixelSolid(Color pixel)
	{
		return pixel.a > 0f;
	}

	public virtual Texture2D GetMainTexture()
	{
		return (Texture2D)GetComponent<Renderer>().sharedMaterial.mainTexture;
	}

}
