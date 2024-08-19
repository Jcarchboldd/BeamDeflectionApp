using System.Text.RegularExpressions;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLAPI.Models;
using Tensorflow;

namespace MLAPI.Services
{
    public class BeamDeflectionMLModel
    {
        ITransformer? Model;

        public void LoadAndTrainModel(List<BeamDeflectionData> data)
        {
            try
            {
                Console.WriteLine("Training the model...");
                var trainingData = SetTrainingData(data);

                var mlContext = new MLContext();
                
                // Load the data into IDataView
                var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

                // Split data into training and test sets
                var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                var pipeline = mlContext.Transforms.Concatenate("Features",
                    nameof(BeamModelTrainingData.LoadNode),
                    nameof(BeamModelTrainingData.LoadValue),
                    nameof(BeamModelTrainingData.AffectedNode))
                    .Append(mlContext.Transforms.CopyColumns("Label", nameof(BeamModelTrainingData.Deflection)))
                    .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features"));

                // Train the model
                Model = pipeline.Fit(splitData.TrainSet);

                Console.WriteLine("Model training completed.");

                // Evaluate the model on the test set
                var predictions = Model.Transform(splitData.TestSet);
                var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

                Console.WriteLine($"R-Squared: {metrics.RSquared}");
                Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError}");
                Console.WriteLine($"Root Mean Squared Error: {metrics.RootMeanSquaredError}");

                if (Model == null)
                {
                    throw new InvalidOperationException("Model training failed, resulting in a null model.");
                }

                Console.WriteLine("Testing the model prediction...");

                TestModelPredicition();

                Console.WriteLine("Model prediction test completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public BeamDeflectionValues Predict(BeamDeflectionData input)
        {
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained yet.");
            }

            // Prepare the prediction data
            var predictionData = SetPredictionData(input);

            var mlContext = new MLContext();

            // Convert the data into an IDataView for batch predictions
            var inputDataView = mlContext.Data.LoadFromEnumerable(predictionData);

            // Make predictions
            IDataView predictions = Model.Transform(inputDataView);

            // Extract the predicted deflections
            var predictionResult = new BeamDeflectionValues();
            var predictedValues = mlContext.Data.CreateEnumerable<BeamDeflectionPrediction>(predictions, reuseRowObject: false);

            foreach (var prediction in predictedValues)
            {
                predictionResult.Deflections.Add(prediction.Deflection);
            }

            return predictionResult;
        }


        public List<BeamModelTrainingData> SetTrainingData(List<BeamDeflectionData> data)
        {
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("The input data cannot be null or empty.");
            }

            var trainingDataList = new List<BeamModelTrainingData>();

            foreach (var item in data)
            {
                trainingDataList.AddRange(ProcessSingleBeamDeflectionData(item));
            }

            return trainingDataList;
        }

        public List<BeamModelTrainingData> SetTrainingData(BeamDeflectionData data)
        {
            if (data == null)
            {
                throw new ArgumentException("The input data cannot be null or empty.");
            }

            return ProcessSingleBeamDeflectionData(data);
        }

        private void TestModelPredicition()
        {
            var testData = new BeamDeflectionData
            {
                LoadNode = 5,
                LoadValue = 1000,
                Node0 = 0.000,
                Node1 = 0.000,
                Node2 = 0.000,
                Node3 = 0.000,
                Node4 = 0.000,
                Node5 = 0.000,
                Node6 = 0.000,
                Node7 = 0.000,
                Node8 = 0.000,
                Node9 = 0.000,
                Node10 = 0.000

            };

            var nodeName = $"Node{testData.LoadNode}";

            var prediction = Predict(testData);

            if (prediction.Deflections.All(p => p == 0))
            {
                Console.WriteLine($"Warning: Model for {nodeName} may not be valid as it returns all zeros for prediction.");
            }
            else
            {
                Console.WriteLine($"Model for {nodeName} passed the initial prediction test.");
                Console.WriteLine($"Deflections: {string.Join(", ", prediction.Deflections)}");
            }

        }

        private List<BeamModelTrainingData> ProcessSingleBeamDeflectionData(BeamDeflectionData data)
        {
            var trainingDataList = new List<BeamModelTrainingData>();

            foreach (var prop in typeof(BeamDeflectionData).GetProperties().Where(p => p.Name.StartsWith("Node")))
            {
                var value = prop.GetValue(data);
                
                if (value is double nodeValue)
                {
                    var trainingData = new BeamModelTrainingData
                    {
                        LoadNode = data.LoadNode,
                        AffectedNode = int.Parse(Regex.Match(prop.Name, @"\d+").Value),
                        LoadValue = (float)data.LoadValue,
                        Deflection = (float)nodeValue
                    };

                    trainingDataList.Add(trainingData);
                }
            }

            var groupedList = trainingDataList
                .GroupBy(td => new { td.LoadNode, td.LoadValue })
                .SelectMany(group => group)
                .ToList();

            foreach (var item in groupedList)
            {
                Console.WriteLine($"LoadNode: {item.LoadNode}, AffectedNode: {item.AffectedNode}, LoadValue: {item.LoadValue}, Deflection: {item.Deflection}");
            }

            return groupedList;
        }

        private List<BeamModelTrainingData> SetPredictionData(BeamDeflectionData input)
        {
            var predictionData = new List<BeamModelTrainingData>();

            foreach (var prop in typeof(BeamDeflectionData).GetProperties().Where(p => p.Name.StartsWith("Node")))
            {
                var value = prop.GetValue(input);
                
                if (value is double nodeValue)
                {
                    var predictionItem = new BeamModelTrainingData
                    {
                        LoadNode = input.LoadNode,
                        AffectedNode = int.Parse(Regex.Match(prop.Name, @"\d+").Value),
                        LoadValue = (float)input.LoadValue
                    };

                    predictionData.Add(predictionItem);
                }
            }

            foreach (var item in predictionData.Where(w => w.AffectedNode.Equals(5)).Take(1))
            {
                Console.WriteLine($"LoadNode: {item.LoadNode}, AffectedNode: {item.AffectedNode}, LoadValue: {item.LoadValue}, Deflection: {item.Deflection}");
            }

            return predictionData;
        }

        
    }

}
