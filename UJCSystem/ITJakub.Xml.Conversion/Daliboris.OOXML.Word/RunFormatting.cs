using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.OOXML.Word
{

	/*
	 			case "w:b": //Bold
				case "w:bCs": //Complex String Bold
				case "w:bdr": //Text Border 
				case "w:caps": //Display All Characters As Capital Letters 
				case "w:color": //Run Content Color 
				case "w:cs": //Use Complex Script Formatting on Run 
				case "w:del": //Deleted Paragraph 
				case "w:dstrike": //Double Strikethrough 
				case "w:eastAsianLayout": //East Asian Typography Settings 
				case "w:effect": //Animated Text Effect 
				case "w:em": //Emphasis Mark 
				case "w:emboss": //Embossing 
				case "w:fitText": //Manual Run Width 
				case "w:highlight": //Text Highlighting 
				case "w:i": //Italics 
				case "w:iCs": //Complex Script Italics 
				case "w:imprint": //Imprinting 
				case "w:ins": //Inserted Paragraph 
				case "w:kern": //Font Kerning 
				case "w:lang": //Languages for Run Content 
				case "w:moveFrom": //Move Source Paragraph 
				case "w:moveTo": //Move Destination Paragraph 
				case "w:noProof": //Do Not Check Spelling or Grammar 
				case "w:oMath": //Office Open XML Math 
				case "w:outline": //Display Character Outline 
				case "w:position": //Vertically Raised or Lowered Text 
				case "w:rFonts": //Run Fonts 
				case "w:rPrChange": //Revision Information for Run Properties on the Paragraph Mark 
				case "w:rStyle": //Referenced Character Style 
				case "w:rtl": //Right To Left Text 
				case "w:shadow": //Shadow 
				case "w:shd": //Run Shading 
				case "w:smallCaps": //Small Caps 
				case "w:snapToGrid": //Use Document Grid Settings For Inter-Character Spacing 
				case "w:spacing": //Character Spacing Adjustment 
				case "w:specVanish": //Paragraph Mark Is Always Hidden 
				case "w:strike": //Single Strikethrough 
				case "w:sz": //Font Size 
				case "w:szCs": //Complex Script Font Size 
				case "w:u": //Underline 
				case "w:vanish": //Hidden Text 
				case "w:vertAlign": //Subscript/Superscript Text 
				case "w:w": //Expanded/Compressed Text 
				case "w:webHidden": //Web Hidden Text 
	*/

    /// <summary>
    /// Třída definující formátování odstavce nebo úseku ve Wordu.
    /// </summary>
	public class RunFormatting : IRunFormatting
	{
        /// <summary>
        /// Tučné písmo
        /// </summary>
        /// <remarks>w:b</remarks>
		public bool? Bold { get; set; }

        /// <summary>
        /// Verzálky (velká písmena)
        /// </summary>
        /// <remarks>w:caps</remarks>
        public bool? Caps { get; set; }

        /// <summary>
        /// Barva
        /// </summary>
        /// <remarks>w:color</remarks>
        public string Color { get; set; }

        /// <summary>
        /// Double Strikethrough
        /// </summary>
        /// <remarks>w:dstrike</remarks>
        public bool? DoubleStrike { get; set; }

        /// <summary>
        /// This element specifies that the contents of this run shall not be automatically displayed based on the width of its
        ///contents, rather its contents shall be resized to fit the width specified by the val attribute. This
        ///expansion/contraction shall be performed by equally increasing/decreasing the size of each character in this
        ///run's contents when displayed.
        /// </summary>
        /// <remarks>w:fitText</remarks>
        public int? FitText { get; set; }

        /// <summary>
        /// Název fontu. This element specifies the fonts which shall be used to display the text contents of this run. Within a single run,
        ///there can be up to four types of content present which shall each be allowed to use a unique font
        /// </summary>
        /// <remarks>w:rFonts[@w:hAnsi]</remarks>
        public string FontName { get; set; }

        /// <summary>
        /// Velikost písma. This element specifies the font size which shall be applied to all non complex script characters in the contents of
        /// this run when displayed. The font sizes specified by this element’s val attribute are expressed as half-point
        /// values.
        /// If this element is not present, the default is to leave the font size at the value applied at the previous level in the
        /// style hierarchy. If this element is never applied in the style hierarchy, then any appropriate font size can be used
        /// for non complex script characters.
        /// </summary>
        ///  <remarks>w:sz</remarks>
        public int? FontSize { get; set; }


        /// <summary>
        /// Kurzivní písmo
        /// </summary>
        /// <remarks>w:i</remarks>
		public bool? Italic { get; set; }

        /// <summary>
        /// Jazyk
        /// </summary>
        /// <remarks>w:lang</remarks>
        public string Lang { get; set; }

        /// <summary>
        /// Bez kontroly pravopisu. Do Not Check Spelling or Grammar
        /// This element specifies that the contents of this run shall not report any errors when the document is scanned for
        /// spelling and grammar.
        /// </summary>
        /// <remarks>w:noProof</remarks>
        public bool? NoProof { get; set; }


        /// <summary>
        /// Pozice. This element specifies the amount by which text shall be raised or lowered for this run in relation to the default
        /// baseline of the surrounding non-positioned text. This allows the text to be repositioned without altering the font
        /// size of the contents.
        /// If the val attribute is positive, then the parent run shall be raised above the baseline of the surrounding text by
        /// the specified number of half-points. If the val attribute is negative, then the parent run shall be lowered below
        /// the baseline of the surrounding text by the specified number of half-points.
        /// </summary>
        /// <remarks>w:position</remarks>
        public int? Position { get; set; }


        /// <summary>
        /// Kapitálky. This element specifies that all small letter characters in this text run shall be formatted for display only as their
        /// capital letter character equivalents in a font size two points smaller than the actual font size specified for this
        /// text. This property does not affect any non-alphabetic character in this run, and does not change the Unicode
        /// character for lowercase text, only the method in which it is displayed. If this font cannot be made two point
        /// smaller than the current size, then it shall be displayed as the smallest possible font size in capital letters.
        /// </summary>
        /// <remarks>w:smallCaps</remarks>
		public bool? SmallCaps { get; set; }

        /// <summary>
        /// Single Strikethrough
        /// </summary>
        /// <remarks>w:strike</remarks>
        public bool? SingleStriketrhough { get; set; }


        /// <summary>
        /// Podtržení.
        /// </summary>
        /// <remarks>w:u</remarks>
		public string Underline { get; set; }
		
        /// <summary>
        /// This element specifies the amount of character pitch which shall be added or removed after each character in
        /// this run before the following character is rendered in the document. This property has an effect equivalent to
        /// the additional character pitched added by a document grid applied to the contents of a run.
        /// </summary>
        /// <remarks>w:spacing</remarks>
		public int? Spacing { get; set; }

        /// <summary>
        /// Horní/dolní index. This element specifies the alignment which shall be applied to the contents of this run in relation to the default
        ///appearance of the run's text. This allows the text to be repositioned as subscript or superscript without altering
        ///the font size of the run properties.
        /// </summary>
        /// <remarks>W:vertAlign</remarks>
        public string VerticalAlign { get; set; }

        public IRunFormatting MergeFormatting(IRunFormatting nestedFormatting)
        {
			RunFormatting newFormatting = new RunFormatting();
			return newFormatting;
		}




    }

}
