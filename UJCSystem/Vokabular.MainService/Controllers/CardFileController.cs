using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class CardFileController : BaseController
    {
        [HttpGet("")]
        public List<CardFileContract> GetCardFiles()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{cardFileId}/bucket")]
        public List<BucketShortContract> GetBuckets(string cardFileId, [FromQuery] string headword)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card")]
        public List<CardContract> GetCards(string cardFileId, string bucketId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card/short")]
        public List<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card/{cardId}")]
        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{cardFileId}/bucket/{bucketId}/card/{cardId}/image/{imageId}")]
        public IActionResult GetCardImage(string cardFileId, string bucketId, string cardId, string imageId, [FromQuery] CardFileImageSizeEnumContract? imageSize)
        {
            //var imageSizeValue = imageSize ?? CardFileImageSizeEnumContract.Full;
            throw new NotImplementedException();
        }
    }
}
