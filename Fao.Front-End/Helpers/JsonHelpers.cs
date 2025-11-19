namespace Fao.Front_End.Helpers;

using System.Text.Json;

public static class JsonHelpers
{
    public static string? SafeString(this JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return null;

        if (!element.TryGetProperty(propertyName, out var value))
            return null;

        return value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    }

    public static int? SafeInt(this JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number)
        {
            return prop.GetInt32();
        }
        return null;
    }

    public static byte[]? SafeByteArray(this JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            var base64String = prop.GetString();
            if (!string.IsNullOrEmpty(base64String))
            {
                return Convert.FromBase64String(base64String);
            }
        }
        return null;
    }

    public static DateOnly SafeStringAsDateOnly(this JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            var dateString = prop.GetString();
            if (DateOnly.TryParse(dateString, out var date))
            {
                return date;
            }
        }
        return default;
    }
}