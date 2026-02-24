namespace SETemplate.McpSrv.Models;

/// <summary>
/// Represents the result of a grade calculation.
/// </summary>
public record GradeResult
{
    /// <summary>
    /// The percentage score achieved.
    /// </summary>
    public double Percentage { get; init; }
    /// <summary>
    /// The corresponding grade as a string.
    /// </summary>
    public string Grade { get; init; } = string.Empty;
}
