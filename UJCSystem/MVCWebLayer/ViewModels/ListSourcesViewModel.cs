using System.Collections.Generic;
using ITJakub.MVCWebLayer.Enums;
using ITJakub.MVCWebLayer.Services.DTOs;

namespace ITJakub.MVCWebLayer.ViewModels
{ 
    public class ListSourcesViewModel
    {
        public SourcesViewMode ViewMode { get; set; }
        public IEnumerable<Source> FoundSources { get; set; }
        public readonly List<AlphabetLetters> Alphabet = new List<AlphabetLetters>
            {
                new AlphabetLetters("A", false), 
                new AlphabetLetters("B", false), 
                new AlphabetLetters("C", true), 
                new AlphabetLetters("Č", true), 
                new AlphabetLetters("D", true), 
                new AlphabetLetters("E", true), 
                new AlphabetLetters("F", true), 
                new AlphabetLetters("G", true), 
                new AlphabetLetters("H", true), 
                new AlphabetLetters("Ch", true), 
                new AlphabetLetters("I", true), 
                new AlphabetLetters("J", true), 
                new AlphabetLetters("K", true), 
                new AlphabetLetters("L", true), 
                new AlphabetLetters("M", true), 
                new AlphabetLetters("N", true), 
                new AlphabetLetters("O", true), 
                new AlphabetLetters("P", true), 
                new AlphabetLetters("Q", true), 
                new AlphabetLetters("R", true), 
                new AlphabetLetters("Ř", true), 
                new AlphabetLetters("S", true), 
                new AlphabetLetters("Š", true), 
                new AlphabetLetters("T", true), 
                new AlphabetLetters("U", true), 
                new AlphabetLetters("V", true), 
                new AlphabetLetters("W", true), 
                new AlphabetLetters("X", true), 
                new AlphabetLetters("Y", true), 
                new AlphabetLetters("Z", true), 
                new AlphabetLetters("Ž", true) 
            };
    }

    public class AlphabetLetters
    {
        public readonly string Letter;
        public readonly bool Disabled;
        
        public AlphabetLetters(string letter, bool disabled)
        {
            Letter = letter;
            Disabled = disabled;
        }
    }

}