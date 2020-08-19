using BepInEx.Configuration;
using BepInEx.Logging;
using System;
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
            GenericConfigFileBindMethod =
                typeof(ConfigFile)
                .GetMethods().FirstOrDefault(
                    m => m.GetParameters().Count() == 4
                    && m.GetParameters()[0].ParameterType == typeof(string)
                    && m.GetParameters()[1].ParameterType == typeof(string)
                    && m.GetParameters()[3].ParameterType == typeof(ConfigDescription)
                );
        }

        /// <summary>
        /// Create and bind the Config Data Model to the supplied ConfigFile. Binding is automatic and immediate on instantiation.
        /// </summary>
        /// <param name="file">The configuration file to bind to.</param>
        /// <param name="logger">The BepInEx Logger to be used. If ommitted, a default instance of the logger will be used that is shared by all ConfigFileModel instances.</param>
        /// <param name="sectionName">The section name that this model will appear under in the ConfigFile. The ConfigModelSectionName attribute will be used if this is set to null/ommitted.</param>
        public ConfigFileModel(ConfigFile file, ManualLogSource logger = null, string sectionName = null)
        {
            if (_StaticLogger == null)
                _StaticLogger = BepInEx.Logging.Logger.CreateLogSource("ConfigFileModel_DefaultLogger");

            if (logger == null)
                Logger = _StaticLogger;
            else
                Logger = logger;

            try
            {
                if (sectionName==null)
                {
                    sectionName = ((ConfigModelSectionName)this.GetType().GetCustomAttributes(typeof(ConfigModelSectionName), true)[0])?.Value;

                    if (sectionName == null || sectionName == "")
                    {
                        Logger.LogError($"{GetType().Name} | The ConfigFileModel.SectionName is null or empty. Did you forget the attribute or to pass in a section name string?");
                    }
                    else
                    {
                        OnModelCreate(file, ref sectionName);   //Pre-Init properties
                        InitAndBindConfigs(file, sectionName);  //Init properties
                        //Post Init: register Event Handlers.
                        file.ConfigReloaded += OnConfigReloaded;
                        file.SettingChanged += OnSettingsChanged;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{GetType().Name} | Constructor Error: ");
                Logger.LogError(e.Message);
            }

        }

        /// <summary>
        /// Internal use only! Uses reflection to parse through all ConfigEntry<> vars in inheriting data model classes and bind them to their associated configuration entries in the BepInEx ConfigFile. Intended to allow Attribute-Based config file design similar to Entity Framework's DbContext.
        /// </summary>
        /// <param name="file">The target config file to bind to.</param>
        /// <param name="sectionName">The section name in the config file this Data model will be placed under.</param>
        private void InitAndBindConfigs(ConfigFile file, string sectionName)
        {
            //Use Reflection to get all of the Property Members.
            PropertyInfo[] propertyInfos = GetType().GetProperties();
            
            //The generic version of PrePropertyBind<T>()
            MethodInfo genericPreBindMethod = 
                GetType().GetMethod(
                    nameof(PrePropertyBind),
                    BindingFlags.Instance
                );

            //The generic version of PostPropertyBind<T>()
            MethodInfo genericPostBindMethod =
                GetType().GetMethod(
                    nameof(PostPropertyBind),
                    BindingFlags.Instance
                );

            foreach (PropertyInfo property in propertyInfos)
            {
                if (property.PropertyType.GetGenericTypeDefinition() == typeof(ConfigEntry<>))
                {
                    try
                    {
                        //Get T value from property
                        Type configEntryInstanceType = property.PropertyType.GetGenericArguments()[0];

                        //Get the default description and warn/log and set defaults on failure. 
                        string descriptionString = ((ConfigEntryDescription)property.GetCustomAttributes(typeof(ConfigEntryDescription), false)[0])?.Value;

                        if (descriptionString == null)
                        {
                            Logger.LogWarning($"{GetType().Name}.{property.Name}: Could not get default description from attribute. Using default.");
                            descriptionString = "<No Description Available>";
                        }

                        ConfigDescription cfgDescription = new ConfigDescription(descriptionString);

                        //Get the default value and warn/log and set defaults on failure. 
                        var defaultValueRaw = ((ConfigEntryDefaultValue)property.GetCustomAttributes(typeof(ConfigEntryDefaultValue), false)[0])?.Value;

                        object defaultValue;
                        if(defaultValueRaw == null)
                        {
                            defaultValue = Activator.CreateInstance(configEntryInstanceType);
                            Logger.LogWarning($"{GetType().Name}.{property.Name}: Could not get default value from attribute. Using default.");
                        }

                        defaultValue = Convert.ChangeType(defaultValueRaw, configEntryInstanceType); //Needs to be converted regardless of Activator. Otherwise MakeGenericMethod does not resolve T properly for some reason.

                        //Key value
                        string key = ((ConfigEntryKey)property.GetCustomAttributes(typeof(ConfigEntryKey), false)[0])?.Value;

                        if (key == null || key  == "")
                            key = property.Name;

                        //should we call ConfigFile.Bind()?
                        bool useStandardPropertyBinding = true;

                        //Make generic for Pre and Post Property binding methods
                        MethodInfo instancedGenericPrePropertyBind = genericPreBindMethod.MakeGenericMethod(configEntryInstanceType);

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
                        cfgDescription = (ConfigDescription)modParams[4];
                        file = (ConfigFile)modParams[5];
                        useStandardPropertyBinding = (bool)modParams[6];

                        //Standard binding will be used?
                        if (useStandardPropertyBinding)
                        {
                            //Get generic for ConfigFile.Bind<T>()
                            MethodInfo instancedGenericConfigBind = GenericConfigFileBindMethod.MakeGenericMethod(configEntryInstanceType);

                            //Get generic for PostBindingCalls
                            MethodInfo instancedGenericPostPropertyBind = genericPostBindMethod.MakeGenericMethod(configEntryInstanceType);

                            // Bind the property.
                            var configBind = instancedGenericConfigBind.Invoke(this, new object[] { 
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
                    }
                }
            }
        }

        /// <summary>
        /// Called during the Constructor sequence. This function is called before any of the Model-Class properties are processed and bound.
        /// </summary>
        /// <param name="file">The configuration file</param>
        /// <param name="sectionName">The section name string to be used in the config file.</param>
        protected virtual void OnModelCreate(ConfigFile file, ref string sectionName) { }

        /// <summary>
        /// This method is called on every ConfigEntry<> Property Member in the Model-Class before Bind() call is made. You can make changes here to customize anything that you wish before it is bound to the config file. You can even choose to override/skip the binding process altogether. Note: that you must perform the binding yourself in this scenario.
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
        /// <param name="value">It's current value.</param>
        /// <param name="sectionName">The assigned section name using in binding.</param>
        /// <param name="key">The current key used in the config file.</param>
        /// <param name="description">The current description used in the config file.</param>
        /// <param name="file">The ConfigFile used by the Property Member.</param>
        protected virtual void PostPropertyBind<T>(T value, string sectionName, string key, ConfigFile file) { }

        /// <summary>
        /// Called upon the configuration being reloaded. Triggered by the ConfigFile.ConfigReloaded Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnConfigReloaded(object sender, EventArgs e) { }

        /// <summary>
        /// Called upon the BepInEx settings being changed.Triggered by the ConfigFile.SettingChanged Event.
        /// <param name="sender"></param>
        /// <param name="args">SettingsChanged data passed from the BepInEx event. For more info, see: [BepInEx.Configuration.SettingChangedEventArgs].</param>
        public virtual void OnSettingsChanged(object sender, BepInEx.Configuration.SettingChangedEventArgs args) { }

        protected static ManualLogSource _StaticLogger { get; private set; }
        protected ManualLogSource Logger { get; set; }

        private static MethodInfo GenericConfigFileBindMethod { get; set; }
    } 
}
