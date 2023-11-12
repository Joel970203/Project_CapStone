// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Distortion"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_DistortionTexture("Distortion Texture", 2D) = "white" {}
		_TextureChannel("Texture Channel", Vector) = (0,1,0,0)
		_ExtraNoiseLerp("Extra Noise Lerp", Float) = 1
		_DistortionStrength("Distortion Strength", Float) = 7
		_DissolveMask("Dissolve Mask", 2D) = "white" {}
		_CameraOffset("Camera Offset", Float) = -40
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cull]
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_Src] [_Dst]
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float4 uv_texcoord;
			float4 vertexColor : COLOR;
			float eyeDepth;
			float4 screenPos;
		};

		uniform float _Dst;
		uniform float _Src;
		uniform float _Cull;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _CameraOffset;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _DistortionTexture;
		uniform float4 _DistortionTexture_ST;
		uniform float _ExtraNoiseLerp;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float4 _TextureChannel;
		uniform sampler2D _DissolveMask;
		uniform float4 _DissolveMask_ST;
		uniform float _DistortionStrength;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			v.vertex.xyz += ( ( ase_worldPos - _WorldSpaceCameraPos ) * ( ( _CameraOffset + v.texcoord3.xy.y ) * 0.01 ) );
			v.vertex.w = 1;
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_DistortionTexture = i.uv_texcoord * _DistortionTexture_ST.xy + _DistortionTexture_ST.zw;
			float2 lerpResult32 = lerp( float2( 0,0 ) , (tex2D( _DistortionTexture, uv_DistortionTexture )).rg , _ExtraNoiseLerp);
			float3 appendResult34 = (float3(lerpResult32 , 1.0));
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float dotResult8 = dot( tex2D( _Texture, uv_Texture ) , _TextureChannel );
			float2 uv_DissolveMask = i.uv_texcoord * _DissolveMask_ST.xy + _DissolveMask_ST.zw;
			float4 temp_output_19_0 = ( i.vertexColor.a * ( saturate( dotResult8 ) * ( saturate( tex2D( _DissolveMask, uv_DissolveMask ) ) + i.uv_texcoord.z ) ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth28_g1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float2 temp_output_20_0_g1 = ( (appendResult34).xy * ( ( temp_output_19_0 * _DistortionStrength ).r / max( i.eyeDepth , 0.1 ) ) * saturate( ( eyeDepth28_g1 - i.eyeDepth ) ) );
			float eyeDepth2_g1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ( float4( temp_output_20_0_g1, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ));
			float2 temp_output_32_0_g1 = (( float4( ( temp_output_20_0_g1 * saturate( ( eyeDepth2_g1 - i.eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
			float2 temp_output_1_0_g1 = ( ( floor( ( temp_output_32_0_g1 * (_CameraDepthTexture_TexelSize).zw ) ) + 0.5 ) * (_CameraDepthTexture_TexelSize).xy );
			float4 screenColor23 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,temp_output_1_0_g1);
			o.Emission = (screenColor23).rgb;
			o.Alpha = saturate( temp_output_19_0 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18921
0;0;2560;1371;-145.2446;485.2692;1.039378;True;True
Node;AmplifyShaderEditor.TexturePropertyNode;2;-4875.82,506.2303;Inherit;True;Property;_Texture;Texture;1;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;1;-4874.569,1333.38;Inherit;True;Property;_DissolveMask;Dissolve Mask;6;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;5;-4490.569,1333.38;Inherit;True;Property;_TextureSample3;Texture Sample 3;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;4;-4490.569,821.3806;Inherit;False;Property;_TextureChannel;Texture Channel;3;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-4528.484,516.6379;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;8;-4106.569,565.38;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;7;-3850.566,1333.38;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-3850.566,1589.38;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;13;-3850.566,565.38;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-4021.934,-312.6901;Inherit;True;Property;_DistortionTexture;Distortion Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-3466.565,1333.38;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;29;-3637.933,-312.6901;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-3466.565,565.38;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;15;-3601.853,248.8702;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-3637.933,-184.6901;Inherit;False;Property;_ExtraNoiseLerp;Extra Noise Lerp;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-3125.933,-312.6901;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-2727.969,186.3177;Inherit;False;Property;_DistortionStrength;Distortion Strength;5;0;Create;True;0;0;0;False;0;False;7;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1368.979,751.0789;Inherit;False;852;849;Camera Offset;8;26;21;20;18;16;14;11;10;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-3217.852,248.8702;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;34;-2869.933,-312.6901;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1318.979,1441.079;Inherit;False;3;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1318.979,1313.079;Inherit;False;Property;_CameraOffset;Camera Offset;7;0;Create;True;0;0;0;False;0;False;-40;-40;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-2727.969,58.31775;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1062.979,1313.079;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;14;-1318.979,801.0788;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;18;-1318.979,1057.079;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;27;-2599.969,-325.6823;Inherit;True;DepthMaskedRefraction;-1;;1;c805f061214177c42bca056464193f81;2,40,0,103,0;2;35;FLOAT3;0,0,0;False;37;FLOAT;0.02;False;1;FLOAT2;38
Node;AmplifyShaderEditor.SimpleSubtractOpNode;21;-1062.979,801.0788;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenColorNode;23;-2215.969,-325.6823;Inherit;False;Global;_GrabScreen0;Grab Screen 0;10;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;40;974,78;Inherit;False;1238;166;Lush was here! <3;5;39;36;38;37;35;Lush was here! <3;0.4445755,0.259434,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-678.9794,1313.079;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;2048,128;Inherit;False;Property;_ZTest;ZTest;12;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;24;-441.737,-303.3669;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;25;-2727.969,442.3177;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-678.9794,801.0788;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;39;1536,128;Inherit;False;Property;_Dst;Dst;10;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;1280,128;Inherit;False;Property;_Src;Src;9;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;1792,128;Inherit;False;Property;_ZWrite;ZWrite;11;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;1024,128;Inherit;False;Property;_Cull;Cull;8;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-82.43175,290.303;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Vefects/SH_Vefects_VFX_Distortion;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;0;True;38;0;True;37;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;True;36;10;True;39;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;35;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;1;0
WireConnection;3;0;2;0
WireConnection;8;0;3;0
WireConnection;8;1;4;0
WireConnection;7;0;5;0
WireConnection;13;0;8;0
WireConnection;12;0;7;0
WireConnection;12;1;6;3
WireConnection;29;0;30;0
WireConnection;17;0;13;0
WireConnection;17;1;12;0
WireConnection;32;1;29;0
WireConnection;32;2;31;0
WireConnection;19;0;15;4
WireConnection;19;1;17;0
WireConnection;34;0;32;0
WireConnection;22;0;19;0
WireConnection;22;1;33;0
WireConnection;16;0;11;0
WireConnection;16;1;10;2
WireConnection;27;35;34;0
WireConnection;27;37;22;0
WireConnection;21;0;14;0
WireConnection;21;1;18;0
WireConnection;23;0;27;38
WireConnection;20;0;16;0
WireConnection;24;0;23;0
WireConnection;25;0;19;0
WireConnection;26;0;21;0
WireConnection;26;1;20;0
WireConnection;0;2;24;0
WireConnection;0;9;25;0
WireConnection;0;11;26;0
ASEEND*/
//CHKSM=314D48579BC3E224C39CED5DE15E9B96E2EFEBD4