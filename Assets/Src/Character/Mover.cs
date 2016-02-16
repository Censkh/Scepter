using System;
using UnityEngine;

class Mover : MonoBehaviour
{

	public struct CollisionData
	{
		public bool xcollision;
		public bool ycollision;

		public CollisionData(bool x, bool y)
		{
			this.xcollision = x;
			this.ycollision = y;
		}
	}

	public Vector2 maxSpeed = new Vector2(12, 12);
	[HideInInspector]
	public Vector2 velocity = Vector2.zero;
	public float friction = 0.98f;
	public Vector3 debugTilePos;

	void Update()
	{
		CollisionData data = Move(velocity * Time.deltaTime * 10f);
		if (data.ycollision)
		{
			velocity.y = 0;
		}
		velocity *= friction;
	}

	public CollisionData Move(Vector2 vector)
	{
		if (vector.x != 0f || vector.y != 0f)
		{
			Vector3 pos = new Vector3(vector.x, vector.y, 0);
			Vector3 tilePos = Vector3.zero;
			bool xcol = false, ycol = false;
			Collider collider = GetComponent<Collider>();
			Bounds bounds = collider.bounds;
			Vector3 dist = Vector3.zero;
			float border = 0.0000005f;
			if (vector.x > 0)
			{
				if (IsCollision(new Vector3(bounds.center.x + vector.x, bounds.center.y, transform.position.z), out tilePos))
				{
					dist.x += Mathf.Min(vector.x, (tilePos.x - border) - bounds.max.x);
					xcol = true;
				}
				else
				{
					dist.x += vector.x;
				}
			}
			else if (vector.x < 0)
			{
				if (IsCollision(new Vector3(bounds.center.x + vector.x, bounds.center.y, transform.position.z), out tilePos))
				{
					dist.x += Mathf.Max(vector.x, (tilePos.x + Tile.TILE_WIDTH + border) - bounds.min.x);
					xcol = true;
				}
				else
				{
					dist.x += vector.x;
				}
			}
			if (vector.y < 0)
			{
				if (IsCollision(new Vector3(bounds.center.x, bounds.center.y + vector.y, transform.position.z), out tilePos))
				{
					dist.y += Mathf.Max(vector.y, (tilePos.y + Tile.TILE_HEIGHT + border) - bounds.min.y);
					ycol = true;
				}
				else
				{
					dist.y += vector.y;
				}
			}
			else if (vector.y > 0)
			{
				if (IsCollision(new Vector3(bounds.center.x, bounds.center.y + vector.y, transform.position.z), out tilePos))
				{
					dist.y += Mathf.Min(vector.y, (tilePos.y - border) - bounds.max.y);
					ycol = true;
				}
				else
				{
					dist.y += vector.y;
				}
			}
			transform.position += dist;
			return new CollisionData(xcol, ycol);

		}
		return new CollisionData(false, false);
	}

	public void AddForce(Vector2 vector)
	{
		velocity += vector;
	}

	public bool IsCollision(Vector2 pos, out Vector3 tileWorldPos)
	{
		Collider collider = GetComponent<Collider>();
		return World.Current.IsColliding(collider.bounds, pos, Tile.DEPTH_CHARACTER, out tileWorldPos);
	}

	public bool IsCollision(Vector2 pos)
	{
		Vector3 v = Vector3.zero;
		return IsCollision(pos, out v);
	}

	void OnGUI()
	{
		Vector3 tilePos = Camera.main.WorldToScreenPoint(debugTilePos);
		Vector3 tileSize = Tile.TILE_SIZE;
		GUI.Box(new Rect(tilePos.x, tilePos.y, tileSize.x, tileSize.y), "Hello World");
	}

}

