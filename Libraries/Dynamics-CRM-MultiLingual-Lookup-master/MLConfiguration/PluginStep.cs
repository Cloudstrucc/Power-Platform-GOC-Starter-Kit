
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.ObjectModel;

namespace MLConfiguration
{
    public class PluginStep
    {
        private Entity step = new Entity("sdkmessageprocessingstep");

        public int CustomizationLevel { get; set; }

        public bool DeleteAsyncOperationIfSuccessful { get; set; }

        public PluginStep.PluginStepDeployment Deployment { get; set; }

        public string EventHandler { get; set; }

        public string FilteringAttributes { get; set; }

        protected Guid ImpersonatingUserId { get; set; }

        public PluginStep.PluginStepInvocationSource InvocationSource { get; set; }

        public string Message { get; set; }

        protected Guid MessageFilterId { get; set; }

        protected Guid MessageId { get; set; }

        public PluginStep.PluginStepMode Mode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        protected Guid PluginId { get; set; }

        public string PrimaryEntity { get; set; }

        public string SecondaryEntity { get; set; }

        public int Rank { get; set; }

        public string SecureConfiguration { get; set; }

        public PluginStep.PluginStepStage Stage { get; set; }

        protected Guid StepId { get; set; }

        public string UnsecureConfiguration { get; set; }

        public string PreImageAlias { get; set; }

        public string PreImageAttributes { get; set; }

        public string PostImageAlias { get; set; }

        public string PostImageAttributes { get; set; }

        private static Entity RetrieveEntity(ref IOrganizationService service, string entityName, string[] entitySearchField, object[] entitySearchFieldValue, ColumnSet columnSet, ConditionOperator op)
        {
            Entity entity = (Entity)null;
            QueryExpression queryExpression = new QueryExpression();
            queryExpression.EntityName = entityName;
            FilterExpression filterExpression = new FilterExpression();
            for (int index = 0; index < entitySearchField.Length; ++index)
            {
                ConditionExpression conditionExpression = new ConditionExpression();
                conditionExpression.AttributeName = entitySearchField[index];
                conditionExpression.Operator = op;
                ((Collection<object>)conditionExpression.Values).Add(entitySearchFieldValue[index]);
                filterExpression.FilterOperator = (LogicalOperator)0;
                filterExpression.AddCondition(conditionExpression);
            }
            queryExpression.ColumnSet = columnSet;
            queryExpression.Criteria = filterExpression;
            EntityCollection entityCollection;
            try
            {
                entityCollection = service.RetrieveMultiple((QueryBase)queryExpression);
            }
            catch (Exception ex)
            {
                throw new Exception("Step Registration Error-RetrieveEntity", ex);
            }
            if (((Collection<Entity>)entityCollection.Entities).Count == 1)
                entity = ((Collection<Entity>)entityCollection.Entities)[0];
            return entity;
        }

        private EntityCollection RetrieveEntityCollection(ref IOrganizationService service, string entityName, string[] entitySearchField, object[] entitySearchFieldValue, ColumnSet columnSet, ConditionOperator op)
        {
            EntityCollection entityCollection1 = (EntityCollection)null;
            QueryExpression queryExpression = new QueryExpression();
            queryExpression.EntityName = entityName;
            FilterExpression filterExpression = new FilterExpression();
            for (int index = 0; index < entitySearchField.Length; ++index)
            {
                ConditionExpression conditionExpression = new ConditionExpression();
                conditionExpression.AttributeName = entitySearchField[index];
                conditionExpression.Operator = op;
                ((Collection<object>)conditionExpression.Values).Add(entitySearchFieldValue[index]);
                filterExpression.FilterOperator = (LogicalOperator)0;
                filterExpression.AddCondition(conditionExpression);
            }
            queryExpression.ColumnSet = columnSet;
            queryExpression.Criteria = filterExpression;
            EntityCollection entityCollection2;
            try
            {
                entityCollection2 = service.RetrieveMultiple((QueryBase)queryExpression);
            }
            catch (Exception ex)
            {
                throw new Exception("Step Registration Error-RetrieveEntityCollection", ex);
            }
            if (((Collection<Entity>)entityCollection2.Entities).Count > 0)
                entityCollection1 = entityCollection2;
            return entityCollection1;
        }

