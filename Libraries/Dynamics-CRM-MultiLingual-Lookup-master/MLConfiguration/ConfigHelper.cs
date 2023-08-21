
using System;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk.Query;
using MLShared;

namespace MLConfiguration
{
    public static class ConfigHelper
    {
        /*
         * Acceptable formats for form attributes and lookups located on another form
            Attributes
            string sample = "fieldSchemanName[fieldEnSchemaName;1033|fieldFrSchemaName;1033]";
            string sampleMultiFields = "field1SchemanName[field1EnSchemaName;1033|field1FrSchemaName;1033],field2SchemanName[field2EnSchemaName;1033|field2FrSchemaName;1033]";

            Lookups: entitySchemaName|Lookup1SchemaName|Lookup2SchemaName,entity2SchemaName|Lookup1SchemaName
            string sampleLookup = "contact|parentid";
            string sampleLookupMulti = "contact|parentid|otherlookup,email|parentid|morelookup";
             */
        /*
         Sample output
            name[new_nameen;1033|new_namefr;1036]
            Settings/Entity/fields

            <Settings>
	            <entity name="account">
		            <fields name="name">
			            <field lcid="1033">new_nameen</field>
			            <field lcid="1036">new_namefr</field>
		            </fields>
		            <fields name="new_name2">
			            <field lcid="1033">new_name2en</field>
			            <field lcid="1036">new_name2fr</field>
		            </fields>
	            </entity>
	            <lookups>
		            <lookup name="contact">
			            <field>parentcustomerid</field>
			            <field>parentcustomerid1</field>
		            </lookup>
		            <lookup name="account">
			            <field>parentaccountid</field>
		            </lookup>
	            </lookups>
            </Settings>
             */

        public static string ProcessAttributes(string entityName, string attributes, ref string preImageAttributes, string lookups = "")
        {
            StringBuilder unsecureConfig = new StringBuilder("<Settings><entity name=\"");
            unsecureConfig.Append(entityName + "\">");

            string[] arr = attributes.Split(MLStatics.COMMA);

            foreach (string str in arr)
            {
                // name[new_nameen;1033|new_namefr;1036]
                if (Regex.IsMatch(str, MLStatics.ATTRIBUTES_REGEX))
                {
                    var match = Regex.Match(str, MLStatics.ATTRIBUTES_REGEX);
                    /*
                        Group 1: name
                        Group 2: new_nameen;1033|new_namefr;1036
                     */
                    unsecureConfig.Append("<fields name=\"");
                    unsecureConfig.Append(match.Groups[1].Value);
                    unsecureConfig.Append("\">");

                    string[] lfields = match.Groups[2].Value.Split(MLStatics.PIPE);
                    foreach(string lfield in lfields)
                    {
                        string[] lfieldattr = lfield.Split(';');

                        unsecureConfig.Append("<field lcid=\"");
                        unsecureConfig.Append(lfieldattr[1]);
                        unsecureConfig.Append("\">");
                        unsecureConfig.Append(lfieldattr[0]);
                        unsecureConfig.Append("</field>");

                        preImageAttributes += lfieldattr[0] + MLStatics.COMMA;
                    }

                    unsecureConfig.Append("</fields>");
                }
            }
            unsecureConfig.Append("</entity>");

            if (preImageAttributes.Length > 0)
            {
                preImageAttributes = preImageAttributes.Substring(0, preImageAttributes.Length - 1);
            }

            if (!string.IsNullOrEmpty(lookups))
            {
                // Process the lookup fields
                unsecureConfig.Append("<lookups>");
                arr = lookups.Split(MLStatics.COMMA);

                foreach (string str in arr)
                {
                    string[] innerArr = str.Split(MLStatics.PIPE);
                    unsecureConfig.Append("<lookup name=\"" + innerArr[0] + "\">");
                    for (int i = 1; i < innerArr.Length; i++)
                    {
                        unsecureConfig.Append("<field>" + innerArr[i] + "</field>");
                    }
                    unsecureConfig.Append("</lookup>");
                }
                unsecureConfig.Append("</lookups>");
            }

            unsecureConfig.Append("</Settings>");
            return unsecureConfig.ToString();
        }

