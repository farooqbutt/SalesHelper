﻿using SalesHelper.Models;
using SalesHelper.Models.InterfaceModels;

namespace SalesHelper.Services
{
    public interface ICabinetQuotationService
    {
        public void Create(CabinetQuotation cabinetQuotation);
        public CabinetQuotation Read(int id);
        public List<CabinetQuotation> ReadAll();
        public void Update(CabinetQuotation cabinetQuotation);
        public void Delete(int id);

        // Cabinet Quotation Interface Creator
        public CabinetQuoteInterface CabinetQuoteInterface(CabinetQuotation cabinetQuotation);
    }
}
