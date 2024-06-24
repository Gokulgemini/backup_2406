using System.Threading.Tasks;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Services.ImageViewerAPI
{
    public interface IImageConverter
    {
        Task<Result<Error, Image>> ConvertImageToPng(Image originalImage);
    }
}
