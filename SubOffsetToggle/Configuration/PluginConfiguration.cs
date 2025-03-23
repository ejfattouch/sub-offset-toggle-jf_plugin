using MediaBrowser.Model.Plugins;

namespace SubOffsetToggle.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        Offset1 = 0;
        Offset2 = 0;
        IndexDir = "/usr/share/jellyfin/web/";
    }

    /// <summary>
    /// Gets or sets the first offset.
    /// </summary>
    public float Offset1 { get; set; }

    /// <summary>
    /// Gets or sets the second offset.
    /// </summary>
    public float Offset2 { get; set; }

    /// <summary>
    /// Gets or sets the directory where index.html is stored.
    /// </summary>
    public string IndexDir { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the web directory is invalid.
    /// </summary>
    public bool InvalidDirectory { get; set; } = false;
}
