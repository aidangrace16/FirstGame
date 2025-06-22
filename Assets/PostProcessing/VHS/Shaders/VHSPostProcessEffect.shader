Shader "Hidden/VHSPostProcessEffect" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _VHSTex  ("VHS (RGB)", 2D)  = "white" {}
    }

    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
					
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest 
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _VHSTex;

            float _yScanline;
            float _xScanline;
            float rand(float3 co) {
                return frac(sin(dot(co, float3(12.9898,78.233,45.5432))) * 43758.5453);
            }
 
            fixed4 frag(v2f_img i) : SV_Target {
                float2 uv = i.uv;
                fixed4 vhs = tex2D(_VHSTex, uv);

                float dx = 1 - abs(distance(uv.y, _xScanline));
                float dy = 1 - abs(distance(uv.y, _yScanline));

                dy = floor(dy * 15) / 15;
                
                // compute horizontal offset and clamp
                float offsetX = dy * 0.01 + rand(float3(dy, dy, dy)) / 1000;
                uv.x = clamp(uv.x + offsetX, 0.0, 1.0);

                // vertical glitch line
                if (dx > 0.99) {
                    uv.y = _xScanline;
                }

                // clamp vertical coordinate
                uv.y = clamp(uv.y, 0.0, 1.0);

                fixed4 c = tex2D(_MainTex, uv);

                float bleed = tex2D(_MainTex, uv + float2(0.01, 0)).r;
                bleed += tex2D(_MainTex, uv + float2(0.02, 0)).r;
                bleed += tex2D(_MainTex, uv + float2(0.01, 0.01)).r;
                bleed += tex2D(_MainTex, uv + float2(0.02, 0.02)).r;
                bleed /= 6;

                if (bleed > 0.1) {
                    vhs += fixed4(bleed * _xScanline, 0, 0, 0);
                }

                float x = floor(uv.x * 320) / 320.0;
                float y = floor(uv.y * 240) / 240.0;
                
                c -= rand(float3(x, y, _xScanline)) * _xScanline / 5;
                return c + vhs;
            }
            ENDCG
        }
    }
    Fallback off
}
