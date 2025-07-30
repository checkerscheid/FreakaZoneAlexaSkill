//###################################################################################
//#                                                                                 #
//#                (C) FreakaZone GmbH                                              #
//#                =======================                                          #
//#                                                                                 #
//###################################################################################
//#                                                                                 #
//# Author       : Christian Scheid                                                 #
//# Date         : 19.05.2025                                                       #
//#                                                                                 #
//# Revision     : $Rev:: 248                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: AlexaSkill.cs 248 2025-07-07 14:24:05Z                   $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpEventLog;
using FreakaZone.Libraries.wpSQL;
using FreakaZone.Libraries.wpSQL.Table;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FreakaZoneAlexaSkill {
	public partial class AlexaSkill: Form {

		private FormWindowState lastState;
		public static List<TableD1Mini> d1Minis = new List<TableD1Mini>();
		public static List<TableShelly> shellys = new List<TableShelly>();
		public AlexaSkill() {
			InitializeComponent();
			Debug.SetRefString(lbl_msg);
			string[] pVersion = Application.ProductVersion.Split('.');
			this.toolStripStatusLabel1.Text = String.Format("{0} V {1}.{2} Build {3}, © {4}",
				Application.ProductName,
				pVersion[0], pVersion[1],
				Program.subversion,
				Application.CompanyName);
			this.SystemIcon.Text = Application.ProductName;
			using(Database sql = new Database("Get Controller")) {
				d1Minis = sql.Select<TableD1Mini>();
				shellys = sql.Select<TableShelly>();
			}
			Task.Run(() => {
				StartListener();
			});
		}

		private void StartListener() {
			Debug.Write(MethodBase.GetCurrentMethod(), "Starting AlexaSkill listener");
			var builder = WebApplication.CreateBuilder();
			builder.WebHost.UseUrls("http://localhost:5134");

			// Add services to the container.

			builder.Services.AddControllers().AddNewtonsoftJson();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			//builder.Services.AddSwaggerGen();

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
			Debug.Write(MethodBase.GetCurrentMethod(), "Stop AlexaSkill listener");
		}

		/// <summary>
		/// Handles the <see cref="Control.Enter"/> event for the label.
		/// </summary>
		/// <param name="sender">The source of the event, typically the label control.</param>
		/// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
		private void lbl_msg_Enter(object sender, EventArgs e) {
			nonsens.Focus();
		}

		private void SystemIcon_MouseClick(object sender, MouseEventArgs e) {
			if(this.WindowState == FormWindowState.Minimized) {
				this.Show();
				this.WindowState = lastState;
			} else {
				this.WindowState = FormWindowState.Minimized;
				this.Hide();
			}
		}

		private void AlexaSkill_ClientSizeChanged(object sender, EventArgs e) {
			if(this.WindowState == FormWindowState.Minimized) {
				this.Hide();
				SystemIcon.BalloonTipTitle = Application.ProductName ?? "AlexaSkillB";
				SystemIcon.BalloonTipText = "wurde minimiert";
				SystemIcon.BalloonTipIcon = ToolTipIcon.Info;
				SystemIcon.ShowBalloonTip(1000);
			} else {
				this.Show();
				lastState = this.WindowState;
			}
		}
	}
}
