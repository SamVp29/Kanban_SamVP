using System.Drawing;
using Kanban.Domain.Models;
using Kanban.Domain.Ports.Out;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Kanban.Infrastructure.Reports;

public class ExcelReportGenerator : IReportGenerator
{
    public string Format => "excel";

    public byte[] Generate(ProyectoReportData data)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Developer");

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Tablero Kanban");

        // Título del proyecto
        ws.Cells["A1:E1"].Merge = true;
        ws.Cells["A1"].Value = $"Proyecto: {data.NombreProyecto}";
        ws.Cells["A1"].Style.Font.Size = 16;
        ws.Cells["A1"].Style.Font.Bold = true;

        ws.Cells["A2:E2"].Merge = true;
        ws.Cells["A2"].Value = data.DescripcionProyecto;
        ws.Cells["A2"].Style.Font.Italic = true;

        ws.Cells["A3:E3"].Merge = true;
        ws.Cells["A3"].Value = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}";

        int row = 5;

        foreach (var col in data.Columnas)
        {
            ws.Cells[row, 1, row, 5].Merge = true;
            ws.Cells[row, 1].Value = $"Columna: {col.NombreColumna}";
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            row++;

            if (!col.Tareas.Any())
            {
                ws.Cells[row, 1, row, 5].Merge = true;
                ws.Cells[row, 1].Value = "Sin tareas";
                ws.Cells[row, 1].Style.Font.Italic = true;
                row += 2;
                continue;
            }

            // Headers
            ws.Cells[row, 1].Value = "Título";
            ws.Cells[row, 2].Value = "Descripción";
            ws.Cells[row, 3].Value = "Prioridad";
            ws.Cells[row, 4].Value = "Responsable";
            ws.Cells[row, 5].Value = "Fecha Creación";

            using (var range = ws.Cells[row, 1, row, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
            row++;

            foreach (var tarea in col.Tareas)
            {
                ws.Cells[row, 1].Value = tarea.Titulo;
                ws.Cells[row, 2].Value = tarea.Descripcion;
                ws.Cells[row, 3].Value = tarea.Prioridad;
                ws.Cells[row, 4].Value = tarea.Responsable ?? "-";
                ws.Cells[row, 5].Value = tarea.FechaCreacion.ToString("dd/MM/yyyy");
                row++;
            }

            row++; // Spacing
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }
}
