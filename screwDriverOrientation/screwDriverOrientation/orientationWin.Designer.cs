namespace screwDriverOrientation
{
    partial class orientationWin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SerialGB = new System.Windows.Forms.GroupBox();
            this.serialPortCombo = new System.Windows.Forms.ComboBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.originGB = new System.Windows.Forms.GroupBox();
            this.rollOriginGB = new System.Windows.Forms.GroupBox();
            this.rollOriginLabel = new System.Windows.Forms.Label();
            this.setOriginButton = new System.Windows.Forms.Button();
            this.yawOriginGB = new System.Windows.Forms.GroupBox();
            this.yawOriginLabel = new System.Windows.Forms.Label();
            this.pitchOriginGB = new System.Windows.Forms.GroupBox();
            this.pitchOriginLabel = new System.Windows.Forms.Label();
            this.orientationGB = new System.Windows.Forms.GroupBox();
            this.rollGB = new System.Windows.Forms.GroupBox();
            this.rollLabel = new System.Windows.Forms.Label();
            this.pitchGB = new System.Windows.Forms.GroupBox();
            this.pitchLabel = new System.Windows.Forms.Label();
            this.yawGB = new System.Windows.Forms.GroupBox();
            this.yawLabel = new System.Windows.Forms.Label();
            this.serialPortObj = new System.IO.Ports.SerialPort(this.components);
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.calibGB = new System.Windows.Forms.GroupBox();
            this.calibProgress = new System.Windows.Forms.ProgressBar();
            this.calibLabel = new System.Windows.Forms.Label();
            this.filterTimer = new System.Windows.Forms.Timer(this.components);
            this.orientationPictureBox = new System.Windows.Forms.PictureBox();
            this.SerialGB.SuspendLayout();
            this.originGB.SuspendLayout();
            this.rollOriginGB.SuspendLayout();
            this.yawOriginGB.SuspendLayout();
            this.pitchOriginGB.SuspendLayout();
            this.orientationGB.SuspendLayout();
            this.rollGB.SuspendLayout();
            this.pitchGB.SuspendLayout();
            this.yawGB.SuspendLayout();
            this.calibGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.orientationPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SerialGB
            // 
            this.SerialGB.Controls.Add(this.serialPortCombo);
            this.SerialGB.Controls.Add(this.refreshButton);
            this.SerialGB.Controls.Add(this.disconnectButton);
            this.SerialGB.Controls.Add(this.connectButton);
            this.SerialGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.SerialGB.Location = new System.Drawing.Point(17, 12);
            this.SerialGB.Name = "SerialGB";
            this.SerialGB.Size = new System.Drawing.Size(214, 119);
            this.SerialGB.TabIndex = 0;
            this.SerialGB.TabStop = false;
            this.SerialGB.Text = "Serial communication";
            // 
            // serialPortCombo
            // 
            this.serialPortCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serialPortCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.serialPortCombo.FormattingEnabled = true;
            this.serialPortCombo.Location = new System.Drawing.Point(6, 34);
            this.serialPortCombo.Name = "serialPortCombo";
            this.serialPortCombo.Size = new System.Drawing.Size(92, 24);
            this.serialPortCombo.TabIndex = 4;
            this.serialPortCombo.SelectedIndexChanged += new System.EventHandler(this.serialPortCombo_SelectedIndexChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.refreshButton.Location = new System.Drawing.Point(6, 74);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(92, 29);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.disconnectButton.Location = new System.Drawing.Point(115, 74);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(92, 29);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.connectButton.Location = new System.Drawing.Point(115, 34);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(92, 29);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // originGB
            // 
            this.originGB.Controls.Add(this.rollOriginGB);
            this.originGB.Controls.Add(this.setOriginButton);
            this.originGB.Controls.Add(this.yawOriginGB);
            this.originGB.Controls.Add(this.pitchOriginGB);
            this.originGB.Enabled = false;
            this.originGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.originGB.Location = new System.Drawing.Point(237, 12);
            this.originGB.Name = "originGB";
            this.originGB.Size = new System.Drawing.Size(294, 119);
            this.originGB.TabIndex = 1;
            this.originGB.TabStop = false;
            this.originGB.Text = "Origin";
            // 
            // rollOriginGB
            // 
            this.rollOriginGB.Controls.Add(this.rollOriginLabel);
            this.rollOriginGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rollOriginGB.Location = new System.Drawing.Point(204, 54);
            this.rollOriginGB.Name = "rollOriginGB";
            this.rollOriginGB.Size = new System.Drawing.Size(84, 59);
            this.rollOriginGB.TabIndex = 8;
            this.rollOriginGB.TabStop = false;
            this.rollOriginGB.Text = "Roll";
            // 
            // rollOriginLabel
            // 
            this.rollOriginLabel.BackColor = System.Drawing.Color.White;
            this.rollOriginLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rollOriginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rollOriginLabel.Location = new System.Drawing.Point(8, 22);
            this.rollOriginLabel.Name = "rollOriginLabel";
            this.rollOriginLabel.Size = new System.Drawing.Size(61, 29);
            this.rollOriginLabel.TabIndex = 4;
            this.rollOriginLabel.Text = " ---.-°";
            this.rollOriginLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // setOriginButton
            // 
            this.setOriginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.setOriginButton.Location = new System.Drawing.Point(67, 23);
            this.setOriginButton.Name = "setOriginButton";
            this.setOriginButton.Size = new System.Drawing.Size(147, 29);
            this.setOriginButton.TabIndex = 3;
            this.setOriginButton.Text = "Set origin";
            this.setOriginButton.UseVisualStyleBackColor = true;
            this.setOriginButton.Click += new System.EventHandler(this.setOriginButton_Click);
            // 
            // yawOriginGB
            // 
            this.yawOriginGB.Controls.Add(this.yawOriginLabel);
            this.yawOriginGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.yawOriginGB.Location = new System.Drawing.Point(6, 54);
            this.yawOriginGB.Name = "yawOriginGB";
            this.yawOriginGB.Size = new System.Drawing.Size(84, 59);
            this.yawOriginGB.TabIndex = 6;
            this.yawOriginGB.TabStop = false;
            this.yawOriginGB.Text = "Yaw";
            // 
            // yawOriginLabel
            // 
            this.yawOriginLabel.BackColor = System.Drawing.Color.White;
            this.yawOriginLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.yawOriginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yawOriginLabel.Location = new System.Drawing.Point(6, 22);
            this.yawOriginLabel.Name = "yawOriginLabel";
            this.yawOriginLabel.Size = new System.Drawing.Size(61, 29);
            this.yawOriginLabel.TabIndex = 4;
            this.yawOriginLabel.Text = " ---.-°";
            this.yawOriginLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pitchOriginGB
            // 
            this.pitchOriginGB.Controls.Add(this.pitchOriginLabel);
            this.pitchOriginGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.pitchOriginGB.Location = new System.Drawing.Point(105, 54);
            this.pitchOriginGB.Name = "pitchOriginGB";
            this.pitchOriginGB.Size = new System.Drawing.Size(84, 59);
            this.pitchOriginGB.TabIndex = 7;
            this.pitchOriginGB.TabStop = false;
            this.pitchOriginGB.Text = "Pitch";
            // 
            // pitchOriginLabel
            // 
            this.pitchOriginLabel.BackColor = System.Drawing.Color.White;
            this.pitchOriginLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pitchOriginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pitchOriginLabel.Location = new System.Drawing.Point(6, 22);
            this.pitchOriginLabel.Name = "pitchOriginLabel";
            this.pitchOriginLabel.Size = new System.Drawing.Size(61, 29);
            this.pitchOriginLabel.TabIndex = 4;
            this.pitchOriginLabel.Text = " ---.-°";
            this.pitchOriginLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // orientationGB
            // 
            this.orientationGB.Controls.Add(this.rollGB);
            this.orientationGB.Controls.Add(this.pitchGB);
            this.orientationGB.Controls.Add(this.yawGB);
            this.orientationGB.Enabled = false;
            this.orientationGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.orientationGB.Location = new System.Drawing.Point(17, 137);
            this.orientationGB.Name = "orientationGB";
            this.orientationGB.Size = new System.Drawing.Size(514, 103);
            this.orientationGB.TabIndex = 2;
            this.orientationGB.TabStop = false;
            this.orientationGB.Text = "Orientation";
            // 
            // rollGB
            // 
            this.rollGB.Controls.Add(this.rollLabel);
            this.rollGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rollGB.Location = new System.Drawing.Point(350, 22);
            this.rollGB.Name = "rollGB";
            this.rollGB.Size = new System.Drawing.Size(143, 66);
            this.rollGB.TabIndex = 5;
            this.rollGB.TabStop = false;
            this.rollGB.Text = "Roll";
            // 
            // rollLabel
            // 
            this.rollLabel.BackColor = System.Drawing.Color.White;
            this.rollLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rollLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rollLabel.Location = new System.Drawing.Point(25, 18);
            this.rollLabel.Name = "rollLabel";
            this.rollLabel.Size = new System.Drawing.Size(94, 31);
            this.rollLabel.TabIndex = 4;
            this.rollLabel.Text = " ---.-°";
            this.rollLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pitchGB
            // 
            this.pitchGB.Controls.Add(this.pitchLabel);
            this.pitchGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.pitchGB.Location = new System.Drawing.Point(186, 22);
            this.pitchGB.Name = "pitchGB";
            this.pitchGB.Size = new System.Drawing.Size(143, 66);
            this.pitchGB.TabIndex = 4;
            this.pitchGB.TabStop = false;
            this.pitchGB.Text = "Pitch";
            // 
            // pitchLabel
            // 
            this.pitchLabel.BackColor = System.Drawing.Color.White;
            this.pitchLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pitchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pitchLabel.Location = new System.Drawing.Point(25, 18);
            this.pitchLabel.Name = "pitchLabel";
            this.pitchLabel.Size = new System.Drawing.Size(94, 31);
            this.pitchLabel.TabIndex = 4;
            this.pitchLabel.Text = " ---.-°";
            this.pitchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // yawGB
            // 
            this.yawGB.Controls.Add(this.yawLabel);
            this.yawGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.yawGB.Location = new System.Drawing.Point(22, 22);
            this.yawGB.Name = "yawGB";
            this.yawGB.Size = new System.Drawing.Size(143, 66);
            this.yawGB.TabIndex = 3;
            this.yawGB.TabStop = false;
            this.yawGB.Text = "Yaw";
            // 
            // yawLabel
            // 
            this.yawLabel.BackColor = System.Drawing.Color.White;
            this.yawLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.yawLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yawLabel.Location = new System.Drawing.Point(25, 18);
            this.yawLabel.Name = "yawLabel";
            this.yawLabel.Size = new System.Drawing.Size(94, 31);
            this.yawLabel.TabIndex = 4;
            this.yawLabel.Text = " ---.-°";
            this.yawLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // serialPortObj
            // 
            this.serialPortObj.BaudRate = 115200;
            this.serialPortObj.PortName = "NULL";
            this.serialPortObj.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPortObj_DataReceived);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 40;
            this.refreshTimer.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // calibGB
            // 
            this.calibGB.Controls.Add(this.calibProgress);
            this.calibGB.Controls.Add(this.calibLabel);
            this.calibGB.Enabled = false;
            this.calibGB.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.calibGB.Location = new System.Drawing.Point(17, 246);
            this.calibGB.Name = "calibGB";
            this.calibGB.Size = new System.Drawing.Size(514, 56);
            this.calibGB.TabIndex = 3;
            this.calibGB.TabStop = false;
            this.calibGB.Text = "Magnetometer calibration";
            // 
            // calibProgress
            // 
            this.calibProgress.Location = new System.Drawing.Point(6, 21);
            this.calibProgress.Name = "calibProgress";
            this.calibProgress.Size = new System.Drawing.Size(502, 29);
            this.calibProgress.TabIndex = 6;
            // 
            // calibLabel
            // 
            this.calibLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.calibLabel.Location = new System.Drawing.Point(104, 21);
            this.calibLabel.Name = "calibLabel";
            this.calibLabel.Size = new System.Drawing.Size(404, 29);
            this.calibLabel.TabIndex = 4;
            this.calibLabel.Text = "Calibration running...";
            this.calibLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.calibLabel.Visible = false;
            // 
            // filterTimer
            // 
            this.filterTimer.Enabled = true;
            this.filterTimer.Interval = 5;
            this.filterTimer.Tick += new System.EventHandler(this.filterTimer_Tick);
            // 
            // orientationPictureBox
            // 
            this.orientationPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.orientationPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.orientationPictureBox.Location = new System.Drawing.Point(537, 20);
            this.orientationPictureBox.Name = "orientationPictureBox";
            this.orientationPictureBox.Size = new System.Drawing.Size(343, 282);
            this.orientationPictureBox.TabIndex = 4;
            this.orientationPictureBox.TabStop = false;
            // 
            // orientationWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 312);
            this.Controls.Add(this.orientationPictureBox);
            this.Controls.Add(this.orientationGB);
            this.Controls.Add(this.originGB);
            this.Controls.Add(this.SerialGB);
            this.Controls.Add(this.calibGB);
            this.MaximizeBox = false;
            this.Name = "orientationWin";
            this.Text = "Screwdriver orientations";
            this.SerialGB.ResumeLayout(false);
            this.originGB.ResumeLayout(false);
            this.rollOriginGB.ResumeLayout(false);
            this.yawOriginGB.ResumeLayout(false);
            this.pitchOriginGB.ResumeLayout(false);
            this.orientationGB.ResumeLayout(false);
            this.rollGB.ResumeLayout(false);
            this.pitchGB.ResumeLayout(false);
            this.yawGB.ResumeLayout(false);
            this.calibGB.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.orientationPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SerialGB;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.GroupBox originGB;
        private System.Windows.Forms.GroupBox orientationGB;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ComboBox serialPortCombo;
        private System.Windows.Forms.Button setOriginButton;
        private System.Windows.Forms.GroupBox rollOriginGB;
        private System.Windows.Forms.Label rollOriginLabel;
        private System.Windows.Forms.GroupBox yawOriginGB;
        private System.Windows.Forms.Label yawOriginLabel;
        private System.Windows.Forms.GroupBox rollGB;
        private System.Windows.Forms.Label rollLabel;
        private System.Windows.Forms.GroupBox pitchGB;
        private System.Windows.Forms.Label pitchLabel;
        private System.Windows.Forms.GroupBox yawGB;
        private System.Windows.Forms.Label yawLabel;
        private System.IO.Ports.SerialPort serialPortObj;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.GroupBox pitchOriginGB;
        private System.Windows.Forms.Label pitchOriginLabel;
        private System.Windows.Forms.GroupBox calibGB;
        private System.Windows.Forms.ProgressBar calibProgress;
        private System.Windows.Forms.Label calibLabel;
        private System.Windows.Forms.Timer filterTimer;
        private System.Windows.Forms.PictureBox orientationPictureBox;
    }
}

