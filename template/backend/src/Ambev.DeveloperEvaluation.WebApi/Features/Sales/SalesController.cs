using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AppCreateSale = Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using AppGetSale = Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using AppUpdateSale = Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using AppDeleteSale = Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using WebApiCreateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using WebApiGetSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using WebApiUpdateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using WebApiDeleteSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<WebApiCreateSale.CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] WebApiCreateSale.CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new WebApiCreateSale.CreateSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<AppCreateSale.CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty, new ApiResponseWithData<WebApiCreateSale.CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<WebApiCreateSale.CreateSaleResponse>(response)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<WebApiGetSale.GetSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new WebApiGetSale.GetSaleRequest { Id = id };
            var validator = new WebApiGetSale.GetSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var query = _mapper.Map<AppGetSale.GetSaleQuery>(request.Id);
            var response = await _mediator.Send(query, cancellationToken);

            if (response == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Sale not found." });
            }

            return Ok(new ApiResponseWithData<WebApiGetSale.GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<WebApiGetSale.GetSaleResponse>(response)
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<WebApiUpdateSale.UpdateSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] WebApiUpdateSale.UpdateSaleRequest request, CancellationToken cancellationToken)
        {
            request.Id = id; // Ensure the ID from the route matches the request body
            var validator = new WebApiUpdateSale.UpdateSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<AppUpdateSale.UpdateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            if (response == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Sale not found." });
            }

            return Ok(new ApiResponseWithData<WebApiUpdateSale.UpdateSaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = _mapper.Map<WebApiUpdateSale.UpdateSaleResponse>(response)
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new WebApiDeleteSale.DeleteSaleRequest { Id = id };
            var validator = new WebApiDeleteSale.DeleteSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<AppDeleteSale.DeleteSaleCommand>(request.Id);
            var response = await _mediator.Send(command, cancellationToken);

            if (!response.Success)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Sale not found or could not be deleted." });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Sale deleted successfully"
            });
        }
    }
}