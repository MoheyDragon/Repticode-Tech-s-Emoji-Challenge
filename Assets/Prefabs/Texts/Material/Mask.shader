Shader "Universal Render Pipeline/Custom/Mask"
{
    
    SubShader
    {
        // With SRP we introduce a new "RenderPipeline" tag in Subshader. This allows to create shaders
        // that can match multiple render pipelines. If a RenderPipeline tag is not set it will match
        // any render pipeline. In case you want your subshader to only run in LWRP set the tag to
        // "UniversalRenderPipeline"
        Tags{"RenderPipeline" = "UniversalRenderPipeline" "Queue"="Transparent+1"}
        Pass
        {
            Blend Zero One
        }
    }
}