        private void SetMessageFilterId(ref IOrganizationService service)
        {
            Entity entity;
            if (!string.IsNullOrEmpty(this.PrimaryEntity) && !string.IsNullOrEmpty(this.SecondaryEntity))
                entity = PluginStep.RetrieveEntity(ref service, "sdkmessagefilter", new string[3]
                {
          "sdkmessageid",
          "primaryobjecttypecode",
          "secondaryobjecttypecode"
                }, (object[])new string[3]
                {
          this.MessageId.ToString(),
          this.PrimaryEntity.ToLower(),
          this.SecondaryEntity.ToLower()
                }, new ColumnSet(true), (ConditionOperator)0);
            else
                entity = PluginStep.RetrieveEntity(ref service, "sdkmessagefilter", new string[2]
                {
          "sdkmessageid",
          "primaryobjecttypecode"
                }, (object[])new string[2]
                {
          this.MessageId.ToString(),
          this.PrimaryEntity.ToLower()
                }, new ColumnSet(true), (ConditionOperator)0);
            if (entity == null)
                return;
            this.MessageFilterId = entity.Id;
        }

        private void SetMessageId(ref IOrganizationService service)
        {
            Entity entity = PluginStep.RetrieveEntity(ref service, "sdkmessage", new string[1]
            {
        "name"
            }, (object[])new string[1]
            {
        this.Message.ToLower()
            }, new ColumnSet(true), (ConditionOperator)0);
            if (entity == null)
                return;
            this.MessageId = entity.Id;
        }

        private void SetPluginId(ref IOrganizationService service)
        {
            Entity entity = PluginStep.RetrieveEntity(ref service, "plugintype", new string[1]
            {
        "name"
            }, (object[])new string[1]
            {
        this.EventHandler
            }, new ColumnSet(true), (ConditionOperator)0);
            if (entity == null)
                return;
            this.PluginId = entity.Id;
        }

        private Guid registerEntityImage(ref IOrganizationService service, Guid stepId, string imageAlias, PluginStep.PluginStepImageType imageType, string attributes)
        {
            Guid guid = Guid.Empty;
            if (!string.IsNullOrEmpty(imageAlias))
            {
                var processingStepImage = new Entity("sdkmessageprocessingstepimage");
                    processingStepImage["entityalias"] = imageAlias;
                    processingStepImage["imagetype"] = new OptionSetValue((int)imageType);
                    processingStepImage["messagepropertyname"] = this.Message == "Create" ? "id" : "Target";
                    processingStepImage["sdkmessageprocessingstepid"] = new EntityReference("sdkmessageprocessingstep", stepId);
                    processingStepImage["attributes"] = attributes;
                try
                {
                    guid = service.Create(processingStepImage);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(new object[8]
                    {
                        (object) "Image creation Error for type ",
                        (object) imageType,
                        (object) " with alias ",
                        (object) imageAlias,
                        (object) " with message ",
                        (object) this.Message,
                        (object) " with attributes ",
                        (object) attributes
                    }), ex);
                }
            }
            return guid;
        }

        private Guid createSecureConfiguration(ref IOrganizationService service, string secureConfiguration)
        {
            Guid guid = Guid.Empty;
            if (!string.IsNullOrEmpty(secureConfiguration))
            {
                var stepSecureConfig = new Entity("sdkmessageprocessingstepsecureconfig");
                    stepSecureConfig["secureconfig"] = secureConfiguration;
                try
                {
                    guid = service.Create(stepSecureConfig);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating secure configuration", ex);
                }
            }
            return guid;
        }

        public void DeleteStep(ref IOrganizationService service, Guid stepId)
        {
            service.Delete("sdkmessageprocessingstep", stepId);
        }

