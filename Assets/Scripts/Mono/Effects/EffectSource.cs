using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Source", menuName = "Scriptable Objects/Effect Source")]
public class EffectSource : ScriptableObject
{
    [SerializeField] private string id;   // Unique identifier (must be unique across all sources)
    public string Id => id;

    // Static registry of all effect sources
    private static Dictionary<string, EffectSource> registry = new();

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning($"EffectSource {name} has empty Id!");
            return;
        }

        // Ensure uniqueness
        if (!registry.ContainsKey(id))
        {
            registry.Add(id, this);
        }
        else
        {
            Debug.LogWarning($"Duplicate EffectSource Id detected: {id}. Ignoring duplicate.");
        }
    }

    /// <summary>
    /// Get an EffectSource by Id string.
    /// </summary>
    public static EffectSource Get(string id)
    {
        if (registry.TryGetValue(id, out var source))
            return source;

        Debug.LogError($"EffectSource with Id '{id}' not found! Did you forget to create the asset?");
        return null;
    }

    /// <summary>
    /// Expose all registered sources (for debugging or UI).
    /// </summary>
    public static IEnumerable<EffectSource> GetAllSources() => registry.Values;
}
