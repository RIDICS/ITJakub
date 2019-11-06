using System;
using System.Collections.Generic;

namespace Vokabular.RestClient
{
    public class FormDataBuilder : BuilderBase<IList<Tuple<string, string>>>
    {
        private readonly IList<Tuple<string, string>> m_dataList;

        private FormDataBuilder()
        {
            m_dataList = new List<Tuple<string, string>>();
        }

        public static FormDataBuilder Create()
        {
            return new FormDataBuilder();
        }

        protected override void AppendParameter(string name, string value)
        {
            m_dataList.Add(new Tuple<string, string>(name, value));
        }

        public override IList<Tuple<string, string>> ToResult()
        {
            return m_dataList;
        }
    }
}