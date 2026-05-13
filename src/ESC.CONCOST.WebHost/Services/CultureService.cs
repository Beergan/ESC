//using Microsoft.AspNetCore.Mvc;
//using ESC.CONCOST.Abstract;
//using System.Reflection;

//namespace ESC.CONCOST.WebHost;

//public class CultureService : ICultureService
//{
//    private readonly Assembly _assembly;
//    private readonly string _resourcesFolderName = "Resources";

//    private string GetFilePath(string cultureName)
//    {
//        var fileName = _assembly
//                            .GetManifestResourceNames()
//                            .SingleOrDefault(s =>
//                                                    s.Contains(_resourcesFolderName) &&
//                                                    (s.Contains($"{cultureName}.yml") ||
//                                                     s.Contains($"{cultureName}.yaml")));
//        return fileName;
//    }

//    public CultureService(Assembly assembly)
//    {
//        _assembly = assembly;
//    }

//    [HttpGet]
//    public async Task<string> LoadKeys(string cultureId)
//    {
//        if (string.IsNullOrWhiteSpace(cultureId))
//            throw new ArgumentNullException(nameof(cultureId));

//        try
//        {
//            var resourcesFileName = GetFilePath(cultureId);
//            using (var fileStream = _assembly.GetManifestResourceStream(resourcesFileName))
//            {
//                using (var streamReader = new StreamReader(fileStream))
//                {
//                    return streamReader.ReadToEnd();
//                }
//            }
//        }
//        catch (System.Exception)
//        {
//            return null;
//        }
//    }
//}