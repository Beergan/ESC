using System.IO.Compression;

namespace ESC.CONCOST.WebApp;

public class ZipUtil
{
    public static Dictionary<string, byte[]> GetFiles(byte[] zippedFile) 
    {
        using (MemoryStream ms = new MemoryStream(zippedFile))
        using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Read)) 
        {
            return archive.Entries.ToDictionary(x => x.FullName, x => ReadStream(x.Open()));
        }
    }

    private static byte[] ReadStream(Stream stream) 
    {
        using (var ms = new MemoryStream()) 
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}