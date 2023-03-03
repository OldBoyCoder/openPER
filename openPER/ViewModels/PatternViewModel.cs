namespace openPER.ViewModels
{
    public class PatternViewModel
    {
        public string FullCode { get; set; }
        public string TypeDescription { get; set; }
        public string CodeDescription { get; set; }
        public string FullDescription
        {
            get
            {
                var rc = $"{FullCode} - {TypeDescription} {CodeDescription}";
                return rc;
            }
        }

    }
}
