using BepInEx.Configuration;
using BepInEx.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    public class ConfigDataModel : IConfigModelDataProvider, IConfigModelBehaviour
    {
        public bool DefaultsSet { get; private set; }
        public string SectionName { get; protected set; }

        public ConfigFile Config { get; protected set; }

        public ManualLogSource Logger { get; set; }

        public virtual void SetDefaults() { }
        public virtual void OnModelCreate(ConfigFile config) { }

        public void SetConfigFile(ConfigFile config)
        {
            throw new NotImplementedException();
        }

        public void BindModel(ConfigFile config, string sectionName, ManualLogSource logger)
        {
            if (sectionName != null)
                SectionName = sectionName;

            if (logger != null)
                Logger = logger;

            if (!DefaultsSet)
                Init(logger);

            OnModelCreate(config);

            if (config == null)
            {
                if (logger != null)
                    logger.LogError($"Unable to bind model for {GetType().Name}, supplied Config instance is null!");
                return;
            }
            
            Config = config;
           
            //Bind all of the members
            foreach(PropertyInfo prop in BindableConfigDataMembers)
            {
                try
                {
                    object iConfigBindablePropVal = prop.GetValue(this, BINDFLAGS_CDM, null, null, null);

                    if (iConfigBindablePropVal != null)
                        prop.PropertyType.GetMethod("Bind")
                            .Invoke(iConfigBindablePropVal, new object[] {
                            config, Logger, SectionName, prop.Name, null, null
                        });
                    else
                    {
                        logger.LogError($"ConfigDataModel::BindModel() | You did not intialize ConfigData {prop.Name} in SetDefaults()! Setting up unbound with defaults.");
                        iConfigBindablePropVal = Activator.CreateInstance(prop.PropertyType);   //Just to stop NREs
                        prop.SetValue(this, iConfigBindablePropVal, null);
                    }
                }
                catch (NullReferenceException e)
                {
                    logger.LogError($"..BindModel: NRE Error: {e}");
                    logger.LogError($"..BindModel: NRE Base Error: {e.GetBaseException().Message}");
                    logger.LogError($"..BindModel: NRE Stack: {e.StackTrace}");
                }
                catch (Exception e)
                {
                    logger.LogError($"..BindModel: Gen Error: {e}");
                    logger.LogError($"..BindModel: Gen Base Error: {e.GetBaseException().Message}");
                    logger.LogError($"..BindModel: Gen Stack: {e.StackTrace}");
                }
            }

        }

        protected PropertyInfo[] BindableConfigDataMembers;
        
        protected void Init(ManualLogSource logger)
        {
            if (DefaultsSet)
                return;

            BindableConfigDataMembers = this.GetType().GetProperties(BINDFLAGS_CDM)
                .Where(                    
                    x => x.PropertyType.IsGenericType && typeof(IConfigBindableTypeComparator).IsAssignableFrom(x.PropertyType)
                ).ToArray();
            logger?.LogWarning($"BindCDMInfo.Length={BindableConfigDataMembers.Length}");

            //Virt call
            SetDefaults();
            //Setup completed
            DefaultsSet = true;
        }

        public const BindingFlags BINDFLAGS_CDM = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public ConfigDataModel() { }    //Activator, default(T) use
    }

    public static class CDMExtensions
    {
        public static T BindModel<T>(this ConfigFile config, ManualLogSource logger = null, string sectionName = "Default") where T : class, IConfigModelBehaviour, IConfigModelDataProvider
        {
            T cdm = (T)Activator.CreateInstance(typeof(T), null);
            cdm.BindModel(config, sectionName, logger);
            return cdm;
        }
    }
}
