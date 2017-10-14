using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Project2.Models;
using Microsoft.EntityFrameworkCore;
using Project2.Services;
using System.Security;
using System.Reflection;

namespace Project2
{
#pragma warning disable CS1591
    public class Startup
    {
        const string _documentDbEndpointKey = "DOCUMENTDB_ENDPOINT";

        const string _documentDbTokenKey = "DOCUMENTDB_TOKEN";

        const string _documentDbDatabaseKey = "DOCUMENTDB_DATABASE";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Grab our connection information from the environment. If it isn't here we should die quickly
            var documentDbEndpoint = Environment.GetEnvironmentVariable(_documentDbEndpointKey);
            var documentDbToken = Environment.GetEnvironmentVariable(_documentDbTokenKey);
            var documentDbDatabase = Environment.GetEnvironmentVariable(_documentDbDatabaseKey);

            if(string.IsNullOrEmpty(documentDbEndpoint))
            {
                throw new StartupException($"{_documentDbEndpointKey} not set in environment. Cannot startup.");
            }

            if(string.IsNullOrEmpty(documentDbToken))
            {
                throw new StartupException($"{_documentDbTokenKey} not set in environment. Cannot startup.");
            }

            documentDbDatabase = string.IsNullOrEmpty(documentDbDatabase) ? "dev" : documentDbDatabase;

            services.AddMvc();

            foreach(var dbType in GetTypesWithAttribute(typeof(DocumentDbCollectionAttribute)))
            {
                var genericInterfaceType = typeof(IDocumentDbService<>).MakeGenericType(dbType);
                var concType = typeof(DocumentDbService<>).MakeGenericType(dbType);

                services.AddSingleton(genericInterfaceType, (sp) =>
                {
                    var attribute = ((DocumentDbCollectionAttribute)dbType.GetCustomAttribute(typeof(DocumentDbCollectionAttribute)));
                    return Activator.CreateInstance(concType, new Uri(documentDbEndpoint), documentDbToken, documentDbDatabase, attribute.CollectionName);
                });
            }

            

            //services.AddSingleton((sp) => {
            //    var service = new DocumentDbService(new Uri(documentDbEndpoint), documentDbToken, documentDbDatabase);
            //    service.Initialize().GetAwaiter().GetResult();
            //    return service;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }

        private IEnumerable<Type> GetTypesWithAttribute(Type attribute)
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(typ => typ.CustomAttributes.Any(x => x.AttributeType == attribute));
        }
    }
#pragma warning restore CS1591
}
