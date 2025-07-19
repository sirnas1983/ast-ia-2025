using System.Linq;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AstApp.Models
{
    public class SugerenciaIA
    {
        public List<string> Peligros { get; set; } = new();
        public List<string> Riesgos { get; set; } = new();
        public List<string> AccionesCorrectivas { get; set; } = new();
        public string? Observaciones { get; set; }
    }

}
