using Amazon.Rekognition.Model;
using Amazon.Rekognition;
using Microsoft.AspNetCore.Mvc;
using Rekognition.Entidades;

namespace Rekognition.Demo.Controllers
{
    public class RekognitionController : Controller
    {
        private readonly IAmazonRekognition _rekognitionClient;

        public RekognitionController(IAmazonRekognition rekognitionClient)
        {
            _rekognitionClient = rekognitionClient;
        }

        [HttpPost("detect-labels")]
        public async Task<IActionResult> DetectLabels(IFormFile file)
        {
            using var memStream = new MemoryStream();
            file.CopyTo(memStream);
            var response = await _rekognitionClient.DetectLabelsAsync(new DetectLabelsRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    Bytes = memStream
                },
                MinConfidence = 95
            });

            var labelResults = response.Labels.Select(label => new LabelResult
            {
                Name = label.Name,
                Confidence = label.Confidence,
                Parents = label.Parents.Select(p => p.Name).ToList(),
                Aliases = label.Aliases.Select(a => new Entidades.LabelAlias { Name = a.Name }).ToList(),
                Categories = label.Categories.Select(c => new Entidades.LabelCategory { Name = c.Name }).ToList(),
                
            }).ToList();

            return Ok(labelResults);
        }
    }
}
