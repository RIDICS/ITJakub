using System;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class AnswerTypeViewModel : ViewModelBase
    {
        private bool m_isNoAnswer;
        private bool m_isFill;
        private bool m_isSelect;
        private AnswerType m_answerType;

        public AnswerTypeViewModel()
        {
            IsNoAnswer = true;
            AnswerType = AnswerType.NoAnswer;
        }

        public bool IsNoAnswer
        {
            get { return m_isNoAnswer; }
            set
            {
                if (m_isNoAnswer == value)
                    return;

                m_isNoAnswer = value;
                RaisePropertyChanged();

                if (value)
                    AnswerType = AnswerType.NoAnswer;
            }
        }

        public bool IsFill
        {
            get { return m_isFill; }
            set
            {
                if (m_isFill == value)
                    return;

                m_isFill = value;
                RaisePropertyChanged();

                if (value)
                    AnswerType = AnswerType.Fill;
            }
        }

        public bool IsSelect
        {
            get { return m_isSelect; }
            set
            {
                if (m_isSelect == value)
                    return;

                m_isSelect = value;
                RaisePropertyChanged();

                if (value)
                    AnswerType = AnswerType.Selection;
            }
        }

        public AnswerType AnswerType
        {
            get { return m_answerType; }
            set
            {
                if (m_answerType == value)
                    return;

                m_answerType = value;
                RaisePropertyChanged();

                switch (value)
                {
                    case AnswerType.Fill:
                        IsFill = true;
                        break;
                    case AnswerType.Selection:
                        IsSelect = true;
                        break;
                    default:
                        IsNoAnswer = true;
                        break;
                }

                if (AnswerChangedCallback != null)
                    AnswerChangedCallback();
            }
        }

        public Action AnswerChangedCallback { get; set; }
    }
}