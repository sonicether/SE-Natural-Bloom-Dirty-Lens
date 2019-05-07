using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sonic Ether/SE Natural Bloom and Dirty Lens")]
public class SENaturalBloomAndDirtyLens : MonoBehaviour
{
	[Range(0.0f, 0.4f)]
	public float bloomIntensity = 0.05f;

	public Shader shader;
	private Material material;

	public Texture2D lensDirtTexture;
	[Range(0.0f, 0.95f)]
	public float lensDirtIntensity = 0.05f;

	private bool isSupported;

	private float blurSize = 4.0f;

	public enum DepthBlendFunction
	{
		Exponential,
		ExponentialSquared
	};

	Camera cam;

	public bool inputIsHDR;
	public bool lowQuality = false;

	public bool depthBloom = true;
	public DepthBlendFunction depthBlendFunction = DepthBlendFunction.Exponential;
	[Range(0.0f, 1.0f)]
	public float depthBlendFactor = 0.1f;

	void OnEnable() 
	{
		isSupported = true;

		if (!material)
			material = new Material(shader);

		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			isSupported = false;
		}

		cam = GetComponent<Camera>();
	}

	void OnDisable()
	{
		if(material)
			DestroyImmediate(material);
	}
	
	void OnRenderImage(RenderTexture source, RenderTexture destination) 
	{
		if (!isSupported)
		{
			Graphics.Blit(source, destination);
			return;
		}

		//if (!material)
		//	material = new Material(shader);

		#if UNITY_EDITOR
		if (source.format == RenderTextureFormat.ARGBHalf)
			inputIsHDR = true;
		else
			inputIsHDR = false;
		#endif

		material.hideFlags = HideFlags.HideAndDontSave;

		material.SetFloat("_BloomIntensity", Mathf.Exp(bloomIntensity) - 1.0f);
		material.SetFloat("_LensDirtIntensity", Mathf.Exp(lensDirtIntensity) - 1.0f);
		material.SetFloat("_DepthBlendFactor", depthBloom ? Mathf.Pow(depthBlendFactor, 2.0f) : 0.0f);
		material.SetInt("_DepthBlendFunction", depthBlendFunction == DepthBlendFunction.Exponential ? 0 : 1);
		material.SetMatrix("ProjectionMatrixInverse", cam.projectionMatrix.inverse);

		source.filterMode = FilterMode.Bilinear;

		RenderTexture clampedSource = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
		Graphics.Blit(source, clampedSource, material, 5);

		int initialDivisor = lowQuality ? 4 : 2;

		int rtWidth = source.width / initialDivisor;
		int rtHeight = source.height / initialDivisor;

		RenderTexture downsampled;
		downsampled = clampedSource;

		/* 
		material.SetTexture("COCK", clampedSource);
		RenderTexture peen = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
		Graphics.Blit(clampedSource, peen, material, 1);
		material.SetTexture("PEEN", peen);
		*/


		float spread = 1.0f;
		int iterations = 1;

		int octaves = lowQuality ? 4 : 8;

		for (int i = 0; i < octaves; i++)
		{
			RenderTexture rt = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
			rt.filterMode = FilterMode.Bilinear;

			Graphics.Blit(downsampled, rt, material, 1);


			if (i > 1)
				spread = 1.0f;
			else
				spread = 0.5f;

			if (i == 2)
				spread = 0.75f;

			if (i >= 1)
			{
				iterations = 2;
			}


			for (int j = 0; j < iterations; j++)
			{
				material.SetFloat("_BlurSize", (blurSize * 0.5f + j) * spread);

				//vertical blur
				RenderTexture rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
				rt2.filterMode = FilterMode.Bilinear;
				Graphics.Blit(rt, rt2, material, 2);
				RenderTexture.ReleaseTemporary(rt);
				rt = rt2;

				rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
				rt2.filterMode = FilterMode.Bilinear;
				Graphics.Blit(rt, rt2, material, 3);
				RenderTexture.ReleaseTemporary(rt);
				rt = rt2;
			}

			downsampled = rt;

			switch (i)
			{
				case 0:
					material.SetTexture("_Bloom0", rt);
					break;
				case 1:
					material.SetTexture("_Bloom1", rt);
					break;
				case 2:
					material.SetTexture("_Bloom2", rt);
					break;
				case 3:
					material.SetTexture("_Bloom3", rt);
					break;
				case 4:
					material.SetTexture("_Bloom4", rt);
					break;
				case 5:
					material.SetTexture("_Bloom5", rt);
					break;
				case 6:
					material.SetTexture("_Bloom6", rt);
					break;
				case 7:
					material.SetTexture("_Bloom7", rt);
					break;
				default: 
					break;
			}

			RenderTexture.ReleaseTemporary(rt);

			rtWidth /= lowQuality ? 3 : 2;
			rtHeight /= lowQuality ? 3 : 2;
		}

//		RenderTexture smear0 = RenderTexture.GetTemporary(source.width / 8, source.height / 8, 0, source.format);

		material.SetTexture("_LensDirt", lensDirtTexture);
		Graphics.Blit(clampedSource, destination, material, lowQuality ? 4 : 0);
		RenderTexture.ReleaseTemporary(clampedSource);

		//RenderTexture.ReleaseTemporary(peen);
	}
}