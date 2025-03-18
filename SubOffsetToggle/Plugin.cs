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

        ConfigurationChanged += OnConfigurationChanged;
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

        if (config is PluginConfiguration pluginConfiguration)
        {
            RenderJSInjectionTemplate(pluginConfiguration);
        }
    }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        ];
    }

    private void RenderJSInjectionTemplate(PluginConfiguration config)
    {
        _logger.LogInformation("RenderJSInjectionTemplate method has been called");
        string web_dir = "/usr/share/jellyfin/web/";
        string inject_dir = web_dir + "injection/";

        FileInfo output = new FileInfo(inject_dir + "offset_btn_injection.js");
        JSInjectionTemplater templater = new JSInjectionTemplater(output, _logger);
        templater.ProcessInjectedJS(config.Offset1, config.Offset2);
    }
}
