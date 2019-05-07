using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SENaturalBloomAndDirtyLens))]
public class SENaturalBloomAndDirtyLensEditor : Editor
{
	SerializedObject serObj;

	SerializedProperty bloomIntensity;
	SerializedProperty lensDirtIntensity;
	SerializedProperty lensDirtTexture;
	SerializedProperty lowQuality;
	SerializedProperty depthBloom;
	SerializedProperty depthBlendFunction;
	SerializedProperty depthBlendFactor;

	SENaturalBloomAndDirtyLens instance;

	void OnEnable()
	{
		serObj = new SerializedObject(target);
		bloomIntensity = serObj.FindProperty("bloomIntensity");
		lensDirtIntensity = serObj.FindProperty("lensDirtIntensity");
		lensDirtTexture = serObj.FindProperty("lensDirtTexture");
		lowQuality = serObj.FindProperty("lowQuality");
		depthBloom = serObj.FindProperty("depthBloom");
		depthBlendFunction = serObj.FindProperty("depthBlendFunction");
		depthBlendFactor = serObj.FindProperty("depthBlendFactor");

		instance = (SENaturalBloomAndDirtyLens)target;		
	}

	public override void OnInspectorGUI()
	{
		serObj.Update();


		if (!instance.inputIsHDR)
		{
			EditorGUILayout.HelpBox("The camera is either not HDR enabled or there is an image effect before this one that converts from HDR to LDR. This image effect is dependant an HDR input to function properly.", MessageType.Warning);
		}

		EditorGUILayout.PropertyField(bloomIntensity, new GUIContent("Bloom Intensity", "The amount of light that is scattered inside the lens uniformly. Increase this value for a more drastic bloom."));
		EditorGUILayout.PropertyField(lensDirtIntensity, new GUIContent("Lens Dirt Intensity", "The amount that the lens dirt texture contributes to light scattering. Increase this value for a dirtier lens."));
		EditorGUILayout.PropertyField(lensDirtTexture, new GUIContent("Lens Dirt Texture", "The texture that controls per-channel light scattering amount. Black pixels do not affect light scattering. The brighter the pixel, the more light that is scattered."));
		EditorGUILayout.PropertyField(lowQuality, new GUIContent("Low Quality", "Enable this for lower quality in exchange for faster rendering."));

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(depthBloom, new GUIContent("Depth Blending", "Enable depth-based bloom blending (useful for fog)."));
		if (depthBloom.boolValue)
		{
			EditorGUILayout.PropertyField(depthBlendFunction, new GUIContent("Blend Function", "Depth-based blend function."));
			EditorGUILayout.PropertyField(depthBlendFactor, new GUIContent("Blend Factor", "Depth-based blend factor. Higher values mean bloom is blended more aggressively."));

		}



		serObj.ApplyModifiedProperties();
	}
}
