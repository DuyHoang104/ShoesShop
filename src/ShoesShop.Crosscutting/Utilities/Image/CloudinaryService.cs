using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class CloudinarySettings
{
    public string CloudName { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public string BaseUrl { get; set; } = null!;
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

        // đọc nội dung của file vào một stream
        await using var stream = file.OpenReadStream();
        // thiết lập các tham số upload
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folderName
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        // lấy URL đầy đủ của hình ảnh đã tải lên
        var fullUrl = uploadResult.SecureUrl?.ToString();
        //Xác định vị trí marker /image/upload/ trong URL
        const string marker = "/image/upload/";
        // tạo URL tương đối bằng cách cắt bỏ phần trước marker
        var index = fullUrl?.IndexOf(marker, StringComparison.Ordinal) ?? -1;
        var relativeUrl = index >= 0
            ? fullUrl.Substring(index + marker.Length)
            : fullUrl;

        return (folderName, uploadResult.PublicId, relativeUrl ?? string.Empty);
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image,
            Invalidate = true
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "Deleted" || result.Result == "not found";
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
