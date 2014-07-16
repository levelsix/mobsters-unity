Shader "Hidden/Custom/MaskClipShader 3" 
{
  Properties 
  {
    _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    _AlphaTex ("MaskTexture", 2D) = "white" {}
  }
 
  SubShader
  {
    LOD 100
 
    Tags{
      "Queue" = "Transparent"
      "IgnoreProjector" = "True"
      "RenderType" = "Transparent"
    }
     Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Offset -1, -1
            Fog { Mode Off }
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaTex;
            float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
            float4 _ClipArgs0 = float4(1000.0, 1000.0, 0.0, 1.0);
            float4 _ClipRange1 = float4(0.0, 0.0, 1.0, 1.0);
            float4 _ClipArgs1 = float4(1000.0, 1000.0, 0.0, 1.0);
            float4 _ClipRange2 = float4(0.0, 0.0, 1.0, 1.0);
            float4 _ClipArgs2 = float4(1000.0, 1000.0, 0.0, 1.0);
 
            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 worldPos2 : TEXCOORD2;
            };

            float2 Rotate (float2 v, float2 rot)
            {
                float2 ret;
                ret.x = v.x * rot.y - v.y * rot.x;
                ret.y = v.x * rot.x + v.y * rot.y;
                return ret;
            }
 
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;
                o.worldPos.xy = TRANSFORM_TEX(v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy, _MainTex);
                o.worldPos.zw = TRANSFORM_TEX(Rotate(v.vertex.xy, _ClipArgs1.zw) * _ClipRange1.zw + _ClipRange1.xy, _MainTex);
                return o;
            }
 
            half4 frag (v2f IN) : COLOR
            {
                // Sample the texture
                half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                half4 a2 = tex2D(_AlphaTex, IN.texcoord);
 
                float2 factor = abs(IN.worldPos.xy);
                float val = 1.0 - max(factor.x, factor.y);
 
                //factor = (float2(1.0, 1.0) - abs(IN.worldPos.zw)) * _ClipArgs1.xy;
                //val = min(val, 1.0 - max(factor.x, factor.y));

                // Option 1: 'if' statement
                if (val < 0.0) col.a = 0.0;
                if (a2.a < col.a) col.a = a2.a;

                // Option 2: no 'if' statement -- may be faster on some devices
                //col.a *= ceil(clamp(val, 0.0, 1.0));
 
                return col;
            }
            ENDCG
        }
  }
}