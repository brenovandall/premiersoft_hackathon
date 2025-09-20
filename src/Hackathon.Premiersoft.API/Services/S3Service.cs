using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace Hackathon.Premiersoft.API.Services
{
    public class S3Service
    {
        private static readonly string accessKey = "AKIA3WB4RELIHVOTFWPR";
        private static readonly string secretKey = "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";
        private static readonly string bucketName = "premiersoft-hackathon-uploads";
        private static readonly RegionEndpoint region = RegionEndpoint.USEast1;

        private static AmazonS3Client GetS3Client()
        {
            return new AmazonS3Client(accessKey, secretKey, region);
        }

        /// <summary>
        /// Retorna um StreamReader para leitura direta do arquivo no S3.
        /// </summary>
        /// <param name="keyName">Caminho completo do arquivo dentro do bucket</param>
        /// <returns>StreamReader para leitura de texto</returns>
        public async Task<StreamReader> ObterLeitorDoArquivoAsync(string keyName)
        {
            try
            {
                var s3Client = GetS3Client();

                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                var response = await s3Client.GetObjectAsync(request);

                return new StreamReader(response.ResponseStream);
            }
            catch (AmazonS3Exception s3Ex)
            {
                // Exceção específica da AWS S3
                Console.WriteLine($"Erro ao acessar o S3: {s3Ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Exceção geral
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw;
            }
        }

    }
}
