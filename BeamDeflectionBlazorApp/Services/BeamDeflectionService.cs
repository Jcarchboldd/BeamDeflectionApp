using MathNet.Numerics.LinearAlgebra;
using Microsoft.JSInterop;

namespace BeamDeflectionBlazorApp.Services
{
    public class BeamDeflectionService : IBeamDeflectionService
    {
        private readonly IJSRuntime _jsRuntime;
        
        public BeamDeflectionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async Task<Dictionary<int, double>> CalculateDeflectionsAsync(int loadNode, double loadValue)
        {
            return await Task.Run(() =>
            {
                double length = 10.0; // Length of the beam in meters
                double E = 210e9; // Young's modulus in Pascals (Steel)
                double I = 8.33e-6; // Moment of inertia in m^4

                int n = 10; // Number of segments (elements)
                double segmentLength = length / n;

                double[] loads = new double[n + 1];
                loads[loadNode] = loadValue; // Apply load at specified node

                var stiffnessMatrix = Matrix<double>.Build.Dense(n + 1, n + 1);
                var forceVector = Vector<double>.Build.Dense(n + 1, 0.0);

                for (int i = 0; i < n; i++)
                {
                    double k = (E * I) / Math.Pow(segmentLength, 3);
                    stiffnessMatrix[i, i] += 12 * k;
                    stiffnessMatrix[i, i + 1] += -6 * k;
                    stiffnessMatrix[i + 1, i] += -6 * k;
                    stiffnessMatrix[i + 1, i + 1] += 4 * k;
                }

                // Boundary conditions (fixed at both ends)
                stiffnessMatrix[0, 0] = 1.0e30;
                stiffnessMatrix[n, n] = 1.0e30;

                for (int i = 0; i <= n; i++)
                {
                    forceVector[i] = loads[i];
                }

                var solution = stiffnessMatrix.Solve(forceVector);

                var deflections = new Dictionary<int, double>();
                for (int i = 0; i <= n; i++)
                {
                    deflections[i] = ScaleDeflectionToMM(solution[i]);
                }

                return deflections;
            });
        }

        public async Task RenderBeamAsync(Dictionary<int, double> deflections, int loadNode)
        {
            await _jsRuntime.InvokeVoidAsync("drawBeam", deflections, loadNode);
        }

        private double ScaleDeflectionToMM(double deflection)
        {
            return Math.Round(deflection * 1000, 3);
        }

    }
}