namespace BabelLibs
{
    public record ExecutionContext(
        string sourceProvider,
        string sourceIdentifier,
        string destinationProvider,
        bool loggingOption,
        string modelOption,
        int tokenLimit,
        int maxTokenLimit);
}
