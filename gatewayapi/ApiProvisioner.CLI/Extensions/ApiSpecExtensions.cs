using System;
namespace ApiProvisioner.CLI.Extensions
{
    public static class ApiSpecExtensions
    {
        public static string Read(string folder, string fileName)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, folder, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("OpenApi spec file not found. Make sure it was marked as [Copy always] in the Visual Studio file properties", fileName);

            return File.ReadAllText(filePath);
        }
    }
}

