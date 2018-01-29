using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class CardFileController : BaseController
    {
        private readonly CardFileManager m_cardFileManager;

        public CardFileController(CardFileManager cardFileManager)
        {
            m_cardFileManager = cardFileManager;
        }

        [HttpGet("")]
        public IList<CardFileContract> GetCardFiles()
        {
            return m_cardFileManager.GetCardFiles();
        }

        [HttpGet("{cardFileId}/bucket")]
        public IList<BucketShortContract> GetBuckets(string cardFileId, [FromQuery] string headword)
        {
            return headword == null
                ? m_cardFileManager.GetBuckets(cardFileId)
                : m_cardFileManager.GetBucketsByHeadword(cardFileId, headword);
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card")]
        public IList<CardContract> GetCards(string cardFileId, string bucketId)
        {
            return m_cardFileManager.GetCards(cardFileId, bucketId);
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card/short")]
        public IList<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            return m_cardFileManager.GetCardsShort(cardFileId, bucketId);
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card/{cardId}")]
        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            return m_cardFileManager.GetCard(cardFileId, bucketId, cardId);
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card/{cardId}/image/{imageId}")]
        public IActionResult GetCardImage(string cardFileId, string bucketId, string cardId, string imageId, [FromQuery] CardImageSizeEnumContract? imageSize)
        {
            var imageSizeValue = imageSize ?? CardImageSizeEnumContract.Full;
            var result = m_cardFileManager.GetImage(cardFileId, bucketId, cardId, imageId, imageSizeValue);
            return File(result.Stream, result.MimeType, result.FileName, result.FileSize);
        }
    }
}