        public void DeletePlugin(ref IOrganizationService service, Guid pluginId)
        {
            service.Delete("plugintype", pluginId);
        }

        public static Entity RetrieveStepById(ref IOrganizationService service, Guid stepId)
        {
            return service.Retrieve("sdkmessageprocessingstep", stepId, new ColumnSet());
        }

        public EntityCollection RetrieveStepsByPluginId(ref IOrganizationService service, Guid pluginId)
        {
            return this.RetrieveEntityCollection(ref service, "sdkmessageprocessingstep", new string[1]
            {
        "plugintypeid"
            }, (object[])new string[1]
            {
        pluginId.ToString()
            }, new ColumnSet(true), (ConditionOperator)0) ?? (EntityCollection)null;
        }

        public Guid RetrievePluginId(ref IOrganizationService service, string pluginName)
        {
            Entity entity = PluginStep.RetrieveEntity(ref service, "plugintype", new string[1]
            {
        "name"
            }, (object[])new string[1]
            {
        pluginName
            }, new ColumnSet(true), (ConditionOperator)0);
            if (entity != null)
                return entity.Id;
            return Guid.Empty;
        }

        public Guid RegisterStep(ref IOrganizationService service)
        {
            this.SetMessageId(ref service);
            this.SetMessageFilterId(ref service);
            this.SetPluginId(ref service);
            this.step["sdkmessageid"] = new EntityReference("sdkmessage", this.MessageId);
            if ((this.MessageFilterId != null) && (this.MessageFilterId != Guid.Empty))
                this.step["sdkmessagefilterid"] = new EntityReference("sdkmessage", this.MessageFilterId);
            this.step["filteringattributes"] = this.FilteringAttributes;
            this.step["eventhandler"] = new EntityReference("plugintype", this.PluginId);
            this.step["name"] = this.Name;
            this.step["rank"] = new int?(this.Rank);
            this.step["stage"] = new OptionSetValue((int)this.Stage);
            this.step["mode"] = new OptionSetValue((int)this.Mode);
            this.step["supporteddeployment"] = new OptionSetValue((int)this.Deployment);
            this.step["description"] = this.Description;
            this.step["configuration"] = this.UnsecureConfiguration;
            if (!string.IsNullOrEmpty(this.SecureConfiguration))
                this.step["sdkmessageprocessingstepsecureconfigid"] = new EntityReference("sdkmessageprocessingstepsecureconfig", this.createSecureConfiguration(ref service, this.SecureConfiguration));
            Guid guid = Guid.Empty;
            Guid stepId;
            try
            {
                stepId = service.Create(this.step);
            }
            catch (Exception ex)
            {
                throw new Exception("Step Registration Error-RegisterStep====MessageID=>" + this.MessageId.ToString() + "<===MessageFilterId==>" + MessageFilterId + "<=====", ex);
            }
            if ((this.Message == "Update") && !string.IsNullOrEmpty(this.PreImageAlias) && !string.IsNullOrEmpty(this.PreImageAttributes))
                this.registerEntityImage(ref service, stepId, this.PreImageAlias, PluginStep.PluginStepImageType.PreImage, this.PreImageAttributes);
            if (!string.IsNullOrEmpty(this.PostImageAlias) && !string.IsNullOrEmpty(this.PostImageAttributes))
                this.registerEntityImage(ref service, stepId, this.PostImageAlias, PluginStep.PluginStepImageType.PostImage, this.PostImageAttributes);
            return stepId;
        }

        public enum PluginStepDeployment
        {
            ServerOnly,
            OfflineOnly,
            Both,
        }

        public enum PluginStepInvocationSource
        {
            Parent,
            Child,
        }

        public enum PluginStepMode
        {
            Synchronous,
            Asynchronous,
        }

        public enum PluginStepImageType
        {
            PreImage,
            PostImage,
            Both,
        }

        public enum PluginStepStage
        {
            PreValidation = 10,
            PreOperation = 20,
            PostOperation = 40,
            PostOperationDeprecated = 50,
        }
    }
}

