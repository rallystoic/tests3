using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SecurityToken.SAML;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using AWSSDK;
using AWSSDK.Runtime.Internal.Util;




namespace LcmsWebApi.Controllers;
    [ApiController]
[Route("[controller]")]
    public class S3Controller : ControllerBase {
        private readonly ILogger<S3Controller> _logger;

        public S3Controller(ILogger<S3Controller> logger
                ) {
            _logger = logger;
        }
        [HttpGet("test03")]
        public async Task<IActionResult> test03([FromQuery]S3Context s3) {
            MemoryStream stream = new MemoryStream();
        RegionEndpoint region = RegionEndpoint.APSoutheast1;
        SessionAWSCredentials tempCredentials = await GetObject.GetTemporaryCredentialsAsync();
        using (var client = new AmazonS3Client(tempCredentials, region))
        {
            var request = new GetObjectRequest
            {
                BucketName = s3.bucketname,
                           Key = s3.filename
            };
        using (var getObjectResponse = await client.GetObjectAsync(request))
        {
            using (var responseStream = getObjectResponse.ResponseStream)
            {
                await responseStream.CopyToAsync(stream);
                stream.Position = 0;
            }
        }
        }

        Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                FileName = s3.filename,
                Inline = true // false = prompt the user for downloading, true = browser to try to show the file inline
                }.ToString());
        return File(stream, "image/jpeg");

        }
        [HttpGet("test02")]
        public async Task<IActionResult> test02([FromQuery]S3Context s3) {
        RegionEndpoint region = RegionEndpoint.APSoutheast1;
        SessionAWSCredentials tempCredentials = await GetObject.GetTemporaryCredentialsAsync();
        
          return Ok(s3.filename);

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

        public static async Task<SessionAWSCredentials> GetTemporaryCredentialsAsync()
        {
            using (var stsClient = new AmazonSecurityTokenServiceClient())
            {
                var getSessionTokenRequest = new GetSessionTokenRequest
                {
                    DurationSeconds = 7200 // seconds
                };

                GetSessionTokenResponse sessionTokenResponse =
                              await stsClient.GetSessionTokenAsync(getSessionTokenRequest);

                Credentials credentials = sessionTokenResponse.Credentials;
                Console.WriteLine(credentials.AccessKeyId);
                Console.WriteLine(credentials.SecretAccessKey);
                Console.WriteLine(credentials.SessionToken);

                var sessionCredentials =
                    new SessionAWSCredentials(credentials.AccessKeyId,
                                              credentials.SecretAccessKey,
                                              credentials.SessionToken);
                return sessionCredentials;
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


                                                           

