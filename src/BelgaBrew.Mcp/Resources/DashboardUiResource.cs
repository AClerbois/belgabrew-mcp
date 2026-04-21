using System.ComponentModel;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Resources;

[McpServerResourceType]
public class DashboardUiResource
{
    [McpServerResource(
        UriTemplate = "ui://belgabrew/inventory-dashboard",
        Name = "Dashboard inventaire",
        MimeType = "text/html;profile=mcp-app")]
    [Description("Widget interactif affichant l'inventaire en temps réel avec filtres.")]
    public string GetDashboardHtml()
    {
        return """
        <!DOCTYPE html>
        <html>
        <head>
        <style>
          body { font-family: system-ui; margin: 0; padding: 16px; background: #faf6f0; }
          .card { background: white; border-radius: 12px; padding: 16px; margin-bottom: 12px;
                  box-shadow: 0 2px 8px rgba(0,0,0,0.06); }
          .low  { border-left: 4px solid #e74c3c; }
          .ok   { border-left: 4px solid #27ae60; }
          button { background: #c0392b; color: white; border: 0; padding: 8px 16px;
                   border-radius: 6px; cursor: pointer; }
          h2 { margin: 0 0 12px; color: #6d4c1f; }
          .qty { font-size: 1.4em; font-weight: 600; }
        </style>
        </head>
        <body>
        <h2>🍺 BelgaBrew — Inventaire</h2>
        <div id="root">Chargement…</div>
        <button onclick="reorder()">Commander les items en rupture</button>
        <script>
        async function callTool(name, args) {
          return new Promise((resolve) => {
            const id = Math.random().toString(36);
            window.addEventListener('message', function onMsg(e) {
              if (e.data?.id === id) { window.removeEventListener('message', onMsg); resolve(e.data.result); }
            });
            window.parent.postMessage({ jsonrpc: '2.0', id, method: 'tools/call', params: { name, arguments: args } }, '*');
          });
        }
        async function render() {
          const skus = ['HOP-SAAZ-001', 'MALT-PILS-001', 'YEAST-T58-001'];
          const results = await Promise.all(skus.map(sku => callTool('check_ingredient_stock', { sku })));
          document.getElementById('root').innerHTML = results.map(r => `
            <div class="card ${r.lowStock ? 'low' : 'ok'}">
              <div><strong>${r.name}</strong> <small>(${r.sku})</small></div>
              <div class="qty">${r.quantityKg} kg ${r.lowStock ? '⚠️' : '✅'}</div>
            </div>`).join('');
        }
        async function reorder() { await callTool('place_reorder', { auto: true }); alert('Commande passée 🚚'); render(); }
        render();
        </script>
        </body>
        </html>
        """;
    }
}
