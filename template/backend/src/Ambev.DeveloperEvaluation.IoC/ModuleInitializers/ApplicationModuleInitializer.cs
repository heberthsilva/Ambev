using Ambev.DeveloperEvaluation.Common.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(Ambev.DeveloperEvaluation.Application.ApplicationLayer))!));
        builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Ambev.DeveloperEvaluation.Application.ApplicationLayer)));
    }
}