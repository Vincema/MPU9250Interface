using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;


namespace screwDriverOrientation
{
    public partial class orientationWin : Form
    {
        mag magnetometer = new mag();
        float yaw, pitch, roll;
        float desired_yaw, desired_pitch;
        float ax = 0, ay = 0, az = 0;
        float gx = 0, gy = 0, gz = 0;
        float mx = 0, my = 0, mz = 0;
        AHRS.MahonyAHRS AHRSFilter = new AHRS.MahonyAHRS(0.005f,15.0f,0.0f);
        private BufferedGraphics g_buff;
        Quaternion raw_quat, quat_orig, quat_trans;

        private class mag
        {
            private const int progress_low_pass_sample_nb = 50;
            private float[] mag_values = new float[3];
            public float[] mag_values_corr { get; set; }
            private float[] mag_max = new float[3];
            private float[] mag_min = new float[3];
            private float[] mag_offsets = new float[3];
            private float[] mag_coefs = new float[3];
            private float[] low_pass_progress_bar = new float[progress_low_pass_sample_nb];  

            public void reset_mag_values()
            {
                mag_values_corr = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    mag_values[i] = 0;
                    mag_values_corr[i] = 0;
                    mag_max[i] = -65535;
                    mag_min[i] = 65535;
                    mag_offsets[i] = 0;
                    mag_coefs[i] = 1;
                }
                for (int i = 0; i < progress_low_pass_sample_nb; i++)
                {
                    low_pass_progress_bar[i] = 0;
                }
            }

            public void save_mag_values(float val0, float val1, float val2)
            {
                mag_values[0] = val0;
                mag_values[1] = val1;
                mag_values[2] = val2;
                bool need_calib = false;
                for (int i = 0; i < 3; i++)
                {
                    if (mag_values[i] > mag_max[i])
                    {
                        mag_max[i] = mag_values[i];
                        need_calib = true;
                    }
                    if (mag_values[i] < mag_min[i])
                    { 
                        mag_min[i] = mag_values[i];
                        need_calib = true;
                    }
                }
                if(need_calib)
                {
                    compute_calibration_values();
                }

                for (int i = 0; i < 3; i++)
                {
                    mag_values_corr[i] = mag_coefs[i] * (mag_values[i] - mag_offsets[i]);
                }
            }

            private void set_mag_coefs()
            {
                for (int i = 0; i < 3; i++)
                {
                    mag_coefs[i] = 2 / (mag_max[i] - mag_min[i]);
                    mag_offsets[i] = (mag_min[i] + mag_max[i]) / 2;
                    mag_values_corr[i] = mag_values[i] * mag_coefs[i] - mag_offsets[i];
                }
            }

            private double get_norm(float[] arr3d)
            {
                return Math.Sqrt(Math.Pow(arr3d[0], 2) + Math.Pow(arr3d[1], 2) + Math.Pow(arr3d[2], 2));
            }

            public void compute_calibration_values()
            {
                set_mag_coefs();
            }

            public int get_value_progress_bar_calib ()
            {
                
                double norm = get_norm(mag_values_corr);
                int pgValue = (int)(100 - (Math.Ceiling(Math.Abs(1 - norm) * 100.0f)));
                if (pgValue < 0)
                    pgValue = 0;

                for (int i = progress_low_pass_sample_nb - 1; i > 0; i--)
                    low_pass_progress_bar[i] = low_pass_progress_bar[i-1];
                low_pass_progress_bar[0] = pgValue;

                float mean_value = 0;
                for (int i = 0; i < progress_low_pass_sample_nb; i++)
                    mean_value += low_pass_progress_bar[i];
                mean_value /= progress_low_pass_sample_nb;

                return (int)Math.Ceiling(mean_value);
            }
        }
        
