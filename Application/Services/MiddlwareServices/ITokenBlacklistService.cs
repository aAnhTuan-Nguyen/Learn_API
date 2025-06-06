namespace TodoWeb.Application.Services.MiddlwareServices
{
    public interface ITokenBlacklistService
    {
        void BanToken(string token);
        bool IsTokenBanned(string token);

    }
}
