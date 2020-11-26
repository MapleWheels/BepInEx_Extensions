using BepInEx.Configuration;
using BepInEx.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    /// <summary>
    /// Acts as a container for config properties. Intended to be used with ConfigData<> but will accept any types that implement IConfigBindable<>. IMPORTANT: Only properties (get/set) will be bound (fields not supported).
    /// </summary>
    public class ConfigDataModel : IConfigModelDataProvider, IConfigModelBehaviour
    {
        /// <summary>
        /// Whether or not SetDefaults() has been run. Used internally.
        /// </summary>
        public bool DefaultsSet { get; private set; }
        /// <summary>
        /// The defaults section name that the ConfigData<> properties
        /// </summary>
        public string SectionName { get; protected set; }
        /// <summary>
        /// The config file that the ConfigData<> entries are bound to.
        /// </summary>
        public ConfigFile Config { get; protected set; }
        /// <summary>
        /// The logger used for errors and warnings.
        /// </summary>
        public ManualLogSource Logger { get; set; }

        /// <summary>
        /// This method is called once per ConfigDataModel instantiation. This should be treated as the constructor and is guaranteed to be run once. You should instantiate your config properties here as well as any event hooks.
        /// </summary>
        public virtual void SetDefaults() { }

        /// <summary>
        /// This is called whenever the ConfigDataModel is being bound. It is called before Bind() is invoked.
        /// </summary>
        /// <param name="config"></param>
        public virtual void OnModelCreate(ConfigFile config) { }

        private event System.Action<ConfigDataModel> PreBindInternal, PostBindInternal;
        /// <summary>
        /// Called immediately before the model is bound.
        /// </summary>
        public event System.Action<ConfigDataModel> PreBind
        {
            add
            {
                PreBindInternal -= value;
                PreBindInternal += value;
            }
            remove
            {
                PreBindInternal -= value;
            }
        }

        /// <summary>
        /// Called immediately after model is bound.
        /// </summary>
        public event System.Action<ConfigDataModel> PostBind
        {
            add
            {
                PostBindInternal -= value;
                PostBindInternal += value;
            }
            remove
            {
                PostBindInternal -= value;
            }
        }


        /// <summary>
        /// Allows you to change the actively bound config file for all members. Intended to help support profile implementation.
        /// </summary>
        /// <param name="config"></param>
        public void SetConfigFile(ConfigFile config)
        {
            if (this.Logger == null)
            {
                this.Logger = BepInEx.Logging.Logger.CreateLogSource("default");
                Logger.LogWarning("ConfigDataModel::SetConfigFile() | Logger is not set, using 'default'");
            }

            if (this.SectionName == null)
            {
                this.SectionName = "default";
                Logger.LogWarning("ConfigDataModel::SetConfigFile() | SectionName is not set, using 'default'");
            }
            this.BindModel(config, null, null);
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
            this.PreBindInternal?.Invoke(this);
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
                    logger.LogError($"..BindModel: NRE PropMember={prop.Name}");
                    logger.LogError($"..BindModel: NRE Error: {e}");
                    logger.LogError($"..BindModel: NRE Base Error: {e.GetBaseException().Message}");
                    logger.LogError($"..BindModel: NRE Stack: {e.StackTrace}");
                }
                catch (Exception e)
                {
                    logger.LogError($"..BindModel: Gen PropMember={prop.Name}");
                    logger.LogError($"..BindModel: Gen Error: {e}");
                    logger.LogError($"..BindModel: Gen Base Error: {e.GetBaseException().Message}");
                    logger.LogError($"..BindModel: Gen Stack: {e.StackTrace}");
                }
            }
            this.PostBindInternal?.Invoke(this);
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
        /// <summary>
        /// Instantiates and binds a type that implements the IConfigModelBehaviour interface. Intended to be used with ConfigDataModel.
        /// </summary>
        /// <typeparam name="T">The config model type.</typeparam>
        /// <param name="config">The config file to bind to.</param>
        /// <param name="logger">The log source to be used. Will use the config model type defined logger or "default" if none is provided.</param>
        /// <param name="sectionName">The section name to be used. The config model type defined section name or "default" if none is provided.</param>
        /// <returns>The new instance of the model, ready to use.</returns>
        public static T BindModel<T>(this ConfigFile config, ManualLogSource logger = null, string sectionName = null) where T : class, IConfigModelBehaviour
        {
            T cdm = (T)Activator.CreateInstance(typeof(T), null);
            cdm.BindModel(config, sectionName, logger);
            return cdm;
        }

        /// <summary>
        /// Instantiates and binds a type that implements the IConfigModelBehaviour interface. Intended to be used with ConfigDataModel.
        /// </summary>
        /// <typeparam name="T">The config model type.</typeparam>
        /// <param name="config">The config file to bind to.</param>
        /// <param name="logger">The log source to be used. Will use the config model type defined logger or "default" if none is provided.</param>
        /// <param name="sectionName">The section name to be used. The config model type defined section name or "default" if none is provided.</param>
        /// <param name="preBindDelegate">A delegate to a function to be run immediately before all members are bound.</param>
        /// <param name="postBindDelegate">A delegate to a function to be run immediately after all members are bound.</param>
        /// <returns>The new instance of the model, ready to use.</returns>
        public static T BindModel<T>(this ConfigFile config, System.Action<ConfigDataModel> preBindDelegate, System.Action<ConfigDataModel> postBindDelegate, ManualLogSource logger = null, string sectionName = null) where T : ConfigDataModel
        {
            T cdm = (T)Activator.CreateInstance(typeof(T), null);
            if (preBindDelegate != null)
                cdm.PreBind += preBindDelegate;
            if (postBindDelegate != null)
                cdm.PostBind += postBindDelegate;
            cdm.BindModel(config, sectionName, logger);
            return cdm;
        }
    }
}
