Shader "VRHands/Rim"
{
	Properties
	{
		[Header(Colors)]
		[HDR] _Color("Main Color(RGBA)", Color) = (0.0, 0.5, 1.0, 1.0)
		[HDR] _SecondColor("Second Main Color(RGBA)", Color) = (1.1, 1.1, 1.1, 1.0)

		[Space]
		[HDR] _InsideColor("Inside Color(RGBA)", Color) = (1.0, 0.5, 0.0, 0.5)
		[HDR] _SecondInside("Second Inside Color(RGBA)", Color) = (1.0, 0.0, 0.0, 0.5)

		[Space]
		[HDR] _BorderColor("Border Color(RGBA)", Color) = (1.0, 1.0, 0.0, 1.0)
		
		[Space]
		[Toggle] _UseSecondColors("Use Second Colors", Float) = 0.0

		[Header(Glow Parametrs)]
		_Thickness("Thickness", Range(0, 1)) = 1.0
		_EdgePosition("Edge position End", Range(0, 1)) = 0.25
		_BorderSize("Intersection Border Size", Range(0.01, 30.0)) = 10.0

		[HideInInspector] _Mode("Blend Mode", Float) = 0.0
		[HideInInspector] _SrcBlend("Blend Source", Float) = 1
		[HideInInspector] _DstBlend ("Blend Destination", Float) = 10
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend [_SrcBlend] [_DstBlend]

		Pass
		{
			ZWrite On
			ColorMask 0
		}
		Pass
		{
			ZWrite Off
			ZTest Equal
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#pragma shader_feature _USESECONDCOLORS_ON
#pragma multi_compile _INNERONLY_OFF _INNERONLY_ON

struct v2f
{
	float4 vertex  : SV_POSITION;
	half4 projPos  : TEXCOORD0;
	half3 normal   : TEXCOORD1;
	half3 vDir     : TEXCOORD2;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert(appdata_base v)
{
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	o.vertex = UnityObjectToClipPos(v.vertex);
	o.projPos = ComputeScreenPos(o.vertex);
	COMPUTE_EYEDEPTH(o.projPos.z);

	o.normal = v.normal;
	o.vDir   = ObjSpaceViewDir(v.vertex);
return o;
}


UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

half4 _Color;
half4 _InsideColor;
half4 _BorderColor;

half4 _SecondColor;
half4 _SecondInside;

half _Thickness;
half _EdgePosition;
half _BorderSize;

fixed4 frag(v2f i) : SV_Target
{
	i.normal = normalize(i.normal);
	i.vDir = normalize(i.vDir);
	half fresnel = dot(i.normal, i.vDir);
	half p = smoothstep(_Thickness, 0.0, abs(fresnel - _EdgePosition));

#if defined(_USESECONDCOLORS_ON)
	_Color = lerp(_SecondColor, _Color, fresnel);
	_InsideColor = lerp(_SecondInside, _InsideColor, fresnel);
#endif

	half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
	half partZ = i.projPos.z;

	half dist = (sceneZ - partZ) * _BorderSize;
#if defined(_INNERONLY_ON)
	if (dist > 0.0) discard;
#endif	

	half edge = saturate(1.0 - abs(dist));

	half4 col = lerp(_Color, _BorderColor, edge);
	p = max(p, edge);

	if (dist < 0.0) col = _InsideColor;

	col *= p;
	col.rgb *= col.a;
return col;
}
ENDCG
		}
	}

CustomEditor "VRHandsRimGUI"
}
