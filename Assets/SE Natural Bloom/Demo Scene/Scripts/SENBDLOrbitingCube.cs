using UnityEngine;
using System.Collections;

public class SENBDLOrbitingCube : MonoBehaviour
{

	Transform transf;

	Vector3 rotationVector;
	float rotationSpeed;

	Vector3 spherePosition;

	Vector3 randomSphereRotation;

	float sphereRotationSpeed;

	Vector3 Vec3(float x)
	{
		return new Vector3(x, x, x);
	}

	void Start() 
	{
		transf = this.transform;
		rotationVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		rotationVector = Vector3.Normalize(rotationVector);

		spherePosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		spherePosition = Vector3.Normalize(spherePosition);
		spherePosition *= Random.Range(16.5f, 18.0f);
		//spherePosition *= 20.0f;

		randomSphereRotation = new Vector3(Random.Range(-1.1f, 1.0f), Random.Range(0f, 0.1f), Random.Range(0.5f, 1f));
		randomSphereRotation = Vector3.Normalize(randomSphereRotation);

		sphereRotationSpeed = Random.Range(10f, 20f);

		rotationSpeed = Random.Range(1f, 90f);

		transf.localScale = Vec3(Random.Range(1.0f, 2.0f));

	}
	
	void Update() 
	{
		Quaternion sphereRotation = Quaternion.Euler(randomSphereRotation * Time.time * sphereRotationSpeed);
		Vector3 pos = sphereRotation * spherePosition;
		pos += spherePosition * (Mathf.Sin(Time.time - spherePosition.magnitude / 10.0f) * 0.5f + 0.5f);
		pos += sphereRotation * spherePosition * (Mathf.Sin((Time.time * 3.1415265f / 4.0f) - spherePosition.magnitude / 10.0f) * 0.5f + 0.5f);
		transf.position = pos;
		transf.rotation = Quaternion.Euler(rotationVector * Time.time * rotationSpeed);
	}
}
