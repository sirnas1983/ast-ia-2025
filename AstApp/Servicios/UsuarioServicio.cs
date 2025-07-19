using AstApp.Models;

namespace AstApp.Servicios
{
    public class UsuarioServicio
    {
        private static List<Usuario> MockUsuarios = new()
    {
        new Usuario { Legajo = "1001", NombreApellido = "Juan Pérez", Email = "juan.perez@empresa.com", Gerencia = "Operaciones", Seccion = "A1" },
        new Usuario { Legajo = "1002", NombreApellido = "Carla López", Email = "carla.lopez@empresa.com", Gerencia = "Mantenimiento", Seccion = "B3" }
    };

        public static Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            var usuario = MockUsuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(usuario);
        }
    }

}
