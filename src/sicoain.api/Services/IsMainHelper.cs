using sicoain.shared.Interfaces;

namespace sicoain.api.Services;

public static class IsMainHelper
{
    /// <summary>
    /// Ensures exactly one item is marked IsMain in the list.
    /// Rules:
    ///   - If items.Count == 0 → return
    ///   - If items.Count == 1 → items[0].IsMain = true
    ///   - If none marked → items[0].IsMain = true
    ///   - If multiple marked → first one marked keeps true, others set to false
    ///   - If exactly one marked → leave as is
    /// </summary>
    public static void EnsureSingleMain<T>(List<T> items) where T : IHasIsMain
    {
        if (items.Count == 0) return;
        if (items.Count == 1) { items[0].IsMain = true; return; }

        var firstMain = items.FindIndex(i => i.IsMain);
        if (firstMain == -1)
        {
            items[0].IsMain = true;
        }
        else
        {
            for (int i = firstMain + 1; i < items.Count; i++)
                items[i].IsMain = false;
        }
    }
}
