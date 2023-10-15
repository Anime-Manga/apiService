using Cesxhin.AnimeManga.Modules.CronJob;
using Cesxhin.AnimeManga.Modules.Generic;
using Cesxhin.AnimeManga.Application.Interfaces.Repositories;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Modules.Schema;
using Cesxhin.AnimeManga.Application.Services;
using Cesxhin.AnimeManga.Persistence.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using Quartz;
using System;

namespace Cesxhin.AnimeManga.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            SchemaControl.Check();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //interfaces
            //services
            services.AddSingleton<IEpisodeService, EpisodeService>();
            services.AddSingleton<IEpisodeRegisterService, EpisodeRegisterService>();
            services.AddSingleton<IChapterRegisterService, ChapterRegisterService>();
            services.AddSingleton<IChapterService, ChapterService>();
            services.AddSingleton<IDescriptionVideoService, DescriptionVideoService>();
            services.AddSingleton<IDescriptionBookService, DescriptionBookService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IProgressEpisodeService, ProgressEpisodeService>();
            services.AddSingleton<IProgressChapterService, ProgressChapterService>();
            services.AddSingleton<IChapterQueueService, ChapterQueueService>();
            services.AddSingleton<IEpisodeQueueService, EpisodeQueueService>();
            services.AddSingleton<IChapterBlackListService, ChapterBlackListService>();
            services.AddSingleton<IEpisodeBlackListService, EpisodeBlackListService>();

            //repositories
            services.AddSingleton<IEpisodeRepository, EpisodeRepository>();
            services.AddSingleton<IEpisodeRegisterRepository, EpisodeRegisterRepository>();
            services.AddSingleton<IChapterRegisterRepository, ChapterRegisterRepository>();
            services.AddSingleton<IChapterRepository, ChapterRepository>();
            services.AddSingleton<IDescriptionRepository, DescriptionRepository>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IProgressEpisodeRepository, ProgressEpisodeRepository>();
            services.AddSingleton<IProgressChapterRepository, ProgressChapterRepository>();
            services.AddSingleton<IChapterQueueRepository, ChapterQueueRepository>();
            services.AddSingleton<IEpisodeQueueRepository, EpisodeQueueRepository>();
            services.AddSingleton<IChapterBlackListRepository, ChapterBlackListRepository>();
            services.AddSingleton<IEpisodeBlackListRepository, EpisodeBlackListRepository>();

            //init repoDb
            RepoDb.PostgreSqlBootstrap.Initialize();

            //rabbit
            services.AddMassTransit(
            x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        Environment.GetEnvironmentVariable("ADDRESS_RABBIT") ?? "localhost",
                        "/",
                        credentials =>
                        {
                            credentials.Username(Environment.GetEnvironmentVariable("USERNAME_RABBIT") ?? "guest");
                            credentials.Password(Environment.GetEnvironmentVariable("PASSWORD_RABBIT") ?? "guest");
                        });
                });
            });


            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cesxhin.AnimeManga.Api", Version = "v1" });
            });

            //cronjob for check health
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.ScheduleJob<HealthJob>(trigger => trigger
                    .StartNow()
                    .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(60)), job => job.WithIdentity("api"));
            });

            //setup nlog
            var level = Environment.GetEnvironmentVariable("LOG_LEVEL")?.ToLower() ?? "info";
            LogLevel logLevel = NLogManager.GetLevel(level);
            NLogManager.Configure(logLevel);

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cesxhin.AnimeManga.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true));

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
