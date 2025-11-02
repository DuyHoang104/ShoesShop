using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class CloudinarySettings
{
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string BaseUrl { get; set; } 
}

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _baseUrl;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
        _baseUrl = config.Value.BaseUrl;
    }

    public async Task<(string Folder, string FileName, string Url)?> UploadImageAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            return null;

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folderName
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        var fullUrl = uploadResult.SecureUrl?.ToString();

        //lấy phần sau "image/upload/"
        const string marker = "/image/upload/";
        var index = fullUrl?.IndexOf(marker, StringComparison.Ordinal) ?? -1;
        var relativeUrl = index >= 0
            ? fullUrl.Substring(index + marker.Length)
            : fullUrl;

        return (folderName, uploadResult.PublicId, relativeUrl ?? string.Empty);
    }
}

public static class CloudinaryExtensions
{
    public static string ToCloudinaryUrl(this string relativePath, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
        relativePath = relativePath.TrimStart('/');
        return $"{baseUrl}{relativePath}";
    }
}
