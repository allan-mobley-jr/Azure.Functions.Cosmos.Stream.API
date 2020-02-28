namespace Mobsites.Azure.Functions.Cosmos.Stream.API
{
    internal class QuerySpec
    {
        public string Query { get; set; }
        public QueryParam[] Parameters { get; set; } = new QueryParam[]{};
    }

    internal class QueryParam
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}