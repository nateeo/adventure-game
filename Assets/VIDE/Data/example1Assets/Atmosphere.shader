Shader "Custom/Atmosphere" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ColorTint("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bump map", 2D) = "bump"{}
		_BumpScale("Normal Strength", Range(0.1,2)) = 0.5
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Strength", Range(1.0, 8.0)) = 3.0 
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float4 color : Color;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		float4 _ColorTint;
		sampler2D _MainText;
		sampler2D _BumpMap;
		float4 _RimColor;
		float _RimPower;

		half _Glossiness;
		half _Metallic;
		half _BumpScale;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			IN.color = _ColorTint;
			//o.Albedo = tex2D(_MainText, IN.uv_MainTex).rgb * IN.color;
			//o.Normal =
		    float3 normal = UnpackNormal(tex2D(_BumpMap , IN.uv_BumpMap )) ;
			normal.z = normal.z / _BumpScale;

			o.Normal = normalize(normal);
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);

			//// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			//// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		float4 frag(v2f_img i) : SV_Target
		{
			float4 col = tex2D(_MainTex, i.uv);
			col.rgb = Luminance(col.rgb) * (1 + col.a * 2);
			return col;
		}

		ENDCG
	} 
	FallBack "Bumped Diffuse"
}
