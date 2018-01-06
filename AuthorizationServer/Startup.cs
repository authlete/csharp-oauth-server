//
// Copyright (C) 2018 Authlete, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific
// language governing permissions and limitations under the
// License.
//


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Authlete.Api;
using Authlete.Conf;


namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            // AddWebApiConventions() is added by WebApiCompatShim.
            // Calling the method enables Web API implementations
            // to return an HttpResponseMessage instance directly.
            services.AddMvc().AddWebApiConventions();

            // Register an instance of IAuthleteApi so controllers
            // can refer to it in order to call Authlete Web APIs.
            // IAuthleteApi instance will be passed to constructors
            // of controllers by 'Dependency Injection'. Read the
            // following article for details.
            //
            //   Introduction to Dependency Injection in ASP.NET Core
            //   https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
            // 
            services.AddSingleton<IAuthleteApi>(CreateAuthleteApi());
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }


        static IAuthleteApi CreateAuthleteApi()
        {
            // Create an instance of IAuthleteConfiguration.
            var conf = CreateAuthleteConfiguration();

            // Create an instance of IAuthleteApi.
            return new AuthleteApi(conf);
        }


        static IAuthleteConfiguration CreateAuthleteConfiguration()
        {
            // Load a configuration file and build an instance of
            // IAuthleteConfiguration interface. By default,
            // "authlete.properties" will be loaded. The name of a
            // configuration file can be specified by the
            // environment variable, AUTHLETE_CONFIGURATION_FILE.
            //
            // AuthetePropertiesConfiguration class has three
            // constructors one of which explicitly takes the name
            // of a configuration file.
            //
            // In Authlete.Conf namespace, there exist some other
            // implementations of IAuthleteConfiguration interface.

            return new AuthletePropertiesConfiguration();
        }
    }
}
