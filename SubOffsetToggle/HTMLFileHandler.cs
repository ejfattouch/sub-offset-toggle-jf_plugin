using System;
using System.IO;
using HtmlAgilityPack;

namespace SubOffsetToggle;

/// <summary>
/// Class that helps with handling the index.html file.
/// </summary>
public class HtmlFileHandler
{
    private const string ScriptID = "-offset_btn_injection";
    private FileInfo indexFile;
    private HtmlDocument htmlDoc;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlFileHandler"/> class.
    /// </summary>
    /// <param name="dir">Web directory root, index.html should be located there.</param>
    public HtmlFileHandler(DirectoryInfo dir)
    {
        indexFile = new FileInfo(Path.Combine(dir.FullName, "index.html"));

        if (!indexFile.Exists)
        {
            throw new FileNotFoundException("index.html not found in the specified directory.");
        }

        // Load the HTML for modification
        htmlDoc = new HtmlDocument();
        htmlDoc.Load(indexFile.FullName);
    }

    /// <summary>
    /// Check for the existence of the script tag.
    /// </summary>
    /// <returns>If the script tag has been added to the index.html file.</returns>
    public bool ScriptExists()
    {
        return htmlDoc.DocumentNode.SelectSingleNode($"//*[@id='{ScriptID}']") != null;
    }

    /// <summary>
    /// Append the script element to the end of the head tag.
    /// </summary>
    public void AppendScriptToHead()
    {
        if (ScriptExists())
        {
            Console.WriteLine("Warning: Script was already added to index.html, skipping");
            return;
        }

        var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");

        if (headNode == null)
        {
            return;
        }

        string scriptContent = @"
            <script defer id=""-offset_btn_injection"">
                var script = document.createElement(""script"");
                console.log(script);
                script.src = ""injection/offset_btn_injection.js?v="" + new Date().getTime(); // Add timestamp to prevent caching
                document.head.appendChild(script);
            </script>";

        HtmlNode scriptNode = HtmlNode.CreateNode(scriptContent);
        headNode.AppendChild(scriptNode);
    }

    /// <summary>
    /// Removes the custom script element from the end of the head tag.
    /// </summary>
    public void RemoveScript()
    {
        if (ScriptExists())
        {
            var scriptNode = htmlDoc.DocumentNode.SelectSingleNode($"//*[@id='{ScriptID}']");

            scriptNode.Remove();

            Console.WriteLine("Script was removed");
        }
        else
        {
            Console.WriteLine("Script did not exist, nothing to remove");
        }
    }

    /// <summary>
    /// Saves the script to index.html.
    /// </summary>
    public void SaveChanges()
    {
        htmlDoc.Save(indexFile.FullName);
    }
}