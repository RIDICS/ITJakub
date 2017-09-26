﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vokabular.MainService.DataContracts.Clients.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class UrlQueryBuilder
    {
        private readonly StringBuilder m_stringBuilder;
        private bool m_containsParameters;

        private UrlQueryBuilder(string url)
        {
            m_stringBuilder = new StringBuilder(url);
            m_containsParameters = url.Contains("?");
        }

        public static UrlQueryBuilder Create(string url)
        {
            return new UrlQueryBuilder(url);
        }

        private void AppendParameter(string name, string value)
        {
            m_stringBuilder
                .Append(m_containsParameters ? '&' : '?')
                .Append(name)
                .Append("=")
                .Append(value.EncodeQueryString());

            m_containsParameters = true;
        }

        public UrlQueryBuilder AddParameter(string name, string value)
        {
            if (value == null)
                return this;

            AppendParameter(name, value);
            return this;
        }

        public UrlQueryBuilder AddParameter(string name, Enum value)
        {
            if (value == null)
                return this;

            AppendParameter(name, value.ToString());
            return this;
        }

        public UrlQueryBuilder AddParameterList(string name, IEnumerable list)
        {
            if (list == null)
                return this;

            foreach (var item in list)
            {
                AppendParameter(name, item.ToString());
            }
            return this;
        }

        public UrlQueryBuilder AddParameterList(string name, IEnumerable<string> list)
        {
            return AddParameterList(name, (IEnumerable) list?.Select(x => x.EncodeQueryString()));
        }

        public string ToQuery()
        {
            return m_stringBuilder.ToString();
        }
    }
}
