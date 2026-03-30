namespace Tudormobile.CommunityService;

public sealed record CommunityResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the API call was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the data returned by the API call, can be <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// May contain error information on failure. Can be null on success when no data is returned.
    /// </remarks>
    public T? Data { get; init; }
}

internal static class CommunityResponse
{
    /// <summary>
    /// Creates a successful community response containing the specified value.
    /// </summary>
    /// <param name="value">The value to include in the response data.</param>
    /// <returns>A new CommunityResponse<T> instance with the specified value set as data and the success flag set to true.</returns>
    public static CommunityResponse<T> IsSuccess<T>(T value) => new() { Data = value, Success = true };

    /// <summary>
    /// Creates a failed response containing the specified value.
    /// </summary>
    /// <param name="value">The value to include in the response's data payload.</param>
    /// <returns>A new CommunityResponse<T> instance with Success set to false and Data set to the specified value.</returns>
    public static CommunityResponse<T> IsFailure<T>(T value) => new() { Data = value, Success = false };
}