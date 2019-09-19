using System.Linq;
using System.Text;
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
            var client = m_communicationProvider.GetMainServiceProjectClient();
            var commentContracts = client.GetCommentsForText(textId);
            var commentMarks = m_markdownCommentAnalyzer.FindAllComments(request.Text);

            var textBuilder = new StringBuilder(request.Text);

            foreach (var commentMark in commentMarks)
            {
                var isInvalid = !commentMark.IsIdValid || !commentMark.ContainsBothTags;
                var commentContract = commentContracts.FirstOrDefault(x => x.TextReferenceId == commentMark.Identifier);

                if (isInvalid || commentContract == null)
                {
                    // Remove tag reference from text
                    if (!string.IsNullOrEmpty(commentMark.StartTag))
                    {
                        textBuilder.Replace(commentMark.StartTag, string.Empty);
                    }

                    if (!string.IsNullOrEmpty(commentMark.EndTag))
                    {
                        textBuilder.Replace(commentMark.EndTag, string.Empty);
                    }
                }
            }

            foreach (var commentContract in commentContracts)
            {
                var commentMark = commentMarks.FirstOrDefault(x => x.Identifier == commentContract.TextReferenceId);

                if (commentMark == null || !commentMark.ContainsBothTags || !commentMark.IsIdValid)
                {
                    // Add missing reference to the text
                    textBuilder.Insert(0, $"${commentContract.TextReferenceId}%%{commentContract.TextReferenceId}$");
                }
            }

            request.Text = textBuilder.ToString();
            
            return SaveWithoutValidation(textId, request);
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
