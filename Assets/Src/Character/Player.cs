using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Mover))]
public class Player : MonoBehaviour
{

	float jumpTimer = 0;
	bool jumping = false;

	void Start()
	{

	}

	void FixedUpdate()
	{
		Mover mover = GetComponent<Mover>();
		bool grounded = IsGrounded();
		float h = Input.GetAxis("Horizontal") * 1.3f;
		float v = grounded ? 0f : -2f;
		if (grounded)
		{
			if (Mathf.Abs(h) < 1f)
			{
				mover.velocity.x*=0.6f;
			}
			else if (h < 0 && mover.velocity.x > 0)
			{
				mover.velocity.x = 0;
			}
			else if (h > 0 && mover.velocity.x < 0)
			{
				mover.velocity.x = 0;
			}
		}
		else
		{
			h*=0.8f;
		}
		if (jumping && !Input.GetButton("Jump"))
		{
			jumping = false;
			jumpTimer = 0f;
			if (mover.velocity.y>0) mover.velocity.y = 0;
		}
		if (jumping)
		{
			jumpTimer += Time.fixedDeltaTime;
			if (jumpTimer > 0.7f || (jumpTimer>0.1f&&grounded))
			{
				jumping = false;
				jumpTimer = 0f;
			}
			v += (3f - (jumpTimer * 6)) * (3f - (jumpTimer * 6)) * Time.fixedDeltaTime * 50f;
		}
		if (Input.GetButtonDown("Jump"))
		{
			if (!jumping && grounded)
			{
				jumping = true;
				v += 40f;
			}
		}
		mover.AddForce(new Vector2(h * Time.fixedDeltaTime, v * Time.fixedDeltaTime));
	}

	bool IsGrounded()
	{
		Collider collider = GetComponent<Collider>();
		return World.Current.IsColliding(collider.bounds, new Vector2(collider.bounds.center.x, collider.bounds.center.y - 0.01f), Tile.DEPTH_CHARACTER);
	}

}
