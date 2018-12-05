// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColorDissolve" {
	
	Properties
	{
	    m_DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)

	    m_DissolveTexture ("Albedo (RGB)", 2D) = "white" {}
	    m_DissolvePercentage ("DissolvePercentage", Range(0,1)) = 0.0

		m_EdgeTexture("Albedo (RGB)", 2D) = "white" {}
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
				half m_DissolvePercentage;
				sampler2D m_EdgeTexture;

				struct Input
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 worldPos : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 worldPos : TEXCOORD0;
				};

				v2f vert (Input v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					//Clip pixel when value is below threshhold 
					half noiseColor = tex2D(m_DissolveTexture, i.worldPos.rg).r;
					half diff = noiseColor - m_DissolvePercentage;
        			
					//If the difference is below 0 clip the pixel
					clip(diff);

					//If it's above, calculate the color
					half edgeColor = tex2D(m_EdgeTexture, float2(diff, 0)).rgb;

					return i.color * m_DiffuseColor * edgeColor;
				}
				ENDCG 
			}
		}	
	}
}
