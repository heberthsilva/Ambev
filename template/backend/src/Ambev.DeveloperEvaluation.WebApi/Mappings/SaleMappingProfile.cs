using AutoMapper;
using AppCreateSale = Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using AppGetSale = Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using AppUpdateSale = Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using AppDeleteSale = Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using WebApiCreateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using WebApiGetSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using WebApiUpdateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using WebApiDeleteSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    public class SaleMappingProfile : Profile
    {
        public SaleMappingProfile()
        {
            // Create Sale
            CreateMap<WebApiCreateSale.CreateSaleRequest, AppCreateSale.CreateSaleCommand>();
            CreateMap<WebApiCreateSale.CreateSaleItemRequest, AppCreateSale.CreateSaleItemCommand>();
            CreateMap<AppCreateSale.CreateSaleResponse, WebApiCreateSale.CreateSaleResponse>();

            // Get Sale
            CreateMap<WebApiGetSale.GetSaleRequest, AppGetSale.GetSaleQuery>();
            CreateMap<AppGetSale.GetSaleResponse, WebApiGetSale.GetSaleResponse>();
            CreateMap<AppGetSale.GetSaleItemResponse, WebApiGetSale.GetSaleItemResponse>();

            // Update Sale
            CreateMap<WebApiUpdateSale.UpdateSaleRequest, AppUpdateSale.UpdateSaleCommand>();
            CreateMap<WebApiUpdateSale.UpdateSaleItemRequest, AppUpdateSale.UpdateSaleItemCommand>();
            CreateMap<AppUpdateSale.UpdateSaleResponse, WebApiUpdateSale.UpdateSaleResponse>();

            // Delete Sale
            CreateMap<WebApiDeleteSale.DeleteSaleRequest, AppDeleteSale.DeleteSaleCommand>();
            CreateMap<AppDeleteSale.DeleteSaleResponse, WebApiDeleteSale.DeleteSaleResponse>();
        }
    }
}