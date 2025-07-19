using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Collections.Generic;
using AstApp.Models;

namespace AstApp.Servicios
{
    public class AstStorageService
    {
        private readonly IJSRuntime _js;
        private const string StorageKey = "ast-list";
        private const string DraftsKey = "ast-drafts";

        public AstStorageService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<List<Ast>> GetAllAsync()
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrEmpty(json))
                return new List<Ast>();

            return JsonSerializer.Deserialize<List<Ast>>(json) ?? new List<Ast>();
        }

        public async Task SaveDraftAsync(Ast borrador)
        {
            var lista = await GetAllDraftsAsync();

            var index = lista.FindIndex(a => a.Id == borrador.Id);
            if (index >= 0)
                lista[index] = borrador;
            else
                lista.Add(borrador);

            var json = JsonSerializer.Serialize(lista);
            await _js.InvokeVoidAsync("localStorage.setItem", DraftsKey, json);
        }
        public async Task<List<Ast>> GetAllDraftsAsync()
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", DraftsKey);
            if (string.IsNullOrEmpty(json))
                return new List<Ast>();

            return JsonSerializer.Deserialize<List<Ast>>(json) ?? new List<Ast>();
        }

        public async Task DeleteDraftAsync(string id)
        {
            var lista = await GetAllDraftsAsync();
            var item = lista.FirstOrDefault(a => a.Id == id);
            if (item != null)
            {
                lista.Remove(item);
                var json = JsonSerializer.Serialize(lista);
                await _js.InvokeVoidAsync("localStorage.setItem", DraftsKey, json);
            }
        }

        public async Task ClearDraftsAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", DraftsKey);
        }

        public async Task SaveAsync(Ast nuevaAst)
        {
            var lista = await GetAllAsync();

            // Buscar si ya existe una AST con el mismo Id
            var existenteIndex = lista.FindIndex(a => a.Id == nuevaAst.Id);

            if (existenteIndex >= 0)
            {
                // Si ya existe, reemplazar
                lista[existenteIndex] = nuevaAst;
            }
            else
            {
                // Si no existe, agregar nueva
                lista.Add(nuevaAst);
            }

            var json = JsonSerializer.Serialize(lista);
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }


        public async Task DeleteAsync(Ast ast)
        {
            var lista = await GetAllAsync();

            var item = lista.FirstOrDefault(a => a.Id == ast.Id);
            if (item != null)
            {
                lista.Remove(item);
                var json = JsonSerializer.Serialize(lista);
                await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            }
        }

        public async Task ClearAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
    }

}