        public static bool IfEntityExist(IOrganizationService service, string entityName)
        {
            bool isExist = false;
            // Retrieve Metadata for all entities
            RetrieveAllEntitiesRequest allEntitiesRequest = new RetrieveAllEntitiesRequest();
            allEntitiesRequest.EntityFilters = EntityFilters.Entity;
            allEntitiesRequest.RetrieveAsIfPublished = true;
            RetrieveAllEntitiesResponse allEntitiesResponse = (RetrieveAllEntitiesResponse)service.Execute(allEntitiesRequest);

            foreach (EntityMetadata entityMetadata in allEntitiesResponse.EntityMetadata)
            {
                if (entityMetadata.LogicalName == entityName)
                {
                    isExist = true;
                    break;
                }
            }

            if (!isExist)
            {
                throw new InvalidPluginExecutionException("Entity '" + entityName + "' doesn't exist.");
            }
            return isExist;
        }

        public static void SetAttribute(Entity entity, string field, string value)
        {
            if (!entity.Contains(field))
            {
                entity.Attributes.Add(field, value);
            }
            else
            {
                entity[field] = value;
            }
        }

        public static Entity GetTargetEntity(IPluginExecutionContext context)
        {
            Entity entity = null;
            if (context.InputParameters.Contains(MLStatics.CONTEXT_TARGET) &&
                context.InputParameters[MLStatics.CONTEXT_TARGET] is Entity)
            {
                entity = (Entity)context.InputParameters[MLStatics.CONTEXT_TARGET];
                if (entity == null)
                {
                    entity = (Entity)context.PreEntityImages[MLStatics.PreImageAlias];
                }
            }
            return entity;
        }

        public static Entity DoesStepAlreadyRegistered(IOrganizationService service, string stepName, string eventHandlerName)
        {
            string fetch = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">" +
              "<entity name=\"sdkmessageprocessingstep\">" +
                "<attribute name=\"name\" />" +
                "<attribute name=\"configuration\" />" +
                "<filter type=\"and\">" +
                  "<condition attribute=\"name\" operator=\"eq\" value=\"" + stepName + "\" />" +
                  "<condition attribute=\"eventhandlername\" operator=\"eq\" value=\"" + eventHandlerName + "\" />" +
                "</filter>" +
              "</entity>" +
            "</fetch > ";

            EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetch));
            return ((result == null) || (result.Entities.Count == 0)) ? null : result.Entities[0];
        }

        public static Entity RetrieveEntity(IOrganizationService service, string entityName, string[] entitySearchField, object[] entitySearchFieldValue, ColumnSet columnSet, ConditionOperator op)
        {
            Entity entity = null;
            QueryExpression queryExpression = new QueryExpression();
            queryExpression.EntityName = entityName;
            FilterExpression filterExpression = new FilterExpression();
            for (int index = 0; index < entitySearchField.Length; ++index)
            {
                ConditionExpression conditionExpression = new ConditionExpression();
                conditionExpression.AttributeName = entitySearchField[index];
                conditionExpression.Operator = op;
                conditionExpression.Values.Add(entitySearchFieldValue[index]);
                filterExpression.FilterOperator = 0;
                filterExpression.AddCondition(conditionExpression);
            }
            queryExpression.ColumnSet = columnSet;
            queryExpression.Criteria = filterExpression;
            EntityCollection entityCollection;
            try
            {
                entityCollection = service.RetrieveMultiple(queryExpression);
            }
            catch (Exception ex)
            {
                throw new Exception("Error-RetrieveEntity", ex);
            }
            if (entityCollection.Entities.Count == 1)
                entity = entityCollection.Entities[0];
            return entity;
        }

        public static Guid RegisterStep(IOrganizationService service,
            string message, PluginStep.PluginStepStage stage, string primaryEntityName, string primaryAttributes, string relatedAttributes)
        {
            string preImageAttributes = string.Empty;
            string unsecureConfig = ProcessAttributes(primaryEntityName, primaryAttributes, ref preImageAttributes, relatedAttributes);
            string peName = "On" + message + primaryEntityName + Guid.NewGuid().ToString();

            Guid pluginStepGuid = new PluginStep()
            {
                Message = message,
                PrimaryEntity = primaryEntityName,

                EventHandler = MLStatics.ML_EVENT_HANDLER,
                Name = peName,
                Rank = 1,
                Stage = stage,
                Mode = PluginStep.PluginStepMode.Synchronous,
                Deployment = PluginStep.PluginStepDeployment.ServerOnly,

                UnsecureConfiguration = unsecureConfig,

                PreImageAlias = MLStatics.PreImageAlias,
                PreImageAttributes = preImageAttributes
            }.RegisterStep(ref service);

            return pluginStepGuid;
        }

        public static void UnRegisterStep(IOrganizationService service, Guid guid)
        {
            service.Delete(MLStatics.SDK_MESSAGE_PROCESSING_STEP_NAME, guid);
        }
    }
}
