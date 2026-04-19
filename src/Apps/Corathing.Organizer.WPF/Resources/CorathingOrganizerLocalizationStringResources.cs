using System.Resources;

namespace Corathing.Organizer.WPF.Resources;

/// <summary>
/// Localized string resource accessor for the Organizer WPF app.
/// Keys live in <c>CorathingOrganizerLocalizationStringResources.resx</c> (+ locale variants);
/// access them through <see cref="ResourceManager"/>.
/// </summary>
internal static class CorathingOrganizerLocalizationStringResources
{
    public static ResourceManager ResourceManager { get; } = new ResourceManager(
        $"{typeof(CorathingOrganizerLocalizationStringResources).Namespace}.{nameof(CorathingOrganizerLocalizationStringResources)}",
        typeof(CorathingOrganizerLocalizationStringResources).Assembly);
}
