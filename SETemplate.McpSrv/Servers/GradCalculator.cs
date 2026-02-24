using System.ComponentModel;
using ModelContextProtocol.Server;
using SETemplate.McpSrv.Models;

namespace SETemplate.McpSrv.Servers;

[McpServerToolType]
public static class GradCalculator
{
    [McpServerTool(Name = "calculate_grade")]
    [Description("Calculates a student grade based on points")]
    public static GradeResult CalculateGrade(
        [Description("Achieved points")] double points,
        [Description("Maximum points")] double maxPoints)
    {
        if (maxPoints <= 0)
        {
            throw new ArgumentException("maxPoints must be greater than zero.");
        }

        var percentage = (points / maxPoints) * 100;

        var grade = percentage switch
        {
            >= 90 => "1 (Very Good)",
            >= 80 => "2 (Good)",
            >= 65 => "3 (Satisfactory)",
            >= 50 => "4 (Sufficient)",
            _ => "5 (Fail)"
        };

        return new GradeResult
        {
            Percentage = Math.Round(percentage, 2),
            Grade = grade
        };
    }

}
