using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data
{
    public class AnswerViewModel
    {
        public int WordPosition { get; set; }

        public IList<ConcreteAnswerViewModel> Answers { get; set; } 
    }

    public class ConcreteAnswerViewModel
    {
        public int StartPosition { get; set; }

        public string Answer { get; set; }
    }
}