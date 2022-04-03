Shader "Outline"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
        _Outline_Thickness("Outline Thickness", Float) = 1
        _Outline_Color("Outline Color", Color) = (0, 0, 0, 0)
        [ToggleUI]CORNERS_ON("SampleCorners", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest[unity_GUIZTestMode]
            ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass

            Stencil{
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }
            ColorMask[_ColorMask]
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float4 interp5 : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.texCoord1;
            output.interp3.xyzw =  input.texCoord2;
            output.interp4.xyzw =  input.texCoord3;
            output.interp5.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.texCoord1 = input.interp2.xyzw;
            output.texCoord2 = input.interp3.xyzw;
            output.texCoord3 = input.interp4.xyzw;
            output.color = input.interp5.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Outline_Thickness;
        float4 _Outline_Color;
        float CORNERS_ON;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_Greater_float(float A, float B, out float Out)
        {
            Out = A > B ? 1 : 0;
        }
        
        void Unity_Reciprocal_float2(float2 In, out float2 Out)
        {
            Out = 1.0/In;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(UnityTexture2D _Texture2D, float _OutlineThickness, float2 _Direction, Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float IN, out float Alpha_2)
        {
        UnityTexture2D _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0 = _Texture2D;
        float4 _UV_38d385f55e094fe390ee88e9b5221304_Out_0 = IN.uv0;
        UnityTexture2D _Property_08bc51b798184f91ba524487321857a5_Out_0 = _Texture2D;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.z;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.w;
        float2 _Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0 = float2(_TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0, _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2);
        float2 _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1;
        Unity_Reciprocal_float2(_Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0, _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1);
        float _Property_120b5ac6b09e44789faf6047782c0457_Out_0 = _OutlineThickness;
        float2 _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2;
        Unity_Multiply_float2_float2(_Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1, (_Property_120b5ac6b09e44789faf6047782c0457_Out_0.xx), _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2);
        float2 _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0 = _Direction;
        float2 _Multiply_3594f1573b1c4de28e65221382407844_Out_2;
        Unity_Multiply_float2_float2(_Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2, _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0, _Multiply_3594f1573b1c4de28e65221382407844_Out_2);
        float2 _Add_d76821ad28294f12ad55378bb96649e5_Out_2;
        Unity_Add_float2((_UV_38d385f55e094fe390ee88e9b5221304_Out_0.xy), _Multiply_3594f1573b1c4de28e65221382407844_Out_2, _Add_d76821ad28294f12ad55378bb96649e5_Out_2);
        float4 _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.tex, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.samplerstate, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.GetTransformedUV(_Add_d76821ad28294f12ad55378bb96649e5_Out_2));
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_R_4 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.r;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_G_5 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.g;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_B_6 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.b;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.a;
        Alpha_2 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0 = SAMPLE_TEXTURE2D(_Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.tex, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.samplerstate, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_R_4 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.r;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_G_5 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.g;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_B_6 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.b;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.a;
            float _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2;
            Unity_Comparison_Greater_float(_SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7, 0.5, _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2);
            float4 _Property_14307aa24ec4413a8fa9202af9061d64_Out_0 = _Outline_Color;
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[0];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[1];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[2];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_A_4 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[3];
            UnityTexture2D _Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9350b3a588c9424a9c84f3499918ca87_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_966df89d819341a29c6d9f475679062d;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv0 = IN.uv0;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv1 = IN.uv1;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv2 = IN.uv2;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv3 = IN.uv3;
            float _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0, _Property_9350b3a588c9424a9c84f3499918ca87_Out_0, float2 (0, 1), _OutlineSample_966df89d819341a29c6d9f475679062d, _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2);
            UnityTexture2D _Property_33362fbb4ddf472bad3456e79fa361d3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv0 = IN.uv0;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv1 = IN.uv1;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv2 = IN.uv2;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv3 = IN.uv3;
            float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_33362fbb4ddf472bad3456e79fa361d3_Out_0, _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0, float2 (0, -1), _OutlineSample_eb6da5863ab54c9985f3cea8029811ff, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2);
            float _Add_4553e02fe6324cf28740dde80690df6f_Out_2;
            Unity_Add_float(_OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2, _Add_4553e02fe6324cf28740dde80690df6f_Out_2);
            UnityTexture2D _Property_98578e7838784417b64bf23147aa54c5_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_af36c28952d542ae9430eff994dda5de_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_a1b6ec712b73434db5431adf597fc469;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv0 = IN.uv0;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv1 = IN.uv1;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv2 = IN.uv2;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv3 = IN.uv3;
            float _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_98578e7838784417b64bf23147aa54c5_Out_0, _Property_af36c28952d542ae9430eff994dda5de_Out_0, float2 (-1, 0), _OutlineSample_a1b6ec712b73434db5431adf597fc469, _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2);
            UnityTexture2D _Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_71b6bac84dd849b4a260f7484e609406_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv0 = IN.uv0;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv1 = IN.uv1;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv2 = IN.uv2;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv3 = IN.uv3;
            float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0, _Property_71b6bac84dd849b4a260f7484e609406_Out_0, float2 (1, 0), _OutlineSample_f1760b3272a447b88c0d965c8f398ae6, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2);
            float _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2;
            Unity_Add_float(_OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2);
            float _Add_1bb8588382a947f38ea605de25df49ac_Out_2;
            Unity_Add_float(_Add_4553e02fe6324cf28740dde80690df6f_Out_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2, _Add_1bb8588382a947f38ea605de25df49ac_Out_2);
            float _Property_b700cecc8402418889e32301db13dc95_Out_0 = CORNERS_ON;
            UnityTexture2D _Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_0788775a4e6345df9f0035391e1f8d14;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv0 = IN.uv0;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv1 = IN.uv1;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv2 = IN.uv2;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv3 = IN.uv3;
            float _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0, _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0, float2 (-1, 1), _OutlineSample_0788775a4e6345df9f0035391e1f8d14, _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2);
            UnityTexture2D _Property_61d1cf3ed3d342329b2e091f7a704850_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv0 = IN.uv0;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv1 = IN.uv1;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv2 = IN.uv2;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv3 = IN.uv3;
            float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_61d1cf3ed3d342329b2e091f7a704850_Out_0, _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0, float2 (-1, -1), _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2);
            float _Add_634e86d417dc494da8f486cc75e0048d_Out_2;
            Unity_Add_float(_OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2, _Add_634e86d417dc494da8f486cc75e0048d_Out_2);
            UnityTexture2D _Property_2505cba659ac45e38ce89aea481edde7_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f801355a388846a5bbf09d40d9b5c624;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv0 = IN.uv0;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv1 = IN.uv1;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv2 = IN.uv2;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv3 = IN.uv3;
            float _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_2505cba659ac45e38ce89aea481edde7_Out_0, _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0, float2 (1, -1), _OutlineSample_f801355a388846a5bbf09d40d9b5c624, _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2);
            UnityTexture2D _Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv0 = IN.uv0;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv1 = IN.uv1;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv2 = IN.uv2;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv3 = IN.uv3;
            float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0, _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0, float2 (1, 1), _OutlineSample_e2af1894da524acbb2348e2cdbc96d20, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2);
            float _Add_6a16a2d658ba438c8a50d344909d002d_Out_2;
            Unity_Add_float(_OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2);
            float _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2;
            Unity_Add_float(_Add_634e86d417dc494da8f486cc75e0048d_Out_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2);
            float _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3;
            Unity_Branch_float(_Property_b700cecc8402418889e32301db13dc95_Out_0, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2, 0, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3);
            float _Add_ed08877f758746f69864b0d56d474afe_Out_2;
            Unity_Add_float(_Add_1bb8588382a947f38ea605de25df49ac_Out_2, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3, _Add_ed08877f758746f69864b0d56d474afe_Out_2);
            float _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1;
            Unity_Saturate_float(_Add_ed08877f758746f69864b0d56d474afe_Out_2, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0 = float4(_Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3;
            Unity_Branch_float4(_Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2, _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0, _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0, _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3);
            float _Split_4696722775fd4cbcb2c8492964bd31ac_R_1 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[0];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_G_2 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[1];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_B_3 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[2];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_A_4 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[3];
            float3 _Vector3_05f249cd9ea64c04995e88d21bfdc65c_Out_0 = float3(_Split_4696722775fd4cbcb2c8492964bd31ac_R_1, _Split_4696722775fd4cbcb2c8492964bd31ac_G_2, _Split_4696722775fd4cbcb2c8492964bd31ac_B_3);
            surface.BaseColor = _Vector3_05f249cd9ea64c04995e88d21bfdc65c_Out_0;
            surface.Alpha = _Split_4696722775fd4cbcb2c8492964bd31ac_A_4;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.uv1 =                                        input.texCoord1;
            output.uv2 =                                        input.texCoord2;
            output.uv3 =                                        input.texCoord3;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
            // Render State
            Cull Off
            ZTest[unity_GUIZTestMode]
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass

            Stencil{
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }
            ColorMask[_ColorMask]

            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_TEXCOORD3
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.texCoord1;
            output.interp2.xyzw =  input.texCoord2;
            output.interp3.xyzw =  input.texCoord3;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.texCoord1 = input.interp1.xyzw;
            output.texCoord2 = input.interp2.xyzw;
            output.texCoord3 = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Outline_Thickness;
        float4 _Outline_Color;
        float CORNERS_ON;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_Greater_float(float A, float B, out float Out)
        {
            Out = A > B ? 1 : 0;
        }
        
        void Unity_Reciprocal_float2(float2 In, out float2 Out)
        {
            Out = 1.0/In;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(UnityTexture2D _Texture2D, float _OutlineThickness, float2 _Direction, Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float IN, out float Alpha_2)
        {
        UnityTexture2D _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0 = _Texture2D;
        float4 _UV_38d385f55e094fe390ee88e9b5221304_Out_0 = IN.uv0;
        UnityTexture2D _Property_08bc51b798184f91ba524487321857a5_Out_0 = _Texture2D;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.z;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.w;
        float2 _Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0 = float2(_TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0, _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2);
        float2 _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1;
        Unity_Reciprocal_float2(_Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0, _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1);
        float _Property_120b5ac6b09e44789faf6047782c0457_Out_0 = _OutlineThickness;
        float2 _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2;
        Unity_Multiply_float2_float2(_Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1, (_Property_120b5ac6b09e44789faf6047782c0457_Out_0.xx), _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2);
        float2 _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0 = _Direction;
        float2 _Multiply_3594f1573b1c4de28e65221382407844_Out_2;
        Unity_Multiply_float2_float2(_Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2, _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0, _Multiply_3594f1573b1c4de28e65221382407844_Out_2);
        float2 _Add_d76821ad28294f12ad55378bb96649e5_Out_2;
        Unity_Add_float2((_UV_38d385f55e094fe390ee88e9b5221304_Out_0.xy), _Multiply_3594f1573b1c4de28e65221382407844_Out_2, _Add_d76821ad28294f12ad55378bb96649e5_Out_2);
        float4 _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.tex, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.samplerstate, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.GetTransformedUV(_Add_d76821ad28294f12ad55378bb96649e5_Out_2));
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_R_4 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.r;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_G_5 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.g;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_B_6 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.b;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.a;
        Alpha_2 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0 = SAMPLE_TEXTURE2D(_Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.tex, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.samplerstate, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_R_4 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.r;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_G_5 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.g;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_B_6 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.b;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.a;
            float _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2;
            Unity_Comparison_Greater_float(_SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7, 0.5, _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2);
            float4 _Property_14307aa24ec4413a8fa9202af9061d64_Out_0 = _Outline_Color;
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[0];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[1];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[2];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_A_4 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[3];
            UnityTexture2D _Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9350b3a588c9424a9c84f3499918ca87_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_966df89d819341a29c6d9f475679062d;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv0 = IN.uv0;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv1 = IN.uv1;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv2 = IN.uv2;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv3 = IN.uv3;
            float _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0, _Property_9350b3a588c9424a9c84f3499918ca87_Out_0, float2 (0, 1), _OutlineSample_966df89d819341a29c6d9f475679062d, _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2);
            UnityTexture2D _Property_33362fbb4ddf472bad3456e79fa361d3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv0 = IN.uv0;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv1 = IN.uv1;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv2 = IN.uv2;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv3 = IN.uv3;
            float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_33362fbb4ddf472bad3456e79fa361d3_Out_0, _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0, float2 (0, -1), _OutlineSample_eb6da5863ab54c9985f3cea8029811ff, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2);
            float _Add_4553e02fe6324cf28740dde80690df6f_Out_2;
            Unity_Add_float(_OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2, _Add_4553e02fe6324cf28740dde80690df6f_Out_2);
            UnityTexture2D _Property_98578e7838784417b64bf23147aa54c5_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_af36c28952d542ae9430eff994dda5de_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_a1b6ec712b73434db5431adf597fc469;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv0 = IN.uv0;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv1 = IN.uv1;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv2 = IN.uv2;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv3 = IN.uv3;
            float _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_98578e7838784417b64bf23147aa54c5_Out_0, _Property_af36c28952d542ae9430eff994dda5de_Out_0, float2 (-1, 0), _OutlineSample_a1b6ec712b73434db5431adf597fc469, _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2);
            UnityTexture2D _Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_71b6bac84dd849b4a260f7484e609406_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv0 = IN.uv0;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv1 = IN.uv1;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv2 = IN.uv2;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv3 = IN.uv3;
            float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0, _Property_71b6bac84dd849b4a260f7484e609406_Out_0, float2 (1, 0), _OutlineSample_f1760b3272a447b88c0d965c8f398ae6, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2);
            float _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2;
            Unity_Add_float(_OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2);
            float _Add_1bb8588382a947f38ea605de25df49ac_Out_2;
            Unity_Add_float(_Add_4553e02fe6324cf28740dde80690df6f_Out_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2, _Add_1bb8588382a947f38ea605de25df49ac_Out_2);
            float _Property_b700cecc8402418889e32301db13dc95_Out_0 = CORNERS_ON;
            UnityTexture2D _Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_0788775a4e6345df9f0035391e1f8d14;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv0 = IN.uv0;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv1 = IN.uv1;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv2 = IN.uv2;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv3 = IN.uv3;
            float _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0, _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0, float2 (-1, 1), _OutlineSample_0788775a4e6345df9f0035391e1f8d14, _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2);
            UnityTexture2D _Property_61d1cf3ed3d342329b2e091f7a704850_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv0 = IN.uv0;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv1 = IN.uv1;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv2 = IN.uv2;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv3 = IN.uv3;
            float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_61d1cf3ed3d342329b2e091f7a704850_Out_0, _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0, float2 (-1, -1), _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2);
            float _Add_634e86d417dc494da8f486cc75e0048d_Out_2;
            Unity_Add_float(_OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2, _Add_634e86d417dc494da8f486cc75e0048d_Out_2);
            UnityTexture2D _Property_2505cba659ac45e38ce89aea481edde7_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f801355a388846a5bbf09d40d9b5c624;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv0 = IN.uv0;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv1 = IN.uv1;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv2 = IN.uv2;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv3 = IN.uv3;
            float _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_2505cba659ac45e38ce89aea481edde7_Out_0, _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0, float2 (1, -1), _OutlineSample_f801355a388846a5bbf09d40d9b5c624, _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2);
            UnityTexture2D _Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv0 = IN.uv0;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv1 = IN.uv1;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv2 = IN.uv2;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv3 = IN.uv3;
            float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0, _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0, float2 (1, 1), _OutlineSample_e2af1894da524acbb2348e2cdbc96d20, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2);
            float _Add_6a16a2d658ba438c8a50d344909d002d_Out_2;
            Unity_Add_float(_OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2);
            float _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2;
            Unity_Add_float(_Add_634e86d417dc494da8f486cc75e0048d_Out_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2);
            float _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3;
            Unity_Branch_float(_Property_b700cecc8402418889e32301db13dc95_Out_0, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2, 0, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3);
            float _Add_ed08877f758746f69864b0d56d474afe_Out_2;
            Unity_Add_float(_Add_1bb8588382a947f38ea605de25df49ac_Out_2, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3, _Add_ed08877f758746f69864b0d56d474afe_Out_2);
            float _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1;
            Unity_Saturate_float(_Add_ed08877f758746f69864b0d56d474afe_Out_2, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0 = float4(_Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3;
            Unity_Branch_float4(_Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2, _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0, _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0, _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3);
            float _Split_4696722775fd4cbcb2c8492964bd31ac_R_1 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[0];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_G_2 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[1];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_B_3 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[2];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_A_4 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[3];
            surface.Alpha = _Split_4696722775fd4cbcb2c8492964bd31ac_A_4;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.uv1 =                                        input.texCoord1;
            output.uv2 =                                        input.texCoord2;
            output.uv3 =                                        input.texCoord3;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
            // Render State
            Cull Back
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
            ZTest[unity_GUIZTestMode]

            Stencil{
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }
            ColorMask[_ColorMask]

            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_TEXCOORD3
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.texCoord1;
            output.interp2.xyzw =  input.texCoord2;
            output.interp3.xyzw =  input.texCoord3;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.texCoord1 = input.interp1.xyzw;
            output.texCoord2 = input.interp2.xyzw;
            output.texCoord3 = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Outline_Thickness;
        float4 _Outline_Color;
        float CORNERS_ON;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_Greater_float(float A, float B, out float Out)
        {
            Out = A > B ? 1 : 0;
        }
        
        void Unity_Reciprocal_float2(float2 In, out float2 Out)
        {
            Out = 1.0/In;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(UnityTexture2D _Texture2D, float _OutlineThickness, float2 _Direction, Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float IN, out float Alpha_2)
        {
        UnityTexture2D _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0 = _Texture2D;
        float4 _UV_38d385f55e094fe390ee88e9b5221304_Out_0 = IN.uv0;
        UnityTexture2D _Property_08bc51b798184f91ba524487321857a5_Out_0 = _Texture2D;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.z;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.w;
        float2 _Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0 = float2(_TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0, _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2);
        float2 _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1;
        Unity_Reciprocal_float2(_Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0, _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1);
        float _Property_120b5ac6b09e44789faf6047782c0457_Out_0 = _OutlineThickness;
        float2 _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2;
        Unity_Multiply_float2_float2(_Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1, (_Property_120b5ac6b09e44789faf6047782c0457_Out_0.xx), _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2);
        float2 _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0 = _Direction;
        float2 _Multiply_3594f1573b1c4de28e65221382407844_Out_2;
        Unity_Multiply_float2_float2(_Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2, _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0, _Multiply_3594f1573b1c4de28e65221382407844_Out_2);
        float2 _Add_d76821ad28294f12ad55378bb96649e5_Out_2;
        Unity_Add_float2((_UV_38d385f55e094fe390ee88e9b5221304_Out_0.xy), _Multiply_3594f1573b1c4de28e65221382407844_Out_2, _Add_d76821ad28294f12ad55378bb96649e5_Out_2);
        float4 _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.tex, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.samplerstate, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.GetTransformedUV(_Add_d76821ad28294f12ad55378bb96649e5_Out_2));
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_R_4 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.r;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_G_5 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.g;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_B_6 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.b;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.a;
        Alpha_2 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0 = SAMPLE_TEXTURE2D(_Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.tex, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.samplerstate, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_R_4 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.r;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_G_5 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.g;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_B_6 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.b;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.a;
            float _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2;
            Unity_Comparison_Greater_float(_SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7, 0.5, _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2);
            float4 _Property_14307aa24ec4413a8fa9202af9061d64_Out_0 = _Outline_Color;
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[0];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[1];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[2];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_A_4 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[3];
            UnityTexture2D _Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9350b3a588c9424a9c84f3499918ca87_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_966df89d819341a29c6d9f475679062d;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv0 = IN.uv0;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv1 = IN.uv1;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv2 = IN.uv2;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv3 = IN.uv3;
            float _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0, _Property_9350b3a588c9424a9c84f3499918ca87_Out_0, float2 (0, 1), _OutlineSample_966df89d819341a29c6d9f475679062d, _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2);
            UnityTexture2D _Property_33362fbb4ddf472bad3456e79fa361d3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv0 = IN.uv0;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv1 = IN.uv1;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv2 = IN.uv2;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv3 = IN.uv3;
            float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_33362fbb4ddf472bad3456e79fa361d3_Out_0, _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0, float2 (0, -1), _OutlineSample_eb6da5863ab54c9985f3cea8029811ff, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2);
            float _Add_4553e02fe6324cf28740dde80690df6f_Out_2;
            Unity_Add_float(_OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2, _Add_4553e02fe6324cf28740dde80690df6f_Out_2);
            UnityTexture2D _Property_98578e7838784417b64bf23147aa54c5_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_af36c28952d542ae9430eff994dda5de_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_a1b6ec712b73434db5431adf597fc469;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv0 = IN.uv0;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv1 = IN.uv1;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv2 = IN.uv2;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv3 = IN.uv3;
            float _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_98578e7838784417b64bf23147aa54c5_Out_0, _Property_af36c28952d542ae9430eff994dda5de_Out_0, float2 (-1, 0), _OutlineSample_a1b6ec712b73434db5431adf597fc469, _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2);
            UnityTexture2D _Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_71b6bac84dd849b4a260f7484e609406_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv0 = IN.uv0;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv1 = IN.uv1;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv2 = IN.uv2;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv3 = IN.uv3;
            float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0, _Property_71b6bac84dd849b4a260f7484e609406_Out_0, float2 (1, 0), _OutlineSample_f1760b3272a447b88c0d965c8f398ae6, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2);
            float _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2;
            Unity_Add_float(_OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2);
            float _Add_1bb8588382a947f38ea605de25df49ac_Out_2;
            Unity_Add_float(_Add_4553e02fe6324cf28740dde80690df6f_Out_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2, _Add_1bb8588382a947f38ea605de25df49ac_Out_2);
            float _Property_b700cecc8402418889e32301db13dc95_Out_0 = CORNERS_ON;
            UnityTexture2D _Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_0788775a4e6345df9f0035391e1f8d14;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv0 = IN.uv0;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv1 = IN.uv1;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv2 = IN.uv2;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv3 = IN.uv3;
            float _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0, _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0, float2 (-1, 1), _OutlineSample_0788775a4e6345df9f0035391e1f8d14, _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2);
            UnityTexture2D _Property_61d1cf3ed3d342329b2e091f7a704850_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv0 = IN.uv0;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv1 = IN.uv1;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv2 = IN.uv2;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv3 = IN.uv3;
            float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_61d1cf3ed3d342329b2e091f7a704850_Out_0, _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0, float2 (-1, -1), _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2);
            float _Add_634e86d417dc494da8f486cc75e0048d_Out_2;
            Unity_Add_float(_OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2, _Add_634e86d417dc494da8f486cc75e0048d_Out_2);
            UnityTexture2D _Property_2505cba659ac45e38ce89aea481edde7_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f801355a388846a5bbf09d40d9b5c624;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv0 = IN.uv0;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv1 = IN.uv1;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv2 = IN.uv2;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv3 = IN.uv3;
            float _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_2505cba659ac45e38ce89aea481edde7_Out_0, _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0, float2 (1, -1), _OutlineSample_f801355a388846a5bbf09d40d9b5c624, _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2);
            UnityTexture2D _Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv0 = IN.uv0;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv1 = IN.uv1;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv2 = IN.uv2;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv3 = IN.uv3;
            float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0, _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0, float2 (1, 1), _OutlineSample_e2af1894da524acbb2348e2cdbc96d20, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2);
            float _Add_6a16a2d658ba438c8a50d344909d002d_Out_2;
            Unity_Add_float(_OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2);
            float _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2;
            Unity_Add_float(_Add_634e86d417dc494da8f486cc75e0048d_Out_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2);
            float _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3;
            Unity_Branch_float(_Property_b700cecc8402418889e32301db13dc95_Out_0, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2, 0, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3);
            float _Add_ed08877f758746f69864b0d56d474afe_Out_2;
            Unity_Add_float(_Add_1bb8588382a947f38ea605de25df49ac_Out_2, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3, _Add_ed08877f758746f69864b0d56d474afe_Out_2);
            float _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1;
            Unity_Saturate_float(_Add_ed08877f758746f69864b0d56d474afe_Out_2, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0 = float4(_Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3;
            Unity_Branch_float4(_Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2, _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0, _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0, _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3);
            float _Split_4696722775fd4cbcb2c8492964bd31ac_R_1 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[0];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_G_2 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[1];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_B_3 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[2];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_A_4 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[3];
            surface.Alpha = _Split_4696722775fd4cbcb2c8492964bd31ac_A_4;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.uv1 =                                        input.texCoord1;
            output.uv2 =                                        input.texCoord2;
            output.uv3 =                                        input.texCoord3;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest[unity_GUIZTestMode]
            ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass

            Stencil{
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }
            ColorMask[_ColorMask]

            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float4 interp5 : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.texCoord1;
            output.interp3.xyzw =  input.texCoord2;
            output.interp4.xyzw =  input.texCoord3;
            output.interp5.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.texCoord1 = input.interp2.xyzw;
            output.texCoord2 = input.interp3.xyzw;
            output.texCoord3 = input.interp4.xyzw;
            output.color = input.interp5.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Outline_Thickness;
        float4 _Outline_Color;
        float CORNERS_ON;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_Greater_float(float A, float B, out float Out)
        {
            Out = A > B ? 1 : 0;
        }
        
        void Unity_Reciprocal_float2(float2 In, out float2 Out)
        {
            Out = 1.0/In;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(UnityTexture2D _Texture2D, float _OutlineThickness, float2 _Direction, Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float IN, out float Alpha_2)
        {
        UnityTexture2D _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0 = _Texture2D;
        float4 _UV_38d385f55e094fe390ee88e9b5221304_Out_0 = IN.uv0;
        UnityTexture2D _Property_08bc51b798184f91ba524487321857a5_Out_0 = _Texture2D;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.z;
        float _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2 = _Property_08bc51b798184f91ba524487321857a5_Out_0.texelSize.w;
        float2 _Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0 = float2(_TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Width_0, _TexelSize_3b558f3e3f544e7b9a88b1e80262c674_Height_2);
        float2 _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1;
        Unity_Reciprocal_float2(_Vector2_1b852d04e8c54a388276736dbfd06f9d_Out_0, _Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1);
        float _Property_120b5ac6b09e44789faf6047782c0457_Out_0 = _OutlineThickness;
        float2 _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2;
        Unity_Multiply_float2_float2(_Reciprocal_78fb2325d3db4ddbbc585b374663222d_Out_1, (_Property_120b5ac6b09e44789faf6047782c0457_Out_0.xx), _Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2);
        float2 _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0 = _Direction;
        float2 _Multiply_3594f1573b1c4de28e65221382407844_Out_2;
        Unity_Multiply_float2_float2(_Multiply_575a35104b6f40e1bba368f2b34596c3_Out_2, _Property_bca37b8c3c5f4fc785f77848bf0d28e5_Out_0, _Multiply_3594f1573b1c4de28e65221382407844_Out_2);
        float2 _Add_d76821ad28294f12ad55378bb96649e5_Out_2;
        Unity_Add_float2((_UV_38d385f55e094fe390ee88e9b5221304_Out_0.xy), _Multiply_3594f1573b1c4de28e65221382407844_Out_2, _Add_d76821ad28294f12ad55378bb96649e5_Out_2);
        float4 _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.tex, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.samplerstate, _Property_4a20f4fa998d45d2bfaee40002cb2c71_Out_0.GetTransformedUV(_Add_d76821ad28294f12ad55378bb96649e5_Out_2));
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_R_4 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.r;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_G_5 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.g;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_B_6 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.b;
        float _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_RGBA_0.a;
        Alpha_2 = _SampleTexture2D_bfc03b4ec6a54bf68b7b96b528e44ab1_A_7;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0 = SAMPLE_TEXTURE2D(_Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.tex, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.samplerstate, _Property_cdfa53bbe705482fad1dad2f7cd78ad4_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_R_4 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.r;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_G_5 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.g;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_B_6 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.b;
            float _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7 = _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0.a;
            float _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2;
            Unity_Comparison_Greater_float(_SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_A_7, 0.5, _Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2);
            float4 _Property_14307aa24ec4413a8fa9202af9061d64_Out_0 = _Outline_Color;
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[0];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[1];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[2];
            float _Split_4ac69857eb404c2cb1ee45ce6b6518b8_A_4 = _Property_14307aa24ec4413a8fa9202af9061d64_Out_0[3];
            UnityTexture2D _Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9350b3a588c9424a9c84f3499918ca87_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_966df89d819341a29c6d9f475679062d;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv0 = IN.uv0;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv1 = IN.uv1;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv2 = IN.uv2;
            _OutlineSample_966df89d819341a29c6d9f475679062d.uv3 = IN.uv3;
            float _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_d0f8b24b175d4ee9a2c97f0bf0b74a8b_Out_0, _Property_9350b3a588c9424a9c84f3499918ca87_Out_0, float2 (0, 1), _OutlineSample_966df89d819341a29c6d9f475679062d, _OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2);
            UnityTexture2D _Property_33362fbb4ddf472bad3456e79fa361d3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv0 = IN.uv0;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv1 = IN.uv1;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv2 = IN.uv2;
            _OutlineSample_eb6da5863ab54c9985f3cea8029811ff.uv3 = IN.uv3;
            float _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_33362fbb4ddf472bad3456e79fa361d3_Out_0, _Property_30cbaf61b4da4594b201b16142efd5a1_Out_0, float2 (0, -1), _OutlineSample_eb6da5863ab54c9985f3cea8029811ff, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2);
            float _Add_4553e02fe6324cf28740dde80690df6f_Out_2;
            Unity_Add_float(_OutlineSample_966df89d819341a29c6d9f475679062d_Alpha_2, _OutlineSample_eb6da5863ab54c9985f3cea8029811ff_Alpha_2, _Add_4553e02fe6324cf28740dde80690df6f_Out_2);
            UnityTexture2D _Property_98578e7838784417b64bf23147aa54c5_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_af36c28952d542ae9430eff994dda5de_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_a1b6ec712b73434db5431adf597fc469;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv0 = IN.uv0;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv1 = IN.uv1;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv2 = IN.uv2;
            _OutlineSample_a1b6ec712b73434db5431adf597fc469.uv3 = IN.uv3;
            float _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_98578e7838784417b64bf23147aa54c5_Out_0, _Property_af36c28952d542ae9430eff994dda5de_Out_0, float2 (-1, 0), _OutlineSample_a1b6ec712b73434db5431adf597fc469, _OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2);
            UnityTexture2D _Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_71b6bac84dd849b4a260f7484e609406_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv0 = IN.uv0;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv1 = IN.uv1;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv2 = IN.uv2;
            _OutlineSample_f1760b3272a447b88c0d965c8f398ae6.uv3 = IN.uv3;
            float _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_45fb7e6ec2f74247a03dd3ecd40410e6_Out_0, _Property_71b6bac84dd849b4a260f7484e609406_Out_0, float2 (1, 0), _OutlineSample_f1760b3272a447b88c0d965c8f398ae6, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2);
            float _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2;
            Unity_Add_float(_OutlineSample_a1b6ec712b73434db5431adf597fc469_Alpha_2, _OutlineSample_f1760b3272a447b88c0d965c8f398ae6_Alpha_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2);
            float _Add_1bb8588382a947f38ea605de25df49ac_Out_2;
            Unity_Add_float(_Add_4553e02fe6324cf28740dde80690df6f_Out_2, _Add_db7d27daa1b1430b9e7a419cb4a859e9_Out_2, _Add_1bb8588382a947f38ea605de25df49ac_Out_2);
            float _Property_b700cecc8402418889e32301db13dc95_Out_0 = CORNERS_ON;
            UnityTexture2D _Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_0788775a4e6345df9f0035391e1f8d14;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv0 = IN.uv0;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv1 = IN.uv1;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv2 = IN.uv2;
            _OutlineSample_0788775a4e6345df9f0035391e1f8d14.uv3 = IN.uv3;
            float _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_b386f06ed69d4938b078a720dbc7ebd3_Out_0, _Property_9b4cfc3406204ddfa673b23cd338e867_Out_0, float2 (-1, 1), _OutlineSample_0788775a4e6345df9f0035391e1f8d14, _OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2);
            UnityTexture2D _Property_61d1cf3ed3d342329b2e091f7a704850_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv0 = IN.uv0;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv1 = IN.uv1;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv2 = IN.uv2;
            _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd.uv3 = IN.uv3;
            float _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_61d1cf3ed3d342329b2e091f7a704850_Out_0, _Property_1e849edf0a304ef18ad10b7e2e276f7f_Out_0, float2 (-1, -1), _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2);
            float _Add_634e86d417dc494da8f486cc75e0048d_Out_2;
            Unity_Add_float(_OutlineSample_0788775a4e6345df9f0035391e1f8d14_Alpha_2, _OutlineSample_3ef7bf02e66a4a2c8120d7f8e68d42dd_Alpha_2, _Add_634e86d417dc494da8f486cc75e0048d_Out_2);
            UnityTexture2D _Property_2505cba659ac45e38ce89aea481edde7_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_f801355a388846a5bbf09d40d9b5c624;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv0 = IN.uv0;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv1 = IN.uv1;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv2 = IN.uv2;
            _OutlineSample_f801355a388846a5bbf09d40d9b5c624.uv3 = IN.uv3;
            float _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_2505cba659ac45e38ce89aea481edde7_Out_0, _Property_f95895b3fde54ccbb1b73784d1701ccf_Out_0, float2 (1, -1), _OutlineSample_f801355a388846a5bbf09d40d9b5c624, _OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2);
            UnityTexture2D _Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0 = _Outline_Thickness;
            Bindings_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv0 = IN.uv0;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv1 = IN.uv1;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv2 = IN.uv2;
            _OutlineSample_e2af1894da524acbb2348e2cdbc96d20.uv3 = IN.uv3;
            float _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2;
            SG_OutlineSample_1595884fcf3419548b7534737c6bfbbb_float(_Property_be52809a7a2a4cf5a856293c6dfda8ba_Out_0, _Property_40d5ac0962de4def927d1622be2fb0b0_Out_0, float2 (1, 1), _OutlineSample_e2af1894da524acbb2348e2cdbc96d20, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2);
            float _Add_6a16a2d658ba438c8a50d344909d002d_Out_2;
            Unity_Add_float(_OutlineSample_f801355a388846a5bbf09d40d9b5c624_Alpha_2, _OutlineSample_e2af1894da524acbb2348e2cdbc96d20_Alpha_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2);
            float _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2;
            Unity_Add_float(_Add_634e86d417dc494da8f486cc75e0048d_Out_2, _Add_6a16a2d658ba438c8a50d344909d002d_Out_2, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2);
            float _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3;
            Unity_Branch_float(_Property_b700cecc8402418889e32301db13dc95_Out_0, _Add_ac03fa06d8d4485487ffcee0956451f9_Out_2, 0, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3);
            float _Add_ed08877f758746f69864b0d56d474afe_Out_2;
            Unity_Add_float(_Add_1bb8588382a947f38ea605de25df49ac_Out_2, _Branch_de8c1ddc0b6f48038f2a31bd0c314598_Out_3, _Add_ed08877f758746f69864b0d56d474afe_Out_2);
            float _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1;
            Unity_Saturate_float(_Add_ed08877f758746f69864b0d56d474afe_Out_2, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0 = float4(_Split_4ac69857eb404c2cb1ee45ce6b6518b8_R_1, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_G_2, _Split_4ac69857eb404c2cb1ee45ce6b6518b8_B_3, _Saturate_620d6474e0bb4ceeb34afa89ef9739d6_Out_1);
            float4 _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3;
            Unity_Branch_float4(_Comparison_193c392a45de4b30ac15a408b5f673e5_Out_2, _SampleTexture2D_9d50799786fa44ceabbb89ed3e8a2422_RGBA_0, _Vector4_85da8252ef6a438ca93b549eddc39a24_Out_0, _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3);
            float _Split_4696722775fd4cbcb2c8492964bd31ac_R_1 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[0];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_G_2 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[1];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_B_3 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[2];
            float _Split_4696722775fd4cbcb2c8492964bd31ac_A_4 = _Branch_84adc242c90946eaabe84c6ae7444b14_Out_3[3];
            float3 _Vector3_05f249cd9ea64c04995e88d21bfdc65c_Out_0 = float3(_Split_4696722775fd4cbcb2c8492964bd31ac_R_1, _Split_4696722775fd4cbcb2c8492964bd31ac_G_2, _Split_4696722775fd4cbcb2c8492964bd31ac_B_3);
            surface.BaseColor = _Vector3_05f249cd9ea64c04995e88d21bfdc65c_Out_0;
            surface.Alpha = _Split_4696722775fd4cbcb2c8492964bd31ac_A_4;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.uv1 =                                        input.texCoord1;
            output.uv2 =                                        input.texCoord2;
            output.uv3 =                                        input.texCoord3;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}