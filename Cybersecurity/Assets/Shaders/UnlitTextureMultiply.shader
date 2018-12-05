// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitTextureMultiply" {
	
	Properties
	{
	    m_DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
	    m_DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
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
				sampler2D m_DiffuseTexture;
				float4 m_DiffuseTexture_ST;

				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};


				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					//Map the diffuse texture
					fixed4 diffuseSample = tex2D(m_DiffuseTexture, (i.uv + m_DiffuseTexture_ST.yz) * m_DiffuseTexture_ST.xy);
					clip(diffuseSample.a - 0.5);

					return diffuseSample * m_DiffuseColor;
				}
				ENDCG 
			}
		}	
	}
}