﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hypercube/3D Texture/RGB Image"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "black"{}		
		_Mod ("Brightness Mod", Range (0, 100)) = 1 
		_Scale("3DScale", Vector) = (1,1,1)
		_Focus ("Range Focus", Range (-2, 2)) = 0.5
		_Clamp ("Clamp", Range (.01, 100)) = 0.5
		_LookupColor ("Color Lookup", 2D) = "white"{}
		_Lookup ("Lookup Value", float) = .1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		Cull Off
		ZWrite On
		ZTest Always
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
		
			struct appdata
			{
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
				float3 normal : NORMAL;
				
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			//	float3 normalViewSpace : TEXCOORD1;    //COMENTED THESE OUT  to hide warning DID NOT TEST!
			//	float3 viewVectorViewspace : TEXCOORD2;
			};

			sampler3D _MainTex;
			float _Mod;
			float3 _Scale;

			float _Focus;
			float _Clamp;
			sampler2D _LookupColor;
			float _Lookup;
						
			v2f vert (appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex.xyz * _Scale.xyz; //this is where the uv's are applied to the model, normally is:  o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				//determine angle relative to the camera
                //o.normalViewSpace = normalize(v.normal);
                //o.viewVectorViewspace = normalize(ObjSpaceViewDir(v.vertex));

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex3D(_MainTex, i.uv); //the data texture
				col *= _Mod;
				return col;
			}
			ENDCG
		}
	}
}
