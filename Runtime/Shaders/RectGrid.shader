Shader "Jenga/RectGrid" {

    Properties {
        origin       ("Origin", Vector) = (0, 0, 0, 0)
        axisA        ("Axis A", Vector) = (1, 0, 0, 0)
        axisB        ("Axis B", Vector) = (0, 0, 1, 0)
        lineColor    ("Line Color", Color) = (1, 0, 0, 1)
        lineWidth    ("Line Width", Range(0, 10)) = 1
        bigLineColor ("Big Line Color", Color) = (1, 0, 0, 1)
        bigLineWidth ("Big Line Width", Range(0, 10)) = 2
        bigLineFreq  ("Big Lines Per Regular", Integer) = 5
        tileColor    ("Tile Color", Color) = (0, 0, 1, 1)
        fogColor     ("Fog Color", Color) = (0, 0, 1, 0)
        fogDensity   ("Fog Density", Range(0, 1)) = .1
        fogStrength   ("Fog Strength", Range(0, 10)) = 2
    }

    SubShader {
        Pass {
            Tags {
                "Queue"="Transparent" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent"
            }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off 

            GLSLPROGRAM

            #include "UnityCG.glslinc"
            #include "GLSLSupport.glslinc"

            #ifdef VERTEX

            in vec4 in_POSITION0;
            in vec4 in_TEXCOORD0;

            out vec4 position;

            void main() {
                gl_Position = gl_ModelViewProjectionMatrix * in_POSITION0;
                position = unity_ObjectToWorld * in_POSITION0;
            }

            #endif

            #ifdef FRAGMENT
            layout(location = 0) out vec4 fragColor;

            uniform vec4 origin;
            uniform vec4 axisA;
            uniform vec4 axisB;
            uniform vec4 lineColor;
            uniform vec4 tileColor;
            uniform vec4 bigLineColor;
            uniform float bigLineWidth;
            uniform int bigLineFreq;
            uniform vec4 fogColor;
            uniform float fogDensity;
            uniform float fogStrength;
            uniform float lineWidth;

            in vec4 position;

            void main() {
                vec4 cameraPos = unity_MatrixInvV * vec4(0, 0, 0, 1);
                float depth = length(position - cameraPos);
                float alpha = 1 - pow(1 + fogStrength, -fogDensity * depth);

                float a = dot(position - origin, axisA) / dot(axisA, axisA);
                float b = dot(position - origin, axisB) / dot(axisB, axisB);

                float tileA = 0;
                float tileB = 0;

                float offsetA = modf(a, tileA);
                float offsetB = modf(b, tileB);

                int tileAi = int(tileA);
                int tileBi = int(tileB);

                bool isBigA = offsetA < .5 && mod(tileAi, bigLineFreq) == 0
                    || offsetA > .5 
                        && mod(tileAi, bigLineFreq) == bigLineFreq - 1;   
                bool isBigB = offsetB < .5 && mod(tileBi, bigLineFreq) == 0
                    || offsetB > .5 
                        && mod(tileBi, bigLineFreq) == bigLineFreq - 1; 

                float worldWidthA = isBigA ? bigLineWidth : lineWidth;   
                float worldWidthB = isBigB ? bigLineWidth : lineWidth; 

                float dAx = dFdx(a);
                float dAy = dFdy(a);

                float dBx = dFdx(b);
                float dBy = dFdy(b);

                float widthA = max(abs(dAx), abs(dAy)) * worldWidthA;
                float widthB = max(abs(dBx), abs(dBy)) * worldWidthB;

                bool isLineA = abs(offsetA) < widthA; 
                bool isLineB = abs(offsetB) < widthB;

                vec4 color 
                    = isLineA && isBigA ? bigLineColor
                    : isLineB && isBigB ? bigLineColor
                    : isLineA || isLineB ? lineColor
                    : tileColor;
                fragColor = mix(color, fogColor, alpha);
            }

            #endif

            ENDGLSL
        }
    }
}
