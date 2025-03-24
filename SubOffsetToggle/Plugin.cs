using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using SubOffsetToggle.Configuration;
using SubOffsetToggle.Templating;

namespace SubOffsetToggle;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ILogger<Plugin> _logger;
    private HtmlFileHandler? _fileHandler;
    private DirectoryInfo indexDirectory;
    private float _offset1;
    private float _offset2;

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{Plugin}"/> interface, used for logging messages related to this plugin.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        _logger = logger;
        _logger.LogInformation("Sub Offset Toggle Plugin has been initialized.");

        Instance = this;

        indexDirectory = new DirectoryInfo("/usr/share/jellyfin/web/");

        _offset1 = _offset2 = 0;

        try
        {
            _fileHandler = new HtmlFileHandler(indexDirectory);
            UpdateIndexHTML();
        }
        catch (FileNotFoundException)
        {
            _fileHandler = null;
        }

        ConfigurationChanged += OnConfigurationChanged;

        _logger.LogInformation("Tried to create dir");
    }

    /// <inheritdoc />
    public override string Name => "Subtitle Offset Toggle";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("22d7e486-c0e3-4849-98ed-a9e64601cbda");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <summary>
    /// Handles the event when the plugin configuration is changed.
    /// This method is triggered whenever the user updates and saves the plugin settings.
    /// </summary>
    private void OnConfigurationChanged(object? sender, BasePluginConfiguration config)
    {
        _logger.LogInformation("OnConfigurationChanged has been called");

        if (config is not PluginConfiguration)
        {
            return;
        }

        var pluginConfiguration = (PluginConfiguration)config;

        DirectoryInfo newDir = new DirectoryInfo(pluginConfiguration.IndexDir);
        // Check if index dir has changed
        if (!indexDirectory.FullName.Equals(pluginConfiguration.IndexDir, StringComparison.Ordinal))
        {
            try
            {
                var newHandler = new HtmlFileHandler(newDir); // Exception would throw here
                pluginConfiguration.InvalidDirectory = false;

                // Cleanup old dir, run before changing filehandler and indexDir
                RemoveIndexAndInject();

                _fileHandler = newHandler;
                indexDirectory = newDir;
                RenderJSInjectionTemplate(pluginConfiguration);
                UpdateIndexHTML();
            }
            catch (FileNotFoundException) // If index.html doesnt exist at new directory, reset the plugin conf to the previous one.
            {
                // Reset to old values
                pluginConfiguration.IndexDir = indexDirectory.FullName;
                pluginConfiguration.Offset1 = _offset1;
                pluginConfiguration.Offset2 = _offset2;
                pluginConfiguration.InvalidDirectory = true;
                _logger.LogInformation("Sub Offset Plugin: Invalid directory for web server {NewDir}.", newDir.FullName);
            }
        }
        else if (_fileHandler == null) // If dir hasnt changed and filehandler is still null
        {
            try
            {
                _fileHandler = new HtmlFileHandler(indexDirectory); // Exception would throw here
                pluginConfiguration.InvalidDirectory = false;
                UpdateIndexHTML();
                RenderJSInjectionTemplate(pluginConfiguration);
            }
            catch (FileNotFoundException) // If index.html doesnt exist at new directory, reset the plugin conf to the previous one.
            {
                // Reset to old values
                pluginConfiguration.InvalidDirectory = true;
                _logger.LogInformation("Sub Offset Plugin: Invalid directory for web server {NewDir}.", newDir.FullName);
            }
        }
        else // If dir hasnt changed
        {
            UpdateIndexHTML();
            RenderJSInjectionTemplate(pluginConfiguration);
            pluginConfiguration.InvalidDirectory = false;
        }

        // Update stored offsets
        _offset1 = pluginConfiguration.Offset1;
        _offset2 = pluginConfiguration.Offset2;

        SaveConfiguration(); // SaveConfig doesnt trigger infinite recursive loop, updateconfig does
    }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html"
            }
        ];
    }

    private void RenderJSInjectionTemplate(PluginConfiguration config)
    {
        _logger.LogInformation("RenderJSInjectionTemplate method has been called");

        JSInjectionTemplater templater = new JSInjectionTemplater(indexDirectory, _logger);
        templater.ProcessInjectedJS(config.Offset1, config.Offset2);
    }

    private void UpdateIndexHTML()
    {
        if (_fileHandler is null)
        {
            return;
        }

        // Check if script tag exists
        if (!_fileHandler.ScriptExists())
        {
            _fileHandler.AppendScriptToHead();
            _fileHandler.SaveChanges();
        }
    }

    private void RemoveIndexAndInject()
    {
        // Undo changes to index.html
        if (_fileHandler != null)
        {
            _fileHandler.RemoveScript();
            _fileHandler.SaveChanges();
        }

        try
        {
            // Remove injection dir
            var dir = new DirectoryInfo(Path.Combine(indexDirectory.FullName, "injection/"));
            if (dir.Exists)
            {
                foreach (var file in dir.EnumerateFiles())
                {
                    file.Delete();
                }

                dir.Delete();
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError("Permission denied while deleting script: {Message}", ex.Message);
        }
        catch (IOException ex)
        {
            _logger.LogError("I/O error while deleting script: {Message}", ex.Message);
        }
    }

    /// <inheritdoc />
    public override void OnUninstalling()
    {
        RemoveIndexAndInject();
    }
}
