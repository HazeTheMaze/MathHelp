using MathHelpApp.Constants;

namespace MathHelpApp.Validation;

/// <summary>
/// Validates table range parameters (min/max) for PDF and practice.
/// </summary>
internal static class TableRangeValidation
{
    /// <summary>
    /// Validates that minTable and maxTable are within 1..12 and minTable &lt;= maxTable.
    /// Returns a resource key for the UI to localize (e.g. Validation.MinTableRange), or null if valid.
    /// </summary>
    /// <returns>Null if valid; otherwise a resource key for the validation message.</returns>
    public static string? Validate(int minTable, int maxTable)
    {
        if (minTable < MathConstants.MinTableNumber || minTable > MathConstants.MaxTableNumber)
        {
            return "Validation.MinTableRange";
        }

        if (maxTable < MathConstants.MinTableNumber || maxTable > MathConstants.MaxTableNumber)
        {
            return "Validation.MaxTableRange";
        }

        if (minTable > maxTable)
        {
            return "Validation.MinGreaterThanMax";
        }

        return null;
    }
}
