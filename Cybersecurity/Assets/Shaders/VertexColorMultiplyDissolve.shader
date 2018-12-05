// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColorMultiplyDissolve" {
	
	Properties
	{
	    m_DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
	    m_DissolveTexture ("Dissolve Texture", 2D) = "white" {}

	    m_OffsetStart ("Offset Start", float) = 0.0
	    m_OffsetEnd ("Offset End", float) = 0.0
	    m_DissolvePercentage ("Fade Percentage", Range(0.0, 1.0)) = 0.0
	}

	Category
	{
		Tags { "RenderType"="Opaque" }
		Lighting Off

		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
				
				fixed4 m_DiffuseColor;
				sampler2D m_DissolveTexture;
				float4 m_DissolveTexture_ST; 

			    float m_OffsetStart;
			    float m_OffsetEnd;
			    float m_DissolvePercentage;

				struct appdata_t
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float3 worldPos: TEXCOORD0;
					float3 worldNormal: TEXCOORD1;
				};

				float hash(float n)
				{
					return frac(sin(n)*43758.5453);
				}

				float noise(float3 x)
				{
					// The noise function returns a value in the range -1.0f -> 1.0f

					float3 p = floor(x);
					float3 f = frac(x);

					f = f * f * (3.0 - 2.0 * f);
					float n = p.x + (p.y * 57.0) + (113.0 * p.z);

					return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
						lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
						lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
						lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
				}

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					//clip(noise(i.worldPos * m_NoiseIntensity) - pow(m_NoiseDissolvePercentage, m_NoiseDissolvePower) * m_NoiseMultiplier);
					//clip(noise(i.worldPos) - m_DissolvePercentage);

					//Map the dissolve texture "correctly"
					if (m_DissolvePercentage > 0) //SUPER CHEAP FIX
					{
						float offsetV = lerp(m_OffsetStart, m_OffsetEnd, m_DissolvePercentage);
						float2 worldUV = float2(0, 0);

						if (abs(i.worldNormal.x) > 0.5) 		{ worldUV = i.worldPos.zy; }
						else if (abs(i.worldNormal.z) > 0.5) 	{ worldUV = i.worldPos.xy; }
						else									{ worldUV = i.worldPos.zy; } //i.worldPos.xz;

						worldUV = (worldUV + float2(0.0, offsetV)) * m_DissolveTexture_ST.xy;
						float dissolveSample = tex2D(m_DissolveTexture, worldUV).r;
						clip(dissolveSample - 1); //Discard when less than zero
					}

					return i.color * m_DiffuseColor;
				}
				ENDCG 
			}
		}	
	}
}

/*
float dot(float3 vec1, float3 vec2)
{
	return ((vec1.x * vec2.x) + (vec1.y * vec2.y) + (vec1.z * vec2.z));
}

float dot(float4 vec1, float4 vec2)
{
	return ((vec1.x * vec2.x) + (vec1.y * vec2.y) + (vec1.z * vec2.z) + (vec1.w * vec2.w));
}

float lerpFloat(float a, float b, float t)
{
	return (a * (1 - t)) + (b * t);
}
*/