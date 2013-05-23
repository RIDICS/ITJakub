using IT_Jakub.Classes.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace IT_Jakub.Classes.Models.Utils {
    /// <summary>
    /// Class can open RTF file.
    /// </summary>
    class RtfFileOpener {

        /// <summary>
        /// Opens the document from URI and loads it to SyncReadingApp.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        internal async Task<string> openDocumentFromUri(string uri) {
            Uri source;
            if (!Uri.TryCreate(uri.Trim(), UriKind.Absolute, out source)) {
                return null;
            }
            Windows.Storage.StorageFile file = await FileDownloader.downloadFileFromUri(source);

            Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

            RichEditBox reb = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();
            reb.IsReadOnly = false;
            reb.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
            reb.IsReadOnly = true;
            return source.ToString();
        }

    }
}
