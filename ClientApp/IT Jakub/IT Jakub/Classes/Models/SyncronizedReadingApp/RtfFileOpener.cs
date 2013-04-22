using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace IT_Jakub.Classes.Models.SyncronizedReadingApp {
    class RtfFileOpener {

        internal async Task<string> openDocumentFromUri(string uri) {
            Uri source;
            if (!Uri.TryCreate(uri.Trim(), UriKind.Absolute, out source)) {
                
            //    rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return null;
            }

            string destination = "tempFile.rtf".Trim();

            if (string.IsNullOrWhiteSpace(destination)) {
                // rootPage.NotifyUser("A local file name is required.", NotifyType.ErrorMessage);
                return null;
            }

            StorageFile destinationFile;
            try {
                var applicationData = Windows.Storage.ApplicationData.Current;
                var temporaryFolder = applicationData.TemporaryFolder;

                destinationFile = await temporaryFolder.CreateFileAsync(
                    destination, CreationCollisionOption.GenerateUniqueName);
            } catch (FileNotFoundException ex) {
                // rootPage.NotifyUser("Error while creating file: " + ex.Message, NotifyType.ErrorMessage);
                return null;
            }

            BackgroundDownloader downloader = new BackgroundDownloader();
            DownloadOperation download = downloader.CreateDownload(source, destinationFile);
            await download.StartAsync();

            Windows.Storage.StorageFile file = destinationFile;

            Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

            RichEditBox reb = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();
            // Load the file into the Document property of the RichEditBox.
            reb.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
            return source.ToString();
        }

    }
}
