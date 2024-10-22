namespace profiler_interface
{

    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            QueriesList = new ListBox();
            QueryBody = new RichTextBox();
            ProfilingButton = new Button();
            SuspendLayout();
            // 
            // QueriesList
            // 
            QueriesList.FormattingEnabled = true;
            QueriesList.ItemHeight = 15;
            QueriesList.Items.AddRange(new object[] { "fasdf" });
            QueriesList.Location = new Point(12, 12);
            QueriesList.Name = "QueriesList";
            QueriesList.Size = new Size(628, 394);
            QueriesList.TabIndex = 0;
            QueriesList.SelectedIndexChanged += QueriesList_SelectedIndexChanged;
            // 
            // QueryBody
            // 
            QueryBody.Location = new Point(12, 421);
            QueryBody.Name = "QueryBody";
            QueryBody.Size = new Size(628, 288);
            QueryBody.TabIndex = 1;
            QueryBody.Text = "";
            QueryBody.TextChanged += QueryBody_TextChanged;
            // 
            // ProfilingButton
            // 
            ProfilingButton.Location = new Point(646, 686);
            ProfilingButton.Name = "ProfilingButton";
            ProfilingButton.Size = new Size(247, 23);
            ProfilingButton.TabIndex = 2;
            ProfilingButton.Text = "Start Profiling";
            ProfilingButton.UseVisualStyleBackColor = true;
            ProfilingButton.Click += ProfilingButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(905, 721);
            Controls.Add(ProfilingButton);
            Controls.Add(QueryBody);
            Controls.Add(QueriesList);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private ListBox QueriesList;
        private RichTextBox QueryBody;
        private Button ProfilingButton;
    }
}
