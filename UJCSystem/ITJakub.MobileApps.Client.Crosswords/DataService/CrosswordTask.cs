using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.Client.Crosswords.DataContract;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public class CrosswordTask
    {
        private readonly List<string> m_correctAnswers;
        private readonly string[] m_userAnswers;

        public CrosswordTask(CrosswordTaskContract taskContract)
        {
            m_correctAnswers = new List<string>(taskContract.RowList.Select(row => row.Answer != null ? row.Answer.ToUpper() : null));
            m_userAnswers = new string[m_correctAnswers.Count];
        }

        public void FillWord(int rowIndex, string word)
        {
            m_userAnswers[rowIndex] = word.ToUpper();
        }

        public bool IsRowFilledCorrectly(int rowIndex)
        {
            return m_userAnswers[rowIndex] == m_correctAnswers[rowIndex];
        }
    }
}