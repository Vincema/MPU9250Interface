using System;
using System.Windows.Forms;
using System.Drawing;
using System.Numerics;

namespace screwDriverOrientation
{
    public partial class orientationWin : Form
    {
        mag magnetometer = new mag();
        float yaw, pitch, roll;
        float yawOrigin, pitchOrigin, rollOrigin;
        float ax = 0, ay = 0, az = 0;
        float gx = 0, gy = 0, gz = 0;
        float mx = 0, my = 0, mz = 0;
        AHRS.MahonyAHRS AHRSFilter = new AHRS.MahonyAHRS(0.005f,15.0f,0.0f);
        private BufferedGraphics g_buff;
        Quaternion quat;

        private class mag
        {
            private const int progress_low_pass_sample_nb = 200;
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
                    // The values should lie from -500 to +500
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
                double norm;
                int pgValue;
                norm = get_norm(mag_values_corr);
                pgValue = (int)(100 - (Math.Ceiling(Math.Abs(1 - norm)) * 10.0f));
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
            g_buff.Graphics.Clear(Color.White);
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

        private void set_zeros_YPR()
        {
            // Set origin
            yawOrigin = 0;
            pitchOrigin = 0;
            rollOrigin = 0;
            yaw = 0;
            pitch = 0;
            roll = 0;
            yawOriginLabel.Text = yawOrigin.ToString("000.0°");
            pitchOriginLabel.Text = pitchOrigin.ToString("000.0°");
            rollOriginLabel.Text = rollOrigin.ToString("000.0°");
        }

        private void print_default_YPR_disp()
        {
            yawLabel.Text = " ---.-°";
            pitchLabel.Text = " ---.-°";
            rollLabel.Text = " ---.-°";
            yawOriginLabel.Text = " ---.-°";
            pitchOriginLabel.Text = " ---.-°";
            rollOriginLabel.Text = " ---.-°";
            calibProgress.Value = 0;
        }

        public void QuatToEuler(Quaternion q, ref float yaw, ref float pitch, ref float roll)
        {
            yaw = 0;
            pitch = 0;
            roll = 0;

            yaw = (float)Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            pitch = (float)Math.Asin(-2.0 * (q.X * q.Z - q.W * q.Y));
            roll = (float)Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);

            yaw *= 180.0f / (float)Math.PI;
            pitch *= 180.0f / (float)Math.PI;
            roll *= 180.0f / (float)Math.PI;
        }

