namespace MathHelpApp.Constants;

/// <summary>
/// Constants used throughout the application for multiplication practice.
/// </summary>
internal static class MathConstants
{
    /// <summary>
    /// Minimum allowed multiplication table (1).
    /// </summary>
    public const int MinTableNumber = 1;

    /// <summary>
    /// Maximum allowed multiplication table (12).
    /// </summary>
    public const int MaxTableNumber = 12;

    /// <summary>
    /// Number of problems generated per practice sheet.
    /// </summary>
    public const int ProblemsPerSheet = 100;

    /// <summary>
    /// Number of problems in each group on a practice sheet.
    /// </summary>
    public const int ProblemsPerGroup = 10;

    /// <summary>
    /// Number of problem groups per practice sheet (ProblemsPerSheet / ProblemsPerGroup).
    /// </summary>
    public const int GroupsPerSheet = ProblemsPerSheet / ProblemsPerGroup;

    /// <summary>
    /// Maximum number of practice sheets that can be generated at once.
    /// </summary>
    public const int MaxSheetCount = 100;

    /// <summary>
    /// Maximum value in the sheet count dropdown on the home page (1..MaxSheetCountSelect).
    /// </summary>
    public const int MaxSheetCountSelect = 10;

    /// <summary>
    /// Number of problems per group for reference PDFs when the range is partial (e.g. 5-10).
    /// </summary>
    public const int ReferenceProblemsPerGroupPartialRange = 5;

    /// <summary>
    /// Application name used in PDF metadata and footers.
    /// </summary>
    public const string SiteName = "MathHelp";
}
