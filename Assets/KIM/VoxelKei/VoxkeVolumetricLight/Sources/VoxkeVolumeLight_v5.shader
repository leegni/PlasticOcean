Shader "VK/VoxkeVolumeLight_v5"
{

    Properties
    {
        _Volume("Volume", 3D) = "" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Intensity("Intensity", Range(0, 1)) = 0.1
        _Step("Step", Float) = 200
        _LoopLimit("LoopLimit", Float) = 200
        [Toggle(ZCORRECT)] _ZCORRECT ("ZCORRECT", Float) = 1
        [Toggle(SMOKE)] _SMOKE ("SMOKE", Float) = 0
        _Noise3D("Noise3D", 3D) = "white"{}
        _NoiseScale("NoiseScale", Float) = 10
        _NoiseDensity("NoiseDensity", Float) = 1
        _NoiseScroll("NoiseScroll", Vector) = (0,0,0,0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #pragma multi_compile_instancing
    #pragma multi_compile _ ZCORRECT
    #pragma multi_compile _ SMOKE

    inline float3 GetCameraForward()     { return -UNITY_MATRIX_V[2].xyz;    }
    inline float  GetCameraNearClip()    { return _ProjectionParams.y;       }

    struct appdata
    {
        float4 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 vertex   : SV_POSITION;
        float4 localPos : TEXCOORD0;
        float4 worldPos : TEXCOORD1;
        float4 projPos : TEXCOORD3;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };

    sampler3D _Volume;
    fixed4 _Color;
    fixed _Intensity;
    float _Step;
    float _LoopLimit;
    #ifdef SMOKE
        float _NoiseScale;
        float _NoiseDensity;
        sampler3D _Noise3D;
        fixed4 _NoiseScroll;
    #endif
    #ifdef ZCORRECT
        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
    #endif

    v2f vert(appdata v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.localPos = v.vertex;
        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        o.projPos  = ComputeScreenPos(o.vertex);
        COMPUTE_EYEDEPTH(o.projPos.z);
        return o;
    }
    fixed4 frag(v2f i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        UNITY_SETUP_INSTANCE_ID(i);
        
        float3 wdir = i.worldPos - _WorldSpaceCameraPos;
        float3 ldir = normalize(mul(unity_WorldToObject, wdir));
        float3 lstep = ldir / _Step;
        float stepLength = length(lstep);

        //ボリュームの内外でレイの開始位置切り替え
        float3 lpos;
        float3 rayStartPosWorld;
        float lengthFromCamToNearclip = GetCameraNearClip() / dot(wdir, GetCameraForward());
        float3 nearClipWorldPos = _WorldSpaceCameraPos + wdir * lengthFromCamToNearclip;
        float3 nearClipLocalPos = mul(unity_WorldToObject, float4(nearClipWorldPos,1));
        float lengthFromCamToRayStartPos = 0;
        //四角錘の内外判定
        if(abs(nearClipLocalPos.x)*2 < (nearClipLocalPos.z + 0.5)
        && abs(nearClipLocalPos.y)*2 < (nearClipLocalPos.z + 0.5) 
        && abs(nearClipLocalPos.z) < 0.5){
            lpos = nearClipLocalPos;
            lengthFromCamToRayStartPos = lengthFromCamToNearclip;
            }else{
            lpos = i.localPos.xyz + lstep* 0.01;//ポリゴン表面でチラつくので0.01ステップ分だけ進める
            lengthFromCamToRayStartPos = length(i.worldPos - _WorldSpaceCameraPos);
        }
        rayStartPosWorld = mul(unity_ObjectToWorld, float4(lpos,1));

        #ifdef ZCORRECT
            //シーンデプス
            float depth = LinearEyeDepth(
            SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
            float rayLimitLength = depth / dot(normalize(wdir), GetCameraForward());
        #endif
        
        fixed4 output = float4(0,0,0,0);
        [loop]
        for (int i = 0; i < _LoopLimit; ++i)
        {
            float3 lstepByDistance = lstep;
            if (abs(lpos.z) < 0.5 && (lpos.z + 0.5 ) > abs(lpos.x)*2  && (lpos.z +0.5) > abs(lpos.y)*2 ) {//四角錐の内部のときだけサンプル
                float2 uv = float2(lpos.x, lpos.y);
                float distance = lpos.z + 0.5;
                float2 pivot = float2(0.5,0.5);
                float3 uvwScaled = float3((lpos.x / distance) + 0.5, (lpos.y / distance) + 0.5, lpos.z + 0.5);
                lstepByDistance *= distance + 0.01 ;

                fixed4 volumeSampling = tex3Dlod(_Volume, float4(uvwScaled,1));
                
                #ifdef SMOKE
                    float3 wpos = mul(unity_ObjectToWorld, float4(lpos,1));
                    fixed4 noise3D = tex3Dlod(_Noise3D, float4((wpos / _NoiseScale) + _Time.y * _NoiseScroll.xyz, 1));
                    float noise = pow(noise3D.r, _NoiseDensity);
                    output += volumeSampling  * noise;
                #else
                    output += volumeSampling;
                #endif
            }
            lpos += lstepByDistance;
            if (!all(max(0.5 - abs(lpos), 0.0))) break;

            #ifdef ZCORRECT
                float rayLength = lengthFromCamToRayStartPos + length(mul(unity_ObjectToWorld, float4(lpos,1)) - rayStartPosWorld);
                if(rayLimitLength < rayLength){
                    //シーンデプスを超えた場合はシーンデプスの位置で一度だけサンプルする
                    float diffWorld = rayLength - rayLimitLength;
                    float3 diffVecWorld = normalize(wdir) * diffWorld;
                    float3 depthPosLocal = lpos + mul(unity_WorldToObject, diffVecWorld);
                    float3 uvwScaledDepthPos = float3((depthPosLocal.x / (depthPosLocal.z + 0.5)) + 0.5, 
                    (depthPosLocal.y / (depthPosLocal.z + 0.5)) + 0.5, 
                    depthPosLocal.z + 0.5);
                    fixed4 volumeSamplingDepthPos = tex3Dlod(_Volume, float4(uvwScaledDepthPos,1));
                    #ifdef SMOKE
                        float3 wpos = mul(unity_ObjectToWorld, float4(depthPosLocal,1));
                        fixed4 noise3D = tex3Dlod(_Noise3D, float4((wpos / _NoiseScale) + _Time.y * _NoiseScroll.xyz, 1));
                        float noise = pow(noise3D.r, _NoiseDensity);
                        output += volumeSamplingDepthPos * noise;
                    #else
                        output += volumeSamplingDepthPos;
                    #endif
                    break;
                }
            #endif
        }

        if(output.a == 0) clip(-1);

        return output * _Intensity * _Color;
    }
    ENDCG

    SubShader
    {

        Tags 
        { 
            "Queue" = "Transparent-100"
            "RenderType" = "Transparent" 
        }
        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always
            Blend One One 
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            ENDCG
        }
    }
}