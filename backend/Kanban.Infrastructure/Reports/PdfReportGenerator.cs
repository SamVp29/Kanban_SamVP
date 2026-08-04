using Kanban.Application.DTOs;
using Kanban.Application.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Kanban.Infrastructure.Reports;

public class PdfReportGenerator : IReportGenerator
{
    public string Format => "pdf";

    public byte[] Generate(ProyectoReportDto data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, data));
                page.Content().Element(c => ComposeContent(c, data));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, ProyectoReportDto data)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text($"Proyecto: {data.NombreProyecto}").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text(data.DescripcionProyecto).FontSize(14).FontColor(Colors.Grey.Darken2);
                column.Item().Text($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
            });
        });
    }

    private void ComposeContent(IContainer container, ProyectoReportDto data)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            foreach (var col in data.Columnas)
            {
                column.Item().PaddingBottom(5).Text(col.NombreColumna).FontSize(16).SemiBold().FontColor(Colors.Black);
                
                if (!col.Tareas.Any())
                {
                    column.Item().PaddingBottom(15).Text("Sin tareas").Italic().FontColor(Colors.Grey.Medium);
                    continue;
                }

                column.Item().PaddingBottom(15).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Título");
                        header.Cell().Element(CellStyle).Text("Descripción");
                        header.Cell().Element(CellStyle).Text("Prioridad");
                        header.Cell().Element(CellStyle).Text("Responsable");
                        header.Cell().Element(CellStyle).Text("Fecha");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var tarea in col.Tareas)
                    {
                        table.Cell().Element(CellStyle).Text(tarea.Titulo);
                        table.Cell().Element(CellStyle).Text(tarea.Descripcion);
                        table.Cell().Element(CellStyle).Text(tarea.Prioridad);
                        table.Cell().Element(CellStyle).Text(tarea.Responsable ?? "Sin asignar");
                        table.Cell().Element(CellStyle).Text(tarea.FechaCreacion.ToString("dd/MM/yyyy"));

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Página ");
            x.CurrentPageNumber();
            x.Span(" de ");
            x.TotalPages();
        });
    }
}
