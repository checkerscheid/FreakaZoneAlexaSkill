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
//# Revision     : $Rev:: 187                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 187 2025-02-17 00:57:15Z                      $ #
//#                                                                                 #
//###################################################################################
using System.Diagnostics;
using System.Reflection;

TextWriterTraceListener twtl = new TextWriterTraceListener(String.Format("Log\\{0}_{1:yyyy_MM_dd}.log", "FreakaZoneAlexaSkill", DateTime.Now));
Trace.Listeners.Add(twtl);
FreakaZone.Libraries.wpEventLog.Debug.withForms = false;

FreakaZone.Libraries.wpEventLog.Debug.Write(MethodInfo.GetCurrentMethod(), "START" +
	"\r\n####################################################################\r\n\r\n");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5134");

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//FreakaZone.Libraries.wpEventLog.Debug debug = new FreakaZone.Libraries.wpEventLog.Debug("FreakaZoneAlexaSkill");

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


