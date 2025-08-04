namespace Flyingdarts.CDK.Constructs;

public static class Constants
{
    public static string PlatformName = "Flyingdarts";
    public static string DomainName = "flyingdarts.net";

    public static class Stacks
    {
        private static readonly string Root = PlatformName;

        public static string GetName(string resourceName) => $"{Root}-{resourceName}";

        private static string GetStack(string stackName) => $"{GetName(stackName)}-Stack";

        public static readonly string Frontend = GetStack("Frontend");
        public static readonly string Backend = GetStack("Backend");
        public static readonly string Auth = GetStack("Auth");
        public static readonly string Domain = GetStack("Domain");
    }
}
