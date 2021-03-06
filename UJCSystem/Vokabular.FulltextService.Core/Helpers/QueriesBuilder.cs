﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.TextConverter.Markdown;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class QueriesBuilder
    {
        private readonly IndexType m_indexType;
        private readonly IMarkdownHtmlEncoder m_markdownHtmlEncoder;

        private const string ReservedChars = "\\.?+*|{}[]()\"#@&<>~";
        private const string RegexpQueryFlags = "INTERSECTION";
        private const string IdField = "_id";

        public QueriesBuilder(IndexType indexType, IMarkdownHtmlEncoder markdownHtmlEncoder)
        {
            m_indexType = indexType;
            m_markdownHtmlEncoder = markdownHtmlEncoder;
        }

        public QueryContainer GetFilterSearchQuery(IList<SearchCriteriaContract> conditionConjunction, string fieldName)
        {
            var snapshotRestrictions = new List<SnapshotResultRestrictionCriteriaContract>();
            foreach (var conjunction in conditionConjunction)
            {
                if (conjunction.Key == CriteriaKey.SnapshotResultRestriction)
                {
                    snapshotRestrictions.Add(conjunction as SnapshotResultRestrictionCriteriaContract);
                }
            }
            return GetSnapshotIdFilterQuery(snapshotRestrictions, fieldName);
        }

        public QueryContainer GetSearchQuery(IList<SearchCriteriaContract> conditionConjunction, string fieldName)
        {
            StringBuilder regexBuilder = new StringBuilder();

            foreach (var conjunction in conditionConjunction)
            {
                if (conjunction.Key == CriteriaKey.Fulltext)
                {
                    var regex = GetRegexFromWordList(conjunction as WordListCriteriaContract);

                    regexBuilder.Append("(");
                    regexBuilder.Append(regex);
                    regexBuilder.Append(")&");
                }
            }

            regexBuilder.Length--;
            return new RegexpQuery
            {
                Field = fieldName,
                Value = regexBuilder.ToString(),
                Flags = RegexpQueryFlags
            };
        }

        public QueryContainer GetFilterByIdSearchQuery(string textResourceId)
        {
            return new TermQuery
            {
                Field = IdField,
                Value = textResourceId,
            };
        }

        public QueryContainer GetFilterByIdSearchQuery(List<string> textResourceId)
        {
            return new TermsQuery
            {
                Field = IdField,
                Terms = textResourceId,
            };
        }

        public QueryContainer GetFilterByFieldSearchQuery(string field, string value)
        {
            return new TermQuery
            {
                Field = field,
                Value = value,
            };
        }

        private string GetRegexFromWordList(WordListCriteriaContract wordListCriteriaContract)
        {
            if (wordListCriteriaContract == null)
            {
                return null;
            }

            StringBuilder regexBuilder = new StringBuilder();

            foreach (var disjunction in wordListCriteriaContract.Disjunctions)
            {
                regexBuilder.Append("(");
                regexBuilder.Append(GetRegexFromDisjunction(disjunction));
                regexBuilder.Append(")|");
            }

            regexBuilder.Length--;
            return regexBuilder.ToString();
        }

        private QueryContainer GetSnapshotIdFilterQuery(List<SnapshotResultRestrictionCriteriaContract> snapshotRestrictions, string snapshotIdFieldName)
        {
            var idList = new List<object>();
            foreach (var restriction in snapshotRestrictions)
            {
                if (restriction != null && restriction.SnapshotIds != null)
                {
                    idList.AddRange(restriction.SnapshotIds.Cast<object>());
                }
            }
            return new QueryContainer(new TermsQuery
            {
                Field = snapshotIdFieldName,
                Terms = idList,
            });
        }

        private string GetRegexFromDisjunction(WordCriteriaContract disjunction)
        {
            if (!string.IsNullOrWhiteSpace(disjunction.ExactMatch))
                return EscapeChars(disjunction.ExactMatch);

            var regexBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(disjunction.StartsWith))
            {
                var escapedText = EscapeChars(disjunction.StartsWith.ToLower());
                regexBuilder.Append(escapedText);
            }

            regexBuilder.Append(".*");

            foreach (var contains in disjunction.Contains)
            {
                if (!string.IsNullOrWhiteSpace(contains))
                {
                    var escapedText = EscapeChars(contains.ToLower());
                    regexBuilder.Append(escapedText);
                    regexBuilder.Append(".*");
                }
            }

            if (!string.IsNullOrWhiteSpace(disjunction.EndsWith))
            {
                var escapedText = EscapeChars(disjunction.EndsWith.ToLower());
                regexBuilder.Append(escapedText);
            }

            return regexBuilder.ToString();
        }

        private string EscapeChars(string text)
        {
            switch (m_indexType)
            {
                case IndexType.Page:
                    text = EscapeForPageIndex(text);
                    break;
                case IndexType.Snapshot:
                    text = EscapeForSnapshotIndex(text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var reservedChar in ReservedChars)
            {
                text = text.Replace(reservedChar.ToString(), $"\\{reservedChar}");
            }

            return text;
        }

        private string EscapeForPageIndex(string text)
        {
            return text;
        }

        private string EscapeForSnapshotIndex(string text)
        {
            var htmlEncoded = m_markdownHtmlEncoder.EscapeHtml(text);
            return htmlEncoded;
        }
    }

    public enum IndexType
    {
        Page,
        Snapshot,
    }
}