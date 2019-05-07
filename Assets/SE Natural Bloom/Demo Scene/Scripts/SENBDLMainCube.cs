using UnityEngine;
using System.Collections;

public class SENBDLMainCube : MonoBehaviour
{
	Color[] glowColors = new Color[4];

	public GameObject orbitingCube;
	public GameObject glowingOrbitingCube;

	public GameObject cubeEmissivePart;
	public GameObject particles;

	const float newColorFrequency = 8f;
	float newColorCounter = 0f;
	Color currentColor;
	Color previousColor;
	[HideInInspector]
	public Color glowColor;
	int currentColorIndex = 0;

	private float bloomAmount = 0.04f;
	private float lensDirtAmount = 0.3f;

	private float fps;

	private float deltaTime;

	SENaturalBloomAndDirtyLens bloomShader;

	void Start() 
	{

		glowColors[0] = new Color(255f / 255f, 120f / 255f,  13f / 255f);
		glowColors[2] = new Color(84f  / 255f, 163f / 255f, 255f / 255f);
		glowColors[1] = new Color(155f / 255f, 255f / 255f,  30f / 255f);
		glowColors[3] = new Color(255f / 255f, 47f  / 255f,   0f / 255f);
		currentColor = glowColors[0];


		SENBDLGlobal.sphereOfCubesRotation = Quaternion.identity;
		for (int i = 0; i < 150; i++)
		{
			Instantiate(orbitingCube, Vector3.zero, Quaternion.identity);
		}

		for (int i = 0; i < 19; i++)
		{
			Instantiate(glowingOrbitingCube, Vector3.zero, Quaternion.identity);
		}

		Camera.main.backgroundColor = new Color(0.08f, 0.08f, 0.08f);

		SENBDLGlobal.mainCube = this;

		bloomShader = Camera.main.GetComponent<SENaturalBloomAndDirtyLens>();
	}

	void OnGUI()
	{
		//GUI.Label(new Rect(25, 15, 125, 25), "Bloom Amount: " + bloomAmount.ToString("0.00"));
		//GUI.Label(new Rect(25, 40, 150, 25), "Lens Dirt Amount: " + lensDirtAmount.ToString("0.00"));
		//GUI.Label(new Rect(25, 65, 150, 25), "Time Scale: " + Time.timeScale.ToString("0.00"));
		//GUI.Label(new Rect(25, 90, 150, 25), "FPS: " + Mathf.Round(fps).ToString());
	}
	
	void Update() 
	{
		deltaTime = Time.deltaTime / Time.timeScale;

		AnimateColor();

		RotateSphereOfCubes();

		float rotationSpeed = 40.0f;
		Vector3 rotationVector = Vector3.up * rotationSpeed;
		rotationVector =  Quaternion.Euler(Vector3.right * Time.time * rotationSpeed * 0.5f) * rotationVector;
		
		transform.Rotate(rotationVector * Time.deltaTime);

		IncrementCounters();

		GetInput();
		UpdateShaderValues();

		SmoothFPSCounter();
	}

	void AnimateColor()
	{
		if (newColorCounter >= newColorFrequency)
		{
			newColorCounter = 0f;
			currentColorIndex = (currentColorIndex + 1) % glowColors.Length;
			previousColor = currentColor;
			currentColor = glowColors[currentColorIndex];
		}

		float colorLerpFactor = Mathf.Clamp01((newColorCounter / newColorFrequency) * 5.0f);

		glowColor = Color.Lerp(previousColor, currentColor, colorLerpFactor);

		//glowColor = glowColors[2];

		Color privateColor = glowColor * Mathf.Pow((Mathf.Sin(Time.time) * 0.48f + 0.52f), 4.0f);

		//privateColor = glowColor * 0.8f;

		cubeEmissivePart.GetComponent<Renderer>().material.SetColor ("_EmissionColor", privateColor);
		GetComponent<Light>().color = privateColor;

		Color invColor = new Color();
		invColor.r = 1.0f - glowColor.r;
		invColor.g = 1.0f - glowColor.g;
		invColor.b = 1.0f - glowColor.b;
		invColor = Color.Lerp(invColor, Color.white, 0.1f);

		particles.GetComponent<Renderer>().material.SetColor("_TintColor", invColor);
		
	}

	void RotateSphereOfCubes()
	{
		SENBDLGlobal.sphereOfCubesRotation = Quaternion.Euler(Vector3.up * Time.time * 20.0f);
	}

	void IncrementCounters()
	{
		newColorCounter += Time.deltaTime;
	}

	void GetInput()
	{
		if (Input.GetKey(KeyCode.RightArrow))
		{
			bloomAmount += 0.2f * deltaTime;
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			bloomAmount -= 0.2f * deltaTime;
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			lensDirtAmount += 0.4f * deltaTime;
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			lensDirtAmount -= 0.4f * deltaTime;
		}

		if (Input.GetKey(KeyCode.Period))
		{
			Time.timeScale += 0.5f * deltaTime;
		}

		if (Input.GetKey(KeyCode.Comma))
		{
			Time.timeScale -= 0.5f * deltaTime;
		}

		bloomAmount = Mathf.Clamp(bloomAmount, 0.0f, 0.4f);
		lensDirtAmount = Mathf.Clamp(lensDirtAmount, 0.0f, 0.95f);
		Time.timeScale = Mathf.Clamp(Time.timeScale, 0.1f, 1.0f);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			bloomAmount = 0.05f;
			lensDirtAmount = 0.1f;
			Time.timeScale = 1.0f;
		}
	}

	void UpdateShaderValues()
	{
		bloomShader.bloomIntensity = bloomAmount;
		bloomShader.lensDirtIntensity = lensDirtAmount;
	}

	void SmoothFPSCounter()
	{
		fps = Mathf.Lerp(fps, 1.0f / deltaTime, 5.0f * deltaTime);
	}
}
