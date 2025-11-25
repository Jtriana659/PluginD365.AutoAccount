using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace AutoAccountPlugin
{
    public class AutoAccountPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // --------------------------------------------
            // 1. Contexto del plugin
            // --------------------------------------------
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            // Solo aplicar en Create de Contact

            if (context.MessageName != "Create" || context.PrimaryEntityName != "contact")
                return;

            // --------------------------------------------
            // 2. Obtener servicios
            // --------------------------------------------
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);
           
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracing.Trace("Inicio del plugin AutoAccountPlugin");
            // --------------------------------------------
            // 3. Obtener el Target
            // --------------------------------------------

            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity target))
                return;

            tracing.Trace("Obteniendo el Target correctamente");
            // --------------------------------------------
            // 4. Validar si ya tiene parentcustomerid
            // --------------------------------------------

            if (target.Contains("parentcustomerid"))
            {
                tracing.Trace("El contacto ya tiene compañía → No se crea account.");
                return;
            }
            tracing.Trace("El contacto NO tiene compañía → Crear Account.");
            // --------------------------------------------
            // 5. Crear la cuenta automáticamente
            // --------------------------------------------
            Entity account = new Entity("account");

            string nombreContacto = target.Contains("fullname") ? target.GetAttributeValue<string>("fullname") : "Cuenta de Contacto";
            account["name"] = $"cuenta generada para el contacto {nombreContacto}";

            Guid accountId = service.Create(account);
            tracing.Trace($"Cuenta creada con Id: {accountId}");

            // --------------------------------------------
            // 6. Relacionar la cuenta con el contacto
            // PreOperation → se modifica el Target directamente
            // --------------------------------------------
            target["parentcustomerid"] = new EntityReference("account", accountId);
            tracing.Trace("Contacto relacionado con la cuenta creada.");

        }
    }
}
