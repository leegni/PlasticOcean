#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class VRHandsDrawer : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> handsList;
	public Material internalHandsMat;
	public Material internalBlitMat;
		
	[Space]
	public bool reduceResolution = false;
	[Range(0.125f, 1.0f)]
	public float reductionAmount = 0.5f;

	private CommandBuffer vrHandsBuffer;
	private Camera thisCamera;
	private int handsTexPropID;
	private List<Renderer> handsRenderers;
	private Material internalHandsMatCopy;
	
	private int antiAliasing = 1;
	private int rtX = -1;
	private int rtY = -1;
	private int srcBlend = 1;
	private int dstBlend = 10;

#if UNITY_EDITOR
	private Camera[] sceneCameras;
#endif

	private void Start()
	{
		Init();
	}

	public void AddObjectToRender(GameObject go)
	{
		if(!handsList.Contains(go))
		{
			handsList.Add(go);
			Init();
		}
	}

	public void RemoveObjectFromRender(GameObject go)
	{
		if(handsList.Contains(go))
		{
			handsList.Remove(go);
			Init();
		}
	}

	private void Update()
	{
		if(handsList == null) return;

#if UNITY_EDITOR
		Init();

		sceneCameras = SceneView.GetAllSceneCameras();
		if(vrHandsBuffer != null)
		{
			for(int i = 0; i < sceneCameras.Length; i++)
			{
				if(sceneCameras[i] != null)
				{
					sceneCameras[i].depthTextureMode |= DepthTextureMode.Depth;
					sceneCameras[i].RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, vrHandsBuffer);
					sceneCameras[i].AddCommandBuffer(CameraEvent.AfterForwardAlpha, vrHandsBuffer);
				}
			}
		}
#endif

		antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);

		if(reduceResolution == true)
		{
			rtX = (int)(thisCamera.pixelWidth * reductionAmount);
			rtY = (int)(thisCamera.pixelHeight * reductionAmount);
		}else{
			rtX = -1;
			rtY = -1;
		}

		vrHandsBuffer.Clear();
		vrHandsBuffer.GetTemporaryRT(handsTexPropID, rtX, rtY, 16, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, antiAliasing);
		vrHandsBuffer.SetRenderTarget(handsTexPropID);
		vrHandsBuffer.ClearRenderTarget(true, true, Color.clear);

		for(int i = 0; i < handsRenderers.Count; i++)
		{
			vrHandsBuffer.DrawRenderer(handsRenderers[i], internalHandsMatCopy);
		}

		vrHandsBuffer.Blit(handsTexPropID, BuiltinRenderTextureType.CameraTarget, internalBlitMat);
		vrHandsBuffer.ReleaseTemporaryRT(handsTexPropID);
	}

	private Material internalHandsMat_old;
	private void Init()
	{
		if(internalBlitMat == null)
		{ 
			Debug.LogWarning("VRHandsDrawer: internalBlitMat not set");
			//this.enabled = false;
			return;
		}

		if(internalHandsMat != null)
		{
			if(internalHandsMatCopy == null || internalHandsMat != internalHandsMat_old)
			{
				internalHandsMatCopy = new Material(internalHandsMat);
				internalHandsMat_old = internalHandsMat;
			}
		} else {
			Debug.LogWarning("VRHandsDrawer: internalHandsMat not set");
			//this.enabled = false;
			return;
		}

		internalHandsMatCopy.CopyPropertiesFromMaterial(internalHandsMat);
		internalHandsMatCopy.EnableKeyword("_INNERONLY_ON");

		int srcBlendID = Shader.PropertyToID("_SrcBlend");
		int dstBlendID = Shader.PropertyToID("_DstBlend");
		if(internalHandsMatCopy.HasProperty(srcBlendID) && internalHandsMatCopy.HasProperty(dstBlendID))
		{
			srcBlend = internalHandsMatCopy.GetInt(srcBlendID);
			dstBlend = internalHandsMatCopy.GetInt(dstBlendID);

			internalBlitMat.SetInt(srcBlendID, srcBlend);
			internalBlitMat.SetInt(dstBlendID, dstBlend);
		} else {
			Debug.LogWarning("VRHandsDrawer: Incorrect internalHandsMat");
			this.enabled = false;
			return;
		}


		if(vrHandsBuffer == null)
		{
			vrHandsBuffer = new CommandBuffer();
			vrHandsBuffer.name = "VRHandsCommandBuffer";
		}

		thisCamera = this.GetComponent<Camera>();
		if(thisCamera != null)
		{
			thisCamera.depthTextureMode |= DepthTextureMode.Depth;
			thisCamera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, vrHandsBuffer);
			thisCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, vrHandsBuffer);
		}

		handsTexPropID = Shader.PropertyToID("_VRHandsTex");

		if(handsRenderers == null)
			handsRenderers = new List<Renderer>();
		else
			handsRenderers.Clear();

		if(handsList != null)
		{
			foreach(GameObject go in handsList)
			{
				if(go == null) continue;

				Renderer r = go.GetComponent<Renderer>();
				if(r == null)
				{
					r = go.GetComponentInChildren<Renderer>();
					if(r == null) continue;
				}
				handsRenderers.Add(r);
			}
		}
	}

	private void OnEnable()
	{
		Init();
	}
	private void OnDisable()
	{
		if(thisCamera != null && vrHandsBuffer != null)
			thisCamera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, vrHandsBuffer);

#if UNITY_EDITOR
		sceneCameras = SceneView.GetAllSceneCameras();
		if(vrHandsBuffer != null)
		{
			for(int i = 0; i < sceneCameras.Length; i++)
			{
				if(sceneCameras[i] != null)
				{
					sceneCameras[i].RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, vrHandsBuffer);
				}
			}
		}
#endif
	}
}