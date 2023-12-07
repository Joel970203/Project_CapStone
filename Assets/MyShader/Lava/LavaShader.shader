Shader "MyShader/LavaShader" 
{
    Properties 
    {
        _powPower("Pow Power", Range(0, 2)) = 2
        _WaveColor("WaveColor", Vector) = (1.6, 0.33, 0.18, 0.0)
        _BasedColor("BaseColor", Vector) = (0.0, 0.0, 0.0, 0.0)
        _wavePower("Wave Power", Range(0, 30)) = 15
    }
    SubShader 
    {
        Tags 
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"
        } 
        Pass
        {
            Name "Universal Forward"
            Tags {"LightMode" = "UniversalForward"}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x 
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float _powPower;
            float4 _WaveColor;
            float4 _BasedColor;
            float _wavePower;
            CBUFFER_END

            struct VertexInput 
            {
                float4 positionOS : POSITION;
                float2 uv		: TEXCOORD0;
                float3 normalOS : NORMAL; 
            };
            struct VertexOutput 
            {
                float4 positionCS : SV_POSITION; 
                float2 uv		: TEXCOORD0;
                float3 normalWS : NORMAL;
            };

            void RadialShear(float2 uv, out float2 output)
            {
                float2 center = float2(0.5, 0.5);
                float2 power = float2(1.0, 1.6);

                float2 delta = uv - center;
                float delta2 = dot(delta.xy, delta.xy);
                float2 delta_offset = delta2 * power;
                output = uv + float2(delta.y, -delta.x) * delta_offset;
            }

            inline float2 Voronoi_Vector(float2 uv)
            {
                float angleOffset = _Time.y * 0.35;
                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                uv = frac(sin(mul(uv, m)) * 46839.32);
                float output = float2(sin(uv.y * angleOffset), cos(uv.x * angleOffset)) * 0.5 + 0.5;
                return output;
            }
                
            void Voronoi(float2 uv, out float output, out float cells)
            {
                float cellDensity = 26;
                float2 grid = floor(uv * cellDensity);
                float2 f = frac(uv * cellDensity);
                float3 result = float3(8.0, 0.0, 0.0);
                
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 lattice = float2(x, y);
                        float2 offset = Voronoi_Vector(lattice + grid);
                        float d = distance(lattice + offset, f);
                        
                        if (d < result.x)
                        {
                            result = float3(d, offset.x, offset.y);
                            output = result.x;
                            cells = result.y;
                        }
                    }    
                }
            }

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz); 
                o.positionCS.y += sin(v.positionOS.x + v.positionOS.z + _Time.y) * _wavePower; 
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;

                return o; 
            }
            
            half4 frag(VertexOutput i) : SV_Target 
            {
                float2 radialShear;
                RadialShear(i.uv, radialShear);
                                
                float output;
                float cells;
                Voronoi(radialShear, output, cells);
                
                float powerResult = pow(output, _powPower);
                float4 color = float4(powerResult.xxxx) * _WaveColor + _BasedColor;                

                float3 light = _MainLightPosition.xyz;
                color.rgb *= saturate(dot(i.normalWS, light));
                return half4(color.xyz, 1.0);
            }
            ENDHLSL
        }
    }
}