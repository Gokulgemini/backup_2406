namespace RDM.Imaging
{
    /// <summary>
    /// An implementation of IImageFactory that creates RdmImage instances
    /// to wrap an ImageMagick image object. 
    /// </summary>
    public class RdmImageFactory : IImageFactory
    {
        public RdmImageFactory()
        {
        }

        public IImage CreateImage(byte[] rawImageBytes)
        {
            return new RdmImage(rawImageBytes);            
        }

        public IImage CreateImage(string filePath)
        {
            return new RdmImage(filePath);
        }
    }
}
