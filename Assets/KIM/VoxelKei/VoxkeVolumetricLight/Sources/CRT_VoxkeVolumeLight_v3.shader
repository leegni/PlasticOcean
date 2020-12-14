Shader "VK/CRT_VoxkeVolumeLight_v3"
{
    Properties
    {
        _InputDepthTex ("InputDepthTex", 2D) = "white" {}
        _InputColorTex("InputColorTex", 2D) = "white"{}
        _NearClip("NearClip", Float) = 0.3
        _FarClip("FarClip", Float) = 10.0
        _LightDecay("LightDecay", Float) = 1
    }

    CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"

    sampler2D _InputDepthTex;
    sampler2D _InputColorTex;
    float _NearClip;
    float _FarClip;
    fixed _LightDecay;

    float CalculateDepth(float depth){
        float x,y,z,w;
        x = 1.0 - _NearClip/ _FarClip;
        y = _NearClip / _FarClip;
        z = x / _NearClip;
        w = y / _NearClip;
        return 1.0 / (z * depth + w);
    }

    fixed4 fragUpdate(v2f_customrendertexture i) : SV_Target
    {
        float3 uvw = i.globalTexcoord;
        fixed4 finalColor = fixed4(0,0,0,0);
        float2 uv = float2(uvw.x, uvw.y);
        
        fixed4 inputColor = tex2D(_InputColorTex, uv);
        float falloff = 1 - uvw.z;

        float depth =  CalculateDepth(SAMPLE_DEPTH_TEXTURE(_InputDepthTex, uv));
        depth =  depth / _FarClip;

        fixed4 lightColor = inputColor * pow(falloff, _LightDecay);
        finalColor = (uvw.z < depth) ? lightColor : fixed4(0,0,0,0);

        finalColor.a = max(max(finalColor.r, finalColor.g), finalColor.b);
        
        return finalColor;
    }
    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            Name "Update"
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment fragUpdate
            ENDCG
        }
    }
}