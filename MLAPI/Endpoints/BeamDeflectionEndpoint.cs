using Microsoft.AspNetCore.Mvc;
using MLAPI.Models;
using MLAPI.Services;

namespace BeamDeflectionApi.Endpoints
{
    public static class BeamDeflectionEndpoints
    {
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        public static void MapBeamDeflectionEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/api/BeamDeflection/train", async (BeamDeflectionMLModel model, [FromBody]  List<BeamDeflectionData> data) =>
            {
                try
                {
                    if (data == null || data.Count == 0)
                    {
                        return Results.BadRequest("Invalid data received.");
                    }

                    await _semaphore.WaitAsync();

                    try
                    {
                        await Task.Run(() => model.LoadAndTrainModel(data));
                    }
                    finally
                    {
                        _semaphore.Release();
                    }

                    return Results.Ok("Model trained successfully.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest($"Training failed: {ex.Message}");
                }
            })
            .WithName("TrainModel")
            .WithOpenApi();

            routes.MapPost("/api/BeamDeflection/predict", async (BeamDeflectionMLModel model, [FromBody] BeamDeflectionData input) =>
            {
                try
                {
                    var prediction = await Task.Run(() => model.Predict(input));

                    return Results.Ok(prediction);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest($"Prediction failed: {ex.Message}");
                }
            })
            .WithName("PredictDeflection")
            .WithOpenApi();
        }

    }
}