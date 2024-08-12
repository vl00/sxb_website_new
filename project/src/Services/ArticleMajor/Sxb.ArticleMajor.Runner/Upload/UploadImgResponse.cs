namespace Sxb.ArticleMajor.Runner
{
    /// <summary>
    /// from iSchool.API
    /// </summary>
    public class UploadImgResponse
    {
        public bool successs => status == 0;

        public int status
        {
            get;
            set;
        }

        public string cdnUrl
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string ErrorDescription
        {
            get;
            set;
        }
    }
}
