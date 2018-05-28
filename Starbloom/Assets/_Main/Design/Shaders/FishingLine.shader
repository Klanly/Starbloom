// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/FishingLine" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecNoise ("SpecNoise", 2D) = "white" {}
		_SpecNoiseScaleX("SpecNoiseScaleX", Range(0, 2)) = 1
		_SpecNoiseScaleY("SpecNoiseScaleY", Range(0, 2)) = 1
		_SpecThreshold ("Spec Threshold", Range(0,1)) = 0.5
		_SpecSpeed ("Spec Speed", Range(0, 1)) = 0.25
		_SpecSpeed2 ("Spec Speed 2", Range(0, 1)) = 0.25
		[HDR] _SpecColor ("Spec Color", Color) = (1,1,1,1)
		[HDR] _SpecColorB ("Spec Color B", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SimpleSpecular fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SpecNoise;

		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);

			half diff = max (0, dot (s.Normal, lightDir));

			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0);
			s.Albedo *= diff;

			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			c.a = s.Alpha;
			return c;
		}
		
		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _SpecThreshold;
		fixed _SpecNoiseScaleX;
		fixed _SpecNoiseScaleY;
		fixed _SpecSpeed;
		fixed _SpecSpeed2;
		float4 _SpecColorB;
		//float4 _SpecColor;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed specISA = tex2D(_SpecNoise, IN.uv_MainTex * float2(_SpecNoiseScaleX, _SpecNoiseScaleY) + float2(-_Time.x, 0) * _SpecSpeed).r;
			fixed specISB = tex2D(_SpecNoise, IN.uv_MainTex * float2(_SpecNoiseScaleX, _SpecNoiseScaleY) + float2(-_Time.x, 0) * _SpecSpeed2).r;

			float noiseFixA = ceil( specISA - _SpecThreshold ); // If threshold LE will result in 0. If GE will result in 1. Done to avoid if-else
			float noiseFixB = ceil( specISB - _SpecThreshold );
			
			float noiseFragA = (specISA - _SpecThreshold) / (1 - _SpecThreshold);
			float noiseFragB = (specISB - _SpecThreshold) / (1 - _SpecThreshold);

			float4 specC = _SpecColor * noiseFragA * noiseFixA + _SpecColorB * noiseFragB * noiseFixB;
			//(specIS1 * noiseFix + specIS2 * noiseFix);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color + specC;
			o.Albedo = c.rgb;
//			// Metallic and smoothness come from slider variables
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
