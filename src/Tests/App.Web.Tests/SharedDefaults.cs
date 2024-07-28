using System.Reflection.Metadata;

namespace App.Web.Tests
{
    public static partial class SharedDefaults
    {
        public static string ConfigurationFile = "appsettings.json";

        public static string ApplicationUrl = "http://localhost:5000";

        public static string HeaderAcceptKey => "Accept";

        public static string ContentTypeJson => "application/json";

        public static string ContentTypeXml => "application/xml";

        public static string ContentTypeMessagePack => "application/x-msgpack";
    }
}
