using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using AWSSDK;



namespace LcmsWebApi.Controllers;
    [ApiController]
[Route("[controller]")]
    public class S3Controller : ControllerBase {
        private readonly ILogger<S3Controller> _logger;
        IAmazonS3 S3Client { get; set; }

        public S3Controller(ILogger<S3Controller> logger,
                IAmazonS3 s3Client
                ) {
            _logger = logger;
            this.S3Client = s3Client;
        }
        [HttpGet("test02")]
        public async Task<IActionResult> test02([FromQuery]S3Context s3) {
            _logger.LogInformation("bucketname :" + s3.bucketname );
            _logger.LogInformation("filename :" + s3.filename );
                var stream = new MemoryStream();
            var request = new GetObjectRequest
            {
                BucketName = s3.bucketname,
                           Key = s3.filename
            };
        using (var getObjectResponse = await S3Client.GetObjectAsync(request)){
                using (var responseStream = getObjectResponse.ResponseStream){
                await responseStream.CopyToAsync(stream);
                stream.Position = 0;
                }
        }

                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    FileName = s3.filename,
                    Inline = true // false = prompt the user for downloading, true = browser to try to show the file inline
                }.ToString());
            return File(stream, "image/jpeg");

        }

        [HttpGet("test")]
        public async Task<IActionResult> Challenge([FromQuery]S3Context s3) {
            _logger.LogInformation("test");
            _logger.LogInformation( "bucketname : " + s3.bucketname);
            _logger.LogInformation( "filename : " + s3.filename);
            Stream tream = new MemoryStream();
            //Stream imageStream ;
            Stream imageStream = await GetObject.ReadObjectDataAsync(s3.bucketname, s3.filename);
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    FileName = s3.filename,
                    Inline = true // false = prompt the user for downloading, true = browser to try to show the file inline
                }.ToString());
            return File(imageStream, "image/jpeg");
        }
        [HttpGet("show")]
        public async Task<IActionResult> show([FromQuery]S3Context s3) {
            _logger.LogInformation("show");
            return Ok($"bucketname : {s3.bucketname} , filename : {s3.filename}");
        }

    } // class
class GetObject
{
    public static async Task<Stream> ReadObjectDataAsync(string bucketname,string fileName){
        RegionEndpoint region = RegionEndpoint.APSoutheast1;

        //string bucketFolder = $"s3://{bucketname}";
        using (var client = new AmazonS3Client(region))
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketname,
                           Key = fileName
            };
        using (var getObjectResponse = await client.GetObjectAsync(request))
        {
            using (var responseStream = getObjectResponse.ResponseStream)
            {
                var stream = new MemoryStream();
                await responseStream.CopyToAsync(stream);
                stream.Position = 0;
                return stream;
            }
        }
        }
    }

}
public class S3Context
{
    [FromQuery(Name = "bucketname")]
    public string bucketname { get; set; } = string.Empty;

    [FromQuery(Name = "filename")] // Attribute is ignored.
        public string filename { get; set; } = string.Empty;
}


                                                           

