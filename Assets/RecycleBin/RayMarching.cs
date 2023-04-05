using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

[SerializeField]
[PostProcess(typeof(RayMarchingRender), PostProcessEvent.AfterStack, "Custome/RayMarching")]
public class RayMarching : PostProcessEffectSettings
{
    
}

public sealed class RayMarchingRender : PostProcessEffectRenderer<RayMarching>
{
    public override void Render(PostProcessRenderContext context)
    {
        CommandBuffer cmd = context.command;
        cmd.BeginSample("RayMarching");

        PropertySheet shader = context.propertySheets.Get(Shader.Find("PostProcess/RayMarching"));
        Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);
        shader.properties.SetMatrix("_InvProjectionM", projectionMatrix.inverse);
        shader.properties.SetMatrix("_InvViewM", context.camera.cameraToWorldMatrix);

        cmd.BlitFullscreenTriangle(context.source, context.destination, shader, 0);
        
        cmd.EndSample("RayMarching");
    }
}