        public orientationWin()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            print_default_YPR_disp();
            refresh_serial_combobox();
            BufferedGraphicsContext buff_context = BufferedGraphicsManager.Current;
            Rectangle disp_area = new Rectangle(0, 0, orientationPictureBox.Width, orientationPictureBox.Height);
            g_buff = buff_context.Allocate(orientationPictureBox.CreateGraphics(), disp_area);
            g_buff.Graphics.TranslateTransform(orientationPictureBox.Width / 2, orientationPictureBox.Height / 2);

            quat_trans = new Quaternion(0, 0, 0, 1);
        }

        private void refresh_serial_combobox()
        {
            serialPortCombo.Items.Clear();
            // Add the list of available ports in the combobox
            string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string port in portNames)
            {
                serialPortCombo.Items.Add(port);
            }
            if (serialPortCombo.Items.Count > 0)
            {
                serialPortCombo.SelectedIndex = 0;
            }
        }

        private void set_zeros_orientation()
        {
            yaw = 0;
            pitch = 0;
            roll = 0;
            desired_yaw = 0;
            desired_pitch = 0;
            quat_orig = new Quaternion(0, 0, 0, 1);
        }

        private void print_default_YPR_disp()
        {
            yawLabel.Text = " ---.-°";
            pitchLabel.Text = " ---.-°";
            rollLabel.Text = " ---.-°";
            desiredYawTB.Text = "000°";
            desiredYawTB.BackColor = Color.Lime;
            desiredPitchTB.Text = "000°";
            desiredPitchTB.BackColor = Color.Lime;
            yawDiffLabel.Text = "---.-°";
            pitchDiffLabel.Text = "---.-°";
            yawDiffLabel.ForeColor = Color.Black;
            pitchDiffLabel.Text = "---.-°";
            pitchDiffLabel.ForeColor = Color.Black;
            calibProgress.Value = 0;
        }

        public void QuatToEuler(Quaternion q, ref float yaw, ref float pitch, ref float roll)
        {
            yaw = 0;
            pitch = 0;
            roll = 0;

            roll = (float)Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            pitch = (float)Math.Asin(-2.0 * (q.X * q.Z - q.W * q.Y));
            yaw = (float)Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);

            yaw *= 180.0f / (float)Math.PI;
            pitch *= 180.0f / (float)Math.PI;
            roll *= 180.0f / (float)Math.PI;
        }

        private void filterTimer_Tick(object sender, EventArgs e)
        {
            if (!float.IsNaN(mx) && !float.IsNaN(my) && !float.IsNaN(mz))
            {
                AHRSFilter.Update(deg2rad(gx), deg2rad(gy), deg2rad(gz), ax, ay, az, mx, my, mz);
                float[] q = AHRSFilter.Quaternion;
                raw_quat = new Quaternion(q[1], q[2], q[3], q[0]);

                // Apply origin offset
                Quaternion delta_quat_orig = raw_quat * Quaternion.Inverse(quat_orig);
                quat_trans = Quaternion.Inverse(quat_orig) * delta_quat_orig * quat_orig;

                QuatToEuler(quat_trans, ref yaw, ref pitch, ref roll);
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (serialPortObj.PortName == "NULL")
            {
                MessageBox.Show("Please select a serial port first.", "Select a serial port", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    serialPortObj.Open();
                    disconnectButton.Enabled = true;
                    connectButton.Enabled = false;
                    orientationGB.Enabled = true;
                    refreshButton.Enabled = false;
                    refreshTimer.Enabled = true;
                    configGB.Enabled = true;
                    calibGB.Enabled = true;
                    calibProgress.Visible = true;
                    calibLabel.Visible = false;
                    displayGB.Enabled = true;
                    filterTimer.Enabled = true;
                    set_zeros_orientation();
                    magnetometer.reset_mag_values();
                    orientationPictureBox.Enabled = true;
                    desiredOrientationGB.Enabled = true;
                    desiredYawGB.Enabled = true;
                    desiredPitchGB.Enabled = true;
                    yawDiffGB.Enabled = false;
                    pitchDiffGB.Enabled = false;
                    orientationPictureBox.Visible = true;
                    cartesianCoordPict.Visible = true;
                    g_buff.Graphics.Clear(Color.White);
                }
                catch
                {
                    MessageBox.Show("Cannot connect to serial port: " + serialPortObj.PortName + "!", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void disconnect_serial()
        {
            if (serialPortObj.IsOpen)
            {
                serialPortObj.Close();
            }
            disconnectButton.Enabled = false;
            connectButton.Enabled = true;
            refreshTimer.Enabled = false;
            refreshButton.Enabled = true;
            configGB.Enabled = false;
            calibGB.Enabled = false;
            calibProgress.Visible = false;
            calibLabel.Visible = false;
            displayGB.Enabled = false;
            filterTimer.Enabled = false;
            orientationPictureBox.Enabled = false;
            desiredOrientationGB.Enabled = false;
            destinationButton.Text = "Reach destination";
            polarChart.Visible = false;
            orientationPictureBox.Visible = false;
            cartesianCoordPict.Visible = false;
            print_default_YPR_disp();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            disconnect_serial();
        }

        private void serialPortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string portName = serialPortCombo.SelectedItem.ToString();
            if (portName != "" && serialPortObj.IsOpen == false)
            {
                serialPortObj.PortName = portName;
            }
        }

        private void resetCalButton_Click(object sender, EventArgs e)
        {
            magnetometer.reset_mag_values();
        }

        private void destinationButton_Click(object sender, EventArgs e)
        {
            if (destinationButton.Text == "Reach destination" && desiredYawTB.BackColor == Color.Lime && desiredPitchTB.BackColor == Color.Lime)
            {
                destinationButton.Text = "Cancel";
                desiredYawGB.Enabled = false;
                desiredPitchGB.Enabled = false;
                yawDiffGB.Enabled = true;
                pitchDiffGB.Enabled = true;
                polarChart.Visible = true;
                orientationPictureBox.Visible = false;
                cartesianCoordPict.Visible = false;
                displayGB.Enabled = false;
                polarChart.Series["destPolarPos"].Points.Clear();
                polarChart.Series["destPolarPos"].Points.AddXY(desired_yaw, desired_pitch);
            }
            else
            {
                destinationButton.Text = "Reach destination";
                desiredYawGB.Enabled = true;
                desiredPitchGB.Enabled = true;
                yawDiffGB.Enabled = false;
                pitchDiffGB.Enabled = false;
                polarChart.Visible = false;
                orientationPictureBox.Visible = true;
                cartesianCoordPict.Visible = true;
                displayGB.Enabled = true;
            }
        }

        private void desiredYawTB_MouseClick(object sender, MouseEventArgs e)
        {
            desiredYawTB.Text = "";
        }

        private void desiredPitchTB_MouseClick(object sender, MouseEventArgs e)
        {
            desiredPitchTB.Text = "";
        }

        private void desiredYawTB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strTB = desiredYawTB.Text;
                strTB = strTB.Replace('°'.ToString(), string.Empty);
                desired_yaw = (float)int.Parse(strTB);
                if (Math.Abs(desired_yaw) > 180)
                    desiredYawTB.BackColor = Color.Red;
                else
                {
                    desiredYawTB.BackColor = Color.Lime;
                    desiredYawTB.Text = strTB + '°';
                    desiredYawTB.SelectionStart = desiredYawTB.Text.Length - 1;
                }
            }
            catch
            {
                desiredYawTB.BackColor = Color.Red;
            }
        }

        private void desiredPitchTB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strTB = desiredPitchTB.Text;
                strTB = strTB.Replace('°'.ToString(), string.Empty);
                desired_pitch = (float)int.Parse(strTB);
                if (Math.Abs(desired_pitch) > 90)
                    desiredPitchTB.BackColor = Color.Red;
                else
                {
                    desiredPitchTB.BackColor = Color.Lime;
                    desiredPitchTB.Text = strTB + '°';
                    desiredPitchTB.SelectionStart = desiredPitchTB.Text.Length - 1;
                }
            }
            catch
            {
                desiredPitchTB.BackColor = Color.Red;
            }
        }

        private void cuboidCB_CheckedChanged(object sender, EventArgs e)
        {
            if (cuboidCB.Checked)
            {
                fillCB.Enabled = true;
            }
            else
            {
                fillCB.Enabled = false;
                fillCB.Checked = false;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            refresh_serial_combobox();
        }

        private void setOriginButton_Click(object sender, EventArgs e)
        {
            quat_orig.X = raw_quat.X;
            quat_orig.Y = raw_quat.Y;
            quat_orig.Z = raw_quat.Z;
            quat_orig.W = raw_quat.W;
        }

        private void serialPortObj_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = serialPortObj.ReadLine();
                string[] datas = line.Split(' ');
                if (datas[0] == "ACCEL")
                {
                    ax = float.Parse(datas[1]);
                    ay = float.Parse(datas[2]);
                    az = float.Parse(datas[3]);
                    //Console.WriteLine("A " + ax.ToString() + " " + ay.ToString() + " " + az.ToString());
                }
                else if (datas[0] == "GYRO")
                {
                    gx = float.Parse(datas[1]);
                    gy = float.Parse(datas[2]);
                    gz = float.Parse(datas[3]);
                    //Console.WriteLine("G " + gx.ToString() + " " + gy.ToString() + " " + gz.ToString());
                }
                else if (datas[0] == "MAG")
                {
                    my = -float.Parse(datas[1]);
                    mx = -float.Parse(datas[2]);
                    mz = float.Parse(datas[3]);

                    //Console.WriteLine("M " + mx.ToString() + " " + my.ToString() + " " + mz.ToString());
                    magnetometer.save_mag_values(mx, my, mz);
                    mx = magnetometer.mag_values_corr[0];
                    my = magnetometer.mag_values_corr[1];
                    mz = magnetometer.mag_values_corr[2];
                    //Console.WriteLine("MC " + mx.ToString() + " " + my.ToString() + " " + mz.ToString());
                }
            }
            catch
            {
            }
        }

        // Return the difference of two angles (-180,+180)
        private float diffAngle(float a, float b)
        {
            if (a-b > 180)
            {
                return 360 - (a - b);
            }
            else if (a-b < -180)
            {
                return 360 + (a - b);
            }
            else
            {
                return a - b;
            }
        }

        public float deg2rad(float degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            yawLabel.Text = yaw.ToString("000.0°");
            pitchLabel.Text = pitch.ToString("000.0°");
            rollLabel.Text = roll.ToString("000.0°");

            calibProgress.Value = (int)magnetometer.get_value_progress_bar_calib();

            if (yawDiffGB.Enabled)
            {
                yawDiffLabel.Text = (desired_yaw - yaw).ToString("000.0°");
                if (Math.Abs(desired_yaw - yaw) < 2f)
                    yawDiffLabel.ForeColor = Color.Lime;
                else
                    yawDiffLabel.ForeColor = Color.Black;
                pitchDiffLabel.Text = (desired_pitch - pitch).ToString("000.0°");
                if (Math.Abs(desired_pitch - pitch) < 2f)
                    pitchDiffLabel.ForeColor = Color.Lime;
                else
                    pitchDiffLabel.ForeColor = Color.Black;
                if (Math.Abs(desired_yaw - yaw) < 2f && Math.Abs(desired_pitch - pitch) < 2f)
                    polarChart.Series["currentPolarPos"].MarkerColor = Color.Lime;
                else
                    polarChart.Series["currentPolarPos"].MarkerColor = Color.Red;

                polarChart.Series["currentPolarPos"].Points.Clear();
                polarChart.Series["currentPolarPos"].Points.AddXY(yaw, pitch);
            }
            else
            {
                draw_orientation(yaw, pitch, roll);
            }
        }

        private void draw_orientation(float yaw, float pitch, float roll)
        {
            // Draw Cube
            if (cuboidCB.Checked)
            {
                float h = 15, w = 75, l = 35;

                float[] pc1 = { -l, 0, 0 };
                float[] pc2 = { -l, 0, -h };
                float[] pc3 = { -l, -w, 0 };
                float[] pc4 = { -l, -w, -h };
                float[] pc5 = { 0, 0, 0 };
                float[] pc6 = { 0, 0, -h };
                float[] pc7 = { 0, -w, 0 };
                float[] pc8 = { 0, -w, -h };

                rotate_matrix(pc1);
                rotate_matrix(pc2);
                rotate_matrix(pc3);
                rotate_matrix(pc4);
                rotate_matrix(pc5);
                rotate_matrix(pc6);
                rotate_matrix(pc7);
                rotate_matrix(pc8);

                Point vert1 = new Point((int)(pc1[1]), (int)(pc1[2]));
                Point vert2 = new Point((int)(pc2[1]), (int)(pc2[2]));
                Point vert3 = new Point((int)(pc3[1]), (int)(pc3[2]));
                Point vert4 = new Point((int)(pc4[1]), (int)(pc4[2]));
                Point vert5 = new Point((int)(pc5[1]), (int)(pc5[2]));
                Point vert6 = new Point((int)(pc6[1]), (int)(pc6[2]));
                Point vert7 = new Point((int)(pc7[1]), (int)(pc7[2]));
                Point vert8 = new Point((int)(pc8[1]), (int)(pc8[2]));

                Point[] edge1 = { vert1, vert2 };
                Point[] edge2 = { vert1, vert3 };
                Point[] edge3 = { vert3, vert4 };
                Point[] edge4 = { vert4, vert2 };
                Point[] edge5 = { vert1, vert5 };
                Point[] edge6 = { vert3, vert7 };
                Point[] edge7 = { vert4, vert8 };
                Point[] edge8 = { vert2, vert6 };
                Point[] edge9 = { vert5, vert6 };
                Point[] edge10 = { vert5, vert7 };
                Point[] edge11 = { vert7, vert8 };
                Point[] edge12 = { vert6, vert8 };

                Pen p_edge = new Pen(Color.Black, 3);
                g_buff.Graphics.DrawLine(p_edge, edge1[0], edge1[1]);
                g_buff.Graphics.DrawLine(p_edge, edge2[0], edge2[1]);
                g_buff.Graphics.DrawLine(p_edge, edge3[0], edge3[1]);
                g_buff.Graphics.DrawLine(p_edge, edge4[0], edge4[1]);
                g_buff.Graphics.DrawLine(p_edge, edge5[0], edge5[1]);
                g_buff.Graphics.DrawLine(p_edge, edge6[0], edge6[1]);
                g_buff.Graphics.DrawLine(p_edge, edge7[0], edge7[1]);
                g_buff.Graphics.DrawLine(p_edge, edge8[0], edge8[1]);
                g_buff.Graphics.DrawLine(p_edge, edge9[0], edge9[1]);
                g_buff.Graphics.DrawLine(p_edge, edge10[0], edge10[1]);
                g_buff.Graphics.DrawLine(p_edge, edge11[0], edge11[1]);
                g_buff.Graphics.DrawLine(p_edge, edge12[0], edge12[1]);
                
                if (fillCB.Checked)
                {
                    SolidBrush brush;
                    brush = new SolidBrush(Color.FromArgb(100, Color.Yellow));
                    Point[] down_side = { vert1, vert3, vert7, vert5 };
                    Point[] x_side = { vert1, vert2, vert6, vert5 };
                    Point[] y_side = { vert5, vert6, vert8, vert7 };
                    g_buff.Graphics.FillPolygon(brush, down_side);
                    g_buff.Graphics.FillPolygon(brush, x_side);
                    g_buff.Graphics.FillPolygon(brush, y_side);
                }
            }

            // Compass
            float norm = 120;

            float[] p1 = { -norm, 0, 0 };
            float[] p2 = { 0, -norm, 0 };
            float[] p3 = { 0, 0, -norm };

            rotate_matrix(p1);
            rotate_matrix(p2);
            rotate_matrix(p3);

            int[] ind = { 0, 1, 2, 3 };
            float[] val = { 0.0f, p1[0], p2[0], p3[0] };
            for (int i=0; i<3; i++)
            {
                if (val[i] > val[i + 1])
                {
                    float tmp_val;
                    tmp_val = val[i];
                    val[i] = val[i + 1];
                    val[i + 1] = tmp_val;
                    int tmp_ind;
                    tmp_ind = ind[i];
                    ind[i] = ind[i + 1];
                    ind[i + 1] = tmp_ind;
                    i = -1;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (ind[i] == 0)
                    disp_compass(0, new float[4]);
                if (ind[i] == 1)
                    disp_compass(1, p1);
                if (ind[i] == 2)
                    disp_compass(2, p2);
                if (ind[i] == 3)
                    disp_compass(3, p3);
            }

            g_buff.Render();
            g_buff.Graphics.Clear(Color.White);
        }

        private void disp_compass(int axis_nb, float[] p)
        {
            float arrow_width = 7;

            if (axis_nb == 0)
            {
                g_buff.Graphics.FillEllipse(Brushes.Black, -arrow_width, -arrow_width, 2*arrow_width, 2*arrow_width);
            }

            if (xaxisCB.Checked && axis_nb == 1)
            {
                Pen tmp_pen = new Pen(Color.Red, arrow_width);
                if (arrowsCB.Checked)
                    tmp_pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(arrow_width * 0.30f, arrow_width * 0.30f, true);
                else
                {
                    Brush tmp_brush = new SolidBrush(Color.Red);
                    g_buff.Graphics.FillEllipse(tmp_brush, p[1] - arrow_width / 2, p[2] - arrow_width / 2, arrow_width, arrow_width);
                    g_buff.Graphics.FillEllipse(tmp_brush, 0 - arrow_width / 2, 0 - arrow_width / 2, arrow_width, arrow_width);
                }
                g_buff.Graphics.DrawLine(tmp_pen, 0, 0, p[1], p[2]);
            }

            if (yaxisCB.Checked && axis_nb == 2)
            {
                Pen tmp_pen = new Pen(Color.Blue, arrow_width);
                if (arrowsCB.Checked)
                    tmp_pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(arrow_width * 0.30f, arrow_width * 0.30f, true);
                else
                {
                    Brush tmp_brush = new SolidBrush(Color.Blue);
                    g_buff.Graphics.FillEllipse(tmp_brush, p[1] - arrow_width / 2, p[2] - arrow_width / 2, arrow_width, arrow_width);
                    g_buff.Graphics.FillEllipse(tmp_brush, 0 - arrow_width / 2, 0 - arrow_width / 2, arrow_width, arrow_width);
                }
                g_buff.Graphics.DrawLine(tmp_pen, 0, 0, p[1], p[2]);
            }

            if (zaxisCB.Checked && axis_nb == 3)
            {
                Pen tmp_pen = new Pen(Color.LimeGreen, arrow_width);
                if (arrowsCB.Checked)
                    tmp_pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(arrow_width * 0.30f, arrow_width * 0.30f, true);
                else
                {
                    Brush tmp_brush = new SolidBrush(Color.LimeGreen);
                    g_buff.Graphics.FillEllipse(tmp_brush, p[1] - arrow_width / 2, p[2] - arrow_width / 2, arrow_width, arrow_width);
                    g_buff.Graphics.FillEllipse(tmp_brush, 0 - arrow_width / 2, 0 - arrow_width / 2, arrow_width, arrow_width);
                }
                g_buff.Graphics.DrawLine(tmp_pen, 0, 0, p[1], p[2]);
            }
        }

        private void rotate_matrix (float[] pts)
        {
            Vector3 vec = new Vector3(pts[0], pts[1], pts[2]);
            vec = Vector3.Transform(vec, quat_trans);
            pts[0] = vec.X;
            pts[1] = vec.Y;
            pts[2] = vec.Z;
        }
    }
}
