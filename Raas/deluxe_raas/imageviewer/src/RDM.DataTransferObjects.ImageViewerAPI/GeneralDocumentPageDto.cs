namespace RDM.DataTransferObjects.ImageViewerAPI
{
    public class GeneralDocumentPageDto
    {
        public int PageNumber { get; set; }

        public ImageDto FrontImage { get; set; } = new ImageDto();

        public ImageDto BackImage { get; set; } = new ImageDto();
    }
}
