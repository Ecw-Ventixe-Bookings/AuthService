

namespace Service.Dtos;

public class PublicKeyinfo
{
    public string PublicKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}
