using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using ImobAPI.Context;
using ImobAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ImobAPI.Integrations.Asaas
{
    public class RepasseJsonOptions : IConfigureOptions<JsonOptions>
    {
        private readonly IHttpContextAccessor _accessor;
        private static readonly object CacheKey = new();
        public RepasseJsonOptions(IHttpContextAccessor accessor) { _accessor = accessor; }

        public void Configure(JsonOptions options)
        {
            var resolver = new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(typeInfo =>
            {
                if (typeInfo.Type != typeof(Contrato) && typeInfo.Type != typeof(Cobranca)) return;
                foreach (var property in typeInfo.Properties)
                {
                    if (property.AttributeProvider is PropertyInfo member && (member.Name == "ContaRepasse1" || member.Name == "ContaRepasse2"))
                        property.ShouldSerialize = (_, _) => PodeVerContas();
                }
            });
            options.JsonSerializerOptions.TypeInfoResolver = resolver;
        }

        private bool PodeVerContas()
        {
            var http = _accessor.HttpContext;
            if (http == null) return false;
            if (http.Items.TryGetValue(CacheKey, out var cached)) return (bool)cached;
            var permitido = RepasseConfiguracao.Administrador(http.RequestServices.GetRequiredService<ImobContext>(), http.User);
            http.Items[CacheKey] = permitido;
            return permitido;
        }
    }
}
