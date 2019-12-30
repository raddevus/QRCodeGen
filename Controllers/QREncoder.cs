using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Drawing;

namespace QRCodeGen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QREncoder : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<QREncoder> _logger;

        public QREncoder(ILogger<QREncoder> logger)
        {
            _logger = logger;
        }

        [HttpGet("Get")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetDate")]
        public String GetDate(){
            return DateTime.Now.ToLongDateString();
        }

        [HttpGet("GenQR")]
        public String GenQR(String inText){
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(inText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            String fileName = $"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.jpg";
            String fullFileName = $".\\wwwroot\\{fileName}";
            Console.WriteLine(fileName);
            qrCodeImage.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            var pathBase = (Request.PathBase == String.Empty) ? String.Empty : $"{Request.PathBase}";
            return $"https://{Request.Host.Value}{pathBase}/{fileName}";
        }

        [HttpGet("GetAsciiQR")]
        public String GetAsciiQR(String inText){
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(inText, QRCodeGenerator.ECCLevel.Q);
            AsciiQRCode qrCode = new AsciiQRCode(qrCodeData);
            return qrCode.GetGraphic(1);
        }

        [HttpGet("GetBase64QR")]
        public String GetBase64QR(String inText){
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
