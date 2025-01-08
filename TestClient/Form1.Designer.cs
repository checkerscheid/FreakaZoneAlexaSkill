namespace TestClient {
	partial class Form1 {
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
			buttonOk = new Button();
			buttonNetflix = new Button();
			buttonLauter = new Button();
			buttonLeiser = new Button();
			SuspendLayout();
			// 
			// buttonOk
			// 
			buttonOk.Location = new Point(12, 12);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(75, 23);
			buttonOk.TabIndex = 0;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			buttonOk.Click += buttonOk_Click;
			// 
			// buttonNetflix
			// 
			buttonNetflix.Location = new Point(12, 41);
			buttonNetflix.Name = "buttonNetflix";
			buttonNetflix.Size = new Size(75, 23);
			buttonNetflix.TabIndex = 1;
			buttonNetflix.Text = "Netflix";
			buttonNetflix.UseVisualStyleBackColor = true;
			buttonNetflix.Click += buttonNetflix_Click;
			// 
			// buttonLauter
			// 
			buttonLauter.Location = new Point(93, 12);
			buttonLauter.Name = "buttonLauter";
			buttonLauter.Size = new Size(75, 23);
			buttonLauter.TabIndex = 2;
			buttonLauter.Text = "lauter";
			buttonLauter.UseVisualStyleBackColor = true;
			buttonLauter.Click += buttonLauter_Click;
			// 
			// buttonLeiser
			// 
			buttonLeiser.Location = new Point(93, 41);
			buttonLeiser.Name = "buttonLeiser";
			buttonLeiser.Size = new Size(75, 23);
			buttonLeiser.TabIndex = 3;
			buttonLeiser.Text = "leiser";
			buttonLeiser.UseVisualStyleBackColor = true;
			buttonLeiser.Click += buttonLeiser_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(buttonLeiser);
			Controls.Add(buttonLauter);
			Controls.Add(buttonNetflix);
			Controls.Add(buttonOk);
			Name = "Form1";
			Text = "Form1";
			ResumeLayout(false);
		}

		#endregion

		private Button buttonOk;
		private Button buttonNetflix;
		private Button buttonLauter;
		private Button buttonLeiser;
	}
}
