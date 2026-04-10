using Identity.Infrastructure.Adapters;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;

namespace Identity.Infrastructure.Extensions.Validation;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection svc)
    {
        svc.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");
        var applicationAssembly = Assembly.Load(ApiConstants.ApplicationProject);
        svc.AddValidatorsFromAssembly(applicationAssembly);
        svc.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        return svc;
    }
}
