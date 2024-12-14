//###################################################################################
//#                                                                                 #
//#                (C) FreakaZone GmbH                                              #
//#                =======================                                          #
//#                                                                                 #
//###################################################################################
//#                                                                                 #
//# Author       : Christian Scheid                                                 #
//# Date         : 05.12.2024                                                       #
//#                                                                                 #
//# Revision     : $Rev:: 146                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 146 2024-12-07 12:43:11Z                      $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Forms.Application;
namespace FreakaZoneAlexaSkill {
	static class Program {
		public static FreakaZoneAlexaSkill MainProg;
		[STAThread]
		static void Main(string[] args) {
			InitWebHost(args);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			MainProg = new FreakaZoneAlexaSkill();
			Application.Run(MainProg);
		}
		private static void InitWebHost(string[] args) {
			var builder = WebApplication.CreateBuilder(args);
			builder.WebHost.UseUrls("http://localhost:5134");

			// Add services to the container.

			builder.Services.AddControllers().AddNewtonsoftJson();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			//if(app.Environment.IsDevelopment()) {
			//	app.UseSwagger();
			//	app.UseSwaggerUI();
			//}

			//app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}


