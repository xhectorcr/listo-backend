using System.Threading.Tasks;

namespace ListoAPI.Aplication.Core.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string para, string asunto, string cuerpoHtml);
    }
}
