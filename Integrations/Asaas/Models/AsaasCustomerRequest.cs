using System.Text.Json.Serialization;

namespace ImobAPI.Integrations.Asaas.Models
{
    public class AsaasCustomerRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("addressNumber")]
        public string AddressNumber { get; set; }

        [JsonPropertyName("complement")]
        public string Complement { get; set; }

        [JsonPropertyName("province")]
        public string Province { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("notificationDisabled")]
        public bool NotificationDisabled { get; set; }

        [JsonPropertyName("additionalEmails")]
        public string AdditionalEmails { get; set; }

        [JsonPropertyName("municipalInscription")]
        public string MunicipalInscription { get; set; }

        [JsonPropertyName("stateInscription")]
        public string StateInscription { get; set; }

        [JsonPropertyName("observations")]
        public string Observations { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("foreignCustomer")]
        public bool ForeignCustomer { get; set; }
    }
}
