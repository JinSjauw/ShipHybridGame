Shader "Unlit/UnlitWaveShader"
{
    Properties
    {
    	_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    	_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WaveA ("Wave A (dir, steepness, wavelength)", Vector) = (1,0,0.5,10)
    	_WaveB ("Wave B", Vector) = (0,1,0.25,20)
    	_WaveC ("Wave C", Vector) = (0,1,0.25,20)
    	
    	_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
	    _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
    	_FoamColor("Foam Color", Color) = (1, 1, 1, 1)
    	_FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
		_FoamMinDistance("Foam Minimum Distance", Float) = 0.04
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
    	_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777
    	_SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
    	
    	_SurfaceDistortion("Surface Distortion", 2D) = "white" {}
    	_SurfaceDistortionAmount("Surface Distortion Amount", Range(0,1)) = 0.27
    }
    SubShader
    {
        Tags { 
        		"RenderType"="Opaque"
        		"Queue" = "Transparent" 
        	}
        LOD 100

        Pass
        {
        	Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #define SMOOTHSTEP_AA 0.01
            #include "UnityCG.cginc"

			float4 alphaBlend(float4 top, float4 bottom)
            {
	            float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
            	float alpha = top.a + bottom.a * (1 - top.a);

            	return float4(color, alpha);
            }
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            	float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            	//float3 normal : NORMAL;
            	float4 screenPosition : TEXCOORD2;
            	float2 noiseUV : TEXCOORD0;
            	float2 distortUV : TEXCOORD1;
            	float3 viewNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half _Glossiness;
	        half _Metallic;
	        fixed4 _Color;
	        float4 _WaveA, _WaveB, _WaveC;
            
            float3 GerstnerWave(float4 wave, float3 p, inout float3 tangent, inout float3 binormal)
	        {
	            float steepness = wave.z;
			    float wavelength = wave.w;
			    float k = 2 * UNITY_PI / wavelength;
				float c = sqrt(9.8 / k);
				float2 d = normalize(wave.xy);
				float f = k * (dot(d, p.xz) - c * _Time.y);
				float a = steepness / k;

				tangent += float3(
					-d.x * d.x * (steepness * sin(f)),
					d.x * (steepness * cos(f)),
					-d.x * d.y * (steepness * sin(f))
				);
				binormal += float3(
					-d.x * d.y * (steepness * sin(f)),
					d.y * (steepness * cos(f)),
					-d.y * d.y * (steepness * sin(f))
				);
				return float3(
					d.x * (a * cos(f)),
					a * sin(f),
					d.y * (a * cos(f))
				);
	        }

            float4 _FoamColor;
            float _FoamMaxDistance;
			float _FoamMinDistance;
			sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;
			float _SurfaceNoiseCutoff;
            float2 _SurfaceNoiseScroll;
			sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            float _SurfaceDistortionAmount;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 gridPoint = v.vertex.xyz;
        		float3 tangent = float3(1, 0, 0);
				float3 binormal = float3(0, 0, 1);
        		float3 p = gridPoint;
        		p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
        		p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
        		p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
	            //float3 normal = normalize(cross(binormal, tangent));
                float3 vert = v.vertex;
                vert.xyz = p;
            	//o.normal = normal;
                o.vertex = UnityObjectToClipPos(vert);
            	o.screenPosition = ComputeScreenPos(o.vertex);
				o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
            	o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
				o.viewNormal = COMPUTE_VIEW_NORMAL;
            	
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			
			float _DepthMaxDistance;
			sampler2D _CameraDepthTexture;
			sampler2D _CameraNormalsTexture;
            
            fixed4 frag (v2f i) : SV_Target
            {
				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
            	float existingDepthLinear = LinearEyeDepth(existingDepth01);

				float depthDifference = existingDepthLinear - i.screenPosition.w;

				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
            	float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

            	float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
            	float3 normalDot = saturate(dot(existingNormal, i.viewNormal));

            	float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
            	float foamDepthDifference01 = saturate(depthDifference / foamDistance);
				float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;

            	float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;
            	float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) * distortSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) * distortSample.y);
				float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;
				float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);
				float4 surfaceNoiseColor = _FoamColor;
            	surfaceNoiseColor.a *= surfaceNoise;
            	
            	return alphaBlend(surfaceNoiseColor, waterColor);
            	
            	//return fixed4(i.uv.x, 0, 0, 1);
            	// sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                //return col;
            }
            ENDCG
        }
    }
}
