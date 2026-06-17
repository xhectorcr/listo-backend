using System.Threading.Tasks;

namespace Listo.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string para, string asunto, string cuerpoHtml);
    }
}
