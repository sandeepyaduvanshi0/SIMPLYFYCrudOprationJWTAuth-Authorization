using Client.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Client.Report
{
    public class PdfGenerator
    {
        public static byte[] GeneratePdf(List<Employee> employees)
        {
            // Create a memory stream to hold the PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create a Document object
                Document document = new Document();

                try
                {
                    // Set up the PDF writer
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Add a title to the document
                    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                    Paragraph title = new Paragraph("Employee Details", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20f
                    };
                    document.Add(title);

                    // Add employee details
                    Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                    foreach (Employee emp in employees)
                    {
                        document.Add(new Paragraph($"Name: {emp.name}", bodyFont));
                        document.Add(new Paragraph($"Department: {emp.department}", bodyFont));
                        document.Add(new Paragraph(" ", bodyFont)); // Spacing between entries
                    }
                }
                finally
                {
                    document.Close();
                }

                // Return the generated PDF as a byte array
                return memoryStream.ToArray();
            }
        }
    }
}