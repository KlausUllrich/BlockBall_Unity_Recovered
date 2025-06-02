Shader "BlockBall3/See-Through" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Alpha ("Alpha", float) = 1
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

	Cull Off
	ZWrite Off

	CGPROGRAM
	#pragma surface surf Lambert alpha

	sampler2D _MainTex;
	fixed4 _Color;
	float _Alpha;

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		fixed a = floor(c.r + c.g + c.b);
		o.Alpha = max(0, c.a - ((1.0f - _Alpha) * a));
	}
	ENDCG

}

Fallback "Transparent/VertexLit"
}
