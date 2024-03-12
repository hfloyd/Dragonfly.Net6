using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.NetModels
{
    //ref: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types

    public class MimeType
    {
        public enum MimeTypeName
        {
            AacAudio,
            AbiWordDoc,
            ArchiveDoc,
            AVI,
            AmazonKindle,
            BinaryData,
            BitmapGraphic,
            BZipArchive,
            BZip2Archive,
            CShellScript,
            CSS,
            CSV,
            MicrosoftWord,
            MicrosoftWordOpenXML,
            MSEmbeddedOpenTypeFonts,
            EPUB,
            ECMAScript,
            GIF,
            HTML,
            Icon,
            iCalendar,
            JAR,
            JPEG,
            JavaScript,
            JSON,
            MIDI,
            MPEG,
            AppleInstallerPackage,
            OpenDocumentPresentation,
            OpenDocumentSpreadsheet,
            OpenDocumentText,
            OGGAudio,
            OGGVideo,
            OGGApplication,
            OpenTypeFont,
            PNG,
            PDF,
            MicrosoftPowerPoint,
            MicrosoftPowerPointOpenXML,
            RAR,
            RTF,
            BourneShellScript,
            SVG,
            SWF,
            TAR,
            TIFF,
            Typescript,
            TrueTypeFont,
            Text,
            MicrosoftVisio,
            WAV,
            WEBMAudio,
            WEBMVideo,
            WEBPImage,
            WOFF,
            WOFF2,
            XHTML,
            MicrosoftExcel,
            MicrosoftExcelOpenXML,
            XML,
            XUL,
            ZIP,
            Video3GPP,
            Audio3GPP,
            Video3GPP2,
            Audio3GPP2,
            Archive7Zip,
            Other
        }

        public IEnumerable<string> Extensions { get; set; }
        public string DisplayName { get; set; }
        public string TypeString { get; set; }
        public MimeTypeName Type { get; set; }
    }
}
