using System.Collections.Generic;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    public class Occurrences : List<IOccurrence> 
    {
         public bool IsOccurrenceWithin(IOccurrence occurrence)
         {
             foreach (Occurrence item in this)
             {
                 if (item.StartIndex <= occurrence.StartIndex && item.EndIndex >= occurrence.EndIndex) return true;
                 if (item.EndIndex > occurrence.EndIndex) return false;
             }
             return false;
         }
    }
}