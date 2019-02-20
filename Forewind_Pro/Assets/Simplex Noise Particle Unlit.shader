Shader "Unlit/Simplex Noise Particle Unlit"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_NoiseSpeedX("Noise Speed X", Range(0, 100)) = 0.0
		_NoiseSpeedY("Noise Speed Y", Range(0, 100)) = 0.0
		_NoiseSpeedZ("Noise Speed Z", Range(0, 100)) = 1.0

		_NoiseFrequency("Noise Frequency", Range(0, 1)) = 0.1
		_NoiseAmplitude("Noise Amplitude", Range(0, 10)) = 2.0

		_NoiseAbs("Noise Abs", Range(0, 1)) = 1.0

		[HDR] _ColourA("Color A", Color) = (0,0,0,0)
		[HDR] _ColourB("Color B", Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
		LOD 100

		Blend One One // 加法混合
		ZWrite Off // 关闭深度测试

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// 实现模糊效果
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "SimplexNoise3D.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float4 tc0 : TEXCOORD0;
				float4 tc1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 tc0 : TEXCOORD0;
				float4 tc1 : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};


			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _NoiseSpeedX;
			float _NoiseSpeedY;
			float _NoiseSpeedZ;

			float _NoiseFrequency;
			float _NoiseAmplitude;

			float _NoiseAbs;

			float4 _ColourA;
			float4 _ColourB;

			v2f vert(appdata v)
			{
				v2f o;

				float3 particleCenter = float3(v.tc0.zw, v.tc1.x);
				float3 noiseOffset = _Time.y * float3(_NoiseSpeedX, _NoiseSpeedY, _NoiseSpeedZ);
				float noise = snoise((particleCenter + noiseOffset) * _NoiseFrequency);

				float noise01 = (noise + 1.0) / 2.0;
				float noiseRemap = lerp(noise, noise01, _NoiseAbs);

				float3 vertexOffset = float3(0.0, noiseRemap * _NoiseAmplitude, 0.0);

				v.vertex.xyz += vertexOffset;
				o.vertex = UnityObjectToClipPos(v.vertex);

				// 从保存在颜色顶点输入的粒子系统接收数据，并将该数据用于初始化颜色。
				o.color = v.color;
				o.tc0.xy = TRANSFORM_TEX(v.tc0, _MainTex);

				// 初始化tex coord变量
				o.tc0.zw = v.tc0.zw;
				o.tc1 = v.tc1;

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// 采样纹理
				fixed4 col = tex2D(_MainTex, i.tc0);

			// 让纹理颜色和粒子系统的顶点颜色输入相乘
			col *= i.color;

			float3 particleCenter = float3(i.tc0.zw, i.tc1.x);
			float3 noiseOffset = _Time.y * float3(_NoiseSpeedX, _NoiseSpeedY, _NoiseSpeedZ);

			float noise = snoise((particleCenter + noiseOffset) * _NoiseFrequency);
			float noise01 = (noise + 1.0) / 2.0;

			col = lerp(col * _ColourA, col * _ColourB, noise01);

			col *= col.a;

			// 应用模糊效果
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
		ENDCG
	}
	}
}