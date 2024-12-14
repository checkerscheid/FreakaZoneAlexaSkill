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
//# Revision     : $Rev:: 149                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 149 2024-12-14 16:13:07Z                      $ #
//#                                                                                 #
//###################################################################################
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

