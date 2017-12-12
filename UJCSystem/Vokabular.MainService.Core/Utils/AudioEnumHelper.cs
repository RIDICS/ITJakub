using System.IO;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.MainService.Core.Utils
{
    public class AudioEnumHelper
    {
        public static AudioTypeEnum FileNameToAudioType(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            switch (extension)
            {
                case ".mp3":
                    return AudioTypeEnum.Mp3;
                case ".ogg":
                    return AudioTypeEnum.Ogg;
                case ".wav":
                    return AudioTypeEnum.Wav;
                default:
                    return AudioTypeEnum.Unknown;
            }
        }
    }
}
