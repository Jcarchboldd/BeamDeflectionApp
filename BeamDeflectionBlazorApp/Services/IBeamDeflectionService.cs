namespace BeamDeflectionBlazorApp.Services
{
    public interface IBeamDeflectionService
    {
        Task<Dictionary<int, double>> CalculateDeflectionsAsync(int loadNode, double loadValue);
        Task RenderBeamAsync(Dictionary<int, double> deflections, int loadNode);
    }
}
