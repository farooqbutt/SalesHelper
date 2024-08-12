using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public class SalesService : ISalesService
    {
        private readonly ApplicationDbContext _context;
        private readonly CabinetQuotationService _cabinetQuotationService;
        private readonly CountertopQuotationService _countertopQuotationService;
        private readonly CustomerService _customerService;
        private readonly VendorService _vendorService;

        public SalesService(
            ApplicationDbContext context,
            CabinetQuotationService cabinetQuotationService,
            CountertopQuotationService countertopQuotationService,
            CustomerService customerService,
            VendorService vendorService
            )
        {
            _context = context;
            _cabinetQuotationService = cabinetQuotationService;
            _countertopQuotationService = countertopQuotationService;
            _customerService = customerService;
            _vendorService = vendorService;
        }
        public void Create(CreateSaleInterface sale)
        {
            try
            {
                _context.Sales.Add(sale.SaleData);
                _context.SaveChanges();
                //assign sales id to each cabinet item and save to database
                foreach (var item in sale.CabinetItems)
                {
                    item.SalesId = sale.SaleData.Id;
                    _context.SalesCabinetItems.Add(item);
                }
                _context.SaveChanges();

                //assign sales id to each countertop item and save to database
                foreach (var item in sale.CountertopItems)
                {
                    item.SalesId = sale.SaleData.Id;
                    _context.SalesCountertopItems.Add(item);
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Sales.Remove(Read(id));
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Sale Read(int id)
        {
            try
            {
                return _context.Sales.Find(id)!;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Sale> ReadAll(int accountNumber)
        {
            try
            {
                return _context.Sales.Where(a => a.CreatedByFK.AccountNumber == accountNumber)
                    .Include(a => a.CreatedByFK)
                    .Include(a => a.CustomerFK)
                    .OrderByDescending(a => a.SalesDate).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Update(Sale sale)
        {
            try
            {
                _context.Sales.Update(sale);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateStatus(int id, string status)
        {
            try
            {
                var sale = Read(id);
                sale.Status = status;
                _context.Sales.Update(sale);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IActionResult SalePreviewGenerator(CreateSaleInterface sale)
        {
            var pdf = GetPdfDocument(sale, _customerService, _vendorService);
            return new FileContentResult(pdf, "application/pdf");
        }

        private static byte[] GetPdfDocument(CreateSaleInterface sale, CustomerService customerService, VendorService vendorService)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            var titleStyle = TextStyle.Default.FontSize(20).FontColor("#00b5b8").SemiBold();
                            column.Item().Height(40).Text("Sales Order".ToUpper()).Style(titleStyle);
                        });

                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Height(25).AlignBottom().AlignRight().Text(text =>
                            {
                                text.Span("Sales Date : ").SemiBold();
                                text.Span(sale.SaleData.SalesDate.ToShortDateString());
                            });
                        });
                    });
                    page.Content().Column(column =>
                    {
                        // fabricator and cabinetry details
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Column(column =>
                            {
                                column.Item().Border(1).AlignCenter().Text("Sales Order Information").FontColor("#00b5b8").SemiBold();
                                column.Item().PaddingLeft(4).Text(text =>
                                {
                                    text.Span("Customer Name: ").SemiBold();
                                    text.Span(customerService.Read(sale.SaleData.CustomerId).FirstName + " " +
                                              customerService.Read(sale.SaleData.CustomerId).LastName);
                                });
                                column.Item().PaddingLeft(4).Text(text =>
                                {
                                    text.Span("Cell Phone : ").SemiBold();
                                    text.Span(customerService.Read(sale.SaleData.CustomerId).CellPhone);
                                });
                                column.Item().PaddingLeft(4).Text(text =>
                                {
                                    text.Span("Email : ").SemiBold();
                                    text.Span(customerService.Read(sale.SaleData.CustomerId).Email);
                                });
                                column.Item().PaddingLeft(4).Text(text =>
                                {
                                    text.Span("Sale Status : ").SemiBold();
                                    text.Span(sale.SaleData.Status);
                                });
                            });
                            row.RelativeItem().Border(1).Column(column =>
                            {
                                column.Item().Border(1).AlignCenter().Text("Home Address").FontColor("#00b5b8").SemiBold();
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.Address1);
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.Address2);
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.City + ", " +
                                                                                customerService.Read(sale.SaleData.CustomerId).AddressIdFK.State + ", " +
                                                                                customerService.Read(sale.SaleData.CustomerId).AddressIdFK.Country);
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.PostalCode);
                            });
                            row.RelativeItem().Border(1).Column(column =>
                            {
                                column.Item().Border(1).AlignCenter().Text("Billing Address").FontColor("#00b5b8").SemiBold();
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.Address1);
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.Address2);
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.City + ", " +
                                                                                customerService.Read(sale.SaleData.CustomerId).AddressIdFK.State + ", " +
                                                                                customerService.Read(sale.SaleData.CustomerId).AddressIdFK.Country);
                                column.Item().AlignRight().PaddingRight(4).Text(customerService.Read(sale.SaleData.CustomerId).AddressIdFK.PostalCode);
                            });
                        });

                        // cabinet items
                        column.Item().PaddingTop(10).Text("Cabinet Items").FontColor("#00b5b8").SemiBold();

                        // table for showing items for cabinet
                        column.Item().PaddingVertical(10).Table(table =>
                        {
                            // define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // add header
                            table.Header(header =>
                            {
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(a => a.FontSize(9).SemiBold()).
                                    BorderVertical(1).BorderHorizontal(1).PaddingVertical(1)
                                    .PaddingHorizontal(2).AlignMiddle().AlignCenter();
                                }
                                header.Cell().Element(CellStyle).Text("Section");
                                header.Cell().Element(CellStyle).Text("Cabinetry");
                                header.Cell().Element(CellStyle).Text("Construction");
                                header.Cell().Element(CellStyle).Text("Box Materials");
                                header.Cell().Element(CellStyle).Text("Wood Species");
                                header.Cell().Element(CellStyle).Text("Door Style");
                                header.Cell().Element(CellStyle).Text("Finish");
                                header.Cell().Element(CellStyle).Text("Description");
                            });

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(a => a.FontSize(8)).
                                BorderVertical(1).BorderHorizontal(1).PaddingVertical(2)
                                .PaddingHorizontal(5).AlignCenter().AlignMiddle();
                            }

                            // table rows
                            foreach (var item in sale.CabinetItems)
                            {
                                table.Cell().Element(CellStyle).Text(item.SectionName);
                                table.Cell().Element(CellStyle).Text(vendorService.Read(item.VendorId).CompanyName);
                                table.Cell().Element(CellStyle).Text(item.Construction);
                                table.Cell().Element(CellStyle).Text(item.BoxMaterial);
                                table.Cell().Element(CellStyle).Text(item.WoodSpecie);
                                table.Cell().Element(CellStyle).Text(item.DoorStyle);
                                table.Cell().Element(CellStyle).Text(item.CabinetFinish);
                                table.Cell().Element(CellStyle).Text(item.Description);
                            }
                        });

                        // countertop items
                        column.Item().Text("Countertop Items").FontColor("#00b5b8").SemiBold();

                        // table for showing items for cabinet
                        column.Item().PaddingVertical(10).Table(table =>
                        {
                            // define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // add header
                            table.Header(header =>
                            {
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(a => a.FontSize(9).SemiBold()).
                                    BorderVertical(1).BorderHorizontal(1).PaddingVertical(1)
                                    .PaddingHorizontal(2).AlignMiddle().AlignCenter();
                                }
                                header.Cell().Element(CellStyle).Text("Material");
                                header.Cell().Element(CellStyle).Text("Brand");
                                header.Cell().Element(CellStyle).Text("Color");
                                header.Cell().Element(CellStyle).Text("Vendor");
                                header.Cell().Element(CellStyle).Text("Room Type");
                                header.Cell().Element(CellStyle).Text("Edge Profile");
                                header.Cell().Element(CellStyle).Text("Est. Sq Ft");
                                header.Cell().Element(CellStyle).Text("Unit Price");
                                header.Cell().Element(CellStyle).Text("Subtotal");
                            });

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(a => a.FontSize(8)).
                                BorderVertical(1).BorderHorizontal(1).PaddingVertical(2)
                                .PaddingHorizontal(5).AlignCenter().AlignMiddle();
                            }

                            // table rows
                            foreach (var item in sale.CountertopItems)
                            {
                                table.Cell().Element(CellStyle).Text(item.Material);
                                table.Cell().Element(CellStyle).Text(item.Brand);
                                table.Cell().Element(CellStyle).Text(item.Color);
                                if (item.VendorId == null)
                                {
                                    table.Cell().Element(CellStyle).Text("N/A");
                                }
                                else
                                {
                                    table.Cell().Element(CellStyle).Text(vendorService.Read(item.VendorId ?? 0).CompanyName);
                                }
                                table.Cell().Element(CellStyle).Text(item.RoomType);
                                table.Cell().Element(CellStyle).Text(item.EdgeProfile);
                                table.Cell().Element(CellStyle).Text(item.EstimatedSquareFeet.ToString());
                                table.Cell().Element(CellStyle).Text("$" + item.UnitPrice.ToString());
                                table.Cell().Element(CellStyle).Text("$" + (item.EstimatedSquareFeet * item.UnitPrice).ToString());
                            }
                            // calculate total of all countertop item's subtotal

                            table.Cell().ColumnSpan(7);
                            table.Cell().Element(CellStyle).Text("Total").AlignRight();
                            table.Cell().Element(CellStyle).Text(sale.CountertopItems.Sum(item => item.EstimatedSquareFeet * item.UnitPrice).ToString());
                        });

                        // sales order total calculations
                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Cabinet Subtotal").SemiBold();
                            });

                            calculationsRow.RelativeItem().AlignRight().Text("$" + sale.SaleData.CabinetSubTotal.ToString());
                        });

                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Countertop Subtotal").SemiBold();
                            });

                            calculationsRow.RelativeItem().AlignRight().Text("$" + sale.SaleData.CountertopSubTotal.ToString());
                        });

                        column.Item().AlignRight().Width(250).BorderBottom(1);

                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Invoice Total").SemiBold();
                            });

                            calculationsRow.RelativeItem().AlignRight().Text("$" + sale.SaleData.TotalSalesOrderAmount.ToString());
                        });

                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Discount").SemiBold();
                            });

                            calculationsRow.RelativeItem().AlignRight().Text("$" + sale.SaleData.Discount.ToString());
                        });

                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Tax").SemiBold();
                            });

                            calculationsRow.RelativeItem().AlignRight().Text(sale.SaleData.Tax.ToString() + "%");
                        });

                        column.Item().AlignRight().Width(250).BorderBottom(1);

                        var grandTotal = sale.SaleData.TotalSalesOrderAmount - sale.SaleData.Discount;
                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Grand Total").SemiBold();
                            });
                            grandTotal += grandTotal * sale.SaleData.Tax / 100;
                            calculationsRow.RelativeItem().AlignRight().Text("$" + grandTotal.ToString());
                        });

                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Paid to Date").SemiBold();
                            });

                            calculationsRow.RelativeItem().AlignRight().Text("$" + sale.SaleData.AdvancePaid.ToString());
                        });

                        column.Item().AlignRight().Width(250).Row(calculationsRow =>
                        {
                            calculationsRow.RelativeItem().Text(text =>
                            {
                                text.Span("Balance USD").SemiBold();
                            });
                            var balance = grandTotal - sale.SaleData.AdvancePaid;
                            calculationsRow.RelativeItem().AlignRight().Text("$" + balance.ToString());
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

            return pdf;
        }
    }
}
