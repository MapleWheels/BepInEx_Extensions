namespace BepInEx.Extensions
{
    [BepInPlugin(LibraryInfo.BepInDependencyID, "BepInEx.Extensions BE_Loader_Plugin", LibraryInfo.PluginVersion)]
    public class BELibraryLoader : BaseUnityPlugin
    { }

    public static class LibraryInfo
    {
        public const string BepInDependencyID = "com.bepinex.extensions.plugin";
        public const string PluginVersion = "2.3.1";
    }
}
