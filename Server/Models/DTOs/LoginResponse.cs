namespace Server.Models.DTOs
{
    public record LoginResponse
         (bool Flag, string Message = null!, string Token = null!);
}
