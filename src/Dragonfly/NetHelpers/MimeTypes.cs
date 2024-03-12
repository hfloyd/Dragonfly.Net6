namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dragonfly.NetModels;

    public static class MimeTypes
    {
        #region Configure MIME constant values HERE

        //ref: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types

        /// <summary>
        /// Stores list of known MIME types in simple CSV format for easy updating
        /// </summary>
        /// <returns></returns>
        private static List<string> MimeCsv()
        {
            var csv = new List<string>();

            csv.Add(".aac,AAC audio,audio/aac,AacAudio");
            csv.Add(".abw,AbiWord document,application/x-abiword,AbiWordDoc");
            csv.Add(".arc,Archive document (multiple files embedded),application/octet-stream,ArchiveDoc");
            csv.Add(".avi,AVI: Audio Video Interleave,video/x-msvideo,AVI");
            csv.Add(".azw,Amazon Kindle eBook format,application/vnd.amazon.ebook,AmazonKindle");
            csv.Add(".bin,Any kind of binary data,application/octet-stream,BinaryData");
            csv.Add(".bmp,Windows OS/2 Bitmap Graphics,image/bmp,BitmapGraphic");
            csv.Add(".bz,BZip archive,application/x-bzip,BZipArchive");
            csv.Add(".bz2,BZip2 archive,application/x-bzip2,BZip2Archive");
            csv.Add(".csh,C-Shell script,application/x-csh,CShellScript");
            csv.Add(".css,Cascading Style Sheets (CSS),text/css,CSS");
            csv.Add(".csv,Comma-separated values (CSV),text/csv,CSV");
            csv.Add(".doc,Microsoft Word,application/msword,MicrosoftWord");
            csv.Add(
                ".docx,Microsoft Word (OpenXML),application/vnd.openxmlformats-officedocument.wordprocessingml.document,MicrosoftWordOpenXML");
            csv.Add(".eot,MS Embedded OpenType fonts,application/vnd.ms-fontobject,MSEmbeddedOpenTypeFonts");
            csv.Add(".epub,Electronic publication (EPUB),application/epub+zip,EPUB");
            csv.Add(".es,ECMAScript (IANA Specification) (RFC 4329 Section 8.2),application/ecmascript,ECMAScript");
            csv.Add(".gif,Graphics Interchange Format (GIF),image/gif,GIF");
            csv.Add(".htm;.html,HyperText Markup Language (HTML),text/html,HTML");
            csv.Add(".ico,Icon format,image/x-icon,Icon");
            csv.Add(".ics,iCalendar format,text/calendar,iCalendar");
            csv.Add(".jar,Java Archive (JAR),application/java-archive,JAR");
            csv.Add(".jpeg;.jpg,JPEG images,image/jpeg,JPEG");
            csv.Add(".js,JavaScript (IANA Specification) (RFC 4329 Section 8.2),application/javascript,JavaScript");
            csv.Add(".json,JSON format,application/json,JSON");
            csv.Add(".mid;.midi,Musical Instrument Digital Interface (MIDI),audio/midi audio/x-midi,MIDI");
            csv.Add(".mpeg,MPEG Video,video/mpeg,MPEG");
            csv.Add(".mpkg,Apple Installer Package,application/vnd.apple.installer+xml,AppleInstallerPackage");
            csv.Add(
                ".odp,OpenDocument presentation document,application/vnd.oasis.opendocument.presentation,OpenDocumentPresentation");
            csv.Add(
                ".ods,OpenDocument spreadsheet document,application/vnd.oasis.opendocument.spreadsheet,OpenDocumentSpreadsheet");
            csv.Add(".odt,OpenDocument text document,application/vnd.oasis.opendocument.text,OpenDocumentText");
            csv.Add(".oga,OGG audio,audio/ogg,OGGAudio");
            csv.Add(".ogv,OGG video,video/ogg,OGGVideo");
            csv.Add(".ogx,OGG,application/ogg,OGGApplication");
            csv.Add(".otf,OpenType font,font/otf,OpenTypeFont");
            csv.Add(".png,Portable Network Graphics,image/png,PNG");
            csv.Add(".pdf,Adobe Portable Document Format (PDF),application/pdf,PDF");
            csv.Add(".ppt,Microsoft PowerPoint,application/vnd.ms-powerpoint,MicrosoftPowerPoint");
            csv.Add(
                ".pptx,Microsoft PowerPoint (OpenXML),application/vnd.openxmlformats-officedocument.presentationml.presentation,MicrosoftPowerPointOpenXML");
            csv.Add(".rar,RAR archive,application/x-rar-compressed,RAR");
            csv.Add(".rtf,Rich Text Format (RTF),application/rtf,RTF");
            csv.Add(".sh,Bourne shell script,application/x-sh,BourneShellScript");
            csv.Add(".svg,Scalable Vector Graphics (SVG),image/svg+xml,SVG");
            csv.Add(".swf,Small web format (SWF) or Adobe Flash document,application/x-shockwave-flash,SWF");
            csv.Add(".tar,Tape Archive (TAR),application/x-tar,TAR");
            csv.Add(".tif;.tiff,Tagged Image File Format (TIFF),image/tiff,TIFF");
            csv.Add(".ts,Typescript file,application/typescript,Typescript");
            csv.Add(".ttf,TrueType Font,font/ttf,TrueTypeFont");
            csv.Add(".txt,Text, (generally ASCII or ISO 8859-n),text/plain,Text");
            csv.Add(".vsd,Microsoft Visio,application/vnd.visio,MicrosoftVisio");
            csv.Add(".wav,Waveform Audio Format,audio/wav,WAV");
            csv.Add(".weba,WEBM audio,audio/webm,WEBMAudio");
            csv.Add(".webm,WEBM video,video/webm,WEBMVideo");
            csv.Add(".webp,WEBP image,image/webp,WEBPImage");
            csv.Add(".woff,Web Open Font Format (WOFF),font/woff,WOFF");
            csv.Add(".woff2,Web Open Font Format (WOFF),font/woff2,WOFF2");
            csv.Add(".xhtml,XHTML,application/xhtml+xml,XHTML");
            csv.Add(".xls,Microsoft Excel,application/vnd.ms-excel,MicrosoftExcel");
            csv.Add(
                ".xlsx,Microsoft Excel (OpenXML),application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,MicrosoftExcelOpenXML");
            csv.Add(".xml,XML,application/xml,XML");
            csv.Add(".xul,XUL,application/vnd.mozilla.xul+xml,XUL");
            csv.Add(".zip,ZIP archive,application/zip,ZIP");
            csv.Add(".3gp,3GPP audio/video container (video),video/3gpp,Video3GPP");
            csv.Add(".3gp,3GPP audio/video container (audio),audio/3gpp,Audio3GPP");
            csv.Add(".3g2,3GPP2 audio/video container (video),video/3gpp2,Video3GPP2");
            csv.Add(".3g2,3GPP2 audio/video container (audio),audio/3gpp2,Audio3GPP2");
            csv.Add(".7z,7-zip archive,application/x-7z-compressed,Archive7Zip");

            return csv;
        }

        #endregion

        /// <summary>
        /// List of All Known MIME Types
        /// </summary>
        /// <returns></returns>
        public static List<MimeType> AllMimeTypesList()
        {
            //TODO: Create version which loads a json or xml config file?

            var mimes = new List<MimeType>();

            var csv = MimeCsv();
            foreach (var item in csv)
            {
                var data = item.Split(',');

                var extensions = data[0].Split(';');
                var display = data[1];
                var mimetext = data[2];
                var type = data[3];

                mimes.Add(new MimeType()
                {
                    DisplayName = display,
                    Extensions = extensions,
                    Type = GetEnumFromString(type),
                    TypeString = mimetext
                });
            }

            return mimes;
        }

        /// <summary>
        /// Return a MimeType using its Enum Type 
        /// </summary>
        /// <param name="Type">MimeType.MimeTypeName</param>
        /// <returns></returns>
        public static MimeType GetMimeByType(MimeType.MimeTypeName Type)
        {
            var allTypes = AllMimeTypesList();

            var matches = allTypes.Where(n => n.Type == Type);

            if (matches.Any())
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return a MimeType using its file extension 
        /// </summary>
        /// <returns></returns>
        public static MimeType GetMimeByExtension(string FileExt)
        {
            var ext = FileExt.StartsWith(".") ? FileExt : "." + FileExt;

            var allTypes = AllMimeTypesList();

            var matches = allTypes.Where(n => n.Extensions.Contains(ext));

            if (matches.Any())
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }

        private static MimeType.MimeTypeName GetEnumFromString(string TypeString)
        {
            try
            {
                MimeType.MimeTypeName enumValue =
                    (MimeType.MimeTypeName)Enum.Parse(typeof(MimeType.MimeTypeName), TypeString);

                if (Enum.IsDefined(typeof(MimeType.MimeTypeName), enumValue) | enumValue.ToString().Contains(","))
                {
                    return enumValue;
                }
                else
                {
                    return MimeType.MimeTypeName.Other;
                }
            }
            catch
            {
                return MimeType.MimeTypeName.Other;
            }
        }


    }
}
