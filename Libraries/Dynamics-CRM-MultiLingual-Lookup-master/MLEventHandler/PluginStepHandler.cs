using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Xml;
using MLShared;
using Microsoft.Xrm.Sdk.Query;
using System.Text;

namespace MLEventHandler
{
    public class PluginStepHandler : Plugin
    {
        private XmlNodeList EntityConfigList;

        public PluginStepHandler(string unsecureConfig, string secureConfig)
            : base(typeof(PluginStepHandler))
        {
            RetrieveUnsecureConfiguration(unsecureConfig);
        }

        private void RetrieveUnsecureConfiguration(string unsecureConfig)
        {
            XmlDocument doc = new XmlDocument();
            if (!String.IsNullOrEmpty(unsecureConfig))
            {
                try
                {
                    doc.LoadXml(unsecureConfig);
                    EntityConfigList = doc["Settings"].ChildNodes;

                    RegisterEntityEvents(EntityConfigList[0].Attributes["name"].Value);

                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    if (EntityConfigList.Count > 1)
                    {
                        XmlNodeList attributeNodes = EntityConfigList[1].SelectNodes("lookup");
                        if (attributeNodes != null)
                        {
                            foreach (XmlNode attributeNode in attributeNodes)
                            {
                                string entityname = attributeNode.Attributes["name"].Value;
                                if (dic.ContainsKey(entityname))
                                {
                                    continue;
                                }
                                dic.Add(entityname, "dummy");

                                base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, PluginMessages.Retrieve, entityname,
                                    new Action<LocalPluginContext>(UnpackNameOnRetrieve)));

                                base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, PluginMessages.RetrieveMultiple, entityname,
                                    new Action<LocalPluginContext>(UnpackNameOnRetrieveMultiple)));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("Invalid xml configuration setting - " + ex.ToString());
                }
            }
        }