        private void filterTimer_Tick(object sender, EventArgs e)
        {
            if (!float.IsNaN(mx) && !float.IsNaN(my) && !float.IsNaN(mz))
            {
                //AHRSFilter.Update(deg2rad(gx), deg2rad(gy), deg2rad(gz), 1,0,0);
                AHRSFilter.Update(deg2rad(gx), deg2rad(gy), deg2rad(gz), ax, ay, az, mx, my, mz);
                float[] q = AHRSFilter.Quaternion;
                quat = new Quaternion(q[1], q[2], q[3], q[0]);
                quat = Quaternion.Normalize(quat);

                // Console.WriteLine(q[0].ToString() + " " + q[1].ToString() + " " + q[2].ToString() + " " + q[3].ToString());

                // quat = Quaternion.Normalize(quat);
                // quat_rel = Quaternion.Inverse(prev_quat) * quat;
                // quat_rel = Quaternion.Multiply(Quaternion.Normalize(quat_rel),0.00001f) ;
                // quat_rel = Quaternion.Lerp(quat, prev_quat, 0.5f);
                
                QuatToEuler(quat, ref yaw, ref pitch, ref roll);

                //yaw = (float)Math.Atan2(2.0f * (q[1] * q[2] + q[0] * q[3]), q[0] * q[0] + q[1] * q[1] - q[2] * q[2] - q[3] * q[3]);
                //pitch = -(float)Math.Asin(2.0f * (q[1] * q[3] - q[0] * q[2]));
                //roll = (float)Math.Atan2(2.0f * (q[0] * q[1] + q[2] * q[3]), q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3]);

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
                    originGB.Enabled = true;
                    calibGB.Enabled = true;
                    calibProgress.Visible = true;
                    calibLabel.Visible = false;
                    set_zeros_YPR();
                    magnetometer.reset_mag_values();
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
            originGB.Enabled = false;
            calibGB.Enabled = false;
            calibProgress.Visible = false;
            calibLabel.Visible = false;
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

        private void refreshButton_Click(object sender, EventArgs e)
        {
            refresh_serial_combobox();
        }

        private void setOriginButton_Click(object sender, EventArgs e)
        {
            yawOrigin = yaw;
            pitchOrigin = pitch;
            rollOrigin = roll;
            yawOriginLabel.Text = yawOrigin.ToString("000.0°");
            pitchOriginLabel.Text = pitchOrigin.ToString("000.0°");
            rollOriginLabel.Text = rollOrigin.ToString("000.0°");
        }

        private void serialPortObj_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = serialPortObj.ReadLine();
                string[] datas = line.Split(' ');
                if (datas[0] == "ACCEL")
                {
                    ax = -float.Parse(datas[1]);
                    ay = -float.Parse(datas[2]);
                    az = float.Parse(datas[3]);
                    Console.WriteLine("A " + ax.ToString() + " " + ay.ToString() + " " + az.ToString());
                }
                else if (datas[0] == "GYRO")
                {
                    gx = -float.Parse(datas[1]);
                    gy = -float.Parse(datas[2]);
                    gz = float.Parse(datas[3]);
                    //Console.WriteLine("G " + gx.ToString() + " " + gy.ToString() + " " + gz.ToString());
                }
                else if (datas[0] == "MAG")
                {
                    my = float.Parse(datas[1]);
                    mx = float.Parse(datas[2]);
                    mz = float.Parse(datas[3]);

                    //Console.WriteLine("M " + mx.ToString() + " " + my.ToString() + " " + mz.ToString());
                    magnetometer.save_mag_values(mx, my, mz);
                    mx = magnetometer.mag_values_corr[0];
                    my = magnetometer.mag_values_corr[1];
                    mz = magnetometer.mag_values_corr[2];
                    Console.WriteLine("MC " + mx.ToString() + " " + my.ToString() + " " + mz.ToString());
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
            float yawTmp = diffAngle(yaw, yawOrigin);
            float pitchTmp = diffAngle(pitch,pitchOrigin);
            float rollTmp = diffAngle(roll,rollOrigin);
            yawLabel.Text = yawTmp.ToString("000.0°");
            pitchLabel.Text = pitchTmp.ToString("000.0°");
            rollLabel.Text = rollTmp.ToString("000.0°");

            calibProgress.Value = (int)magnetometer.get_value_progress_bar_calib();
            draw_orientation(yaw, pitch, roll);
        }

        private void draw_orientation(float yaw, float pitch, float roll)
        {
            float yaw_rad = (yaw - yawOrigin) * (float)Math.PI / 180.0f;
            float pitch_rad = (pitch - pitchOrigin) * (float)Math.PI / 180.0f;
            float roll_rad = (roll - rollOrigin) * (float)Math.PI / 180.0f;

            int width = orientationPictureBox.Width;
            int height = orientationPictureBox.Height;

            float origX = width / 2;
            float origY = height / 2;

            // Draw Cube
            float h = 20, w = 100, l = 50;

            float[] pc1 = { l / 2, w / 2, h / 2 };
            float[] pc2 = { l / 2, w / 2, -h / 2 };
            float[] pc3 = { l / 2, -w / 2, h / 2 };
            float[] pc4 = { l / 2, -w / 2, -h / 2 };
            float[] pc5 = { -l / 2, w / 2, h / 2 };
            float[] pc6 = { -l / 2, w / 2, -h / 2 };
            float[] pc7 = { -l / 2, -w / 2, h / 2 };
            float[] pc8 = { -l / 2, -w / 2, -h / 2 };

            rotate_matrix(pc1, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc2, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc3, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc4, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc5, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc6, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc7, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(pc8, yaw_rad, pitch_rad, roll_rad);

            Point vert1 = new Point((int)(pc1[1] + origX), (int)(pc1[2] + origY));
            Point vert2 = new Point((int)(pc2[1] + origX), (int)(pc2[2] + origY));
            Point vert3 = new Point((int)(pc3[1] + origX), (int)(pc3[2] + origY));
            Point vert4 = new Point((int)(pc4[1] + origX), (int)(pc4[2] + origY));
            Point vert5 = new Point((int)(pc5[1] + origX), (int)(pc5[2] + origY));
            Point vert6 = new Point((int)(pc6[1] + origX), (int)(pc6[2] + origY));
            Point vert7 = new Point((int)(pc7[1] + origX), (int)(pc7[2] + origY));
            Point vert8 = new Point((int)(pc8[1] + origX), (int)(pc8[2] + origY));

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

            Pen p_edge = new Pen(Color.Red, 3);
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

            Point[] sidef = { vert1, vert2, vert4, vert3 };
            Point[] sideb = { vert5, vert6, vert8, vert7 };
            Point[] sidel = { vert1, vert2, vert6, vert5 };
            Point[] sider = { vert3, vert4, vert8, vert7 };
            Point[] sideu = { vert1, vert3, vert7, vert5 };
            Point[] sided = { vert2, vert4, vert8, vert6 };

            SolidBrush b;
            b = new SolidBrush(Color.FromArgb(100, Color.Blue));
            g_buff.Graphics.FillPolygon(b, sidef);
            g_buff.Graphics.FillPolygon(b, sideb);
            g_buff.Graphics.FillPolygon(b, sidel);
            g_buff.Graphics.FillPolygon(b, sider);
            g_buff.Graphics.FillPolygon(b, sideu);
            g_buff.Graphics.FillPolygon(b, sided);

            // Test
            Vector3 v_mag = new Vector3(mx, my, mz);
            v_mag = Vector3.Multiply(v_mag, 80.0f);
            v_mag = Vector3.Transform(v_mag, (quat));
            if (v_mag.X < 0)
                g_buff.Graphics.DrawLine(new Pen(Color.LightBlue, 5), origX, origY, origX + v_mag.Y, origY + v_mag.Z);
            else
                g_buff.Graphics.DrawLine(new Pen(Color.DarkBlue, 5), origX, origY, origX + v_mag.Y, origY + v_mag.Z);

            Vector3 v_acc = new Vector3(ax, ay, az);
            v_acc = Vector3.Multiply(v_acc, 80.0f);
            v_acc = Vector3.Transform(v_acc, (quat));
            if (v_acc.X < 0)
                g_buff.Graphics.DrawLine(new Pen(Color.Yellow, 5), origX, origY, origX + v_acc.Y, origY + v_acc.Z);
            else
                g_buff.Graphics.DrawLine(new Pen(Color.Gold, 5), origX, origY, origX + v_acc.Y, origY + v_acc.Z);

            // Compass
            float unit_coef = 100;

            g_buff.Graphics.FillEllipse(Brushes.Black, origX - 5, origY - 5, 10, 10);

            float[] p1 = { unit_coef, 0, 0 };
            float[] p2 = { 0, unit_coef, 0 };
            float[] p3 = { 0, 0, -unit_coef };

            rotate_matrix(p1, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(p2, yaw_rad, pitch_rad, roll_rad);
            rotate_matrix(p3, yaw_rad, pitch_rad, roll_rad);

            Pen tmp_pen;
            Color tmp_color;
            float lw;

            tmp_color = Color.Red;
            lw = 6 - 4 * ((unit_coef - p1[0]) / (2 * unit_coef));
            tmp_pen = new Pen(tmp_color, lw);
            if (Math.Sqrt(Math.Pow(p1[1],2) + Math.Pow(p1[2], 2)) > 20)
                tmp_pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(lw * 0.75f, lw * 0.75f, true);
            g_buff.Graphics.DrawLine(tmp_pen, origX, origY, origX + p1[1], origY + p1[2]);

            tmp_color = Color.Black;
            lw = 6 - 4 * ((unit_coef - p2[0]) / (2 * unit_coef));
            tmp_pen = new Pen(tmp_color, lw);
            if (Math.Sqrt(Math.Pow(p2[1], 2) + Math.Pow(p2[2], 2)) > 20)
                tmp_pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(lw * 0.75f, lw * 0.75f, true);
            g_buff.Graphics.DrawLine(tmp_pen, origX, origY, origX + p2[1], origY + p2[2]);

            tmp_color = Color.Green;
            lw = 6 - 4 * ((unit_coef - p3[0]) / (2 * unit_coef));
            tmp_pen = new Pen(tmp_color, lw);
            if (Math.Sqrt(Math.Pow(p3[1], 2) + Math.Pow(p3[2], 2)) > 20)
                tmp_pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(lw * 0.75f, lw * 0.75f, true);
            g_buff.Graphics.DrawLine(tmp_pen, origX, origY, origX + p3[1], origY + p3[2]);

            g_buff.Render();
            g_buff.Graphics.Clear(Color.White);
        }

        private void rotate_matrix (float[] pts, float yaw, float pitch, float roll)
        {
            Vector3 vec = new Vector3(pts[0], pts[1], pts[2]);
            vec = Vector3.Transform(vec, quat);
            pts[0] = vec.X;
            pts[1] = vec.Y;
            pts[2] = vec.Z;
            //rotate_vect3D(0, roll, pts);
            //rotate_vect3D(1, pitch, pts);
            //rotate_vect3D(2, yaw, pts);
        }

        private void rotate_vect3D (int axis, float angle, float[] vect)
        {
            float x = vect[0];
            float y = vect[1];
            float z = vect[2];
            if (axis == 0)
            {
                vect[0] = x;
                vect[1] = y * (float)Math.Cos(angle) + z * (float)Math.Sin(angle);
                vect[2] = -y * (float)Math.Sin(angle) + z * (float)Math.Cos(angle);
            }
            else if (axis == 1)
            {
                vect[0] = x * (float)Math.Cos(angle) - z * (float)Math.Sin(angle);
                vect[1] = y;
                vect[2] = x * (float)Math.Sin(angle) + z * (float)Math.Cos(angle);
            }
            else if (axis == 2)
            {
                vect[0] = x * (float)Math.Cos(angle) + y * (float)Math.Sin(angle);
                vect[1] = -x * (float)Math.Sin(angle) + y * (float)Math.Cos(angle);
                vect[2] = z;
            }
        }
    }
}
