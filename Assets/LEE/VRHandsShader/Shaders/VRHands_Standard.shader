// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "VRHands/Standard"
{
	Properties
	{
		[HDR] _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		[Space]
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		[NoScaleOffset] _MetallicGlossMap("Metallic", 2D) = "white" {}

		[Space]
		_BumpScale("Scale", Float) = 1.0
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

		[Space]
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		[NoScaleOffset] _OcclusionMap("Occlusion", 2D) = "white" {}
		
		[Space]
		_EmissionColor("Color", Color) = (0.0, 0.0, 0.0)
		[NoScaleOffset] _EmissionMap("Emission", 2D) = "white" {}

	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		Pass
		{
			ZWrite On
			ColorMask 0
		}

		ZWrite Off
		ZTest Equal

CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
#include "UnityStandardUtils.cginc"

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _MetallicGlossMap;
sampler2D _OcclusionMap;
sampler2D _EmissionMap;

struct Input
{
	float2 uv_MainTex;
};

fixed4 _Color;
half _Glossiness;
half _Metallic;
half _BumpScale;
half3 _EmissionColor;
half _OcclusionStrength;

// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
// #pragma instancing_options assumeuniformscaling
UNITY_INSTANCING_BUFFER_START(Props)
	// put more per-instance properties here
UNITY_INSTANCING_BUFFER_END(Props)

void surf (Input IN, inout SurfaceOutputStandard o)
{
	fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	
	o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
	
	o.Emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor;
	
	half2 mg = tex2D(_MetallicGlossMap, IN.uv_MainTex).ra;
	o.Metallic = mg.x * _Metallic;
	o.Smoothness = mg.y * _Glossiness;
	
	half occ = tex2D(_OcclusionMap, IN.uv_MainTex).g;
	o.Occlusion = LerpOneTo(occ, _OcclusionStrength);

	o.Alpha = c.a;
}
ENDCG

	}
FallBack "Diffuse"
}
