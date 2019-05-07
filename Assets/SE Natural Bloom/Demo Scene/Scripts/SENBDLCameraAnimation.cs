using UnityEngine;
using System.Collections;

public class SENBDLCameraAnimation : MonoBehaviour
{
	Vector3 randomRotation;
	Vector3 randomModRotation;

	void Start() 
	{
		randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		randomRotation = Vector3.Normalize(randomRotation);
		randomModRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		randomModRotation = Vector3.Normalize(randomModRotation);
	}
	
	void Update() 
	{
		float orbitDistance = 15.0f + (Mathf.Pow(Mathf.Cos(Time.time * 3.14159265f / 15.0f) * 0.5f + 0.5f, 3.0f)) * 35.0f;
		//orbitDistance = 30.0f;

		Vector3 pos = Quaternion.Euler(randomRotation * Time.time * 25.0f) * (Vector3.up * orbitDistance);
		transform.position = pos;
		transform.LookAt(Vector3.zero);
	}
}
