using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <author>PerfidousLeaf | MapleWheels</author>
/// <date>2020-08-13</date>
namespace BepInEx.Extensions.Configuration
{
    /// <summary>
    /// This class is designed to allow Attribute-based Configuration File definitions, similar to Entity Framework's DbContext files. WARNING: All members must be declared as Properties with "get"/"set". Fields will not be detected. Usage: Extend/Derive this class and define all of your ConfigEntry<T> variables as Properties (with get/set). Then just instantiate the class and all members will be auto-bound.
    /// 
    /// To us ethis class. Simply inherit it and declare ConfigEntry<> properties (add { get; set; }) along with description attributes. See ConfigModelTests for a more detailed example.
    /// </summary>
    public class ConfigFileModel
    {
        static ConfigFileModel()
        {
            //The generic version of ConfigFile.Bind<T>(). Only needs to be resolved once.
            CFM_GenericConfigFileBindMethod =
                typeof(ConfigFile)
                .GetMethods().FirstOrDefault(
                    m => m.GetParameters().Count() == 4
                    && m.GetParameters()[0].ParameterType == typeof(string)
                    && m.GetParameters()[1].ParameterType == typeof(string)
                    && m.GetParameters()[3].ParameterType == typeof(ConfigDescription)
                );

            //The generic version of ConfigFile.OrphanedEntries, used for ConfigReloaded events.
            ConfigFile_OrphanedEntriesDefinition = typeof(ConfigFile).GetProperty("OrphanedEntries", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Create and bind the Config Data Model to the supplied ConfigFile. To bind the model, call Bind() after initialization.
        /// </summary>
        /// <param name="file">The configuration file to bind to.</param>
        /// <param name="logger">The BepInEx Logger to be used. If ommitted, a default instance of the logger will be used that is shared by all ConfigFileModel instances.</param>
        /// <param name="sectionName">The section name that this model will appear under in the ConfigFile. The ConfigModelSectionName attribute will be used if this is set to null/ommitted.</param>
        public ConfigFileModel(ConfigFile file = null, string sectionName = null, ManualLogSource logger = null)
        {
            //--Generics definitions--//
            //The generic version of OperphanedPropertyPostConfigReloaded<T>()
            CFM_GenericOrphanedPropertyPostConfigReloadMethod = GetType().GetMethod(nameof(OrphanedPropertyPostConfigReload), BindingFlags.Instance );
            //The generic version of PrePropertyBind<T>()
            CFM_GenericPreBindMethod = GetType().GetMethod(nameof(PrePropertyBind), BindingFlags.Instance | BindingFlags.NonPublic);
            //The generic version of PostPropertyBind<T>()
            CFM_GenericPostBindMethod = GetType().GetMethod(nameof(PostPropertyBind), BindingFlags.Instance | BindingFlags.NonPublic);
            //The model's ConfigEntry<> properties. Use Reflection to get all of the Property Members.
            CFM_ConfigFileEntryProperties = GetType().GetProperties().Where(
                prop => prop.PropertyType.IsGenericType == true &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(ConfigEntry<>)
                ).ToArray();

            if (_StaticLogger == null)
                _StaticLogger = BepInEx.Logging.Logger.CreateLogSource("ConfigFileModel_DefaultLogger");

            if (logger != null)
                Logger = logger;

            if (sectionName != null)
                SectionName = sectionName;

            if (file != null)
                CurrentFile = file; //Initialization done here.
        }


        /// <summary>
        /// Internal use only! Uses reflection to parse through all ConfigEntry<> vars in inheriting data model classes and bind them to their associated configuration entries in the BepInEx ConfigFile. Intended to allow Attribute-Based config file design similar to Entity Framework's DbContext.
        /// </summary>
        /// <param name="file">The target config file to bind to.</param>
        /// <param name="sectionName">The section name in the config file this Data model will be placed under.</param>
        private void InitAndBindConfigs(ConfigFile file, string sectionName)
        {            
            foreach (PropertyInfo property in CFM_ConfigFileEntryProperties)
            {
                if (property.PropertyType.IsGenericType == true &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(ConfigEntry<>))
                {
                    try
                    {
                        //Get T value from property
                        Type configEntryInstanceType = property.PropertyType.GetGenericArguments()[0];
                        Logger.LogInfo($"{property.Name}: GenericType Args= {configEntryInstanceType.Name}");
                        //Get the default description and warn/log and set defaults on failure. 
                        string descriptionString = null;
                        object defaultValueRaw = null;
                        object defaultValue = null;
                        string key = null;

                        object[] rawAttributeBuffer = null;

                        //Get config entry's attributes
                        //Get the default description string and warn/log and set defaults on failure. 
                        rawAttributeBuffer = property.GetCustomAttributes(typeof(ConfigEntryDescription), false);

                        if (rawAttributeBuffer != null && rawAttributeBuffer.Length > 0)
                            descriptionString = ((ConfigEntryDescription)rawAttributeBuffer[0]).Value;

                        if (descriptionString == null)
                        {
                            Logger.LogWarning($"{GetType().Name}.{property.Name}: Could not get default description from attribute. Using default.");
                            descriptionString = "<No Description Available>";
                        }

                        ConfigDescription cfgDescription = new ConfigDescription(descriptionString);

                        //Get the default value and warn/log and set defaults on failure. 
                        rawAttributeBuffer = property.GetCustomAttributes(typeof(ConfigEntryDefaultValue), false);
                        
                        if (rawAttributeBuffer != null && rawAttributeBuffer.Length > 0)
                            defaultValueRaw = ((ConfigEntryDefaultValue)rawAttributeBuffer[0]).Value;  //Value can be null

                        if(defaultValueRaw == null)
                        {
                            defaultValueRaw = Activator.CreateInstance(configEntryInstanceType);
                            Logger.LogWarning($"{GetType().Name}.{property.Name}: Could not get default value from attribute. Using default.");
                        }

                        defaultValue = Convert.ChangeType(defaultValueRaw, configEntryInstanceType); //Needs to be converted regardless of Activator. Otherwise MakeGenericMethod does not resolve T properly for some reason.

                        //Key value if set
                        rawAttributeBuffer = property.GetCustomAttributes(typeof(ConfigEntryKey), false);

                        if (rawAttributeBuffer != null && rawAttributeBuffer.Length > 0)
                            key = ((ConfigEntryKey)rawAttributeBuffer[0]).Value;

                        if (key == null)
                            key = property.Name;

                        //should we call ConfigFile.Bind()?
                        bool useStandardPropertyBinding = true;

                        //Make generic for Pre and Post Property binding methods
                        MethodInfo instancedGenericPrePropertyBind = CFM_GenericPreBindMethod.MakeGenericMethod(configEntryInstanceType);

                        object[] modParams = new object[]
                        {
                            property,
                            sectionName,
                            key,
                            defaultValue,
                            cfgDescription,
                            file,
                            useStandardPropertyBinding
                        };

                        instancedGenericPrePropertyBind.Invoke(this, modParams);

                        //Reassignment
                        sectionName = (string)modParams[1];
                        key = (string)modParams[2];
                        defaultValue = modParams[3];
                        cfgDescription = (ConfigDescription)modParams[4];   //So ref types still need to be copied..why MS?
                        file = (ConfigFile)modParams[5];
                        useStandardPropertyBinding = (bool)modParams[6];

                        //Standard binding will be used?
                        if (useStandardPropertyBinding)
                        {
                            //Get generic for ConfigFile.Bind<T>()
                            MethodInfo instancedGenericConfigBind = CFM_GenericConfigFileBindMethod.MakeGenericMethod(configEntryInstanceType);

                            //Get generic for PostBindingCalls
                            MethodInfo instancedGenericPostPropertyBind = CFM_GenericPostBindMethod.MakeGenericMethod(configEntryInstanceType);

                            // Bind the property.
                            var configBind = instancedGenericConfigBind.Invoke(file, new object[] { 
                                sectionName,
                                key,
                                defaultValue,
                                cfgDescription
                            });

                            property.SetValue(this, configBind, null); 
                            var currentValue = property.GetValue(this, null);

                            // Call post binding hook
                            instancedGenericPostPropertyBind.Invoke(this, new object[] { 
                                currentValue,
                                sectionName,
                                key,
                                file
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"{this.GetType().Name} | Initialization error for: {property.Name}");
                        Logger.LogError(e.Message);
                        Logger.LogError(e.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the active ConfigFile has been changed by ChangeConfigFile(). Called after migration is completed.
        /// </summary>
        /// <param name="oldFile">The old ConfigFile.</param>
        /// <param name="newFile">The new ConfigFile.</param>
        protected virtual void OnConfigFileMigration(ConfigFile oldFile, ConfigFile newFile) { }

        /// <summary>
        /// Changes the active ConfigFile used by the model. Calls OnConfigFileMigration and forces a re-initialization of the model.
        /// </summary>
        /// <param name="newFile"></param>
        public void ChangeConfigFile(ConfigFile newFile, string sectionName = null)
        {
            if (CurrentFile == newFile)
            {
                Logger.LogError($"{this.GetType()}::ChangeConfigFile() | The current/old and target/new ConfigFiles are the same. Skipping!");
                return;
            }

            ConfigFile oldFile = CurrentFile;
            SetCurrentConfigFile(newFile, sectionName);
            OnConfigFileMigration(oldFile, newFile);
        }

        /// <summary>
        /// Must be called after Instantiation. Initializes the binding process for the ConfigEntries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The ConfigFile to use</param>
        /// <param name="sectionName">The SectionName for the model. The Attribute value will be used if left at default.</param>
        /// <param name="logger">The Logger to be used for debugging.</param>
        /// <returns>Initialized self.</returns>
        public T Bind<T>(ConfigFile file = null, string sectionName = null, ManualLogSource logger = null) where T : ConfigFileModel
        {
            if (this.alreadyBound)
            {
                this.Logger.LogError("Error: ConfigFileModel::Bind() | Bind has already been called once. Please use ChangeConfigFile instead if you wish to change active config files.");
                return (T)this;
            }

            if (logger != null)
                this.Logger = logger;

            if (file == null)
            {
                if (this.CurrentFile == null)
                {
                    this.Logger.LogError("Error: ConfigFileModel::Bind() | ConfigFile Param is null and CurrentFile was not set via the constructor");
                    return (T)this;
                }
                else
                {
                    file = this.CurrentFile;
                }
            }

            this.SetCurrentConfigFile(file, sectionName);
            this.alreadyBound = true;
            return (T)this;
        }

        /// <summary>
        /// Internal use only! Sets the currently active config file and runs model initialization.
        /// </summary>
        /// <param name="file">The new config file to set.</param>
        private void SetCurrentConfigFile(ConfigFile file, string sectionName = null)
        {
            try
            {
                if (file == null)
                {
                    Logger.LogError($"{GetType().Name} | The ConfigFile argument is null! Model initialization aborted!!");
                    return;
                }

                _currentFile = file;

                if (sectionName == null)
                {
                    sectionName = this.SectionName;
                }

                OnModelCreate(file, ref sectionName);   //Pre-Init properties
                InitAndBindConfigs(file, sectionName);  //Init properties
                SectionName = sectionName;          //For use with external events
                                                    //Post Init: (Re)register Event Handlers.
                file.ConfigReloaded -= OnConfigReloaded;
                file.ConfigReloaded += OnConfigReloaded;

                file.SettingChanged -= OnSettingsChanged;
                file.SettingChanged += OnSettingsChanged;
            }
            catch (Exception e)
            {
                Logger.LogError($"{GetType().Name}::{nameof(SetCurrentConfigFile)}() | Error: ");
                Logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Called during the model's initialization sequence. This function is called BEFORE any of the Model-Class properties are processed and bound.
        /// </summary>
        /// <param name="file">The configuration file</param>
        /// <param name="sectionName">The section name string to be used in the config file.</param>
        protected virtual void OnModelCreate(ConfigFile file, ref string sectionName) { }

        /// <summary>
        /// This method is called on every ConfigEntry<> Property Member in the Model-Class before the ConfigFile.Bind() call is made. You can make changes here to customize anything that you wish before it is bound to the config file. You can even choose to override/skip the binding process altogether. Note: that you must perform the binding yourself in this scenario.
        /// </summary>
        /// <typeparam name="T">The Type of the default value. Same as the [T] in ConfigEntry<T>.</typeparam>
        /// <param name="property">The PropertyInfo of the ConfigEntry<> variable to be bound.</param>
        /// <param name="sectionName">The section name that the ConfigEntry will be put under.</param>
        /// <param name="defaultKey">The NAME of the variable in the config file. Default is the variable name from the Model-Class.</param>
        /// <param name="defaultValue">The default value of the variable. This is taken from the [Attribute] if set.</param>
        /// <param name="description">The default description for the variable. This is taken from the [Attribute] if set.</param>
        /// <param name="file">The configuration file that planned to be used for tha variable. You can set a different ConfigFile.</param>
        /// <param name="useStandardBindingMethod">[Default=true] If set to FALSE, the ConfigFile.Bind() call will be skipped. NOTE that this will make [PostPropertyBind<T>()] skipped as well.</param>
        protected virtual void PrePropertyBind<T>(PropertyInfo property, ref string sectionName, ref string key, ref T defaultValue, ConfigDescription description, ref ConfigFile file, ref bool useStandardBindingMethod) { }

        /// <summary>
        /// Called for each Model-Class property member after ConfigFile.Bind() has been called. Please note that [PrePropertyBind()] can stop this method from being run.
        /// </summary>
        /// <typeparam name="T">The Type of the Property Member</typeparam>
        /// <param name="value">The post-bound ConfigEntry property.</param>
        /// <param name="sectionName">The assigned section name using in binding.</param>
        /// <param name="key">The current key used in the config file.</param>
        /// <param name="description">The current description used in the config file.</param>
        /// <param name="file">The ConfigFile used by the Property Member.</param>
        protected virtual void PostPropertyBind<T>(ConfigEntry<T> value, string sectionName, string key, ConfigFile file) { }

        /// <summary>
        /// Called upon the configuration being reloaded. Triggered by the ConfigFile.ConfigReloaded Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnConfigReloaded(object sender, EventArgs e) { }

        /// <summary>
        /// Called upon the configuration being reloaded. Is called once for each ConfigEntry<> that has been orphaned (unbound/unable to be read from the config file). Allows custom handling of orphaned entries' values. 
        /// </summary>
        /// <typeparam name="T">The entry underlying type T.</typeparam>
        /// <param name="orphanedEntry">The orphaned ConfigEntry<> </param>
        /// <param name="sectionName">The section name used by the orphaned entry.</param>
        public virtual void OrphanedPropertyPostConfigReload<T>(ConfigEntry<T> orphanedEntry, string sectionName, ConfigDefinition oprhanDictConfigDef, string orphanDictStringValue, ConfigFile file) { }
        
        /// <summary>
        /// Used to process Config Reloaded events.
        /// </summary>
        private void InternalOnConfigReloaded()
        {
            Dictionary<ConfigDefinition, string> OrphanedEntriesProperty = (Dictionary<ConfigDefinition, string>)ConfigFile_OrphanedEntriesDefinition.GetValue(CurrentFile, null);

            foreach (PropertyInfo property in CFM_ConfigFileEntryProperties)
            {
                //Search for a matching key/section and call OrphanedPropertyPostConfigReload<T>() for the property.
                foreach (KeyValuePair<ConfigDefinition, string> orphanEntry in OrphanedEntriesProperty)
                {
                    if (orphanEntry.Key.Key == property.Name && orphanEntry.Key.Section == SectionName)
                    {
                        MethodInfo instancedGenericOrphanedPropCfgReload = CFM_GenericOrphanedPropertyPostConfigReloadMethod.MakeGenericMethod(property.PropertyType.GetGenericArguments()[0]);

                        var cfgEntry = Convert.ChangeType(property.GetValue(this, null), property.PropertyType);

                        object[] param = new object[] 
                        { 
                            cfgEntry,
                            SectionName,
                            orphanEntry.Key,
                            orphanEntry.Value,
                            CurrentFile
                        };

                        //Calling OrphanedPropertyPostConfigReload<T>()
                        instancedGenericOrphanedPropCfgReload.Invoke(this, param);

                        property.SetValue(this, param[0], null);
                    }
                    break;  //No need to search the rest of the orphans for this same property.
                }
            }
        }

        /// <summary>
        /// Called upon the BepInEx settings being changed.Triggered by the ConfigFile.SettingChanged Event.
        /// <param name="sender"></param>
        /// <param name="args">SettingsChanged data passed from the BepInEx event. For more info, see: [BepInEx.Configuration.SettingChangedEventArgs].</param>
        public virtual void OnSettingsChanged(object sender, BepInEx.Configuration.SettingChangedEventArgs args) { }

        //---Var Defs---//
        protected static ManualLogSource _StaticLogger { get; private set; }
        public ManualLogSource Logger 
        {
            get
            {
                if (_logger == null)
                    _logger = _StaticLogger;
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        private ManualLogSource _logger;

        protected string SectionName 
        { 
            get
            {
                if (_sectionName == null)
                {
                    object[] attribRaw = this.GetType().GetCustomAttributes(typeof(ConfigModelSectionName), true);

                    if (attribRaw != null && attribRaw[0] != null)
                    {
                        _sectionName = ((ConfigModelSectionName)attribRaw[0])?.Value;
                    }

                    if (_sectionName == null || _sectionName == "")
                    {
                        Logger.LogError($"{GetType().Name} | The ConfigFileModel.SectionName is null or empty. Did you forget the attribute or to pass in a section name string? Using 'You_Forgot_To_Put_A_Section_Name'");
                        _sectionName =  "You_Forgot_To_Put_A_Section_Name";
                    }
                }
                return _sectionName;
            }
            private set
            {
                _sectionName = value;
            }
        }

        private string _sectionName;
        protected ConfigFile CurrentFile 
        { 
            get
            {
                return _currentFile;
            }
            set
            {
                if (value == null)
                    return;

                _currentFile = value;
            }
        }
        private ConfigFile _currentFile;

        private bool alreadyBound = false;

        private static MethodInfo CFM_GenericConfigFileBindMethod { get; }

        private MethodInfo CFM_GenericOrphanedPropertyPostConfigReloadMethod { get; }
        private MethodInfo CFM_GenericPreBindMethod { get; }
        private MethodInfo CFM_GenericPostBindMethod { get; }

        private PropertyInfo[] CFM_ConfigFileEntryProperties { get; }
        private static PropertyInfo ConfigFile_OrphanedEntriesDefinition { get; }
    } 
}
