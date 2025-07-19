namespace AstApp.Models
{
    public class Ast
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TrabajoARealizar { get; set; } = string.Empty;
        public string AreaDeTrabajo { get; set; } = string.Empty;
        public string Gerencia { get; set; } = string.Empty;
        public string Subgerencia { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string TipoReferencia { get; set; } = "N/A";
        public string NumeroReferencia { get; set; } = string.Empty;
        public string Herramientas { get; set; } = string.Empty;
        public Usuario Ejecutor { get; set; } = new();
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public List<TareaAst> Tareas { get; set; } = new();
        public List<UsuarioAsistente> Asistentes { get; set; } = new();
        public List<string> EppRequeridos { get; set; } = new();
        public string? EppOtrosTexto { get; set; }
        public bool EstaSincronizada { get; set; } = false;
        public PuntajeAST PuntajeAst { get; set; } = new();
    }

    public class TareaAst
    {
        public string Descripcion { get; set; } = "";
        public List<string> Peligros { get; set; } = new();
        public List<string> Riesgos { get; set; } = new();
        public List<string> AccionesCorrectivas { get; set; } = new();
    }

    public class PuntajeAST
    {
        public int Puntaje { get; set; } = 0;
        public string Comentario { get; set; } = "";
    }
}
