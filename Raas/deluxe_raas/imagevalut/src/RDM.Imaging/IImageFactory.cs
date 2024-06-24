namespace RDM.Imaging
{
    public interface IImageFactory
    {
        IImage CreateImage(byte[] rawImageBytes);
        IImage CreateImage(string filePath);
    }
}
