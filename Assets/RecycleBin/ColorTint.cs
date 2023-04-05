using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

[SerializeField]
[PostProcess(typeof(ColorTintRender), PostProcessEvent.AfterStack, "Custome/ColorTint")]
public class ColorTint : PostProcessEffectSettings
{
    [Tooltip("ColorTint")]
    public ColorParameter color = new ColorParameter { value = new Color(1f,1f,1f,1f)};

    [Range(0f, 1f), Tooltip("ColorTint Intensity")]
    public FloatParameter blend = new FloatParameter { value = 0.5f};
}

public sealed class ColorTintRender: PostProcessEffectRenderer<ColorTint>
{
    public override void Render(PostProcessRenderContext context)
    {
        CommandBuffer cmd = context.command;
        cmd.BeginSample("ScreenColorTint");

        PropertySheet shader = context.propertySheets.Get(Shader.Find("PostProcess/ColorTint"));
        shader.properties.SetColor("_Color", settings.color);
        shader.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, shader, 0);
        
        cmd.EndSample("ScreenColorTint");
    }
}
