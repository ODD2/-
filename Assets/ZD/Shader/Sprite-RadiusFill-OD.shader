// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/RadiusFill"
{
    Properties
    {
        _Color("Tint", Color) = (1,1,1,1)
        _Fill ("Fill", Range(0,1)) =  1
        [MaterialToggle] _ClockWise ("ClockWise",  Float) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFragRadialFill
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float _Fill;
            float _ClockWise;

            fixed4 SpriteFragRadialFill(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                float2 delta = IN.texcoord - float2(0.5f,0.5f);
                float prop = (atan2(delta.y,delta.x)/(2*3.1415926));
                prop = prop < 0 ? prop + 1:prop;
                prop = _ClockWise > 0 ?prop:1-prop;
                if( prop > _Fill){
                    c.a = 0;
                }
                c.rgb *= c.a;
                return c;
            }
            
        ENDCG
        }
    }
    FallBack "Sprites/Default" // optional fallback
}
