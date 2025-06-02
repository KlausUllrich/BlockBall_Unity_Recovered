// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "BlockBall3/GreaterZ" {
Properties {
	_MainTex ("Main (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "Queue" = "Transparent+5" "RenderType"="Opaque" }
	LOD 150

	ZTest Greater
	ZWrite On
	ColorMask 0

CGPROGRAM
#pragma surface surf Lambert noforwardadd exclude_path:prepass

sampler2D _MainTex;
float _Scale;

struct Input {
	float2 uv_MainTex;
	float3 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {

	o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	o.Albedo = o.Albedo * IN.color;
	o.Alpha = 1.0f;
}
ENDCG
}

Fallback "BlockBall3/Internal/VertexLit"
}
