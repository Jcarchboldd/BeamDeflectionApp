using System;

namespace MLAPI.Models
{
    public class BeamDeflectionData
    {
        public int LoadNode { get; set; }
        public double LoadValue { get; set; }
        public double Node0 { get; set; }
        public double Node1 { get; set; }
        public double Node2 { get; set; }
        public double Node3 { get; set; }
        public double Node4 { get; set; }
        public double Node5 { get; set; }
        public double Node6 { get; set; }
        public double Node7 { get; set; }
        public double Node8 { get; set; }
        public double Node9 { get; set; }
        public double Node10 { get; set; }
    }

    public class BeamDeflectionValues
    {
        public List<double> Deflections { get; set; } = [];
    }

    public class BeamModelTrainingData
    {
        public float LoadNode { get; set; }
        public float AffectedNode { get; set; }
        public float LoadValue { get; set; }
        public float Deflection { get; set; }
    }

    public class BeamDeflectionPrediction
    {
        public float Deflection { get; set; }
    }
}
