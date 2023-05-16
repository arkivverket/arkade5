namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    internal abstract class Registration
    {
        public string Status { get; set; }
        public string RegistrationId { get; set; }

        public bool Utgaar => Status?.ToLower() == "utgår";
    }
}
