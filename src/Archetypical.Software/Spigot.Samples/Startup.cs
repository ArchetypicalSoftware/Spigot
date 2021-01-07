using System;
using System.Collections.Generic;
using Archetypical.Software.Spigot.Extensions;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spigot.Samples.Common;
using Spigot.Samples.EventualConsistency.MaterializedView;
using Spigot.Samples.EventualConsistency.MaterializedView.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Knobs;
using OrderCompletedKnob = Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs.OrderCompletedKnob;

namespace Spigot.Samples
{
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
            services.AddControllersWithViews();

            // Add the spigot
            services.AddSpigot(Configuration)
                .AddKnob<ElementAddedKnob, ElementAddedEvent>()
                .AddKnob<ElementUpdatedKnob, ElementUpdatedEvent>()
                .AddKnob<ReSyncRequestKnob, ReSyncRequestEvent>()
                .AddKnob<SyncResponseKnob, SyncResponseEvent>()
                .AddKnob<RecordKnob, Record>()
                .AddKnob<OrderCompletedKnob, OrderCompletedEvent>()
                .AddKnob<OrderPlacedKnob, OrderPlacedEvent>()
                .AddKnob<CreditResultKnob, CreditResultEvent>()
                .AddKnob<ReadOnlyOrderCompletedKnob, OrderCompletedEvent>()

                ;

            services
                .AddTransient<IDemonstrator, MaterializedViewDemonstrator>()
                .AddTransient<IDemonstrator, TwoPhaseCommitDemonstrator>()
                .AddTransient<OrderMicroService>()
                .AddSingleton<Dictionary<Guid, DataElement>>()
                .AddSingleton<List<DataElement>>()
                .AddSingleton<List<Customer>>()
                .AddSingleton<CustomerMicroService>()
                .AddSingleton<OrderCompletedKnob>()
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }
        }

        public async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 940,
                Show = false
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("Electron.NET API Demos");
        }
    }
}