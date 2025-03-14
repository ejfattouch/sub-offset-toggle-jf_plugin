using System.IO;

namespace SubOffsetToggle.Templating
{
    /// <summary>
    /// Handles file templating.
    /// </summary>
    public class JSInjectionTemplater : Templater
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSInjectionTemplater"/> class.
        /// Requires an inject_template.js file present in the templates subdirectory of this class.
        /// </summary>
        /// <param name="output">Output file.</param>
        public JSInjectionTemplater(FileInfo output) : base(new FileInfo("templates/inject_template.js"), output)
        {
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

            ProcessTemplate(data);
        }
    }
}
