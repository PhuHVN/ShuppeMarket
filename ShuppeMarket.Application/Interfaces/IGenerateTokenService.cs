using ShuppeMarket.Domain.Entities;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IGenerateTokenService
    {
        string GenerateToken(Account accounts);
    }
}
