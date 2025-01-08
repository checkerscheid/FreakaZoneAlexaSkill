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
//# Revision     : $Rev:: 150                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 150 2024-12-14 16:20:21Z                      $ #
//#                                                                                 #
//###################################################################################
using System.Diagnostics;

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

TextWriterTraceListener twtl = new TextWriterTraceListener(String.Format("Log\\{0}_{1:yyyy_MM_dd}.log", "FreakaZoneAlexaSkill", DateTime.Now));
Trace.Listeners.Add(twtl);

app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}