        private void RegisterEntityEvents(string entityName)
        {
            // Registrations for Packing each translation field into the name field
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PreOperation, PluginMessages.Create, entityName,
                new Action<LocalPluginContext>(PackNameTranslations)));

            // Registrations for Packing each translation field into the name field on update
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PreOperation, PluginMessages.Update, entityName,
                new Action<LocalPluginContext>(PackNameTranslations)));

            // Registrations for unpacking the name field on Retrieve of Entity
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, PluginMessages.Retrieve, entityName,
                new Action<LocalPluginContext>(UnpackNameOnRetrieve)));

            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, PluginMessages.RetrieveMultiple, entityName,
                new Action<LocalPluginContext>(UnpackNameOnRetrieveMultiple)));
        }

        /// <summary>
        /// Pack the translations into the name field when an Entity is Created or Updated
        /// </summary>
        protected void PackNameTranslations(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            Entity target = GetTargetEntity(context);
            Entity preImageEntity = GetPreImageEntity(context);
            StringBuilder targetname = new StringBuilder();
            if (EntityConfigList != null)
            {
                XmlNodeList eFields = EntityConfigList[0].SelectNodes("fields");
                foreach (XmlNode eField in eFields)
                {
                    XmlNodeList fields = eField.SelectNodes("field");
                    foreach (XmlNode fieldNode in fields)
                    {
                        targetname.Append(GetAttributeValue<string>(fieldNode.InnerText, preImageEntity, target));
                        targetname.Append(MLStatics.PIPE);
                    }

                    targetname.Remove(targetname.Length - 1, 1);
                    SetTargetAttribute(target, eField.Attributes["name"].Value, targetname.ToString());
                    targetname.Clear();
                }

            }
        }

        /// <summary>
        ///  Unpack the name field when an entity is Retreived
        /// </summary>
        protected void UnpackNameOnRetrieve(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            Entity target = (Entity)context.OutputParameters["BusinessEntity"];

            UnpackAttributeValue(localContext, target);
        }

        /// <summary>
        /// Unpack the name field when entities are retrieved via Lookup Search or Advanced Find
        /// </summary>
        protected void UnpackNameOnRetrieveMultiple(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            EntityCollection entityCollection = (EntityCollection)localContext.PluginExecutionContext.OutputParameters["BusinessEntityCollection"];
            // In a grid without any entries.
            if (entityCollection.Entities.Count == 0)
            {
                return;
            }

            Entity targetEntity = new Entity();
            foreach (Entity target in entityCollection.Entities)
            {
                targetEntity = target;
                UnpackAttributeValueMultiple(localContext, target);
            }
        }

        private void UnpackAttributeValue(LocalPluginContext localContext, Entity target)
        {
            if (EntityConfigList != null)
            {
                XmlNodeList attributeNodes = EntityConfigList[0].SelectNodes("fields");
                string fullname = string.Empty;
                string targetname = string.Empty;
                string attribute = string.Empty;

                foreach (XmlNode attributeNode in attributeNodes)
                {
                    attribute = attributeNode.Attributes["name"].Value;
                    targetname = string.Empty;
                    if (target.Contains(attribute))
                    {
                        fullname = GetAttributeValue<string>(attribute, null, target);

                        targetname = UnpackName(localContext, fullname, attributeNode);

                        if (target[attribute].GetType() == typeof(EntityReference))
                        {
                            ((EntityReference)target[attribute]).Name = targetname;
                        }
                        else
                        {
                            SetTargetAttribute(target, attribute, targetname);
                        }
                    }
                }

                if (EntityConfigList.Count > 1)
                {
                    attributeNodes = EntityConfigList[1].SelectNodes("lookup");
                    XmlNodeList mainNodes = EntityConfigList[0].SelectNodes("fields");

                    if (attributeNodes != null)
                    {
                        foreach (XmlNode attributeNode in attributeNodes)
                        {
                            XmlNodeList fields = attributeNode.SelectNodes("field");
                            if (fields != null)
                            {
                                foreach (XmlNode field in fields)
                                {
                                    string lookupname = field.InnerText;
                                    if (target.Contains(lookupname))
                                    {
                                        targetname = UnpackName(localContext, ((EntityReference)target[lookupname]).Name, mainNodes[0]);
                                        ((EntityReference)target[lookupname]).Name = targetname;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private void UnpackAttributeValueMultiple(LocalPluginContext localContext, Entity target)
        {
            if (EntityConfigList != null)
            {
                XmlNodeList attributeNodes = EntityConfigList[0].SelectNodes("fields");
                string fullname = string.Empty;
                string targetname = string.Empty;

                foreach (XmlNode attributeNode in attributeNodes)
                {
                    string attribute = attributeNode.Attributes["name"].Value;
                    if (target.Contains(attribute))
                    {
                        fullname = GetAttributeValue<string>(attribute, null, target);
                        targetname = UnpackName(localContext, fullname, attributeNode);

                        if (target[attribute].GetType() == typeof(EntityReference))
                        {
                            ((EntityReference)target[attribute]).Name = targetname;
                        }
                        else
                        {
                            target[attribute] = targetname;
                        }
                    }
                }

                if (EntityConfigList.Count > 1)
                {
                    attributeNodes = EntityConfigList[1].SelectNodes("lookup");
                    XmlNodeList mainNodes = EntityConfigList[0].SelectNodes("fields");

                    if (attributeNodes != null)
                    {
                        foreach (XmlNode attributeNode in attributeNodes)
                        {
                            XmlNodeList fields = attributeNode.SelectNodes("field");
                            if (fields != null)
                            {
                                foreach (XmlNode field in fields)
                                {
                                    string lookupname = field.InnerText;
                                    if (target.Contains(lookupname))
                                    {
                                        targetname = UnpackName(localContext, ((EntityReference)target[lookupname]).Name, mainNodes[0]);
                                        ((EntityReference)target[lookupname]).Name = targetname;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Unpack the entity name field
        /// </summary>
        protected string UnpackName(LocalPluginContext localContext, string name, XmlNode lcids = null)
        {
            // Get the language of the user
            int userLanguageId = GetUserLanguageId(localContext);
            //int[] locales = new int[] { 1033, 1036 };

            XmlNodeList fields = lcids.SelectNodes("field");
            int[] locales = new int[fields.Count];
            for(int index = 0; index < fields.Count; index++)
            {
                locales[index] = int.Parse(fields[index].Attributes["lcid"].Value);
            }
            // Split the name
            string[] labels = name.Split(MLStatics.PIPE);

            // Which language is set for the user?
            int labelIndex = Array.IndexOf<int>(locales, userLanguageId);
            if ((labelIndex < 0) || (labelIndex > (labels.Length - 1)))
            {
                labelIndex = 0;
            }
            // Return the correct translation
            return labels[labelIndex];
        }

        private int GetUserLanguageId(LocalPluginContext localContext)
        {
            int userLanguageId = 0;
            string UILanguageId = "uilanguageid";
            string uLocalId = "UserLocaleId";

            if (localContext.PluginExecutionContext.SharedVariables.ContainsKey(uLocalId))
            {
                userLanguageId = (int)localContext.PluginExecutionContext.SharedVariables[uLocalId];
            }
            else if (localContext.PluginExecutionContext.SharedVariables.ContainsKey(UILanguageId))
            {
                Entity userSettings = localContext.OrganizationService.Retrieve(
                    "usersettings",
                    localContext.PluginExecutionContext.InitiatingUserId,
                    new ColumnSet(UILanguageId));
                if (userSettings != null)
                {
                    userLanguageId = userSettings.GetAttributeValue<int>(UILanguageId);
                    localContext.PluginExecutionContext.SharedVariables.Add(uLocalId, (object)userLanguageId);

                } 
               
            } else
            {
                return userLanguageId;
            }
            return userLanguageId;
        }

        /// <summary>
        /// Get a value from the target if present, otherwise from the preImage
        /// </summary>
        private T GetAttributeValue<T>(string attributeName, Entity preImage, Entity targetImage)
        {
            if (targetImage.Contains(attributeName))
            {
                return targetImage.GetAttributeValue<T>(attributeName);
            }
            else if ((preImage != null) && (preImage.Contains(attributeName)))
            {
                return preImage.GetAttributeValue<T>(attributeName);
            }
            else
                return default(T);
        }

        private void SetTargetAttribute(Entity target, string primaryAttributeName, string name)
        {
            if (!target.Contains(primaryAttributeName))
            {
                target.Attributes.Add(primaryAttributeName, name);
            }
            else
            {
                target[primaryAttributeName] = name;
            }
        }

        private Entity GetTargetEntity(IPluginExecutionContext context)
        {
            return (Entity)context.InputParameters[MLStatics.CONTEXT_TARGET];
        }

        private Entity GetPreImageEntity(IPluginExecutionContext context)
        {
            return (context.PreEntityImages != null && context.PreEntityImages.Contains(MLStatics.PreImageAlias)) ? context.PreEntityImages[MLStatics.PreImageAlias] : null;
        }

    }

}