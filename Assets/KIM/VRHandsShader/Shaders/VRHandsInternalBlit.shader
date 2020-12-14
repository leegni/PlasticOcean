Shader "Hidden/VRHandsInternalBlit"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Blend [_SrcBlend] [_DstBlend]

		Pass
		{
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_ST;
float4 frag (v2f_img i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
return col;
}
ENDCG
		}
	}
}
