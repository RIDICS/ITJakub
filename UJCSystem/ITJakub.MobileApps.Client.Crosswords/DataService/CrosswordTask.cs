using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.Client.Crosswords.DataContract;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public class CrosswordTask
    {
        private readonly List<string> m_correctAnswers;
        private readonly bool[] m_correctllyFilledRows;

        public CrosswordTask(CrosswordTaskContract taskContract)
        {
            m_correctAnswers = new List<string>(taskContract.RowList.Where(row => row.Answer != null).Select(row => row.Answer.ToUpper()));
            m_correctllyFilledRows = new bool[m_correctAnswers.Count];
            Win = false;
        }

        public void FillWord(int rowIndex, string word)
        {
            var uppercaseWord = word.ToUpper();
            m_correctllyFilledRows[rowIndex] = uppercaseWord == m_correctAnswers[rowIndex];

            Win = m_correctllyFilledRows.All(isCorrect => isCorrect);
        }

        public bool IsRowFilledCorrectly(int rowIndex)
        {
            return m_correctllyFilledRows[rowIndex];
        }

        public bool Win { get; private set; }
    }
}