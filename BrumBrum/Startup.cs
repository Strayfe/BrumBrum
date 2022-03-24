using System;
using BrumBrum.Commands;
using BrumBrum.Configuration;
using BrumBrum.Consumers;
using BrumBrum.Models;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BrumBrum
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration) => _configuration = configuration;
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            
            var queueConfiguration = new QueueConfiguration();
            _configuration.GetSection(nameof(QueueConfiguration)).Bind(queueConfiguration);
            
            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumersFromNamespaceContaining<ConsumerAnchor>();
                
                EndpointConvention.Map<WebhookCommand>(new Uri($"queue:{queueConfiguration.Queue}"));
                
                configurator.AddBus(context => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(context);

                    config.Host(queueConfiguration.Host, queueConfiguration.VirtualHost, host =>
                        {
                            host.Username(queueConfiguration.Username);
                            host.Password(queueConfiguration.Password);
                        });

                    config.ReceiveEndpoint(queueConfiguration.Queue, consumerEndpoint =>
                    {
                        consumerEndpoint.DiscardFaultedMessages();
                        consumerEndpoint.DiscardSkippedMessages();
                        consumerEndpoint.ConfigureConsumers(context);
                    });
                }));
            });
            
            services.AddMassTransitHostedService();

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddXmlSerializerFormatters();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready")
                });

                endpoints.MapHealthChecks("health/live", new HealthCheckOptions
                {
                    // Exclude all checks and return a 200/Ok
                    Predicate = _ => false
                });
            });
        }
    }
}