using Google.Apis.Auth;

namespace TodoWeb.Application.Services
{
    public interface IGoogleCredentialService
    {
        public Task<GoogleJsonWebSignature.Payload> VerifyCredential(string clientId, string credential);

    }
}
