using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PixelPerfectTranslation : IComponentData
{
	public float3 m_realTranslation;
	public float3 m_pixelPerfectTranslation;
}