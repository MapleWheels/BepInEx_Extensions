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
           
            foreach(PropertyInfo prop in BindableConfigDataMembers)
            {
                logger.LogWarning($"..BindModel: Prop={prop}");

                try
                {
                    prop.PropertyType.GetMethod("Bind")
                        .Invoke(prop.GetValue(this, BINDFLAGS_CDM, null, null, null), new object[] {
                        config, Logger, SectionName, prop.Name, null, null
                    });
                }
                catch (NullReferenceException e)
                {
                    logger.LogError($"..BindModel: NRE Error: {e}");
                    logger.LogError($"..BindModel: NRE Base Error: {e.GetBaseException().Message}");
                    logger.LogError($"..BindModel: NRE Stack: {e.StackTrace}");
                }
            }

        }

        protected PropertyInfo[] BindableConfigDataMembers;
        
        protected void Init(ManualLogSource logger)
        {
            if (DefaultsSet)
                return;

            logger.LogWarning($"ConfigDataModel::Init() invoked.");


            BindableConfigDataMembers = GetType().GetProperties(BINDFLAGS_CDM)
                .Where(                    
                    x => 
                    {
                        return
                            x.PropertyType.IsGenericType; //TODO: Implement proper Interface checks  
                    }
                ).ToArray();

            logger.LogWarning($"ConfigDataModel::Init() BindableConfigDataMembers={BindableConfigDataMembers}");
            logger.LogWarning($"ConfigDataModel::Init() BindableConfigDataMembers.Length={BindableConfigDataMembers.Length}");


            SetDefaults();

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
            logger.LogWarning($"CDMExtensions::BindModel() | pre-bind | cdm={cdm}");
            cdm.BindModel(config, sectionName, logger);
            logger.LogWarning($"CDMExtensions::BindModel() | post-bind | cdm={cdm}");
            return cdm;
        }
    }
}
