using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestPDF.Infrastructure;
using static TerrainEnumClass;
using static TextUtilities;
using UnityEngine.UI;
using System.Text;

public class PdfGenerator : MonoBehaviour
{
    public List<TerrainPieces> scenarioTerrain;
    public Text scenarioName;
    public Text descriptionText;
    public Text goalText;
    public Text setupText;
    public Text victoryText;
    public Text rulesText;

    public Button printButton;

    // Start is called before the first frame update
    void Start()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        printButton.onClick.AddListener(PrintOnClick);
    }

    void PrintOnClick()
    {
        // code in your main method
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                /*page.Header()
                    .Text(scenarioName.text)
                    .SemiBold().FontSize(28).FontColor(Colors.Black);*/

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(10);

                        x.Item().Text(scenarioName.text)
                        .SemiBold().FontSize(20).FontColor(Colors.Black);
                        x.Item().Text(descriptionText.text).Italic();
                        x.Item().Text(text =>
                        {
                            text.ParagraphSpacing(20);
                            text.Span("GOAL: ").Bold();
                            text.Span(goalText.text);
                        });
                        x.Item().Text(text =>
                        {
                            text.ParagraphSpacing(20);
                            text.Span("SETUP: ").Bold();
                            text.Span(setupText.text);
                        });
                        x.Item().Text(text =>
                        {
                            text.ParagraphSpacing(20);
                            text.Span("VICTORY: ").Bold();
                            text.Span(victoryText.text);
                        });
                        x.Item().Text(text =>
                        {
                            text.ParagraphSpacing(20);
                            text.Span("SPECIAL RULES: ").Bold();
                            text.Span(rulesText.text);
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf(TextUtilities.RemoveSpecialCharacters(scenarioName.text) + ".pdf");
    }
}

