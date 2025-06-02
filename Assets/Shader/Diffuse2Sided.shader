// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "BlockBall3/Diffuse2sided" {
Properties {
	_MaterialColor ("MainColor", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "Queue" = "Geometry"  "RenderType"="Opaque" }
	LOD 150

	Cull Off

CGPROGRAM
#pragma surface surf Lambert noforwardadd exclude_path:prepass

sampler2D _MainTex;
fixed4 _MaterialColor;

struct Input {
	float2 uv_MainTex;
	float3 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * IN.color * _MaterialColor;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "BlockBall3/Internal/VertexLit"
}
