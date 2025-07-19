namespace AstApp.Models
{
    public class Actividad
    {
        public string Descripcion { get; set; } = string.Empty;
        public bool YaTieneSugerencias { get; set; } = false;
        public List<OpcionSugerida> PeligrosSugeridos { get; set; } = new();
        public List<OpcionSugerida> RiesgosSugeridos { get; set; } = new();
        public List<OpcionSugerida> AccionesSugeridas { get; set; } = new();
        public bool EstaCompleta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Descripcion))
                    return false;

                int peligros = PeligrosSugeridos.Count(p => p.Seleccionado);
                int riesgos = RiesgosSugeridos.Count(r => r.Seleccionado);
                int acciones = AccionesSugeridas.Count(a => a.Seleccionado);

                return peligros >= 3 && riesgos >= 3 && acciones >= 3;
            }
        }
    }

    public class CheckboxEpp
    {
        public string Texto { get; set; } = string.Empty;
        public bool Seleccionado { get; set; } = false;
    }
}
