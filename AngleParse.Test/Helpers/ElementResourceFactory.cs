using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AngleParse.Resource;

namespace AngleParse.Test.Helpers;

internal static class ElementResourceFactory
{
    public static async Task<ElementResource> CreateElementResourceAsync(
        string filename,
        [CallerFilePath] string rootPath = "",
        string assetsPath = "../../assets")
    {
        var path = Path.Combine(rootPath, assetsPath, filename);
        var content = await File.ReadAllTextAsync(path);
        return await ElementResource.CreateAsync(content);
    }
}