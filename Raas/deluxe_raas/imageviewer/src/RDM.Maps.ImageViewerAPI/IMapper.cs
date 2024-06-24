namespace RDM.Maps.ImageViewerAPI
{
    public interface IMapper<TDomain, TDto>
    {
        TDto Map(TDomain domainObj);
    }
}
