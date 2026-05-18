public class RegistroClienteDTO
{
    // Solo los campos que el cliente llena en el formulario
    public string Nombre { get; set; }
    public string Dni { get; set; }
    public string Correo { get; set; }
    public string Password { get; set; }
    public string Telefono { get; set; }
}