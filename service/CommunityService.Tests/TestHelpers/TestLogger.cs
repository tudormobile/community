namespace CommunityService.Tests.TestHelpers;

/// <summary>
/// Simple test logger implementation for unit testing.
/// </summary>
internal class TestLogger : ILogger
{
    public List<string> LoggedMessages { get; } = [];
    public List<LogLevel> LoggedLevels { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        LoggedLevels.Add(logLevel);
        LoggedMessages.Add(formatter(state, exception));
    }

    public void Clear()
    {
        LoggedMessages.Clear();
        LoggedLevels.Clear();
    }
}
