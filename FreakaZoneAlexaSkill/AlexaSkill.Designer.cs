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
//# Revision     : $Rev:: 187                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 187 2025-02-17 00:57:15Z                      $ #
//#                                                                                 #
//###################################################################################
namespace FreakaZoneAlexaSkill {
	partial class AlexaSkill {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlexaSkill));
			lbl_msg = new TextBox();
			statusStrip1 = new StatusStrip();
			toolStripStatusLabel1 = new ToolStripStatusLabel();
			txt_msg = new Label();
			nonsens = new TextBox();
			SystemIcon = new NotifyIcon(components);
			statusStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// lbl_msg
			// 
			lbl_msg.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			lbl_msg.BackColor = SystemColors.Control;
			lbl_msg.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lbl_msg.Location = new Point(130, 13);
			lbl_msg.Margin = new Padding(4, 2, 4, 2);
			lbl_msg.Multiline = true;
			lbl_msg.Name = "lbl_msg";
			lbl_msg.ScrollBars = ScrollBars.Both;
			lbl_msg.Size = new Size(1092, 433);
			lbl_msg.TabIndex = 0;
			lbl_msg.Enter += lbl_msg_Enter;
			// 
			// statusStrip1
			// 
			statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
			statusStrip1.Location = new Point(0, 459);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new Padding(1, 0, 12, 0);
			statusStrip1.Size = new Size(1234, 22);
			statusStrip1.TabIndex = 1;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new Size(12, 17);
			toolStripStatusLabel1.Text = "-";
			// 
			// txt_msg
			// 
			txt_msg.AutoSize = true;
			txt_msg.Location = new Point(13, 16);
			txt_msg.Name = "txt_msg";
			txt_msg.Size = new Size(88, 13);
			txt_msg.TabIndex = 2;
			txt_msg.Text = "Last Message:";
			// 
			// nonsens
			// 
			nonsens.Location = new Point(144, 30);
			nonsens.Name = "nonsens";
			nonsens.Size = new Size(100, 21);
			nonsens.TabIndex = 3;
			// 
			// SystemIcon
			// 
			SystemIcon.Icon = (Icon)resources.GetObject("SystemIcon.Icon");
			SystemIcon.Visible = true;
			SystemIcon.MouseClick += SystemIcon_MouseClick;
			// 
			// AlexaSkill
			// 
			AutoScaleDimensions = new SizeF(7F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1234, 481);
			Controls.Add(txt_msg);
			Controls.Add(lbl_msg);
			Controls.Add(nonsens);
			Controls.Add(statusStrip1);
			Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(3, 2, 3, 2);
			Name = "AlexaSkill";
			Text = "FreakaZone Alexa Skill";
			ClientSizeChanged += AlexaSkill_ClientSizeChanged;
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox lbl_msg;
		private StatusStrip statusStrip1;
		private ToolStripStatusLabel toolStripStatusLabel1;
		private Label txt_msg;
		private TextBox nonsens;
		private NotifyIcon SystemIcon;
	}
}
