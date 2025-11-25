

# Dataverse Plugin – Auto Account Creation

Versión: 1.0
Tecnologías: C#, .NET Framework, Dataverse, Dynamics 365, PRT

<img src="./logo.svg" width="420"/>
📘 Descripción General

Este repositorio contiene un plugin para Microsoft Dataverse / Dynamics 365 CE desarrollado en C# que automatiza la creación de una entidad Account al registrarse un nuevo Contact.

El plugin sigue buenas prácticas de arquitectura, estructura limpia y uso correcto de los servicios:

IPluginExecutionContext

IOrganizationServiceFactory

IOrganizationService

ITracingService

🧱 Arquitectura del Plugin
🔹 Pipeline

Mensaje: Create

Entidad primaria: contact

Stage: PostOperation

Modo: Synchronous

Filtering Attributes: fullname, telephone1

🔹 Flujo General

Validar la presencia del Target.

Obtener atributos del contacto.

Crear automáticamente una cuenta.

Registrar trazas para auditoría.

🧩 Código Principal
public class ContactCreateAccountPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        var service = factory.CreateOrganizationService(context.UserId);
        var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity target))
            return;

        tracing.Trace("Target obtenido correctamente.");

        Entity account = new Entity("account");

        string contactName = target.GetAttributeValue<string>("fullname") ?? "Contacto sin nombre";

        account["name"] = $"Cuenta generada para {contactName}";

        Guid accountId = service.Create(account);
        tracing.Trace($"Cuenta creada con Id: {accountId}");
    }
}

📐 Diagrama UML

Consulta el archivo /docs/uml-sequence.md para el diagrama UML del pipeline interno del plugin.

📦 Estructura del Repositorio
/
│── src/
│    └── ContactCreateAccountPlugin.cs
│── bin/
│── obj/
│── docs/
│    └── uml-sequence.md
│── logo.svg
│── README.md
│── PluginCreateAccount.csproj

🛠️ Registro del Plugin
Requisitos

Plugin Registration Tool (PRT)

Dynamics 365 con permisos de System Administrator

Visual Studio 2022

Pasos

Build → Build

Abrir PRT

Registrar → New Assembly

Agregar Step:

Message: Create

Primary Entity: contact

Stage: PostOperation

Execution Mode: Synchronous

(Opcional) Agregar Post Image

🧪 Pruebas

Crear un nuevo Contacto en CRM.

Guardar.

Ir a Cuentas y verificar que se generó un registro automáticamente.

Revisar trazas con:

Plugin Profiler

XrmToolBox → Plugin Trace Viewer

🔒 Buenas Prácticas Aplicadas

✔ Validación estricta del Target
✔ Uso correcto del OrganizationService
✔ Trazabilidad con ITracingService
✔ Código desacoplado del UI
✔ Plugin seguro para ejecución síncrona

📈 Roadmap
Versión	Descripción
1.1	Asociar automáticamente el contacto a la cuenta
1.2	Validar existencia de cuenta previa
1.3	Crear configuración global en entidad personalizada
👨‍💻 Autor

Jeisson Triana
Desarrollador – Power Platform & .NET
En camino a Arquitecto de Soluciones
