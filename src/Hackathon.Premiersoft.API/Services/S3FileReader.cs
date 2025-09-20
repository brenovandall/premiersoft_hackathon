using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class S3FileReader
{
    private readonly AmazonS3Client _s3Client;

    public S3FileReader(string accessKey, string secretKey, Amazon.RegionEndpoint region)
    {
        _s3Client = new AmazonS3Client(accessKey, secretKey, region);
    }

    // Construtor alternativo usando perfil da AWS configurado
    public S3FileReader(Amazon.RegionEndpoint region)
    {
        _s3Client = new AmazonS3Client(region);
    }

    /// <summary>
    /// Lê um arquivo do S3 e retorna como string
    /// </summary>
    /// <param name="bucketName">Nome do bucket</param>
    /// <param name="key">Chave/caminho do arquivo no S3</param>
    /// <returns>Conteúdo do arquivo como string</returns>
    public async Task<string> ReadFileAsStringAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            using (var response = await _s3Client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception($"Erro ao ler arquivo do S3: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Lê um arquivo do S3 e retorna como array de bytes
    /// </summary>
    /// <param name="bucketName">Nome do bucket</param>
    /// <param name="key">Chave/caminho do arquivo no S3</param>
    /// <returns>Conteúdo do arquivo como byte array</returns>
    public async Task<byte[]> ReadFileAsBytesAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            using (var response = await _s3Client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            using (var memoryStream = new MemoryStream())
            {
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception($"Erro ao ler arquivo do S3: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Lê um arquivo do S3 e retorna como Stream
    /// </summary>
    /// <param name="bucketName">Nome do bucket</param>
    /// <param name="key">Chave/caminho do arquivo no S3</param>
    /// <returns>Stream do arquivo</returns>
    public async Task<Stream> ReadFileAsStreamAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception($"Erro ao ler arquivo do S3: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verifica se um arquivo existe no S3
    /// </summary>
    /// <param name="bucketName">Nome do bucket</param>
    /// <param name="key">Chave/caminho do arquivo no S3</param>
    /// <returns>True se o arquivo existe</returns>
    public async Task<bool> FileExistsAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}