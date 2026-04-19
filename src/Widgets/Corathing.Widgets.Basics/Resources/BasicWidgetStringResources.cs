using System.Resources;

namespace Corathing.Widgets.Basics.Resources;

/// <summary>
/// Localized string resource accessor for the Basics widget package.
/// Keys live in <c>BasicWidgetStringResources.resx</c> (+ locale variants);
/// access them through <see cref="ResourceManager"/>.
/// </summary>
internal static class BasicWidgetStringResources
{
    public static ResourceManager ResourceManager { get; } = new ResourceManager(
        $"{typeof(BasicWidgetStringResources).Namespace}.{nameof(BasicWidgetStringResources)}",
        typeof(BasicWidgetStringResources).Assembly);
}
