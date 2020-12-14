using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class VRHandsRimGUI : ShaderGUI
{
	enum blendMode
	{
		Additive = 0,
		Alpha = 1
	}

	string[] modeNames = new string[] { "Additive", "Alpha"};
	int mode = 0;

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		Material targetMat = materialEditor.target as Material;
		mode = targetMat.GetInt("_Mode");
		
		EditorGUI.BeginChangeCheck();
			mode = EditorGUILayout.Popup("Blending Mode", mode, modeNames);
		if(EditorGUI.EndChangeCheck())
		{
			materialEditor.RegisterPropertyChangeUndo("_Mode");
			targetMat.SetInt("_Mode", mode);

			if(mode == 0)
			{
				targetMat.SetInt("_SrcBlend", (int)BlendMode.One);
				targetMat.SetInt("_DstBlend", (int)BlendMode.One);
			} else if(mode == 1)
			{
				targetMat.SetInt("_SrcBlend", (int)BlendMode.One);
				targetMat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
			}
		}

		base.OnGUI(materialEditor, properties);
	}
}