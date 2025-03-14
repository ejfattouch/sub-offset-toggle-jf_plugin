using System;
using System.IO;
using Scriban;

namespace SubOffsetToggle.Templating
{
    /// <summary>
    /// Handles file templating.
    /// </summary>
    public class Templater
    {
        /// <summary>
        /// Input template file, this file will be parsed.
        /// </summary>
        private readonly FileInfo inputFile;

        /// <summary>
        /// Output template file, this file will be generated once the templater is done.
        /// </summary>
        private readonly FileInfo outputFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Templater"/> class.
        /// </summary>
        /// <param name="input">Input template file.</param>
        /// <param name="output">Output file.</param>
        public Templater(FileInfo input, FileInfo output)
        {
            inputFile = input ?? throw new ArgumentNullException(nameof(input));
            outputFile = output ?? throw new ArgumentNullException(nameof(output));
        }

        /// <summary>
        /// Processes the template and saves the rendered output.
        /// </summary>
        /// <param name="data">Data model for templating.</param>
        public void ProcessTemplate(object data)
        {
            if (!inputFile.Exists)
            {
                throw new FileNotFoundException("Template file not found", inputFile.FullName);
            }

            // Read the template content
            string templateContent = File.ReadAllText(inputFile.FullName);

            // Parse the template
            var template = Template.Parse(templateContent);

            // Render the template with the provided data
            string result = template.Render(data);

            // Write to output file
            File.WriteAllText(outputFile.FullName, result);

            Console.WriteLine($"Template processed and saved to {outputFile.FullName}");
        }
    }
}
