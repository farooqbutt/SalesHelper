using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SalesHelper.Data;
using SalesHelper.Models;
using System.Security.Claims;

namespace SalesHelper.Services.QuestPDF
{
    public class GeneratePDFService : IGeneratePDFService
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CabinetQuotationService _cabinetQuotationService;
        private readonly CountertopQuotationService _countertopQuotationService;
        private readonly VendorService _vendorService;
        private readonly CustomerService _customerService;
        private readonly AddressService _addressService;
        public GeneratePDFService(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            CabinetQuotationService cabinetQuotationService,
            VendorService vendorService,
            CountertopQuotationService countertopQuotationService,
            CustomerService customerService,
            AddressService addressService
            )
        {
            _context = context;
            _signInManager = signInManager;
            _cabinetQuotationService = cabinetQuotationService;
            _vendorService = vendorService;
            _countertopQuotationService = countertopQuotationService;
            _customerService = customerService;
            _addressService = addressService;
        }

        public byte[] GenerateCabinetQuoteEstimateRequestPDF(int id, ClaimsPrincipal user)
        {
            CabinetQuotation quoteData = _cabinetQuotationService.Read(id);
            Account dealerData = _context.Account.Include(a => a.BusinessAddress).FirstOrDefault(a => a.AccountNumber ==
                                 _signInManager.UserManager.GetUserAsync(user).Result.AccountNumber)!;
            Vendor cabinetryData = _vendorService.Read(quoteData.VendorId);
            var quoteItems = _context.QuotationItems.Where(a => a.QuotationId == id).ToList().GroupBy(a => a.DoorStyleName);

            // generate pdf
            return
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Header().Height(80).Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            var titleStyle = TextStyle.Default.FontSize(20).FontColor("#00b5b8").SemiBold();
                            column.Item().Height(40).Text("Estimate Request").Style(titleStyle);

                            column.Item().Height(20);

                            column.Item().Text(text =>
                            {
                                text.Span("Reference # ").SemiBold();
                                text.Span(quoteData.Id.ToString());
                            });
                        });
                        row.RelativeItem().AlignRight().Column(column =>
                        {
                            // logo for company
                            column.Item().Height(40);

                            column.Item().Height(20);

                            column.Item().Text(text =>
                            {
                                text.Span("Requested Date: ").SemiBold();
                                text.Span(DateTime.Now.ToShortDateString());
                            });
                        });
                    });
                    page.Content().PaddingVertical(30).Column(column =>
                    {
                        // dealer and cabinetry details
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Dealer").SemiBold();
                                column.Item().BorderBottom(1);
                                if (!string.IsNullOrEmpty(dealerData.CompanyName))
                                {
                                    column.Item().Text(dealerData.CompanyName);
                                }
                                // make sure business address is not null
                                if (dealerData.BusinessAddress != null)
                                {
                                    if (!string.IsNullOrEmpty(dealerData.BusinessAddress.Address1))
                                    {
                                        column.Item().Text(dealerData.BusinessAddress.Address1);
                                    }
                                    if (!string.IsNullOrEmpty(dealerData.BusinessAddress.Address2))
                                    {
                                        column.Item().Text(dealerData.BusinessAddress.Address2);
                                    }
                                }
                                if (!string.IsNullOrEmpty(dealerData.Email))
                                {
                                    column.Item().Text(dealerData.Email);
                                }
                                if (!string.IsNullOrEmpty(dealerData.PrimaryContactPhone))
                                {
                                    column.Item().Text("Phone: " + dealerData.PrimaryContactPhone);
                                }
                                if (!string.IsNullOrEmpty(dealerData.Fax))
                                {
                                    column.Item().Text("Fax: " + dealerData.Fax);
                                }

                            });
                            row.ConstantItem(100);
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Cabinetry").SemiBold();
                                column.Item().BorderBottom(1);
                                if (!string.IsNullOrEmpty(cabinetryData.CompanyName))
                                {
                                    column.Item().Text(cabinetryData.CompanyName);
                                }
                                // make sure business address is not null
                                if (cabinetryData.BusinessAddressIdFK != null)
                                {
                                    if (!string.IsNullOrEmpty(cabinetryData.BusinessAddressIdFK.Address1))
                                    {
                                        column.Item().Text(cabinetryData.BusinessAddressIdFK.Address1);
                                    }
                                    if (!string.IsNullOrEmpty(cabinetryData.BusinessAddressIdFK.Address2))
                                    {
                                        column.Item().Text(cabinetryData.BusinessAddressIdFK.Address2);
                                    }
                                }
                                if (!string.IsNullOrEmpty(cabinetryData.Email))
                                {
                                    column.Item().Text(cabinetryData.Email);
                                }
                                if (!string.IsNullOrEmpty(cabinetryData.PrimaryContactPhone))
                                {
                                    column.Item().Text("Phone: " + cabinetryData.PrimaryContactPhone);
                                }
                                if (!string.IsNullOrEmpty(cabinetryData.Fax))
                                {
                                    column.Item().Text("Fax: " + cabinetryData.Fax);
                                }
                            });
                        });

                        // door style name if door style name if this is a one color design cabinet
                        if (quoteData.isOneColorDesign == true)
                        {
                            column.Item().PaddingTop(20).Text(text =>
                            {
                                text.Span("DOOR STYLE : ").SemiBold().FontColor("#00b5b8");
                                text.Span(quoteData.DoorStyleForOneColorDesign!.ToUpper());
                            });
                        }

                        // table for showing items
                        column.Item().PaddingVertical(20).Table(table =>
                        {
                            // define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                            });

                            // add header
                            table.Header(header =>
                            {
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(a => a.SemiBold()).
                                    BorderVertical(1).BorderHorizontal(1).PaddingVertical(1)
                                    .PaddingHorizontal(2).AlignCenter();
                                }
                                header.Cell().Element(CellStyle).Text("Qty");
                                header.Cell().Element(CellStyle).Text("Item");
                                header.Cell().Element(CellStyle).Text("Description");
                                header.Cell().Element(CellStyle).Text("Hinge");
                                header.Cell().Element(CellStyle).Text("Finish");
                                header.Cell().Element(CellStyle).Text("Price");
                                header.Cell().Element(CellStyle).Text("Modification");
                            });


                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(a => a.FontSize(10)).
                                BorderVertical(1).BorderHorizontal(1).PaddingVertical(2)
                                .PaddingHorizontal(5);
                            }

                            // add items
                            foreach (var doorStyles in quoteItems)
                            {
                                // add door style name when there are multiple door styles
                                if (quoteData.isOneColorDesign == false)
                                {
                                    table.Cell().ColumnSpan(7).Element(CellStyle).AlignCenter().Text(doorStyles.Key).SemiBold();
                                }
                                foreach (var item in doorStyles)
                                {
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                                    table.Cell().Element(CellStyle).Text(item.Item);
                                    table.Cell().Element(CellStyle).Text(item.Description);
                                    table.Cell().Element(CellStyle).Text(item.Hinge);
                                    table.Cell().Element(CellStyle).Text(item.Finish);
                                    table.Cell().Element(CellStyle).AlignRight().Text("");
                                    table.Cell().Element(CellStyle).Text(""); // for modification
                                }
                            }
                        });
                    });
                    page.Footer().Height(20).Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Thanks for your business.");
                        });
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GenerateCountertopQuoteEstimateRequestPDF(int quoteId, int vendorId, ClaimsPrincipal user)
        {
            CountertopQuotation quoteData = _countertopQuotationService.Read(quoteId);
            quoteData.CustomerIdFk = _customerService.Read(quoteData.CustomerId);
            quoteData.CustomerIdFk.AddressIdFK = _addressService.Read(quoteData.CustomerIdFk.AddressId);
            var quotationMaterials = _context.CountertopMaterials.Where(a => a.CountertopQuotationId == quoteId &&
                                                                             a.VendorId == vendorId).Include(a => a.VendorIdFk).ToList()
                                                                             .GroupBy(cm => cm.Material)
                                                                             .Select(g1 => new
                                                                             {
                                                                                 Material = g1.Key,
                                                                                 Brands = g1.GroupBy(cm => cm.Brand)
                                                                                            .Select(g2 => new
                                                                                            {
                                                                                                Brand = g2.Key,
                                                                                                Colors = g2.Select(cm => cm.Color)
                                                                                            })
                                                                             });
            Vendor fabricator = _vendorService.Read(vendorId);

            // generate pdf
            return
            Document.Create(container =>
            {
                _ = container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Header().Height(80).Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            var titleStyle = TextStyle.Default.FontSize(20).FontColor("#00b5b8").SemiBold();
                            column.Item().Height(40).Text("Estimate Request").Style(titleStyle);

                            column.Item().Height(20);

                            column.Item().Text(text =>
                            {
                                text.Span("Reference # ").SemiBold();
                                text.Span(quoteData.Id.ToString());
                            });
                        });
                        row.RelativeItem().AlignRight().Column(column =>
                        {
                            // logo for company
                            column.Item().Height(40);

                            column.Item().Height(20);

                            column.Item().Text(text =>
                            {
                                text.Span("Requested Date: ").SemiBold();
                                text.Span(DateTime.Now.ToShortDateString());
                            });
                        });
                    });
                    page.Content().PaddingVertical(30).Column(column =>
                    {
                        // dealer and cabinetry details
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Fabricator").SemiBold();
                                column.Item().BorderBottom(1);
                                if (!string.IsNullOrEmpty(fabricator.CompanyName))
                                {
                                    column.Item().Text(fabricator.CompanyName);
                                }
                                // make sure business address is not null
                                if (fabricator.BusinessAddressIdFK != null)
                                {
                                    if (!string.IsNullOrEmpty(fabricator.BusinessAddressIdFK.Address1))
                                    {
                                        column.Item().Text(fabricator.BusinessAddressIdFK.Address1);
                                    }
                                    if (!string.IsNullOrEmpty(fabricator.BusinessAddressIdFK.Address2))
                                    {
                                        column.Item().Text(fabricator.BusinessAddressIdFK.Address2);
                                    }
                                }
                                if (!string.IsNullOrEmpty(fabricator.Email))
                                {
                                    column.Item().Text(fabricator.Email);
                                }
                                if (!string.IsNullOrEmpty(fabricator.PrimaryContactPhone))
                                {
                                    column.Item().Text("Phone: " + fabricator.PrimaryContactPhone);
                                }
                                if (!string.IsNullOrEmpty(fabricator.Fax))
                                {
                                    column.Item().Text("Fax: " + fabricator.Fax);
                                }
                            });
                            row.ConstantItem(100);
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Countertop Details").SemiBold();
                                column.Item().BorderBottom(1);
                                if (!string.IsNullOrEmpty(quoteData.RoomType))
                                {
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Room Type: ").SemiBold().FontSize(11);
                                        text.Span(quoteData.RoomType);
                                    });
                                }
                                if (!string.IsNullOrEmpty(quoteData.EdgeProfile))
                                {
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Edge Profile: ").SemiBold().FontSize(11);
                                        text.Span(quoteData.EdgeProfile);
                                    });
                                }
                                if (!string.IsNullOrEmpty(quoteData.EstimateSquareFeet))
                                {
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Estimate Square Feet: ").SemiBold().FontSize(11);
                                        text.Span(quoteData.EstimateSquareFeet);
                                    });
                                }

                                // add additional information if there are any
                                int itemCount = 0;
                                string comma = "";

                                column.Item().PaddingTop(5).BorderBottom(1).Text("Additional Information").SemiBold();
                                column.Item().Text(text =>
                                {
                                    if (quoteData.MiteredEdge == true)
                                    {
                                        text.Span($"{comma}Mitered Edge");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.FarmhouseSink == true)
                                    {
                                        text.Span($"{comma}Farmhouse Sink");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.Cooktop == true)
                                    {
                                        text.Span($"{comma}Cooktop");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.FullBacksplash == true)
                                    {
                                        text.Span($"{comma}Full Backsplash");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.MiteredEdgeOnIsland == true)
                                    {
                                        text.Span($"{comma}Mitered Edge on Island");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.IslandPrepSink == true)
                                    {
                                        text.Span($"{comma}Island Prep Sink");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.Rangetop == true)
                                    {
                                        text.Span($"{comma}Rangetop");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.FourIncBacksplash == true)
                                    {
                                        text.Span($"{comma}4\" Backsplash");
                                        itemCount++;
                                        comma = ", ";
                                    }
                                    if (quoteData.WaterfallOnIsland == true)
                                    {
                                        text.Span($"{comma}Waterfall on Island");
                                        itemCount++;
                                        comma = ", ";
                                    }

                                    // If there are no items, add "None"
                                    if (itemCount == 0)
                                    {
                                        text.Span("None");
                                    }
                                });
                            });
                        });

                        // table for showing items
                        column.Item().PaddingVertical(20).Table(table =>
                        {
                            // define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // add header
                            table.Header(header =>
                            {
                                // style for header
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(a => a.FontSize(10).SemiBold()).
                                    BorderVertical(1).BorderHorizontal(1).PaddingVertical(1)
                                    .PaddingHorizontal(2).AlignCenter();
                                }
                                header.Cell().Element(CellStyle).Text("Material");
                                header.Cell().Element(CellStyle).Text("Brand");
                                header.Cell().Element(CellStyle).Text("Color");
                                header.Cell().Element(CellStyle).Text("Square Feet");
                                header.Cell().Element(CellStyle).Text("Vendor Rate");
                                header.Cell().Element(CellStyle).Text("Price");
                            });


                            // style for cells
                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(a => a.FontSize(10)).
                                BorderVertical(1).BorderHorizontal(1).PaddingVertical(2)
                                .PaddingHorizontal(5).AlignCenter().AlignMiddle();
                            }

                            // add items
                            foreach (var material in quotationMaterials)
                            {
                                // get the total number of colors for the material to set the row span
                                int colorCount = 0;
                                foreach (var brands in material.Brands)
                                {
                                    foreach (var color in brands.Colors)
                                    {
                                        colorCount++;
                                    }
                                }
                                table.Cell().RowSpan((uint)colorCount).Element(CellStyle).Text(material.Material);
                                foreach (var brands in material.Brands)
                                {
                                    // get the total number of colors for the brand to set the row span
                                    int colorCountForBrand = 0;
                                    foreach (var color in brands.Colors)
                                    {
                                        colorCountForBrand++;
                                    }
                                    table.Cell().RowSpan((uint)colorCountForBrand).Element(CellStyle).Text(brands.Brand);
                                    foreach (var color in brands.Colors)
                                    {
                                        table.Cell().Element(CellStyle).Text(color);
                                        table.Cell().Element(CellStyle).Text(quoteData.EstimateSquareFeet);
                                        table.Cell().Element(CellStyle).Text("");
                                        table.Cell().Element(CellStyle).Text("");
                                    }
                                }
                            }
                        });
                    });
                    page.Footer().Height(20).Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Thanks for your business.");
                        });
                    });
                });
            }).GeneratePdf();
        }
    }
}
