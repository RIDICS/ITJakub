using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.MVCWebLayer.Enums;

namespace ITJakub.MVCWebLayer.ViewModels
{ 
    public class ListSourcesViewModel
    {
        public SourcesViewType ViewType { get; set; }
        public IEnumerable<SearchResult> FoundSources { get; set; }
        public readonly List<AlphabetLetters> Alphabet = new List<AlphabetLetters>
            {
                new AlphabetLetters("A", false), 
                new AlphabetLetters("B", false), 
                new AlphabetLetters("C", false), 
                new AlphabetLetters("Č", false), 
                new AlphabetLetters("D", false), 
                new AlphabetLetters("E", false), 
                new AlphabetLetters("F", false), 
                new AlphabetLetters("G", false), 
                new AlphabetLetters("H", false), 
                new AlphabetLetters("Ch", false), 
                new AlphabetLetters("I", false), 
                new AlphabetLetters("J", false), 
                new AlphabetLetters("K", false), 
                new AlphabetLetters("L", false), 
                new AlphabetLetters("M", false), 
                new AlphabetLetters("N", false), 
                new AlphabetLetters("O", false), 
                new AlphabetLetters("P", false), 
                new AlphabetLetters("Q", false), 
                new AlphabetLetters("R", false), 
                new AlphabetLetters("Ř", false), 
                new AlphabetLetters("S", false), 
                new AlphabetLetters("Š", false), 
                new AlphabetLetters("T", false), 
                new AlphabetLetters("U", false), 
                new AlphabetLetters("V", false), 
                new AlphabetLetters("W", false), 
                new AlphabetLetters("X", false), 
                new AlphabetLetters("Y", false), 
                new AlphabetLetters("Z", false), 
                new AlphabetLetters("Ž", false) 
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