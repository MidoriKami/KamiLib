using System.Collections.Generic;
using System.Threading.Tasks;

namespace KamiLib.Classes;

public abstract class Cache<T, TU> where T : notnull {
    private readonly Dictionary<T, TU?> internalCache = [];

    public TU? GetValue(T key) {
        // Do we already know this key? If so return value.
        if (internalCache.TryGetValue(key, out var result)) {
            return result;
        }
        
        // If we don't know this key,
        else {
            
            // Add a default placeholder value
            if (internalCache.TryAdd(key, default)) {
                
                // And schedule the async loading of the corresponding value
                Task.Run(() => internalCache[key] = LoadValue(key));
                
                // And likely return the default we set above, but if the value is loaded superfast, it might be the real value.
                return internalCache[key];
            }
            
            // Else, we couldn't add a default placeholder value to this key, and we have never seen that key before.
            // This shouldn't be possible, but just in case, we'll give you a default value. Probably null.
            else {
                return default;
            }
        }
    }

    protected abstract TU? LoadValue(T key);
}