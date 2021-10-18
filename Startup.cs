using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Orders.API.Interceptors;
using Orders.Application;
using Orders.Application.Features.Orders.Commands.ChangeOrderStatus;
using Orders.Infrastructure;
using Stripe;

namespace Orders.API
{
    public class SubdirectoryHandler : DelegatingHandler
    {
        private readonly string _subdirectory;

        public SubdirectoryHandler(HttpMessageHandler innerHandler, string subdirectory)
            : base(innerHandler)
        {
            _subdirectory = subdirectory;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}";
            url += $"{_subdirectory}{request.RequestUri.AbsolutePath}";
            request.RequestUri = new Uri(url, UriKind.Absolute);

            return base.SendAsync(request, cancellationToken);
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<gRPCAuthInterceptor>();
            // services.AddHttpsRedirection(options => options.HttpsPort = 5005);

            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders.API", Version = "v1" });
            });
            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.Authority = Configuration.GetValue<string>("IdentityAuthority");
                      options.Audience = "orders";
                      options.BackchannelHttpHandler = new HttpClientHandler
                      { ServerCertificateCustomValidationCallback = delegate { return true; } };
                  });

            var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder("Bearer")
               .RequireAuthenticatedUser()
               .Build();


            services.AddGrpcClient<rpcOrders.rpcOrdersClient>((services, options) =>
            {
                options.Address = new Uri(Configuration.GetValue<string>("OrdersGRPC"));
            })
              .ConfigureChannel((channel) =>
              {
                  AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                  var handler = new SubdirectoryHandler(
                      new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } },
                      Configuration.GetValue<string>("OrdersGRPCSubdirectory"));
                  var client = new HttpClient(handler)
                  {
                      DefaultRequestVersion = new Version(2, 0)
                  };
                  channel.HttpClient = client;
              })  
            .AddInterceptor<gRPCAuthInterceptor>();

            services.AddControllers(configure =>
            {
                configure.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // app.UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders.API v1"));
            }

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                var response = new { error = exception.Message };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
