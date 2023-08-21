
using System;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MLShared;

namespace MLConfiguration
{
    public class PluginConfig : Plugin
    {
        public PluginConfig() : base(typeof(PluginConfig))
        {
            RegisterEntityEvents();
        }

        private void RegisterEntityEvents()
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, 
                PluginMessages.Create,
                MLStatics.ML_CONFIGURATION, new Action<LocalPluginContext>(OnCreateEntity)));

            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, 
                PluginMessages.Update,
                MLStatics.ML_CONFIGURATION, new Action<LocalPluginContext>(OnUpdateEntity)));

            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(PluginStages.PostOperation, 
                PluginMessages.Delete,
                MLStatics.ML_CONFIGURATION, new Action<LocalPluginContext>(OnDeleteEntity)));
        }

        private void OnCreateEntity(LocalPluginContext localContext)
        {
            string primaryEntityName = string.Empty;
            string primaryAttributes = string.Empty;
            string relatedAttributes = string.Empty;
            IOrganizationService service = localContext.OrganizationService;
            IPluginExecutionContext context = localContext.PluginExecutionContext;

            Entity entity = ConfigHelper.GetTargetEntity(context);

            if (entity.Attributes.Contains(MLStatics.ML_PRIMARY_ENTITY))
                primaryEntityName = entity.GetAttributeValue<string>(MLStatics.ML_PRIMARY_ENTITY);

            if(String.IsNullOrEmpty(primaryEntityName))
            {
                throw new InvalidPluginExecutionException("Primary Entity Can Not be NULL.");
            }

            // if entity does not exist, an exception is thrown
            ConfigHelper.IfEntityExist(service, primaryEntityName);

            if (entity.Attributes.Contains(MLStatics.ML_PRIMARY_FIELDS))
                primaryAttributes = entity.GetAttributeValue<string>(MLStatics.ML_PRIMARY_FIELDS);
            if (entity.Attributes.Contains(MLStatics.ML_RELATED_FIELDS))
                relatedAttributes = entity.GetAttributeValue<string>(MLStatics.ML_RELATED_FIELDS);

            StringBuilder responseGuidList = new StringBuilder();

            responseGuidList.Append(ConfigHelper.RegisterStep(service, PluginMessages.Create, PluginStep.PluginStepStage.PreOperation, primaryEntityName, primaryAttributes, relatedAttributes));
            responseGuidList.Append(MLStatics.COMMA);
            responseGuidList.Append(ConfigHelper.RegisterStep(service, PluginMessages.Update, PluginStep.PluginStepStage.PreOperation, primaryEntityName, primaryAttributes, relatedAttributes) );
            responseGuidList.Append(MLStatics.COMMA);
            responseGuidList.Append(ConfigHelper.RegisterStep(service, PluginMessages.Retrieve, PluginStep.PluginStepStage.PostOperation, primaryEntityName, primaryAttributes, relatedAttributes));
            responseGuidList.Append(MLStatics.COMMA);
            responseGuidList.Append(ConfigHelper.RegisterStep(service, PluginMessages.RetrieveMultiple, PluginStep.PluginStepStage.PostOperation, primaryEntityName, primaryAttributes, relatedAttributes));

            ConfigHelper.SetAttribute(entity, MLStatics.ML_PRIMARY_STEP_GUIDS, responseGuidList.ToString());

            // Two more registration for the entity where the lookup to multilingual record is to be placed
            if (!string.IsNullOrEmpty(relatedAttributes))
            {
                responseGuidList = new StringBuilder();

                string[] all = relatedAttributes.Split(MLStatics.COMMA);
                foreach (string str in all)
                {
                    string[] pair = str.Split(MLStatics.PIPE);
                    if (primaryEntityName != pair[0])
                    {
                        responseGuidList.Append(ConfigHelper.RegisterStep(service, PluginMessages.Retrieve, PluginStep.PluginStepStage.PostOperation, pair[0], primaryAttributes, relatedAttributes));
                        responseGuidList.Append(MLStatics.COMMA);
                        responseGuidList.Append(ConfigHelper.RegisterStep(service, PluginMessages.RetrieveMultiple, PluginStep.PluginStepStage.PostOperation, pair[0], primaryAttributes, relatedAttributes));
                        responseGuidList.Append(MLStatics.COMMA);
                    }
                }
                if (!string.IsNullOrEmpty(responseGuidList.ToString()))
                {
                    responseGuidList.Remove(responseGuidList.Length - 1, 1);
                    ConfigHelper.SetAttribute(entity, MLStatics.ML_RELATED_STEP_GUIDS, responseGuidList.ToString());
                }
            }

            service.Update(entity);
        }

        private void OnUpdateEntity(LocalPluginContext localContext)
        {
            if(localContext.PluginExecutionContext.Depth > 1)
            {
                return;
            }

            // Using pre and post images for update
            Entity preEntity = localContext.PluginExecutionContext.PreEntityImages[MLStatics.PreImageAlias];
            Entity postEntity = localContext.PluginExecutionContext.PostEntityImages[MLStatics.PostImageAlias];

            string primaryEntityName = postEntity.GetAttributeValue<string>(MLStatics.ML_PRIMARY_ENTITY);
            string primaryAttributes = postEntity.GetAttributeValue<string>(MLStatics.ML_PRIMARY_FIELDS);
            string preImageAttributes = string.Empty;
            string relatedAttributes = postEntity.GetAttributeValue<string>(MLStatics.ML_RELATED_FIELDS);

            string[] GUIDList = postEntity.GetAttributeValue<string>(MLStatics.ML_PRIMARY_STEP_GUIDS).Split(MLStatics.COMMA);
            string targetName = "OnUpdate" + primaryEntityName;

            foreach (var GUIDString in GUIDList)
            {
                Guid GUID = new Guid(GUIDString);
                if (GUID != Guid.Empty)
                {
                    Entity RetrievedStepById = localContext.OrganizationService.Retrieve(
                        "sdkmessageprocessingstep", GUID, new ColumnSet(true));

                    if (RetrievedStepById != null)
                    {
                        preImageAttributes = string.Empty;

                        RetrievedStepById.Attributes["configuration"] = ConfigHelper.ProcessAttributes(
                            primaryEntityName, primaryAttributes, ref preImageAttributes, relatedAttributes);

                        string stepName = RetrievedStepById.GetAttributeValue<string>("name");
                        localContext.OrganizationService.Update(RetrievedStepById);                            

                        if (stepName.StartsWith(targetName, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(preImageAttributes))
                        {
                            Entity stepEntityImage = ConfigHelper.RetrieveEntity(localContext.OrganizationService, "sdkmessageprocessingstepimage", new string[1]
                            {
                                "sdkmessageprocessingstepid"
                            }, new string[1]
                            {
                                GUIDString
                            }, new ColumnSet(new string[] { "attributes" }), 0);

                            if (stepEntityImage != null && stepEntityImage.Attributes.Contains("attributes"))
                            {
                                stepEntityImage["attributes"] = preImageAttributes;
                                localContext.OrganizationService.Update(stepEntityImage);
                            }
                        }
                    }
                }
            }

            string preFields = preEntity.GetAttributeValue<string>(MLStatics.ML_RELATED_FIELDS);
            string postFields = postEntity.GetAttributeValue<string>(MLStatics.ML_RELATED_FIELDS);

            // Original nothing
            if(string.IsNullOrEmpty(preFields))
            {
                if (string.IsNullOrEmpty(postFields))
                {
                    // Nothing to do
                    return;
                }
                else
                {
                    // register the new ones
                    StringBuilder responseGuidList = new StringBuilder();

                    string[] all = postFields.Split(MLStatics.COMMA);
                    foreach (string str in all)
                    {
                        string[] pair = str.Split(MLStatics.PIPE);
                        if (primaryEntityName != pair[0])
                        {
                            responseGuidList.Append(ConfigHelper.RegisterStep(localContext.OrganizationService, PluginMessages.Retrieve, PluginStep.PluginStepStage.PostOperation, pair[0],
                            primaryAttributes, relatedAttributes));
                            responseGuidList.Append(MLStatics.COMMA);
                            responseGuidList.Append(ConfigHelper.RegisterStep(localContext.OrganizationService, PluginMessages.RetrieveMultiple, PluginStep.PluginStepStage.PostOperation, pair[0],
                                primaryAttributes, relatedAttributes));
                            responseGuidList.Append(MLStatics.COMMA);
                        }
                    }
                    if (!string.IsNullOrEmpty(responseGuidList.ToString()))
                    {
                        responseGuidList.Remove(responseGuidList.Length - 1, 1);
                        ConfigHelper.SetAttribute(postEntity, MLStatics.ML_RELATED_STEP_GUIDS, responseGuidList.ToString());
                        localContext.OrganizationService.Update(postEntity);
                    }
                }
            }
            else
            {
                if (preFields == postFields)
                {
                    // nothing to do
                    return;
                }
                else
                {
                    // get the difference and register/unregister
                    string[] relatedStepGuids = preEntity.GetAttributeValue<string>(MLStatics.ML_RELATED_STEP_GUIDS).Split(MLStatics.COMMA);
                    Guid guidToDelete = Guid.Empty;
                    foreach (string str in relatedStepGuids)
                    {
                        guidToDelete = Guid.Empty;
                        if ((!string.IsNullOrEmpty(str)) && (Guid.TryParse(str, out guidToDelete)))
                        {
                            ConfigHelper.UnRegisterStep(localContext.OrganizationService, guidToDelete);
                        }
                    }

                    if (string.IsNullOrEmpty(postFields))
                    {
                        // nothing to do
                        ConfigHelper.SetAttribute(postEntity, MLStatics.ML_RELATED_STEP_GUIDS, null);
                        localContext.OrganizationService.Update(postEntity);
                        return;
                    }

                    StringBuilder responseGuidList = new StringBuilder();

                    string[] all = postFields.Split(MLStatics.COMMA);
                    foreach (string str in all)
                    {
                        string[] pair = str.Split(MLStatics.PIPE);
                        if (primaryEntityName != pair[0])
                        {
                            responseGuidList.Append(ConfigHelper.RegisterStep(localContext.OrganizationService, PluginMessages.Retrieve, PluginStep.PluginStepStage.PostOperation, pair[0],
                            primaryAttributes, relatedAttributes));
                            responseGuidList.Append(MLStatics.COMMA);
                            responseGuidList.Append(ConfigHelper.RegisterStep(localContext.OrganizationService, PluginMessages.RetrieveMultiple, PluginStep.PluginStepStage.PostOperation, pair[0],
                                primaryAttributes, relatedAttributes));
                            responseGuidList.Append(MLStatics.COMMA);
                        }
                    }
                    if (!string.IsNullOrEmpty(responseGuidList.ToString()))
                    {
                        responseGuidList.Remove(responseGuidList.Length - 1, 1);
                        ConfigHelper.SetAttribute(postEntity, MLStatics.ML_RELATED_STEP_GUIDS, responseGuidList.ToString());
                        localContext.OrganizationService.Update(postEntity);
                    }
                }
            }
        }

        private void OnDeleteEntity(LocalPluginContext localContext)
        {
            // Using pre image for delete
            Entity entity = (Entity)localContext.PluginExecutionContext.PreEntityImages[MLStatics.PreImageAlias];
            Guid guidToDelete = Guid.Empty;

            string[] GUIDList = entity.Attributes[MLStatics.ML_PRIMARY_STEP_GUIDS].ToString().Split(MLStatics.COMMA);
            foreach (var GUIDString in GUIDList)
            {
                guidToDelete = Guid.Empty;
                if ((!string.IsNullOrEmpty(GUIDString)) && (Guid.TryParse(GUIDString, out guidToDelete)))
                {
                    ConfigHelper.UnRegisterStep(localContext.OrganizationService, guidToDelete);
                }
            }

            if (entity.Attributes.Contains(MLStatics.ML_RELATED_STEP_GUIDS))
            {
                GUIDList = entity.Attributes[MLStatics.ML_RELATED_STEP_GUIDS].ToString().Split(MLStatics.COMMA);
                foreach (var GUIDString in GUIDList)
                {
                    guidToDelete = Guid.Empty;
                    if ((!string.IsNullOrEmpty(GUIDString)) && (Guid.TryParse(GUIDString, out guidToDelete)))
                    {
                        ConfigHelper.UnRegisterStep(localContext.OrganizationService, guidToDelete);
                    }
                }
            }
        }
    }
}
