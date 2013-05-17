using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Enums;
using Ujc.Naki.MVCWebLayer.Services.DTOs;

namespace Ujc.Naki.MVCWebLayer.ViewModels
{
    public class ListSourcesViewModel
    {
        public SourcesViewMode ViewMode { get; set; }
        public IEnumerable<Source> FoundSources { get; set; }
        public List<AlphabetLetters> Alphabet = new List<AlphabetLetters>
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
        public string Letter;
        public bool Disabled;
        
        public AlphabetLetters(string letter, bool disabled)
        {
            Letter = letter;
            Disabled = disabled;
        }
    }

}