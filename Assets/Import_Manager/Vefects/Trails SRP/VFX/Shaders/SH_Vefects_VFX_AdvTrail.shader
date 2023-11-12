// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_AdvTrail"
{
	Properties
	{
		_Erosion("Erosion", 2D) = "white" {}
		_Color("Color", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Color_01("Color_01", Color) = (1,1,1,0)
		_Color_02("Color_02", Color) = (1,1,1,0)
		_Distortion("Distortion", 2D) = "white" {}
		_Erosion_Scale("Erosion_Scale", Vector) = (1,1,0,0)
		_CameraOffset("CameraOffset", Float) = 0
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_Color_Scale("Color_Scale", Vector) = (1,1,0,0)
		_Src("Src", Float) = 1
		_Dst("Dst", Float) = 1
		_Distortion_Scale("Distortion_Scale", Vector) = (1,1,0,0)
		_Mask_Speed("Mask_Speed", Vector) = (0,0,0,0)
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		_Erosion_Speed("Erosion_Speed", Vector) = (0,0,0,0)
		_Mask_Scale("Mask_Scale", Vector) = (1,1,0,0)
		_Distortion_Speed("Distortion_Speed", Vector) = (0,0,0,0)
		_Color_Speed("Color_Speed", Vector) = (0,0,0,0)
		_Mask_Multiply("Mask_Multiply", Float) = 1
		_Erosion_Multiply("Erosion_Multiply", Float) = 1
		_Mask_Power("Mask_Power", Float) = 1
		_Erosion_Power("Erosion_Power", Float) = 1
		_Distortion_Amount("Distortion_Amount", Float) = 0
		_DistortionMaskIntensity("Distortion Mask Intensity", Float) = 1
		_EmissiveIntensity("Emissive Intensity", Float) = 1
		_WindSpeed("Wind Speed", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_Cull]
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_Src] [_Dst]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float _Dst;
		uniform float _Src;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		uniform float _CameraOffset;
		uniform sampler2D _Mask;
		uniform sampler2D _Distortion;
		uniform float _WindSpeed;
		uniform float2 _Distortion_Speed;
		uniform float2 _Distortion_Scale;
		uniform float _Distortion_Amount;
		uniform float _DistortionMaskIntensity;
		uniform float2 _Mask_Speed;
		uniform float2 _Mask_Scale;
		uniform float _Mask_Power;
		uniform float _Mask_Multiply;
		uniform float4 _Color_01;
		uniform float4 _Color_02;
		uniform sampler2D _Color;
		uniform float2 _Color_Speed;
		uniform float2 _Color_Scale;
		uniform float _EmissiveIntensity;
		uniform sampler2D _Erosion;
		uniform float2 _Erosion_Speed;
		uniform float2 _Erosion_Scale;
		uniform float _Erosion_Power;
		uniform float _Erosion_Multiply;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 CameraOffset280 = ( ( ase_worldPos - _WorldSpaceCameraPos ) * float3( ( ( v.texcoord3.xy + _CameraOffset ) * float2( 0.01,0 ) ) ,  0.0 ) );
			v.vertex.xyz += CameraOffset280;
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float windSpeed200 = ( _WindSpeed * _Time.y );
			float2 panner79 = ( windSpeed200 * _Distortion_Speed + ( i.uv_texcoord * _Distortion_Scale ));
			float Distortion64 = ( ( tex2D( _Distortion, panner79 ).r * 0.1 ) * _Distortion_Amount );
			float2 uv_TexCoord216 = i.uv_texcoord * _Mask_Scale + float2( 0.28,0 );
			float2 panner377 = ( windSpeed200 * _Mask_Speed + uv_TexCoord216);
			float clampResult373 = clamp( ( pow( tex2D( _Mask, ( ( Distortion64 * _DistortionMaskIntensity ) + panner377 ) ).r , _Mask_Power ) * _Mask_Multiply ) , 0.0 , 1.0 );
			float2 panner320 = ( windSpeed200 * _Color_Speed + ( i.uv_texcoord * _Color_Scale ));
			float4 lerpResult285 = lerp( _Color_01 , _Color_02 , tex2D( _Color, panner320 ).r);
			float4 Color329 = lerpResult285;
			o.Emission = ( ( ( (i.vertexColor).rgb * clampResult373 ) * (Color329).rgb ) * _EmissiveIntensity );
			float2 panner78 = ( windSpeed200 * _Erosion_Speed + ( i.uv_texcoord * _Erosion_Scale ));
			float noises205 = saturate( ( pow( tex2D( _Erosion, ( panner78 + Distortion64 ) ).r , _Erosion_Power ) * _Erosion_Multiply ) );
			float temp_output_308_0 = ( ( clampResult373 - ( noises205 - ( 1.0 - i.vertexColor.a ) ) ) * clampResult373 );
			o.Alpha = saturate( temp_output_308_0 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18921
0;0;2560;1371;2051.055;858.7012;2.630982;True;True
Node;AmplifyShaderEditor.CommentaryNode;296;-6544.695,-2853.138;Inherit;False;786;417;Register Wind Speed;4;198;199;197;200;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;198;-6494.695,-2547.138;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;197;-6494.695,-2803.138;Inherit;False;Property;_WindSpeed;Wind Speed;30;0;Create;True;0;0;0;False;0;False;1;-8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-6238.695,-2803.138;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;77;-5461.079,-2909.315;Inherit;False;2502.5;663.612;Heat Haze;13;50;64;44;52;43;45;79;32;204;30;31;301;391;;0,0,0,1;0;0
Node;AmplifyShaderEditor.Vector2Node;31;-5263.003,-2627.204;Inherit;False;Property;_Distortion_Scale;Distortion_Scale;13;0;Create;True;0;0;0;False;0;False;1,1;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;-5982.695,-2803.138;Inherit;False;windSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-5263.003,-2755.204;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-4860.003,-2755.204;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;32;-4617.001,-2643.375;Inherit;False;Property;_Distortion_Speed;Distortion_Speed;19;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;204;-4512.125,-2493.569;Inherit;False;200;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;79;-4367.003,-2755.204;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3934.658,-2557.986;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-4111.003,-2755.204;Inherit;True;Property;_Distortion;Distortion;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-3702.779,-2575.815;Inherit;False;Property;_Distortion_Amount;Distortion_Amount;26;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;210;-8042.875,-1147.271;Inherit;False;3074.332;597.0625;EROSION;17;380;242;205;241;245;243;299;24;54;78;69;29;297;203;26;25;390;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-3727.003,-2755.204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-3471.003,-2755.204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-7808,-1024;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;25;-7884,-872;Inherit;False;Property;_Erosion_Scale;Erosion_Scale;7;0;Create;True;0;0;0;False;0;False;1,1;0.5,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;313;-4771.846,206;Inherit;False;2128.653;489.2563;Flame Mask;13;223;304;221;214;222;271;377;378;216;379;217;383;392;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;64;-3215.003,-2755.204;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;29;-7424,-896;Inherit;False;Property;_Erosion_Speed;Erosion_Speed;17;0;Create;True;0;0;0;False;0;False;0,0;0.25,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;-7552,-1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-7168,-768;Inherit;False;200;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-6912,-768;Inherit;False;64;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;78;-7168,-1024;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;303;-4466.335,-508.6366;Inherit;False;980;550;Distortion Mask;3;272;273;274;;0,0,0,1;0;0
Node;AmplifyShaderEditor.Vector2Node;217;-4641.34,271.3923;Inherit;False;Property;_Mask_Scale;Mask_Scale;18;0;Create;True;0;0;0;False;0;False;1,1;0.5,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-6912,-1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;216;-4237,243;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.28,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;379;-4082.303,374.3753;Inherit;False;Property;_Mask_Speed;Mask_Speed;14;0;Create;True;0;0;0;False;0;False;0,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;272;-4416.335,-202.6369;Inherit;False;64;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-4416.335,-74.63676;Inherit;False;Property;_DistortionMaskIntensity;Distortion Mask Intensity;27;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;378;-4070.613,542.5769;Inherit;False;200;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;24;-6656,-1024;Inherit;True;Property;_Erosion;Erosion;1;0;Create;True;0;0;0;False;0;False;-1;None;d0edf1cc670b20e46b71dc6f84e2fd83;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;377;-3835.945,269.9912;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;328;-2892.27,-2886.208;Inherit;False;2718.794;1253.04;COLOR;13;385;393;329;285;287;260;283;320;318;319;327;316;317;;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-6264.854,-885.5441;Inherit;False;Property;_Erosion_Power;Erosion_Power;25;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;274;-3904.338,-202.6369;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;271;-3587,251;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;243;-6008.854,-885.5441;Inherit;False;Property;_Erosion_Multiply;Erosion_Multiply;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;299;-6016,-1024;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;316;-2801.907,-2185.482;Inherit;False;Property;_Color_Scale;Color_Scale;10;0;Create;True;0;0;0;False;0;False;1,1;0.5,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;317;-2801.907,-2313.482;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;-5760,-1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;318;-2338.907,-2221.482;Inherit;False;Property;_Color_Speed;Color_Speed;20;0;Create;True;0;0;0;False;0;False;0,0;-0.1,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;327;-2285.027,-2054.01;Inherit;False;200;windSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-2454.906,-2312.482;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;214;-3444.538,224.8561;Inherit;True;Property;_Mask;Mask;3;0;Create;True;0;0;0;False;0;False;-1;None;ef8946c7fbe595f4a9f13d229eac058d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;222;-3267.227,454.5204;Inherit;False;Property;_Mask_Power;Mask_Power;23;0;Create;True;0;0;0;False;0;False;1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;320;-2107.907,-2312.482;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;304;-3089.568,254.8864;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-3039.161,454.683;Inherit;False;Property;_Mask_Multiply;Mask_Multiply;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;241;-5551,-1017;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-5257,-1029;Inherit;False;noises;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;287;-1912.423,-2836.208;Inherit;False;Property;_Color_01;Color_01;4;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;260;-1912.423,-2580.208;Inherit;False;Property;_Color_02;Color_02;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;283;-1919.328,-2344.922;Inherit;True;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;-1;None;d0edf1cc670b20e46b71dc6f84e2fd83;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;302;-396.1363,1395.882;Inherit;False;1170;806;Camera Offset;9;254;256;253;257;280;252;251;250;255;;0,0,0,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;306;-1153.77,-164.5662;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-2844.161,252.683;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;285;-1528.422,-2836.208;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;373;-1157.08,264.9601;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;250;-346.1363,2085.883;Inherit;False;Property;_CameraOffset;CameraOffset;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;406;-939.8428,192.3697;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;367;-843.2787,61.80598;Inherit;False;205;noises;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;251;-346.1363,1957.883;Inherit;False;3;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;368;-628.6322,151.3577;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;329;-845.6066,-2835.616;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;254;-346.1363,1445.882;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;252;-90.13617,1955.151;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;253;-346.1363,1701.883;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;405;-936.0196,-166.7882;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;401;-831.2152,452.7103;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;255;165.8638,1957.883;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.01,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;330;-80.0872,145.572;Inherit;False;329;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;412;-640.3797,-172.1292;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;312;-271.811,820.6317;Inherit;False;710.4392;185;Particle System Opacity;4;309;308;397;398;;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;256;37.86378,1445.882;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;394;-513.4633,436.9194;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;404;-748.8759,845.6967;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;411;185.8988,-111.638;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;308;-221.8111,870.6317;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;364;105.5774,151.1836;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;257;293.8638,1445.882;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;414;2640,-48;Inherit;False;1238;166;Lush was here! <3;5;419;418;417;416;415;Lush was here! <3;0.4445755,0.259434,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;309;225.0456,868.8965;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;240;1152,128;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;29;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;280;549.8638,1445.882;Inherit;False;CameraOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;333.4678,-3.069275;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;1664,256;Inherit;False;280;CameraOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;413;1727.343,746.9919;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;417;3456,0;Inherit;False;Property;_ZWrite;ZWrite;15;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;397;-105.8887,927.6664;Inherit;False;Property;_Opacity_Boost;Opacity_Boost;28;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;415;3200,0;Inherit;False;Property;_Dst;Dst;12;0;Create;True;0;0;0;True;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;389;-7943.601,-319.6602;Inherit;False;global_scale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-2595.617,-2057.328;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;380;-7710.452,-792.4993;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;391;-5261.102,-2442.72;Inherit;False;389;global_scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;398;82.14514,866.4969;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;236;1408,0;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;381;-8127.982,-311.9346;Inherit;False;Property;_Global_Scale;Global_Scale;24;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;387;-5026.718,-2535.282;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;390;-7984.341,-663.1378;Inherit;False;389;global_scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;393;-2796.688,-1924.68;Inherit;False;389;global_scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;418;3712,0;Inherit;False;Property;_ZTest;ZTest;16;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;419;2688,0;Inherit;False;Property;_Cull;Cull;9;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;416;2944,0;Inherit;False;Property;_Src;Src;11;0;Create;True;0;0;0;True;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;392;-4607.022,469.561;Inherit;False;389;global_scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;383;-4416.979,360.9252;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;362;2076.931,-28.71708;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Vefects/SH_Vefects_VFX_AdvTrail;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;2;True;417;3;True;418;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;True;416;1;True;415;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;419;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;199;0;197;0
WireConnection;199;1;198;0
WireConnection;200;0;199;0
WireConnection;301;0;30;0
WireConnection;301;1;31;0
WireConnection;79;0;301;0
WireConnection;79;2;32;0
WireConnection;79;1;204;0
WireConnection;43;1;79;0
WireConnection;44;0;43;1
WireConnection;44;1;45;0
WireConnection;50;0;44;0
WireConnection;50;1;52;0
WireConnection;64;0;50;0
WireConnection;297;0;26;0
WireConnection;297;1;25;0
WireConnection;78;0;297;0
WireConnection;78;2;29;0
WireConnection;78;1;203;0
WireConnection;54;0;78;0
WireConnection;54;1;69;0
WireConnection;216;0;217;0
WireConnection;24;1;54;0
WireConnection;377;0;216;0
WireConnection;377;2;379;0
WireConnection;377;1;378;0
WireConnection;274;0;272;0
WireConnection;274;1;273;0
WireConnection;271;0;274;0
WireConnection;271;1;377;0
WireConnection;299;0;24;1
WireConnection;299;1;242;0
WireConnection;245;0;299;0
WireConnection;245;1;243;0
WireConnection;319;0;317;0
WireConnection;319;1;316;0
WireConnection;214;1;271;0
WireConnection;320;0;319;0
WireConnection;320;2;318;0
WireConnection;320;1;327;0
WireConnection;304;0;214;1
WireConnection;304;1;222;0
WireConnection;241;0;245;0
WireConnection;205;0;241;0
WireConnection;283;1;320;0
WireConnection;223;0;304;0
WireConnection;223;1;221;0
WireConnection;285;0;287;0
WireConnection;285;1;260;0
WireConnection;285;2;283;1
WireConnection;373;0;223;0
WireConnection;406;0;306;4
WireConnection;368;0;367;0
WireConnection;368;1;406;0
WireConnection;329;0;285;0
WireConnection;252;0;251;0
WireConnection;252;1;250;0
WireConnection;405;0;306;0
WireConnection;401;0;373;0
WireConnection;255;0;252;0
WireConnection;412;0;405;0
WireConnection;412;1;373;0
WireConnection;256;0;254;0
WireConnection;256;1;253;0
WireConnection;394;0;401;0
WireConnection;394;1;368;0
WireConnection;404;0;373;0
WireConnection;411;0;412;0
WireConnection;308;0;394;0
WireConnection;308;1;404;0
WireConnection;364;0;330;0
WireConnection;257;0;256;0
WireConnection;257;1;255;0
WireConnection;309;0;308;0
WireConnection;280;0;257;0
WireConnection;307;0;411;0
WireConnection;307;1;364;0
WireConnection;413;0;309;0
WireConnection;389;0;381;0
WireConnection;385;0;316;0
WireConnection;385;1;393;0
WireConnection;380;0;25;0
WireConnection;380;1;390;0
WireConnection;398;0;308;0
WireConnection;398;1;397;0
WireConnection;236;0;307;0
WireConnection;236;1;240;0
WireConnection;387;0;31;0
WireConnection;387;1;391;0
WireConnection;383;0;217;0
WireConnection;383;1;392;0
WireConnection;362;2;236;0
WireConnection;362;9;413;0
WireConnection;362;11;281;0
ASEEND*/
//CHKSM=64B8BB2A7D18F7BF8ABC996B53EB56979825AA33