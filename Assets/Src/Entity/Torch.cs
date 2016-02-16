using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour
{

	void Start()
	{

	}

	void Update()
	{
		float range = 5;
		GetComponent<Light>().intensity += (Random.Range(0, range * 2) - range) * Time.deltaTime;
	}
}
