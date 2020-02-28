using System.Text.Json;

namespace Mobsites.Azure.Functions.Cosmos.Stream.API
{
    internal static class Serialization
    {
        static Serialization()
        {
            Options = new JsonSerializerOptions();
            Options.PropertyNameCaseInsensitive = true;
        }

        public static JsonSerializerOptions Options { get; set; }
    }
}