// Renders vertex colors on a mesh. 
// Flat Shaded flag will ignore light. Useful for static Quill models

Shader "Custom/VertexColor" {

	Properties {
		[Toggle(FLAT_SHADED)]
        _FlatShaded ("Flat Shaded", Float) = 0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 4.0

		#pragma shader_feature FLAT_SHADED

		struct Input {
			float4 vertColor;
		};

		void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertColor = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			#ifdef FLAT_SHADED
			o.Emission = IN.vertColor.rgb;
			#else 
			o.Albedo = IN.vertColor.rgb;
			#endif 

			o.Alpha = IN.vertColor.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}