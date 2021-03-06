using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Navigation;
using OrchardCore.GitHub.Configuration;
using OrchardCore.GitHub.Drivers;
using OrchardCore.GitHub.Services;
using OrchardCore.Modules;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using OrchardCore.Recipes;
using OrchardCore.GitHub.Recipes;

namespace OrchardCore.GitHub
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionProvider, Permissions>();
        }
    }

    [Feature(GitHubConstants.Features.GitHubAuthentication)]
    public class GitHubLoginStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGitHubAuthenticationService, GitHubAuthenticationService>();
            services.AddScoped<IDisplayDriver<ISite>, GitHubAuthenticationSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenuGitHubLogin>();
            services.AddRecipeExecutionStep<GitHubAuthenticationSettingsStep>();
            // Register the options initializers required by the GitHub Handler.
            services.TryAddEnumerable(new[]
            {
                // Orchard-specific initializers:
                ServiceDescriptor.Transient<IConfigureOptions<AuthenticationOptions>, GitHubOptionsConfiguration>(),
                ServiceDescriptor.Transient<IConfigureOptions<GitHubOptions>, GitHubOptionsConfiguration>(),
                // Built-in initializers:
                ServiceDescriptor.Transient<IPostConfigureOptions<GitHubOptions>, OAuthPostConfigureOptions<GitHubOptions,GitHubHandler>>()
            });
        }
    }
}
