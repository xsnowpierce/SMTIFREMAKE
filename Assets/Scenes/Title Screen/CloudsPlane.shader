// Based on Unity's Unlit alpha-blended shader. Created by Chris Cunningham to achieve 2d clouds effect.

Shader "Custom/CloudsPlane" {
Properties {
	_MainColor ("Main Color", Color) = (1.0,1.0,1.0,1.0)
	_Multiply ("Multiply",  Range(0,2)) = 1
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_DistortionTex ("Distortion", 2D) = "white" {}
	_SpeedX ("SpeedX", Float) = 1
	_SpeedY ("SpeedX", Float) = 1
	_Distance ("Fade Distance", Float) = 1500.0
	_DistanceStrength ("Fade Strength", Float) = 3000.0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	Cull Off 
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 wPos : TEXCOORD1;
				float2 uv_DistortionTex : TEXCOORD2;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 wPos : TEXCOORD1;
				float2 uv_DistortionTex : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DistortionTex;
			float4 _DistortionTex_ST;
			float4 _MainColor;
			float _SpeedX;
			float _SpeedY;
			float _Distance;
			float _DistanceStrength;
			float _Multiply;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_DistortionTex = TRANSFORM_TEX(v.uv_DistortionTex, _DistortionTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float dist = distance(_WorldSpaceCameraPos, i.wPos);
				float4 noiseDistortion = tex2D(_DistortionTex, (float2(i.uv_DistortionTex.x+frac(_Time.x*_SpeedX),i.uv_DistortionTex.y+frac(_Time.x*_SpeedY))));
				float4 col = tex2D(_MainTex, fixed2(i.texcoord.x + noiseDistortion.x + frac(_Time.x*_SpeedX),i.texcoord.y + noiseDistortion.y + frac(_Time.x*_SpeedY))) * _MainColor * _Multiply;
				float distDiff = saturate((dist-_Distance)/_DistanceStrength);
				if (dist>_Distance)
					col.a = lerp(clamp(col.a-distDiff,0,2),0,distDiff);
				return col;
			}
		ENDCG
	}
}

}
