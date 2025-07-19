using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AstApp.Models;

namespace AstApp.Servicios
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model = "gpt-4o";
        private readonly List<MensajeIA> _historial = new();
        public OpenAIService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            InicializarSystemPrompt();
        }
        private void InicializarSystemPrompt()
        {
            if (_historial.Any()) return;

            _historial.Add(new MensajeIA
            {
                Role = "user",
                Content = $$$"""
                CONTEXTO: 
                Soy trabajador de la empresa transportista de gas natural más importante del país. Antes de iniciar un trabajo, es mi deber como ejecutor evaluar los peligros, los riesgos (derivados de esos peligros) y las acciones correctivas necesarias para minimizar la probabilidad de incidentes. Este análisis se conoce como Análisis de Seguridad en el Trabajo (AST). Cada trabajo general se divide en una serie de tareas concretas, ejecutadas paso a paso por el personal. Estos trabajos pueden realizarse sobre gasoductos, estaciones de regulación, estaciones de medición, plantas compresoras, equipos de compresión de aire, equipos de generación eléctrica, motocompresores, motogeneradores, calentadores directos e indirectos, equipos críticos, oficinas, zonas de alto riesgo operativo, entre otros. Salvando la cañería y algunas válvulas, la mayor parte de las instalaciones son de superficie. Es tu responsabilidad interpretar lo mejor posible a qué me refiero cuando te detallo un trabajo y una tarea determinada. También debés considerar la incidencia del uso de herramientas, y en qué tarea se utiliza cada una.
                QUÉ NECESITO: 
                A partir de una tarea puntual dentro de un trabajo general, necesito que analices solamente esa tarea específica, ignorando cualquier otra parte del trabajo.
                GLOSARIO DE TÉRMINOS (jerga habitual):
                - Venteo: liberación controlada de gas natural a la atmósfera desde una cañería o sistema del gasoducto, normalmente por razones operativas o de seguridad.
                - Tubing: cañerías de acero inoxidable generalmente utilizadas para sistemas auxiliares o de control, que usan virolas y tuercas de asiento cónico.
                - Separador: elemento filtrante de forma cilíndrica, ubicado en forma horizontal. Es de mayor diámetro que la cañería y contiene filtros Nomex o de poliéster.
                - Barrido: desplazamiento de gas en una línea para eliminar aire o contaminantes.
                - EM, EMyR y ER: Estación de Medición, Estación de Medición y Regulación, y Estación de Regulación, respectivamente.
                - MC: Moto Compresor.
                - TC: Turbo Compresor.
                OTRAS CONSIDERACIONES:
                - En la mayoría de los casos —salvo que se indique explícitamente como "No Habilitado", "No Hab." o si el trabajo se realiza en oficinas— se debe asumir la posible presencia de gas en el ambiente y/o atmósferas explosivas.
                - Las estaciones EM, EMyR y ER están al aire libre, montadas sobre skids. Generalmente se encuentran cercadas con portones metálicos o alambrado romboidal.
                - Los motocompresores (MC) y módulos de medición y control (MMCC) suelen estar instalados dentro de grandes galpones.
                - Los generadores utilizan gas combustible y también se encuentran dentro de galpones cerrados. Las turbinas y motocompresores tambien usan gas combustible.
                - GTO: Gasoducto. Los gasoductos atraviesan terrenos diversos: campos, zonas boscosas, bañados, desiertos, punas, montañas, entre otros. Se debe analizar cuidadosamente el entorno natural según la localidad.
                - Considerar la fauna local al momento de planificar el trabajo: serpientes, arañas, yaguaretés, lagartos, avispas, abejas, etc.                CÓMO LO NECESITO:
                Para la tarea indicada, devolveme:
                - Una lista de peligros específicos (fuentes directas de daño) vinculados exclusivamente a esa tarea.
                - Una lista de riesgos concretos, que derivan de uno de esos peligros.
                - Una lista de acciones correctivas inmediatas, que reduzcan o eliminen los riesgos en el momento (sin incluir EPP, capacitaciones ni medidas a largo plazo).
                ⚠️ NO INCLUIR:
                - Elementos de protección personal (EPP)
                - Capacitaciones, simulacros ni controles administrativos
                - Medidas que no puedan aplicarse inmediatamente durante la ejecución de la tarea
                REGLAS DE RELACIÓN ENTRE ELEMENTOS:
                - Un mismo peligro puede dar lugar a múltiples riesgos distintos.
                - Un mismo riesgo puede abordarse con varias acciones correctivas inmediatas.
                - Las listas no necesariamente deben tener la misma longitud ni simetría.
                - No debe forzarse una correspondencia 1 a 1.
                - Escribí los peligros como fuentes de daño físicas, químicas, mecánicas, biológicas, ergonómicas, psicosociales o naturales (por ejemplo: "estructura baja", no "riesgo de golpe").
                - El orden de aparición de los riesgos debe guardar relación con el de los peligros del que derivan. Lo mismo se aplica entre los riesgos y sus acciones correctivas. Esto no implica una relación 1:1, pero sí una secuencia coherente.
                    Por Ejemplo: 
                    peligros: ["Estructura baja", "Superficie resbaladiza", ...],
                    riesgos: ["Golpes en la cabeza", "Traumatismos", "Caídas por resbalones", ...],
                    accionesCorrectivas: ["Colocar señalización de estructura baja", "Colocar protecciones mecanicas", "Colocar alfombras antideslizantes", ...]),
                FORMATO DE RESPUESTA: 
                Devolvé la información en formato JSON estricto con las siguientes claves:
                - peligros: lista de 10 a 15 fuentes concretas de daño.
                - riesgos:  lista de 10 a 15 riesgos distintos derivados de los peligros.
                - accionesCorrectivas:  lista de 10 a 15 acciones inmediatas, orientadas a mitigar los riesgos.
                - observaciones: debe ser null si todo está correcto, o una cadena de texto explicando por qué no se puede analizar la tarea.
                🧪 EJEMPLO DE RESPUESTA FORMATEADA:
                { "peligros": [ ... ], "riesgos": [ ... ], "accionesCorrectivas": [ ... ], "observaciones": ... } 
                """
            });
        }
        private string ConstruirPromptUsuario(string tarea, string actividad, string areaTrabajo, string herramientas)
        {
            return $$$"""
            TRABAJO GENERAL: "{{{tarea}}}"
            Herramientas y equipos previstos para este trabajo: {{{herramientas}}}. Si no se especifican, inferí herramientas comunes según la tarea.
            Área de trabajo (lugar, locación, planta, etc.): {{{areaTrabajo}}}
            TAREA A ANALIZAR: "{{{actividad}}}"
            """;
        }
        public async Task<SugerenciaIA?> AgregarTareaYObtenerRespuestaAsync(string tarea, string actividad, string areaTrabajo, string herramientas)
        {
            var promptUser = ConstruirPromptUsuario(tarea, actividad, areaTrabajo, herramientas);

            _historial.Add(new MensajeIA
            {
                Role = "user",
                Content = promptUser
            });

            return await ObtenerSugerenciasAsync(_historial);
        }
        public async Task<SugerenciaIA?> AgregarMensajeUsuarioAsync(string mensajeUsuario)
        {
            _historial.Add(new MensajeIA
            {
                Role = "user",
                Content = mensajeUsuario
            });
            return await ObtenerSugerenciasAsync(_historial);
        }
        private async Task<SugerenciaIA?> ObtenerSugerenciasAsync(List<MensajeIA> historial)
        {
            var requestBody = new
            {
                model = _model,
                messages = historial.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
                temperature = 0.2,
                max_tokens = 6000
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var content = JsonDocument.Parse(json).RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var jsonLimpio = ExtraerJson(content);

            try
            {
                var sugerencia = JsonSerializer.Deserialize<SugerenciaIA>(jsonLimpio!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return sugerencia;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error al deserializar:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Contenido recibido:");
                Console.WriteLine(jsonLimpio);
                return null;
            }
        }
        private string? ExtraerJson(string? contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
                return null;

            var inicio = contenido.IndexOf('{');
            var fin = contenido.LastIndexOf('}');
            if (inicio >= 0 && fin > inicio)
            {
                return contenido.Substring(inicio, fin - inicio + 1);
            }

            return contenido;
        }
        public List<MensajeIA> ObtenerHistorial() => _historial;
        public Task LimpiarHistorialAsync()
        {
            _historial.Clear();
            return Task.CompletedTask;
        }

        public async Task<PuntajeAST?> PuntuarAstDesdeJsonAsync(Ast ast)
        {
            var json = JsonSerializer.Serialize(ast);
            var prompt = $$$"""
    A continuación se presenta una AST (Análisis de Seguridad en el Trabajo) en formato JSON. Necesito que evalúes la calidad general de esta AST y devuelvas:

    - Un puntaje numérico del 0 al 100 (entero), que va a ser la suma de cada uno de los puntajes por punto clave.
    - Un comentario con una devolución general sobre la calidad de los ítems, redacción, relación entre elementos y completitud.

    Consideraciones importantes:
    - ¿Los peligros están bien redactados como fuentes concretas de daño (físicas, químicas, etc.)? - 10 ptos.
    - ¿Los riesgos derivan lógicamente de los peligros? - 10 ptos.
    - ¿Las acciones correctivas se aplican directamente para mitigar esos riesgos? - 10 ptos.
    - ¿Las listas son variadas, completas y coherentes? - 10 ptos.
    - ¿Se evita repetir peligros genéricos o acciones poco útiles? - 10 ptos.
    - ¿La cantidad de ítems es suficiente (al menos 3 por lista y por tarea)? - 10 ptos.
    - ¿Las tareas guardan relacion con el trabajo general y entre sí? - 10 ptos.
    - ¿Se evita el uso de EPP, capacitaciones o medidas a largo plazo en las acciones correctivas? - 10 ptos.
    - ¿Se respetan las reglas de relación entre peligros, riesgos y acciones correctivas? - 10 ptos.
    - ¿Los EPP Necesarios, son suficientes y adecuados? - 10 ptos.

    FORMATO DE RESPUESTA ESTRICTO:
    {{
        "puntaje": <número entre 0 y 100>,
        "comentario": "<devolución detallada>"
    }}

    AST A EVALUAR (JSON):
    {{{json}}}
    """;

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
            new { role = "user", content = prompt }
        },
                temperature = 0.2,
                max_tokens = 2000
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            json = await response.Content.ReadAsStringAsync();
            var content = JsonDocument.Parse(json).RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var jsonLimpio = ExtraerJson(content);
            if (jsonLimpio == null) return null;

            try
            {
                return JsonSerializer.Deserialize<PuntajeAST>(jsonLimpio, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al deserializar PuntajeAST: " + ex.Message);
                Console.WriteLine("Contenido: " + jsonLimpio);
                return null;
            }
        }
    }
}
