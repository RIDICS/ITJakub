using System;
namespace Daliboris.OOXML.Word
{
   public interface IRunFormatting
    {
        bool? Bold { get; set; }
        bool? Caps { get; set; }
        string Color { get; set; }
        bool? DoubleStrike { get; set; }
        int? FitText { get; set; }
        string FontName { get; set; }
        int? FontSize { get; set; }
        bool? Italic { get; set; }
        string Lang { get; set; }
        bool? NoProof { get; set; }
        int? Position { get; set; }
        bool? SingleStriketrhough { get; set; }
        bool? SmallCaps { get; set; }
        int? Spacing { get; set; }
        string Underline { get; set; }
        string VerticalAlign { get; set; }

        IRunFormatting MergeFormatting(IRunFormatting nestedFormatting);

    }
}
