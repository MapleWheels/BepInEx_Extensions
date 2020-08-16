using BepInEx.Configuration;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace BepInEx_Extensions.Configuration
{
    /// <summary>
    /// This class is designed to allow Attribute-based Configuration File definitions, similar to Entity Framework's DbContext files. WARNING: All members must be declared as Properties with "get"/"set". Fields will not be detected. Usage: Extend/Derive this class and define all of your ConfigEntry<T> variables as Properties (with get/set). Then just instantiate the class and all members will be auto-bound.
    /// </summary>
    /// <author> PerfidiousLeaf </author>
    /// <date>2020-08-13</date>
    /// <version>1.0.0</version>
    public class ConfigFileModel
    {
        /// <summary>
        /// Create and bind the Config Data Model to the supplied ConfigFile. Binding is automatic and immediate on instantiation.
        /// </summary>
        /// <param name="file">The configuration file to bind to.</param>
        /// <param name="sectionName">The section name that this model will appear under in the ConfigFile. Intended for use with multiple models in one file.</param>
        public ConfigFileModel(ConfigFile file, string sectionName)
        {
            OnModelCreate(file, ref sectionName);
            InitAndBindConfigs(file, sectionName);
        }

        /// <summary>
        /// Internal use only! Uses reflection to parse through all ConfigEntry<> vars in inheriting data model classes and bind them to their associated configuration entries in the BepInEx ConfigFile. Intended to allow Attribute-Based config file design similar to Entity Framework's DbContext.
        /// </summary>
        /// <param name="file">The target config file to bind to.</param>
        /// <param name="sectionName">The section name in the config file this Data model will be placed under.</param>
        private void InitAndBindConfigs(ConfigFile file, string sectionName)
        {
            try
            {
                UnityEngine.Debug.Log("Current Type: " + GetType().Name);

                string desc;
                Type genericType;
                object defaultValRaw;
                ConfigDescription cfgDesc;
                PropertyInfo[] propertyInfos = GetType().GetProperties();

                foreach (PropertyInfo property in propertyInfos)
                {
                    if (property.PropertyType.GetGenericTypeDefinition() == typeof(ConfigEntry<>))
                    {
                        try
                        {
                            //ConfigEntry description string                            
                            //desc = property.GetCustomAttribute<ConfigEntryDesc>()?.Value; //Net 4.x
                            desc = ((ConfigEntryDesc)property.GetCustomAttributes(typeof(ConfigEntryDesc), false)[0])?.Value;   //Net 3.5
                            //The underlying type 'T'
                            genericType = property.PropertyType.GetGenericArguments()[0];
                            //The default value if set. Otherwise, use the default value.
                            //defaultValRaw = property.GetCustomAttribute<ConfigDefaultValue>()?.Value; //Net 4.x
                            defaultValRaw = ((ConfigDefaultValue)property.GetCustomAttributes(typeof(ConfigDefaultValue), false)[0])?.Value; //Net 3.5
                            //Set the description if available
                            cfgDesc = new ConfigDescription(
                                (desc == null) ? "<No Description>" : desc
                                );

                            UnityEngine.Debug.Log("ConfigFileModel::InitAndBindConfigs() | propertyName=" + property.Name);
                            UnityEngine.Debug.Log("ConfigFileModel::InitAndBindConfigs() | genericType=" + genericType.Name);

                            //Debugging tests; data logging

                            MethodInfo bindMethod = typeof(ConfigFile).GetMethods().FirstOrDefault(
                                m => m.Name == "Bind"
                                && m.GetParameters()[0].ParameterType == typeof(string)
                                && m.GetParameters()[1].ParameterType == typeof(string)
                                && m.GetParameters()[3].ParameterType == typeof(ConfigDescription)
                                );

                            MethodInfo bindMethodGeneric = bindMethod.MakeGenericMethod(genericType);

                            if (defaultValRaw == null)
                            {
                                defaultValRaw = Convert.ChangeType(Activator.CreateInstance(genericType), genericType);
                            }

                            var defaultValue = Convert.ChangeType(defaultValRaw, genericType);

                            UnityEngine.Debug.Log("ConfigFileModel::InitAndBindConfigs() | defaultValRaw.Type=" + defaultValRaw.GetType());
                            UnityEngine.Debug.Log("ConfigFileModel::InitAndBindConfigs() | defaultValue.Type=" + defaultValue.GetType());

                            var cfgBindInstance = bindMethodGeneric.Invoke(file, new object[] { 
                                sectionName,
                                property.Name,
                                defaultValue,
                                cfgDesc
                            });

                            property.SetValue(this, cfgBindInstance);

                        }
                        catch (TypeLoadException tle)
                        {
                            UnityEngine.Debug.Log("ConfigModelTest::InitAndBindConfigs() | Reflection fail.");
                            UnityEngine.Debug.LogError(tle.Source);
                            UnityEngine.Debug.LogError(tle.StackTrace);
                            UnityEngine.Debug.LogError(tle.Message);
                            UnityEngine.Debug.Log("TypeLoadException: InnerException Data.");
                            UnityEngine.Debug.Log(tle.InnerException);
                            UnityEngine.Debug.Log(tle.InnerException?.Source);
                            UnityEngine.Debug.Log(tle.InnerException?.StackTrace);
                            UnityEngine.Debug.Log(tle.InnerException?.Message);
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.Log("ConfigModelTest::InitAndBindConfigs() | Generic Exception.");
                            UnityEngine.Debug.LogError(e.Source);
                            UnityEngine.Debug.LogError(e.StackTrace);
                            UnityEngine.Debug.LogError(e.Message);
                            UnityEngine.Debug.Log("Exception: InnerException Data.");
                            UnityEngine.Debug.Log(e.InnerException);
                            UnityEngine.Debug.Log(e.InnerException?.Source);
                            UnityEngine.Debug.Log(e.InnerException?.StackTrace);
                            UnityEngine.Debug.Log(e.InnerException?.Message);
                        }
                    }
                }
            }
            catch (TypeLoadException tle)
            {
                UnityEngine.Debug.Log("ConfigModelTest::InitAndBindConfigs() | #FOREACH.");
                UnityEngine.Debug.LogError(tle.Source);
                UnityEngine.Debug.LogError(tle.StackTrace);
                UnityEngine.Debug.LogError(tle.Message);
                UnityEngine.Debug.Log("TypeLoadException: InnerException Data.");
                UnityEngine.Debug.Log(tle.InnerException);
                UnityEngine.Debug.Log(tle.InnerException?.Source);
                UnityEngine.Debug.Log(tle.InnerException?.StackTrace);
                UnityEngine.Debug.Log(tle.InnerException?.Message);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("ConfigModelTest::InitAndBindConfigs() | Generic3 Exception.");
                UnityEngine.Debug.LogError(e.Source);
                UnityEngine.Debug.LogError(e.StackTrace);
                UnityEngine.Debug.LogError(e.Message);
                UnityEngine.Debug.Log("Exception: InnerException Data.");
                UnityEngine.Debug.Log(e.InnerException);
                UnityEngine.Debug.Log(e.InnerException?.Source);
                UnityEngine.Debug.Log(e.InnerException?.StackTrace);
                UnityEngine.Debug.Log(e.InnerException?.Message);
            }                
        }

        /// <summary>
        /// Called during the Constructor sequence. This function is called before any of the Model-Class properties are processed and bound.
        /// </summary>
        /// <param name="file">The configuration file</param>
        /// <param name="sectionName">The section name string to be used in the config file.</param>
        protected virtual void OnModelCreate(ConfigFile file, ref string sectionName) { }

        /// <summary>
        /// This method is called on every ConfigEntry<> Property Member in the Model-Class before Bind() call is made. You can make changes here to customize anything that you wish before it is bound to the config file. You can even choose to override/skip the binding process altogether. 
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
        protected virtual void PostPropertyBind<T>(T value, string sectionName, string key, ConfigDescription description, ConfigFile file) { }

        /// <summary>
        /// Called upon the configuration being reloaded. Triggered by the ConfigFile.ConfigReloaded Event.
        /// </summary>
        public virtual void OnConfigReloaded() { }

        /// <summary>
        /// Called upon the BepInEx settings being changed. Triggered by the ConfigFile.SettingChanged Event.
        /// </summary>
        /// <param name="args">SettingsChanged data passed from the BepInEx event. For more info, see: [BepInEx.Configuration.SettingChangedEventArgs].</param>
        public virtual void OnSettingsChanged(BepInEx.Configuration.SettingChangedEventArgs args) { }
    } 
}
