Shader "URPShader/BlendTexShader" 
{
	Properties {
		[MainTexture] _BaseMap("Main Texture", 2D) = "white" {}
		[MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)

		_BlendMap("Blend Texture",2D) = "white" {}
		_Blend("Blend", Range(0, 1)) = 0.0
			
		[Toggle(_NORMALMAP)] _NormalMapToggle ("Use Normal", Float) = 0
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

		_Cutoff ("AlphaCut", Float) = 0.5

		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_Smoothness("Smoothness", Float) = 0.5
	}
	SubShader {
		Tags {
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		TEXTURE2D(_BlendMap);

		CBUFFER_START(UnityPerMaterial)
		float4 _BaseMap_ST;
		float4 _BaseColor;
		float4 _BlendMap_ST;
		float _Blend;
		float4 _SpecColor;
		float _Cutoff;
		float _Smoothness;
		CBUFFER_END
		ENDHLSL

		Pass {
			Name "ForwardLit"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment

			#pragma shader_feature_local _NORMALMAP
			#define _SPECULAR_COLOR 

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

			struct Attributes {
				float4 positionOS	: POSITION;
				float4 normalOS		: NORMAL;
				#ifdef _NORMALMAP
					float4 tangentOS 	: TANGENT;
				#endif
				float2 uv		    : TEXCOORD0;
				float2 lightmapUV	: TEXCOORD1;
			};

			struct Varyings {
				float4 positionCS 					: SV_POSITION;
				float3 positionWS					: TEXCOORD0;
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
				float2 uv    : TEXCOORD2;
				float2 uv2    : TEXCOORD3;
				#ifdef _NORMALMAP
					half4 normalWS					: TEXCOORD4;   
					half4 tangentWS					: TEXCOORD5;   
					half4 bitangentWS				: TEXCOORD6;
				#else
					half3 normalWS					: TEXCOORD4;
				#endif
			};
			
			void InitalizeSurfaceData(Varyings IN, out SurfaceData surfaceData){
				surfaceData = (SurfaceData)0; 

				half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
				half4 blendTex = SAMPLE_TEXTURE2D(_BlendMap, sampler_BaseMap, IN.uv2);
				
				half4 color = lerp(baseTex, blendTex, _Blend) * _BaseColor;


				#ifdef _ALPHATEST_ON
					clip(baseMap.a - _Cutoff);
				#endif

				surfaceData.albedo = color.rgb;
				surfaceData.normalTS = SampleNormal(IN.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
				surfaceData.emission = half3(0.0, 0.0, 0.0);
				surfaceData.occlusion = 1.0;

				half4 specular = _SpecColor; 
				surfaceData.specular = specular.rgb;
				surfaceData.smoothness = specular.a * _Smoothness;
			}

			void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData) {
				inputData = (InputData)0;

				inputData.positionWS = input.positionWS;

				#ifdef _NORMALMAP
					half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
					inputData.normalWS = TransformTangentToWorld(normalTS,half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
				#else
					half3 viewDirWS = GetWorldSpaceNormalizeViewDir(inputData.positionWS);
					inputData.normalWS = input.normalWS;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);

				viewDirWS = SafeNormalize(viewDirWS);
				inputData.viewDirectionWS = viewDirWS;

				inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);

				inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
			}

			Varyings LitPassVertex(Attributes IN) {
				Varyings OUT;

				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				#ifdef _NORMALMAP
					VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS.xyz, IN.tangentOS);
				#else
					VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS.xyz);
				#endif

				OUT.positionCS = positionInputs.positionCS;
				OUT.positionWS = positionInputs.positionWS;

				half3 viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
				half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
				half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
				
				#ifdef _NORMALMAP
					OUT.normalWS = half4(normalInputs.normalWS, viewDirWS.x);
					OUT.tangentWS = half4(normalInputs.tangentWS, viewDirWS.y);
					OUT.bitangentWS = half4(normalInputs.bitangentWS, viewDirWS.z);
				#else
					OUT.normalWS = NormalizeNormalPerVertex(normalInputs.normalWS);
				#endif

				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
				OUT.uv2 = TRANSFORM_TEX(IN.uv, _BlendMap);
				return OUT;
			}

			half4 LitPassFragment(Varyings IN) : SV_Target {
				SurfaceData surfaceData;
				InitalizeSurfaceData(IN, surfaceData);

				InputData inputData;
				InitializeInputData(IN, surfaceData.normalTS, inputData);

				half4 color = UniversalFragmentBlinnPhong(inputData, surfaceData);

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
