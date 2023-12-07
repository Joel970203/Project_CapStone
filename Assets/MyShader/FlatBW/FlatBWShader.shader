Shader "MyShader/FlatBWShader" {
	Properties {
		[MainTexture] _BaseMap("Main Texture", 2D) = "white" {}
		
		_brightness("Brightness", Range(0, 30)) = 1.5
        _metallic("Metallic", Range(0, 10)) = 0.5

		_Smoothness("Smoothness", Float) = 0.73
	}
	SubShader {
		Tags {
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
		float4 _BaseMap_ST;
		//float4 _SpecColor;
		float _brightness;
		float _metallic;
		float _Smoothness;
		CBUFFER_END
		ENDHLSL

		Pass {
			Name "ForwardLit"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x 
            #pragma vertex vert
            #pragma fragment frag

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION 
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING 
			#pragma multi_compile _ SHADOWS_SHADOWMASK 

			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

			struct VertexInput {
				float4 positionOS		: POSITION;
				float4 normalOS			: NORMAL;
				float4 tangentOS 		: TANGENT;
				float2 uv		    	: TEXCOORD0;
				float2 lightmapUV		: TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput {
				float4 positionCS 					: SV_POSITION;
				float3 positionWS					: TEXCOORD0;
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
				float2 uv							: TEXCOORD2;
				float3 viewDirWS 					: TEXCOORD3;
				float3 normalWS 					: TEXCOORD4;
				float3 tangentWS 					: TEXCOORD5;      
				float3 bitangentWS 					: TEXCOORD6;    
				float fogFactor						: TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
    			UNITY_VERTEX_OUTPUT_STEREO  
			};
			
			void InitializeStandardLitSurfaceData(VertexOutput i, out SurfaceData surfaceData){
				surfaceData = (SurfaceData)0; 
				
				half4 baseMapTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
				half3 bwColor = (baseMapTex.r + baseMapTex.g + baseMapTex.b) / 3;

				surfaceData.albedo = bwColor * _brightness;
				surfaceData.metallic = _metallic;

				//surfaceData.specular = half3(0.0, 0.0, 0.0);
				surfaceData.smoothness = _Smoothness;

				float3x3 TBNMatrix = float3x3(i.tangentWS, i.bitangentWS, i.normalWS);
				float3 crossResult = cross(ddy(i.positionWS), ddx(i.positionWS));
				half3 normalTS = TransformWorldToTangent(crossResult, TBNMatrix, true);

				surfaceData.normalTS = normalTS;
				surfaceData.emission = half3(0.0, 0.0, 0.0);
				surfaceData.occlusion = half(1.0);
			}

			void InitializeInputData(VertexOutput i, half3 normalTS, out InputData inputData) {
				inputData = (InputData)0;

				inputData.positionWS = i.positionWS;
				
				half3x3 TBNMatrix = half3x3(i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
				inputData.normalWS = TransformTangentToWorld(normalTS, TBNMatrix);
				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				
				inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.positionWS);

				inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				inputData.fogCoord = InitializeInputDataFog(float4(i.positionWS, 1.0), i.fogFactor);
				inputData.bakedGI = SAMPLE_GI(i.lightmapUV, i.vertexSH, inputData.normalWS);

				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
    			inputData.shadowMask = SAMPLE_SHADOWMASK(i.lightmapUV);
			}

			VertexOutput vert(VertexInput v) {
				VertexOutput o;

				UNITY_SETUP_INSTANCE_ID(v);
    			UNITY_TRANSFER_INSTANCE_ID(v, o);
    			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz); 
 				o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
    			
				o.uv = TRANSFORM_TEX(v.uv, _BaseMap);

				o.viewDirWS = GetWorldSpaceNormalizeViewDir(o.positionWS.xyz);

    			o.normalWS = TransformObjectToWorldNormal(v.normalOS);

    			float sign = v.tangentOS.w * GetOddNegativeScale();
    			o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * sign;

    			OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.lightmapUV);
    			OUTPUT_SH(o.normalWS.xyz, o.vertexSH);

			    o.fogFactor = ComputeFogFactor(o.positionCS.z);				
				return o;
			}


			half4 frag(VertexOutput i) : SV_Target {
				UNITY_SETUP_INSTANCE_ID(i);
			    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(i, surfaceData);

				InputData inputData;
				InitializeInputData(i, surfaceData.normalTS, inputData);

				half4 color = UniversalFragmentPBR(inputData, surfaceData);
				color.rgb = MixFog(color.rgb, inputData.fogCoord);

				return color;
			}
			ENDHLSL
		}

		Pass {
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual		
		}
	}
}
