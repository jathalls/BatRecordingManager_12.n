namespace UniversalToolkit
{
    /// <summary>
    /// Local version of APIKeys which contains my personal API keys
    /// </summary>
    public static partial class APIKeys
    {
        static APIKeys()
        {
            // BingMapsLicenseKey = System.Text.Encoding.UTF8.GetString(
            //     System.Convert.FromBase64String(
            //         "AhhVL9x6bq6w0NbyqlwjmXDh3Qd64GbWowQQlFzrqx0ChD1MvaLkMTDQxuh2bhzh"));
            BingMapsLicenseKey = "AhhVL9x6bq6w0NbyqlwjmXDh3Qd64GbWowQQlFzrqx0ChD1MvaLkMTDQxuh2bhzh";

            VisualCrossingsLicenseKey = "KMNSDWKHLFJNQKD7PP4RPN8LQ";

            DarkSkyApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("MGE5MjAzYzQ1YzAxNDYyMjUzMGU0MWQ5MGM0NzIxZjU="));



        }
    }
}
