namespace RDM.DataTransferObjects.ImageViewerAPI
{
    public class GeneralDocumentDto
    {
        public string DocumentName { get; set; }

        public GeneralDocumentPageDto[] Pages { get; set; } = new GeneralDocumentPageDto[] { };
    }
}
