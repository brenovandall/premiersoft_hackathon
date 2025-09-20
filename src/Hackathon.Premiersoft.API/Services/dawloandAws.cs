using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Hackathon.Premiersoft.API.Services
{
    public class dawloandAws
    {
        private static string accessKey = "AKIA3WB4RELIHVOTFWPR";
        private static string secretKey = "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";
        private static string bucketName = "premiersoft-hackathon-uploads";
        private static string region = "us-east-1";

        public async Task Main()
        {
            // Use o key completo conforme a URL do objeto S3
            string keyName = "uploads/manual-import/2025-09-20/1758396548897-pacientes.xml";

            // Ajuste o caminho onde quer salvar o arquivo localmente
            string downloadFilePath = @"C:\caminho\para\salvar\arquivo.ext";

            var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);

            try
            {
                Console.WriteLine("Iniciando download...");

                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                {
                    await response.WriteResponseStreamToFileAsync(downloadFilePath, false, default);
                }

                Console.WriteLine($"Download concluído. Arquivo salvo em: {downloadFilePath}");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Erro ao acessar o S3: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro geral: {e.Message}");
            }
        }
    }
}
