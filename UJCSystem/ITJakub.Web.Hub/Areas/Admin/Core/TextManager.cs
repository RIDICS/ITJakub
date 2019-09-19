﻿using System.Linq;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.TextConverter.Markdown.Extensions.CommentMark;

namespace ITJakub.Web.Hub.Areas.Admin.Core
{
    public class TextManager
    {
        private readonly MarkdownCommentAnalyzer m_markdownCommentAnalyzer;
        private readonly CommunicationProvider m_communicationProvider;

        public TextManager(MarkdownCommentAnalyzer markdownCommentAnalyzer, CommunicationProvider communicationProvider)
        {
            m_markdownCommentAnalyzer = markdownCommentAnalyzer;
            m_communicationProvider = communicationProvider;
        }

        private bool HasOnlyValidCommentSyntax(string text)
        {
            var commentMarks = m_markdownCommentAnalyzer.FindAllComments(text);
            var result = commentMarks.All(x => x.IsIdValid && x.ContainsBothTags);
            return result;
        }

        private bool HasOnlyValidCommentsWithValues(string text, long textId)
        {
            var commentMarks = m_markdownCommentAnalyzer.FindAllComments(text);
            var isValidSyntax = commentMarks.All(x => x.IsIdValid && x.ContainsBothTags);
            if (!isValidSyntax)
            {
                return false;
            }

            var client = m_communicationProvider.GetMainServiceProjectClient();
            var commentsContract = client.GetCommentsForText(textId);

            foreach (var markdownCommentData in commentMarks)
            {
                var commentContract = commentsContract.FirstOrDefault(x => x.TextReferenceId == markdownCommentData.Identifier);
                if (commentContract == null)
                {
                    return false;
                }
            }

            if (commentMarks.Count != commentsContract.Count)
            {
                return false;
            }

            return true;
        }

        private SaveTextResponse ValidationFailResponse()
        {
            return new SaveTextResponse
            {
                IsValidationSuccess = false
            };
        }

        public SaveTextResponse SaveTextFullValidate(long textId, CreateTextRequestContract request)
        {
            var isValid = HasOnlyValidCommentsWithValues(request.Text, textId);

            return !isValid ? ValidationFailResponse() : SaveWithoutValidation(textId, request);
        }

        public SaveTextResponse SaveTextFullValidateAndRepair(long textId, CreateTextRequestContract request)
        {
            // The most difficult function
            throw new System.NotImplementedException();
        }

        public SaveTextResponse SaveTextValidateSyntax(long textId, CreateTextRequestContract request)
        {
            var isValidSyntax = HasOnlyValidCommentSyntax(request.Text);

            return !isValidSyntax ? ValidationFailResponse() : SaveWithoutValidation(textId, request);
        }

        public SaveTextResponse SaveWithoutValidation(long textId, CreateTextRequestContract request)
        {
            var client = m_communicationProvider.GetMainServiceProjectClient();
            var resourceVersionId = client.CreateTextResourceVersion(textId, request);
            return new SaveTextResponse
            {
                IsValidationSuccess = true,
                ResourceVersionId = resourceVersionId,
            };
        }
    }
}
