using MathHelpApp.Constants;

namespace MathHelpApp.Validation;

/// <summary>
/// Validates table range parameters (min/max) for PDF and practice.
/// </summary>
public static class TableRangeValidation
{
    /// <summary>
    /// Validates that minTable and maxTable are within 1..12 and minTable &lt;= maxTable.
    /// </summary>
    /// <returns>Null if valid; otherwise an error message.</returns>
    public static string? Validate(int minTable, int maxTable)
    {
        if (minTable < MathConstants.MinTableNumber || minTable > MathConstants.MaxTableNumber)
        {
            return $"minTable must be between {MathConstants.MinTableNumber} and {MathConstants.MaxTableNumber}.";
        }

        if (maxTable < MathConstants.MinTableNumber || maxTable > MathConstants.MaxTableNumber)
        {
            return $"maxTable must be between {MathConstants.MinTableNumber} and {MathConstants.MaxTableNumber}.";
        }

        if (minTable > maxTable)
        {
            return "minTable must be less than or equal to maxTable.";
        }

        return null;
    }
}
