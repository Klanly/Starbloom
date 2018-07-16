Shader "GlobalSnow/DecalDraw" {
Properties {
	_MainTex ("Main RGBA", 2D) = "black" {}
    _GS_DepthTexture ("Depth Texture (RG Float)", 2D) = "white" {}
	_TargetUV("Target UV", Vector) = (0,0,0,0)
	_DrawDist("Draw Distance", Float) = 0
	_EraseSpeed("Erase Speed", Float) = 0.005
	_WorldPos ("WorldPos", Vector) = (0,0,0)
}

SubShader {
   	ZTest Always Cull Off ZWrite Off
   	Fog { Mode Off }

	CGINCLUDE
	#include "UnityCG.cginc"

	uniform sampler2D_float _MainTex;
    uniform sampler2D_float _GS_DepthTexture;
	uniform float4 _MainTex_TexelSize;
	uniform float4 _TargetUV;
	uniform float3 _WorldPos;
	uniform float _DrawDist;
	uniform float _EraseSpeed;
	uniform float4 _GS_SnowCamPos;
	uniform float4 _GS_SnowData2;

	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
	};
	
	v2f vert( appdata_base v ) {
	    v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = v.texcoord;
    	return o;
	}

	float4 fragFoot(v2f i) : SV_Target {
		float4 pixel = tex2D(_MainTex, i.uv);
		float2 dist = abs(i.uv - _TargetUV.xy);
		if (max(dist.x, dist.y)<_DrawDist) {
			// don't draw incorrectly over roof
			float4 st = float4(_WorldPos.xz - _GS_SnowCamPos.xz, 0, 0);
			st *= _GS_SnowData2.z;
			st += 0.5;
			float zd = tex2Dlod(_GS_DepthTexture, st).r;
			float aboveRoof = zd > (_WorldPos.y * 0.999);
			float3 newPixel = float3(_TargetUV.z, 1.0, _TargetUV.w);
			pixel.xyz = lerp(pixel.xyz, newPixel, aboveRoof);
//			if (aboveRoof) {
//				pixel.xz = _TargetUV.zw;
//				pixel.y  = 1.0;
//			}
		} else {
			pixel.y = saturate(pixel.y - _EraseSpeed);
		}
		return pixel;
	}
		
	float4 fragStamp(v2f i) : SV_Target {
		float4 pixel = tex2D(_MainTex, i.uv);
		
		// clear marks at far borders
		float2 dd = abs(i.uv - _TargetUV.xy);
		float inRange = dd.x < 0.498 || dd.x>0.5;
		float inRange2 = dd.y < 0.498 || dd.y>0.5;
		pixel *= inRange * inRange2;

		// avoid seams on edges
		dd = min(abs(dd - 1.0.xx), dd);
		
		// draw or clear
		float dist = dot(dd,dd);
		if (dist<_DrawDist) {
			// don't draw incorrectly over roof
			float4 st = float4(_WorldPos.xz - _GS_SnowCamPos.xz, 0, 0);
			st *= _GS_SnowData2.z;
			st += 0.5;
			float zd = tex2Dlod(_GS_DepthTexture, st).r;
			float aboveRoof = zd > (_WorldPos.y * 0.999);
			float mark = 1.25 - clamp(dist / _DrawDist, 0, 0.8);
			pixel.y = lerp(pixel.y, max(pixel.y, mark), aboveRoof);
		} else {
			pixel.y = max(pixel.y - _EraseSpeed, 0);
		}

		return pixel;
	}
	
	float4 fragCleaner(v2f i) : SV_Target {
		return float4(0,0,0,0);
	}	
		
	ENDCG
	
	Pass { // 0: footprint render
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragFoot
		#pragma target 3.0
		ENDCG
	}
	
	Pass { // 1: generic stamp render
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragStamp
		#pragma target 3.0
		ENDCG
	}
	
	Pass { // 2: rt initializer
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragCleaner
		ENDCG
	}	

}
}
