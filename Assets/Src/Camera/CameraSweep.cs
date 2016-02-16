using UnityEngine;
using System.Collections;

public class CameraSweep : MonoBehaviour
{
	
	public float speed = 6f;
	private GameObject player;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		GetComponent<Camera>().orthographicSize = (Screen.height / (2 * Tile.PIXEL_HEIGHT ))/5;
	}

	void Update()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z), speed*Time.deltaTime);
	}
}
