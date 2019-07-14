using Microsoft.Xrm.Sdk;
using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class SetModifiedOn : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetModifiedOn(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                tracer.Trace("Plugin gestart");

                Entity entity = (Entity)context.InputParameters["Target"];
                tracer.Trace("Entiteit gevonden");

                if (context.PreEntityImages.Contains("preImage"))
                {
                    Entity preImageEntity = context.PreEntityImages["preImage"];
                    tracer.Trace("preImage gevonden");

                    if (preImageEntity.Contains("createdon"))
                    {
                        tracer.Trace("dnbs_OverridenModifiedOn bevat geen gegevens, " + preImageEntity["createdon"]);

                        entity["modifiedon"] = preImageEntity["createdon"];
                        tracer.Trace("ModifiedOn overschreven met dnbs_OverridenModifiedOn");
                    }
                }
                else
                {

                    if (entity.Attributes.Contains("createdon") && entity.Attributes.Contains("modifiedon") == false)
                    {
                        tracer.Trace("ModifiedOn bevat geen gegevens");

                        entity.Attributes.Add("modifiedon", entity["createdon"]);
                        tracer.Trace("ModifiedOn gevuld met dnbs_OverridenModifiedOn, " + entity["createdon"]);
                    }
                    else if (entity.Attributes.Contains("createdon") && entity.Attributes.Contains("modifiedon"))
                    {
                        tracer.Trace("ModifiedOn bevat al gegevens");

                        entity["modifiedon"] = entity["createdon"];
                        tracer.Trace("ModifiedOn overschreven met dnbs_OverridenModifiedOn, " + entity["createdon"]);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
