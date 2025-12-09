//API manejador de solicitudes HTTP relacionadas con la gestión de reportes en la base de datos.
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FolioService _folioService;

        public ReportesController(AppDbContext context, FolioService folioService)
        {
            _context = context;
            _folioService = folioService;
        }

        [HttpPost]
        public IActionResult CrearReporte([FromBody] Reporte reporte)
        {
            // Generar folio unico
            reporte.Folio = _folioService.GenerarFolio(reporte.Categoria, reporte.Subcategoria);

            _context.Reportes.Add(reporte);
            _context.SaveChanges();

            return Ok(new
            {
                mensaje = "Reporte creado correctamente",
                folio = reporte.Folio
            });
        }

        [HttpGet]
        public IActionResult ObtenerReportes()
        {
            return Ok(_context.Reportes.ToList());
        }
    }
}
