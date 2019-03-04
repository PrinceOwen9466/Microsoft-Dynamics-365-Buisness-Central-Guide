using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Extensions
{
    public static class ResourceHelper
    {
        #region Properties

        #region Internals
        static List<string> LocalResources { get; } = new List<string>(); 
        #endregion

        #endregion

        #region Methods
        public static string GetEmbeddedResource(string resourceName, Assembly assembly)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    DisplayResourceNames(assembly);
                    return null;
                }

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static Stream GetEmbeddedResourceStream(string resourceName, Assembly assembly)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            
            Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceName == null) DisplayResourceNames(assembly);
            return resourceStream;
        }

        public static string CreateLocalInstance(string resourceName, Assembly assembly, string extension="")
        {
            resourceName = FormatResourceName(assembly, resourceName);
            int read = -1;
            byte[] buffer = new byte[4096];

            if (string.IsNullOrWhiteSpace(extension))
                extension = Path.GetExtension(resourceName);

            string outputPath = Path.Combine(Core.TEMP_DIR, Guid.NewGuid().ToString()) + extension;

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    output.Write(buffer, 0, read);

            File.SetAttributes(outputPath, FileAttributes.Hidden);
            LocalResources.Add(outputPath);
            return outputPath;
        }

        public static void DissolveLocalInstance(string path)
        {
            if (!LocalResources.Contains(path)) return;

            LocalResources.Remove(path);
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Core.Log.Debug($"An error occured while disolving ({path})\n{ex}");
            }
        }

        /*
        public static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return $@"pack://application:,,,/{assembly.GetName().Name};component/{resourceName}";
        }
        */

        static void DisplayResourceNames(Assembly assembly)
        {
            Core.Log.Debug("Here is a list of all the current embedded resources.");
            Core.Log.Debug("");
            foreach (var resource in assembly.GetManifestResourceNames())
                Core.Log.Debug(resource);
        }

        private static string FormatResourceName(Assembly assembly, string resourceName) => assembly.GetName().Name + "." + resourceName.Replace("\\", ".")
                                                               .Replace("/", ".");

        #endregion
    }
}
