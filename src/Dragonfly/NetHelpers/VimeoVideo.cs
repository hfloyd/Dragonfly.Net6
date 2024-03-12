namespace Dragonfly.NetHelpers
{
    using System;
    using System.Linq;

    public static class VimeoVideo
    {
        public static bool TryGetVimeoThumbnail(string VimeoVideoUrl, out string VimeoImgUrl)
        {
            var errorMsg = "";

            var result = TryGetVimeoThumbnail(VimeoVideoUrl,out VimeoImgUrl,out errorMsg);

            return result;
        }

        public static bool TryGetVimeoThumbnail(string VimeoVideoUrl, out string VimeoImgUrl, out string ErrorMessage)
        {
            var imgUrl = "";
            var success = false;
            ErrorMessage = "";

            if (VimeoVideoUrl.Contains("player.vimeo.com/video/"))
            {
                var vimUri = new Uri(VimeoVideoUrl);
                var vimeoId = vimUri.Segments.Last();

                imgUrl = GenerateVimeoThumbUrl(vimeoId);
                success = true;
            }
            else
            {
                ErrorMessage = string.Format("TryGetVimeoThumbnail : Unable to Parse Vimeo Url '{0}' to retrieve VideoId", VimeoVideoUrl);
            }

            VimeoImgUrl = imgUrl;
            return success;
        }

        public static string GenerateVimeoThumbUrl(string VimeoVideoId)
        {
            const string vimeoThumbFormat = "https://i.vimeocdn.com/video/{0}_639x360.webp"; //{0}= vimeo video id
            return string.Format(vimeoThumbFormat, VimeoVideoId);
        }
    }
}
