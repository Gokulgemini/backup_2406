namespace RDM.DataTransferObjects.ImageViewerAPI
{
    public class ChequeDto
    {
        public ImageDto FrontImage { get; set; } = new ImageDto();

        public ImageDto BackImage { get; set; } = new ImageDto();
    }
}
