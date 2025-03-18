using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace SubOffsetToggle.Templating
{
    /// <summary>
    /// Handles templating the JavaScript code that will be injected in the Media Playback.
    /// </summary>
    public class JSInjectionTemplater
    {
        private readonly ILogger<Plugin> _logger;
        private readonly Templater _templater;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSInjectionTemplater"/> class.
        /// Requires an inject_template.js file present in the templates subdirectory of this class.
        /// </summary>
        /// <param name="output">Output file.</param>
        /// <param name="logger">Logger passed on from Plugin.</param>
        public JSInjectionTemplater(FileInfo output, ILogger<Plugin> logger)
        {
            _logger = logger;
            _logger.LogInformation("JSInjectionTemplater Constructor");

            string templateContent = ExtractTemplate();
            SaveTemplateToFile(templateContent);

            FileInfo templatePath = new FileInfo("/tmp/templates/inject_template.js");
            if (!templatePath.Exists)
            {
                throw new FileNotFoundException("/tmp/templates/inject_template.js was not supplied");
            }

            _templater = new Templater(templatePath, output);
        }

        /// <summary>
        /// Extracts the template file from the Embedded resources.
        /// </summary>
        /// <returns>String representation of the template file.</returns>
        public static string ExtractTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Define the embedded resource name (namespace + folder structure + file name)
            string resourceName = "SubOffsetToggle.Templating.templates.inject_template.js";

            // Extract the resource to a string
            using (Stream? resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException("Embedded resource not found", resourceName);
                }

                // Read the stream into a string (assuming it's text)
                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Processes the template and saves the rendered output.
        /// </summary>
        /// <param name="templateContent">String value of the contents of the template file.</param>
        public void SaveTemplateToFile(string templateContent)
        {
            try
            {
                // Define the output file path
                string outputPath = "/tmp/templates/inject_template.js";

                // Ensure the directory exists
                string? directory = Path.GetDirectoryName(outputPath);

                if (directory is null)
                {
                    throw new DirectoryNotFoundException("Directory could not be found from the string");
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);  // Create the directory if it doesn't exist
                }

                // Write the template content to the file
                File.WriteAllText(outputPath, templateContent);

                _logger.LogInformation("Template saved to {OutputPath}", outputPath);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error saving template: {Error}", ex.Message);
            }
        }

        /// <summary>
        /// Processes the template and saves the rendered output.
        /// </summary>
        /// <param name="offset1">Value of the first offset in seconds.</param>
        /// <param name="offset2">Value of the second offset in seconds.</param>
        public void ProcessInjectedJS(float offset1, float offset2)
        {
            object data = new
            {
                offset_value_1 = offset1,
                offset_value_2 = offset2
            };

            _templater.ProcessTemplate(data);
        }
    }
}
