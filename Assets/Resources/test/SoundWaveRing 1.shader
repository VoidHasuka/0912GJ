// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SoundWaveRing"
{
	Properties
	{
		[PerRendererData]_MainTex("_MainTex", 2D) = "white" {}
		[HDR]_LineColor("_LineColor", Color) = (0,0.003921569,0.003921569,0.003921569)
		_Radius("_Radius", Range( 0 , 0.7)) = 0.4
		_Width("_Width", Range( 0 , 0.2)) = 0.031
		_Amp("_Amp", Range( 0 , 0.2)) = 0.06
		_Damp("_Damp", Range( 0 , 8)) = 2
		_Freq("_Freq", Range( 0 , 12)) = 4
		_TimeScale("_TimeScale", Range( 0 , 2)) = 1
		_ShockTime("_ShockTime", Float) = 0
		_BurstDuration("_BurstDuration", Float) = 1       // 爆发持续时间（秒）
		_BurstGain("_BurstGain", Float) = 10              // 爆发增益（强度倍数）


	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTex;
			uniform float _Damp;
			uniform float _TimeScale;
			uniform float _ShockTime;
			uniform float _Freq;
			uniform float _Amp;
			uniform float _Radius;
			uniform float _Width;
			uniform float4 _LineColor;
			uniform float _BurstDuration;
			uniform float _BurstGain;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float temp_output_17_0 = max( ( ( _Time.y * _TimeScale ) - _ShockTime ) , 0.0 );
				// t：从 _ShockTime 开始计算的经过时间（你已有）
				float t = temp_output_17_0;

				// 基础“呼吸”= 指数衰减 * 正弦 * 振幅
				float wobble = exp(-(_Damp * t)) * sin((t * 6.283185) * _Freq) * _Amp;

				// 爆发门控（前 _BurstDuration 秒从 1 线性衰减到 0；之后为 0）
				float dur  = max(_BurstDuration, 1e-4);                 // 防止除零
				float gate = saturate(1.0 - t / dur);                   // 0..1，t<dur 时为正

				// 给爆发乘一个很大的增益（只在 gate>0 的时段生效）
				float temp_output_27_0 = wobble * (_BurstGain * gate);

				// ——如果你想“到 1 秒瞬间硬截止”，把上面两行换成：
				// float gate = 1.0 - step(_BurstDuration, t);          // t<dur 为 1，>=dur 为 0（硬开关）
				// float temp_output_27_0 = wobble * (_BurstGain * gate);

				float2 texCoord11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_35_0 = saturate( ( 1.0 - ( abs( ( length( ( texCoord11 - float2( 0.5,0.5 ) ) ) - ( temp_output_27_0 + _Radius ) ) ) / ( _Width * 0.5 ) ) ) );
				float2 texCoord48 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 normalizeResult51 = normalize( ( texCoord48 - float2( 0.5,0.5 ) ) );
				float4 appendResult54 = (float4(normalizeResult51 , 0.0 , 0.0));
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = ( ( ( 0.05 * temp_output_27_0 ) * temp_output_35_0 ) * appendResult54 ).xyz;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 texCoord11 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_17_0 = max( ( ( _Time.y * _TimeScale ) - _ShockTime ) , 0.0 );
				float temp_output_27_0 = ( exp( -( _Damp * temp_output_17_0 ) ) * ( sin( ( ( temp_output_17_0 * 6.283185 ) * _Freq ) ) * _Amp ) );
				float temp_output_35_0 = saturate( ( 1.0 - ( abs( ( length( ( texCoord11 - float2( 0.5,0.5 ) ) ) - ( temp_output_27_0 + _Radius ) ) ) / ( _Width * 0.5 ) ) ) );
				float4 break37 = _LineColor;
				float4 appendResult42 = (float4(break37.r , break37.g , break37.b , 0.0));
				float4 break44 = ( temp_output_35_0 * appendResult42 );
				float4 appendResult46 = (float4(break44.x , break44.y , break44.z , temp_output_35_0));
				
				
				finalColor = appendResult46;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleTimeNode;14;-2360.897,-80.70227;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2375.61,16.04023;Inherit;False;Property;_TimeScale;_TimeScale;7;0;Create;True;0;0;0;True;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2074.128,-70.27412;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;16;-1853.053,-73.40253;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2085.454,57.55804;Inherit;False;Property;_ShockTime;_ShockTime;8;0;Create;True;0;0;0;True;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1849.925,37.13387;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1681.787,44.80244;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;6.283185;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1499.728,46.16078;Inherit;False;Property;_Freq;_Freq;6;0;Create;True;0;0;0;True;0;False;4;5.32;0;12;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1410.023,-58.95862;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1206.787,-31.19756;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;22;-1019.31,-22.80776;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;17;-1669.522,-64.01736;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1748.057,-184.1233;Inherit;False;Property;_Damp;_Damp;5;0;Create;True;0;0;0;True;0;False;2;2;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1425.472,-186.5402;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;24;-1221.472,-183.5402;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-820.4724,-16.54016;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1105.057,76.87653;Inherit;False;Property;_Amp;_Amp;4;0;Create;True;0;0;0;True;0;False;0.06;0.06;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;25;-811.4724,-174.5402;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-600.5853,-43.24414;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-682.0341,77.41508;Inherit;False;Property;_Radius;_Radius;2;0;Create;True;0;0;0;True;0;False;0.4;0.4;0;0.7;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-289.7171,12.8315;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-680.5889,174.9963;Inherit;False;Property;_Width;_Width;3;0;Create;True;0;0;0;True;0;False;0.031;0.021;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-610.9501,269.1689;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-301.6066,204.2846;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-793.2281,-332.2883;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;12;-499.1617,-331.2454;Inherit;False;2;0;FLOAT2;0.5,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;13;-299.9868,-324.9886;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;31;-93.95007,9.168884;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;32;108.5157,29.73233;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;33;273.7518,40.97141;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;34;422.2626,51.25836;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;223.6517,167.9404;Inherit;False;Property;_LineColor;_LineColor;1;1;[HDR];Create;True;0;0;0;True;0;False;0,0.003921569,0.003921569,0.003921569;0.2,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;37;501.6761,204.4328;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;42;637.7794,206.1729;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;812.1036,181.4413;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;44;981.7794,175.1729;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;46;1212.779,87.17291;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;35;679.2626,6.258362;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-641.0988,-157.8738;Inherit;False;Constant;_OffsetAmp;_OffsetAmp;9;0;Create;True;0;0;0;False;0;False;0.05;0;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-96.61366,-154.0682;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;1235.506,282.2288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;795.5813,-209.6276;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;353.1229,441.1714;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;50;612.1226,440.1714;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalizeNode;51;804.1226,443.1714;Inherit;False;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;54;993.1226,437.1714;Inherit;False;FLOAT4;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1437.925,86.98442;Float;False;True;-1;2;ASEMaterialInspector;100;5;SoundWaveRing;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SamplerNode;1;-1029.06,360.8233;Inherit;True;Property;_MainTex;_MainTex;0;1;[PerRendererData];Create;True;0;0;0;True;0;False;1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;15;0;14;0
WireConnection;15;1;8;0
WireConnection;16;0;15;0
WireConnection;16;1;9;0
WireConnection;19;0;17;0
WireConnection;19;1;20;0
WireConnection;21;0;19;0
WireConnection;21;1;7;0
WireConnection;22;0;21;0
WireConnection;17;0;16;0
WireConnection;17;1;18;0
WireConnection;23;0;6;0
WireConnection;23;1;17;0
WireConnection;24;0;23;0
WireConnection;26;0;22;0
WireConnection;26;1;5;0
WireConnection;25;0;24;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;28;0;27;0
WireConnection;28;1;3;0
WireConnection;29;0;4;0
WireConnection;29;1;30;0
WireConnection;12;0;11;0
WireConnection;13;0;12;0
WireConnection;31;0;13;0
WireConnection;31;1;28;0
WireConnection;32;0;31;0
WireConnection;33;0;32;0
WireConnection;33;1;29;0
WireConnection;34;0;33;0
WireConnection;37;0;2;0
WireConnection;42;0;37;0
WireConnection;42;1;37;1
WireConnection;42;2;37;2
WireConnection;38;0;35;0
WireConnection;38;1;42;0
WireConnection;44;0;38;0
WireConnection;46;0;44;0
WireConnection;46;1;44;1
WireConnection;46;2;44;2
WireConnection;46;3;35;0
WireConnection;35;0;34;0
WireConnection;55;0;47;0
WireConnection;55;1;27;0
WireConnection;57;0;56;0
WireConnection;57;1;54;0
WireConnection;56;0;55;0
WireConnection;56;1;35;0
WireConnection;50;0;48;0
WireConnection;51;0;50;0
WireConnection;54;0;51;0
WireConnection;0;0;46;0
WireConnection;0;1;57;0
ASEEND*/
//CHKSM=5B843A2B2F17E35F773F12DFA7C5448C720B1642