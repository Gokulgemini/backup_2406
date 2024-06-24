namespace RDM.DataTransferObjects.ImageViewerAPI
{
    public class RemittanceDto
    {
        public bool IsVirtual { get; set; }

        public ImageDto FrontImage { get; set; } = new ImageDto();

        public ImageDto BackImage { get; set; } = new ImageDto();
    }
}
