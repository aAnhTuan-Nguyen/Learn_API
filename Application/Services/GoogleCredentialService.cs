using Google.Apis.Auth;

namespace TodoWeb.Application.Services
{
    public class GoogleCredentialService: IGoogleCredentialService
    {
        public Task<GoogleJsonWebSignature.Payload> VerifyCredential(string clientId, string credential)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };
            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(credential, settings);
                return payload;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to verify Google credential", ex);
            }
        }
    }
}
