using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace EmployeeProgress.Functions;

public class ResizeImageFunction
{
    private readonly ILogger<ResizeImageFunction> _logger;

    public ResizeImageFunction(ILogger<ResizeImageFunction> logger)
    {
        _logger = logger;
    }

    [Function("ResizeImage")]
    [BlobOutput("employee-thumbnail/{name}", Connection = "AzureWebJobsStorage")]
    public async Task<byte[]> Run([BlobTrigger("employee-images/{name}", Connection = "AzureWebJobsStorage")] byte[] blob, string name)
    {
        _logger.LogInformation($"C# Blob trigger function processed blob\n Name:{name} \n Size: {blob.Length} Bytes");
        // Here you would add your image resizing logic
        using MemoryStream inputStream = new(blob);
        using Image image = Image.Load(inputStream);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            //Mode = ResizeMode.Max,
            Size = new Size(200, 200)
        }));
        using MemoryStream outputStream = new();
        await image.SaveAsJpegAsync(outputStream, new JpegEncoder());
        _logger.LogInformation($"Image resized and saved to output stream. Size: {outputStream.Length} Bytes");
        return outputStream.ToArray();
        // Here you would add your image resizing logic
        //return image    ;
    }
}