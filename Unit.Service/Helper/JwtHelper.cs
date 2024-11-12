using System.Text;
using System.Text.Json;

namespace Unit.Service.Helper
{

    public static class JwtHelper
    {
        public static string GetPayloadFromJwt(string token)
        {
            // Tách token thành các phần
            var parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT token format.");

            // Giải mã phần payload (phần thứ hai)
            var payload = parts[1];
            var jsonBytes = Base64UrlDecode(payload);
            return Encoding.UTF8.GetString(jsonBytes);
        }

        private static byte[] Base64UrlDecode(string input)
        {
            // Thêm padding nếu cần thiết
            input = input.Replace('-', '+').Replace('_', '/');
            switch (input.Length % 4)
            {
                case 2: input += "=="; break;
                case 3: input += "="; break;
            }
            return Convert.FromBase64String(input);
        }

        public static string? GetPayloadData(string token, string data)
        {
            string payloadJson = GetPayloadFromJwt(token);
            var payloadData = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);
            return payloadData[data].ToString();
        }
    }
}
