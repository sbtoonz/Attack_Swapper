using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AttackSwapper
{
    public static class Utilities
    {
	    internal enum ConnectionState
	    {
		    Server,
		    Client,
		    Local,
		    Unknown
	    }
	    internal static ConnectionState GetConnectionState()
	    {
			if (ZNet.instance == null) return ConnectionState.Local;
		    if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated()) //server
		    {
			    return ConnectionState.Server;
		    }
		    
		    if (ZNet.m_isServer && ZNet.m_openServer) // Local server
		    {
			    return ConnectionState.Server;
		    }
		    if (!ZNet.instance.IsServer() && !ZNet.instance.IsDedicated()) //client
		    {
			    return ConnectionState.Client;
		    }

		    if (ZNet.IsSinglePlayer) 
		    {
			    return ConnectionState.Local;
		    }

		    return ConnectionState.Unknown;
	    }
        internal static AssetBundle? LoadAssetBundle(string bundleName)
        {
            var resource = typeof(AttackSwapperMod).Assembly.GetManifestResourceNames().Single
                (s => s.EndsWith(bundleName));
            using var stream = typeof(AttackSwapperMod).Assembly.GetManifestResourceStream(resource);
            return AssetBundle.LoadFromStream(stream);
        }

        internal static void LoadAssets(AssetBundle? bundle, ZNetScene zNetScene)
        {
            var tmp = bundle?.LoadAllAssets();
            if (zNetScene.m_prefabs.Count <= 0) return;
            if (tmp == null) return;
            foreach (var o in tmp)
            {
                var obj = (GameObject)o;
                zNetScene.m_prefabs.Add(obj);
                var hashcode = obj.GetHashCode();
                zNetScene.m_namedPrefabs.Add(hashcode, obj);
            }
        }
        
        internal static RuntimeAnimatorController MakeAoc(RuntimeAnimatorController ORIGINAL, Dictionary<string, string> replacements , Dictionary<string, AnimationClip> ExternalAnimations)
        {
	        AnimatorOverrideController aoc = new AnimatorOverrideController(ORIGINAL);
	        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
	        foreach (var animation in aoc.animationClips)
	        {
		        string name = animation.name;
		        if (replacements.TryGetValue(name, out var replacement))
		        {
			        AnimationClip newClip = MonoBehaviour.Instantiate<AnimationClip>(ExternalAnimations[replacement]);
			        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, newClip));
		        }
		        else
		        {
			        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, animation));
		        }
	        }
	        aoc.ApplyOverrides(anims);
	        return aoc;
        }
    }
}