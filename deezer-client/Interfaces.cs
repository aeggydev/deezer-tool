namespace deezer_client
{
    public interface IApiMethod
    {
        public static string MethodName { get; }
    }

    public record Dimensions
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}