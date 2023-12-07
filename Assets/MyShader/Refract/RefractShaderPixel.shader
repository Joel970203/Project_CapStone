Shader "MyShader/RefractShaderPixel" 
{
    Properties 
    {
        _IOR ("IOR", Float) = 0.06
        _power ("power", Float) = 3.5
        _fresnelColor("fresnelColor", Vector) = (0.9, 0.3, 0.3, 0.0)
    }
    SubShader 
    {
        Tags 
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent"
        } 
        Pass
        {
            Name "Universal Forward"
            Tags {"LightMode" = "UniversalForward"}
            Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off
            
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x 
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float _IOR;
            float _power;
            float4 _fresnelColor;
            CBUFFER_END

            struct VertexInput 
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL; 
                float4 tangentOS : TANGENT;
            };
            struct VertexOutput 
            {
                float4 positionCS : SV_POSITION;
                float4 positionNDC : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            };

            void computeNDC(float4 positionSS, out float4 positionNDC)
            {
                positionNDC.x = positionSS.x / positionSS.w;
                positionNDC.y = positionSS.y / positionSS.w;
                positionNDC.z = positionSS.z / positionSS.w;
                positionNDC.w = 1.0;
            }

            void computeFresnel(float3 normalWS, float3 viewDirWS, float power, out float output)
            {
                output = pow((1.0 - saturate(dot(normalize(normalWS), normalize(viewDirWS)))), power);
            }

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz); 
                
                //월드 공간에서의 굴절벡터 계산을 위한 정보 입력
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.viewDirWS = normalize(_WorldSpaceCameraPos.xyz - TransformObjectToWorld(v.positionOS.xyz));

                //굴절벡터 값을 탄젠트 공간으로 변환하기 위한 정보 입력 
                float sign = v.tangentOS.w * GetOddNegativeScale();
                o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * sign;
                
                //NDC 포지션 계산
                computeNDC(ComputeScreenPos(o.positionCS), o.positionNDC);
                return o; 
            }
            
            half4 frag(VertexOutput i) : SV_Target 
            {
                //월드 공간에서의 굴절벡터 계산 및 탄젠트 공간으로 변환
                float3 refractVectorWS = refract(i.viewDirWS, i.normalWS, _IOR);
                float3x3 TBNMatrix = float3x3(i.tangentWS, i.bitangentWS, i.normalWS);
                float3 refractVectorTS = TransformWorldToTangentDir(refractVectorWS, TBNMatrix, true);
                
                //굴절된 NDC 값 계산
                float3 refractedNDC = i.positionNDC.xyz + refractVectorTS;

                //프레넬 값 계산
                float fresnel;
                computeFresnel(i.normalWS, i.viewDirWS, _power, fresnel);
                float4 coloredFresnel = fresnel * _fresnelColor;

                //씬컬러 계산
                float3 sceneColor = SampleSceneColor(refractedNDC.xy); /// input.screenPosition.w);

                //프레넬 값과 덧셈 연산
                sceneColor += coloredFresnel.xyz;
                return half4(sceneColor.xyz,1);
            }

            ENDHLSL
        }
    }
}