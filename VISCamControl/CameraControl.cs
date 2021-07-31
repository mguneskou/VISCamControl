using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

namespace VISCamControl
{
    public partial class CameraControl: UserControl
    {
        FilterInfoCollection videoDevices = null;
        Camera Cam = null;
        bool Connected = false;
        bool Cross = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CameraControl()
        {
            InitializeComponent();
            this.HandleDestroyed += CameraControl_Closed;
            this.HandleCreated += CameraControl_Load;
        }

        /// <summary>
        /// Handle created event procedure for user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraControl_Load(object sender, EventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if(videoDevices.Count != 0)
                {
                    for (int i = 0; i < videoDevices.Count; i++)
                    {
                        CmbCameraList.Items.Add(videoDevices[i].Name);
                    }
                    CmbCameraList.SelectedIndex = 0;
                    BtnStart.Enabled = true;
                    BtnStop.Enabled = false;
                    BtnCross.Enabled = false;
                    BtnSettings.Enabled = false;
                }
                else
                {
                    LblError.Text = "MainForm_Load - No camera detected!";
                }
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_Load - " + ex.Message;
            }
        }

        /// <summary>
        /// Handle destroyed event procedure for user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CameraControl_Closed(object sender, EventArgs e)
        {
            DisconnectCamera();
        }

        /// <summary>
        /// Connects to the selected camera
        /// </summary>
        /// <param name="CamIndex">If more than one camera is connected to the computer</param>
        public void ConnectCamera(int CamIndex = 0)
        {
            try
            {
                typeof(PictureBox).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, CamView, new object[] { true });
                Cam = new Camera(videoDevices[CamIndex].MonikerString, CamView);
                Cam.EventMessage += Cam_EventMessage;
                Cam.Start();
                Connected = true;
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_ConnectCamera - " + ex.Message;
            }
        }

        /// <summary>
        /// Stops camera and cleans up the memory
        /// </summary>
        public void DisconnectCamera()
        {
            try
            {
                Cam.Stop();
                Cam.Dispose();
                Connected = false;
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_DisconnectCamera - " + ex.Message;
            }
        }

        /// <summary>
        /// Handles error messages which sent by camera class
        /// </summary>
        /// <param name="Message"></param>
        private void Cam_EventMessage(string Message)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate { LblError.Text = Message; });
                }
                else
                {
                    LblError.Text = Message;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MainForm_EventMessage - " + ex.Message);
            }
        }

        /// <summary>
        /// Puts a cross marker at the centre of the camera image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CamView_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (Connected && Cross)
                {
                    e.Graphics.DrawLine(Pens.Red, 0, CamView.Height / 2, CamView.Width, CamView.Height / 2);
                    e.Graphics.DrawLine(Pens.Red, CamView.Width / 2, 0, CamView.Width / 2, CamView.Height);
                }
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_PictureBoxPaint - " + ex.Message;
            }
        }

        /// <summary>
        /// starts the connected camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Connected) ConnectCamera(CmbCameraList.SelectedIndex);
                Cam.Start();
                BtnStart.Enabled = false;
                BtnStop.Enabled = true;
                BtnCross.Enabled = true;
                BtnSettings.Enabled = true;
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_StartButton - " + ex.Message;
            }
        }

        /// <summary>
        /// stops the connected camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                Cam.Stop();
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;
                BtnCross.Enabled = true;
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_StopButton - " + ex.Message;
            }
        }

        /// <summary>
        /// shows camera settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                Cam.ShowProperties(this);
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_SettingsButton - " + ex.Message;
            }
        }

        /// <summary>
        /// enables/disables cross marker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCross_Click(object sender, EventArgs e)
        {
            Cross = !Cross;
        }

        /// <summary>
        /// disconnects from previously connected camera (if connected to any)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbCameraList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Connected) DisconnectCamera();
        }

        private void CameraControl_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                this.CamView.Height = this.Height - ToolStripMenu.Height;
                this.CamView.Width = this.Width;
            }
            catch (Exception ex)
            {
                LblError.Text = "MainForm_SizeChanged - " + ex.Message;
            }
        }
    }

    public delegate void SendMessage(string Message);

    public class Camera : IDisposable
    {
        //class instances
        SettingsForm FrmSettings;
        VideoCaptureDevice VideoSource = null;
        PictureBox PbView = null;
        ResizeBilinear Resizer = null;
        Invert NegativeFilter = new Invert();
        HomogenityEdgeDetector EdgeDetector = new HomogenityEdgeDetector();
        EncoderParameter qualityParam = null;
        ImageCodecInfo imageCodec = null;
        EncoderParameters parameters = null;
        Pen CropPen = new Pen(Color.Red, 1);
        Bitmap image = null;
        //structure instances
        internal Rectangle ImageArea;
        Rectangle Crop;
        Point CropStartCoordinate, CropEndCoordinate;
        //enums
        PixelFormat pixelFormat = PixelFormat.Format16bppRgb555;
        RotateFlipType rotateType = RotateFlipType.RotateNoneFlipNone;
        PixelFormat tempPixelFormat = PixelFormat.Format16bppRgb555;
        //variables
        bool StretchImage = true, Negative = false, Compress = true;
        bool InvertBackground = false;
        bool CropReset = true;
        bool CropSelection = false;
        bool EdgeDetection = false;
        bool AngleCheck = false;
        internal int DesiredWidth = 1280;
        internal int DesiredHeight = 720;
        int AngleTolerence = 0;
        int BackgroundFilter = 128;
        long Quality = 100;
        string strResolution = "1280x720";
        //lists
        List<Point> BlobSelectCoordinate = new List<Point>();
        //events
        internal event SendMessage EventMessage;

        public Camera(string MonikerString, PictureBox Viewer)
        {
            try
            {
                VideoSource = new VideoCaptureDevice(MonikerString);
                VideoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                FrmSettings = new SettingsForm();
                FrmSettings.settingsChanged += FrmSettings_settingsChanged;
                VideoCapabilities[] cap = VideoSource.VideoCapabilities;
                for (int i = 0; i < cap.Length; i++)
                {
                    FrmSettings.Resolution.Add(cap[i].FrameSize.Width.ToString() + "x" + cap[i].FrameSize.Height.ToString());
                }
                FrmSettings.cmbResolution.DataSource = FrmSettings.Resolution;
                FrmSettings.cmbResolution.SelectedIndex = FrmSettings.Resolution.IndexOf("1280x720");
                FrmSettings.cmbAngleTolerance.SelectedIndex = 3;
                this.PbView = Viewer;
                this.PbView.Paint += View_Paint;
                this.PbView.MouseDown += View_MouseDown;
                this.PbView.MouseDoubleClick += View_MouseDoubleClick;
                this.PbView.MouseMove += View_MouseMove;
                this.PbView.MouseUp += View_MouseUp;
                this.PbView.MouseLeave += View_MouseLeave;
                this.PbView.SizeChanged += View_SizeChanged;
                ImageArea = new Rectangle(0, 0, PbView.Width, PbView.Height);
                //image compression default settings
                imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                parameters = new EncoderParameters(1);
            }
            catch
            {
                EventMessage("Cam_Constructor - Cannot initialize camera class");
            }
        }

        /// <summary>
        /// resets crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    CropReset = true;
                    FrmSettings.SettingsForm_imageCropped(false);
                    BlobSelectCoordinate.Clear();//clear selected blobs and let user to select new blobs in cropped image
                }
            }
            catch (Exception ex)
            {
                EventMessage("Cam_DoubleClick - " + ex.Message);
            }
        }

        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)//crop
                {
                    //reset rectangle
                    CropStartCoordinate.X = 0;
                    CropStartCoordinate.Y = 0;
                    CropEndCoordinate.X = 0;
                    CropEndCoordinate.Y = 0;
                    //reset rectangle   
                    CropStartCoordinate.X = e.X;
                    CropStartCoordinate.Y = e.Y;
                    //select blobs
                    if (CropReset)
                    {
                        BlobSelectCoordinate.Add(new Point(e.X * DesiredWidth / ImageArea.Width, e.Y * DesiredHeight / ImageArea.Height));
                    }
                    else
                    {
                        BlobSelectCoordinate.Add(new Point(e.X * Crop.Width / ImageArea.Width, e.Y * Crop.Height / ImageArea.Height));
                    }
                    if (BlobSelectCoordinate.Count > 2) BlobSelectCoordinate.RemoveAt(1);
                }
                if (e.Button == MouseButtons.Middle)//reset crop
                {
                    CropReset = true;
                    FrmSettings.SettingsForm_imageCropped(false);
                    BlobSelectCoordinate.Clear();//clear selected blobs and let user to select new blobs in cropped image
                }
                if(e.Button == MouseButtons.Right) //clear blobs selection
                {
                    BlobSelectCoordinate.Clear();
                }
            }
            catch (Exception ex)
            {
                EventMessage("Cam_MouseDown - " + ex.Message);
            }
        }

        /// <summary>
        /// drwas cropping red rectangle on the picturebox image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    CropEndCoordinate.X = e.X;
                    CropEndCoordinate.Y = e.Y;
                    CropSelection = ((Math.Abs(CropEndCoordinate.X - CropStartCoordinate.X) > 5 && Math.Abs(CropEndCoordinate.Y - CropStartCoordinate.Y) > 5)) ? true : false;
                }
            }
            catch (Exception ex)
            {
                EventMessage("Cam_MouseMove - " + ex.Message);
            }
        }

        /// <summary>
        /// crops the selected region of image and stretches in picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && CropSelection)
                {
                    if (CropEndCoordinate.X == 0 || CropEndCoordinate.Y == 0) return;
                    if (CropEndCoordinate.X < CropStartCoordinate.X || CropEndCoordinate.Y < CropStartCoordinate.Y) return;
                    if (CropEndCoordinate.X > ImageArea.Width) CropEndCoordinate.X = ImageArea.Width;
                    if (CropEndCoordinate.Y > ImageArea.Height) CropEndCoordinate.Y = ImageArea.Height;
                    this.Crop = new Rectangle(CropStartCoordinate.X, CropStartCoordinate.Y, CropEndCoordinate.X - CropStartCoordinate.X, CropEndCoordinate.Y - CropStartCoordinate.Y);
                    Crop.X = Convert.ToInt32((double)DesiredWidth * (double)Crop.X / (double)this.ImageArea.Width);
                    Crop.Y = Convert.ToInt32((double)DesiredHeight * (double)Crop.Y / (double)this.ImageArea.Height);
                    Crop.Width = Convert.ToInt32((double)DesiredWidth * (double)Crop.Width / (double)this.ImageArea.Width);
                    Crop.Height = Convert.ToInt32((double)DesiredHeight * (double)Crop.Height / (double)this.ImageArea.Height);
                    CropSelection = false;
                    CropReset = false;
                    FrmSettings.SettingsForm_imageCropped(true);
                    BlobSelectCoordinate.Clear();//clear selected blobs and let user to select new blobs in cropped image
                }
            }
            catch (Exception ex)
            {
                EventMessage("Cam_MouseUp - " + ex.Message);
            }
        }

        /// <summary>
        /// in case user starts selecting the crop region but leaves the picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                CropSelection = false;
            }
            catch (Exception ex)
            {
                EventMessage("Cam_MouseLeave - " + ex.Message);
            }
        }

        /// <summary>
        /// Overlays the camera image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (CropSelection) e.Graphics.DrawRectangle(CropPen, CropStartCoordinate.X, CropStartCoordinate.Y, CropEndCoordinate.X - CropStartCoordinate.X, CropEndCoordinate.Y - CropStartCoordinate.Y);
            }
            catch (Exception ex)
            {
                EventMessage("Cam_Paint - " + ex.Message);
            }
        }

        /// <summary>
        /// ImegaArea rectangle gets picturebox size in here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                ImageArea = new Rectangle(0, 0, PbView.Width, PbView.Height);
            }
            catch (Exception ex)
            {
                EventMessage("Cam_SizeChanged - " + ex.Message);
            }
        }

        public bool IsRunning { get { return VideoSource.IsRunning; } }

        /// <summary>
        /// occurs when settings changed in settings form
        /// </summary>
        /// <param name="CamEventArgs"></param>
        private void FrmSettings_settingsChanged(SettingsChangedEventArgs CamEventArgs)
        {
            try
            {
                this.StretchImage = CamEventArgs.StretchImage;
                this.Negative = CamEventArgs.Negative;
                this.rotateType = CamEventArgs.Rotate;
                this.Compress = CamEventArgs.Compress;
                this.Quality = CamEventArgs.Quality;
                this.tempPixelFormat = this.pixelFormat;
                this.pixelFormat = CamEventArgs.pixelFormat;
                this.strResolution = CamEventArgs.Resolution;
                this.EdgeDetection = CamEventArgs.EdgeDetection;
                this.AngleCheck = CamEventArgs.AngleCheck;
                this.AngleTolerence = CamEventArgs.AngleTolerence;
                this.InvertBackground = CamEventArgs.InvertBackground;
                this.BackgroundFilter = CamEventArgs.BackgroundFilter;
                DesiredWidth = Convert.ToInt32(strResolution.Split('x')[0]);
                DesiredHeight = Convert.ToInt32(strResolution.Split('x')[1]);
            }
            catch (Exception ex)
            {
                EventMessage("Cam_SettingsChanged - " + ex.Message);
            }
        }

        /// <summary>
        /// shows settings form as a dialog
        /// </summary>
        /// <param name="parent"></param>
        public void ShowProperties(IWin32Window parent)
        {
            try
            {
                FrmSettings.ShowDialog(parent);
            }
            catch
            {
                EventMessage("Cam_ShowProperties - Cannot show camera settings");
            }
        }

        /// <summary>
        /// starts the camera
        /// </summary>
        public void Start()
        {
            try
            {
                VideoSource.Start();
            }
            catch
            {
                EventMessage("Cam_Start - Cannot start camera");
            }
        }

        /// <summary>
        /// stops the camera
        /// </summary>
        public void Stop()
        {
            try
            {
                VideoSource.Stop();
            }
            catch (Exception ex)
            {
                EventMessage("Cam_Stop - Cannot stop camera: " + ex.Message);
            }
        }

        /// <summary>
        /// triggered each time camera grabs a new frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                PbView.BackgroundImage = PrepareImage(eventArgs.Frame, Quality);
                image = null;
            }
            catch (Exception ex)
            {
                EventMessage("Cam_NewFrame - " + ex.Message);
            }
        }

        /// <summary>
        /// prepares grabbed camera frame based on the settings (crop, resolution, flip/rotate etc.)
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        System.Drawing.Image PrepareImage(Bitmap bmp, long quality)
        {
            try
            {
                Resizer = new ResizeBilinear(DesiredWidth, DesiredHeight);
                image = Resizer.Apply(bmp);
                image.SetResolution(DesiredWidth, DesiredHeight);
                qualityParam = new EncoderParameter(Encoder.Quality, quality);
                parameters.Param[0] = qualityParam;
                if (StretchImage) { PbView.BackgroundImageLayout = ImageLayout.Stretch; } else { PbView.BackgroundImageLayout = ImageLayout.Zoom; }
                if (Negative) NegativeFilter.ApplyInPlace(image);
                if (EdgeDetection)
                {
                    image = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)image);
                    EdgeDetector.ApplyInPlace(image);
                }
                if (CropReset)
                {
                    Crop = new Rectangle(0, 0, DesiredWidth, DesiredHeight);
                }
                image.RotateFlip(rotateType);
                switch (rotateType)
                {
                    case RotateFlipType.Rotate90FlipNone:
                    case RotateFlipType.Rotate90FlipX:
                    case RotateFlipType.Rotate90FlipXY:
                    case RotateFlipType.Rotate90FlipY:
                        {
                            Crop = new Rectangle(Crop.Y, Crop.X, Crop.Height, Crop.Width);
                            break;
                        }
                }
                image = image.Clone(Crop, pixelFormat);
                if (AngleCheck) CheckAngle(image);
                if (Compress)
                {
                    using (var mss = new MemoryStream())
                    {
                        image.Save(mss, imageCodec, parameters);
                        return System.Drawing.Image.FromStream(mss);
                    }
                }
                else
                {
                    return image;
                }
            }
            catch (Exception ex)
            {
                pixelFormat = tempPixelFormat;
                EventMessage("Cam_PrepareImage - " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// detects only the corners and draws rectangles on the image
        /// </summary>
        /// <param name="image"></param>
        public void DetectCorners(Bitmap image)
        {
            try
            {
                Graphics graphics = Graphics.FromImage(image);
                SolidBrush brush = new SolidBrush(Color.Red);
                Pen pen = new Pen(brush);
                MoravecCornersDetector mcd = new MoravecCornersDetector();
                List<AForge.IntPoint> corners = mcd.ProcessImage(image);
                foreach (AForge.IntPoint corner in corners)
                {
                    graphics.DrawRectangle(pen, corner.X - 1, corner.Y - 1, 3, 3);
                }
            }
            catch(Exception ex)
            {
                EventMessage("Cam_DetectCorners - " + ex.Message);
            }
        }

        /// <summary>
        /// checks angle between two selected blobs (compares first blob's right edge with second blob's left edge)
        /// </summary>
        /// <param name="image"></param>
        void CheckAngle(Bitmap image)
        {
            try
            {
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
                ColorFiltering colourFilter = new ColorFiltering();
                colourFilter.Red = new AForge.IntRange(0, BackgroundFilter);
                colourFilter.Green = new AForge.IntRange(0, BackgroundFilter);
                colourFilter.Blue = new AForge.IntRange(0, BackgroundFilter);
                colourFilter.FillOutsideRange = this.InvertBackground;
                colourFilter.ApplyInPlace(bitmapData);
                BlobCounter blobCounter = new BlobCounter();
                blobCounter.FilterBlobs = true;
                blobCounter.MinHeight = 15;
                blobCounter.MinWidth = 15;
                blobCounter.ProcessImage(bitmapData);
                Blob[] blobs = blobCounter.GetObjectsInformation();
                Blob[] SelectedBlobs = new Blob[2];
                image.UnlockBits(bitmapData);
                Graphics g = Graphics.FromImage(image);
                Pen redPen = new Pen(Color.Red, 1);
                Pen greenPen = new Pen(Color.Green, 1);
                Pen ParallelPen = new Pen(Color.Red, 1);
                Pen RectanglePen = new Pen(Color.Yellow, 1);
                //draw selected blobs' rectangles and assign them to selected blobs list
                for (int k = 0; k < BlobSelectCoordinate.Count; k++)
                {
                    for (int i = 0; i < blobs.Length; i++)
                    {
                        if (BlobSelectCoordinate[k].X > blobs[i].Rectangle.X && BlobSelectCoordinate[k].X < blobs[i].Rectangle.X + blobs[i].Rectangle.Width)
                        {
                            if (BlobSelectCoordinate[k].Y > blobs[i].Rectangle.Y && BlobSelectCoordinate[k].Y < blobs[i].Rectangle.Y + blobs[i].Rectangle.Height)
                            {
                                SelectedBlobs[k] = blobs[i];
                                g.DrawRectangle(RectanglePen, blobs[i].Rectangle);
                            }
                        }
                    }
                }
                //check angle between two blobs' edges (firts blob's right vs second blob's left edge)
                if(SelectedBlobs[0] != null && SelectedBlobs[1] != null)
                {
                    blobCounter.GetBlobsLeftAndRightEdges(SelectedBlobs[0], out List<AForge.IntPoint> leftEdgeFirst, out List<AForge.IntPoint> rightEdgeFirst);
                    blobCounter.GetBlobsLeftAndRightEdges(SelectedBlobs[1], out List<AForge.IntPoint> leftEdgeSecond, out List<AForge.IntPoint> rightEdgeSecond);
                    //leftedges
                    if(rightEdgeFirst.Count > 20 && leftEdgeSecond.Count > 20)
                    {
                        ParallelPen = (GeometryTools.GetAngleBetweenLines(rightEdgeFirst[rightEdgeFirst.Count / 2 + 10], rightEdgeFirst[rightEdgeFirst.Count / 2 - 10], leftEdgeSecond[leftEdgeSecond.Count / 2 + 10], leftEdgeSecond[leftEdgeSecond.Count / 2 - 10]) < this.AngleTolerence) ? greenPen : redPen;
                    }
                    g.DrawLines(ParallelPen, ToPointsArray(rightEdgeFirst));
                    g.DrawLines(ParallelPen, ToPointsArray(leftEdgeSecond));
                }
                redPen.Dispose();
                greenPen.Dispose();
                g.Dispose();
            }
            catch (Exception ex)
            {
                EventMessage("Cam_CheckAngle - " + ex.Message);
            }
        }

        /// <summary>
        /// converts Aforge.IntPoint list to Syste.Drawing.Point array
        /// </summary>
        /// <param name="points">Aforge.IntPoint list to convert</param>
        /// <returns></returns>
        private Point[] ToPointsArray(List<AForge.IntPoint> points)
        {
            Point[] array = new Point[points.Count];
            try
            {
                for (int i = 0, n = points.Count; i < n; i++)
                {
                    array[i] = new Point(points[i].X, points[i].Y);
                }
                return array;
            }
            catch (Exception ex)
            {
                EventMessage("Cam_ToPointArray - " + ex.Message);
                return array;
            }
        }

        /// <summary>
        /// Disposes instances from memory (this class inherits IDisposable interface)
        /// </summary>
        public void Dispose()
        {
            try
            {
                VideoSource = null;
                FrmSettings.Dispose();
                PbView.Dispose();
                imageCodec = null;
                parameters.Dispose();
                Resizer = null;
                qualityParam.Dispose();
                NegativeFilter = null;
            }
            catch (Exception ex)
            {
                EventMessage("Cam_Dispose - " + ex.Message);
            }
        }
    }

    /// <summary>
    /// delegates settingsChanged event to run FrmSettings_settingsChanged procedure in Camera class, thus changes take effect immediately
    /// </summary>
    /// <param name="CamEventArgs">SettingsChangedEventArgs class instance to send settings to the Camera class</param>
    public delegate void SettingsChanged(SettingsChangedEventArgs CamEventArgs);
    public class SettingsForm : Form
    {
        SettingsChangedEventArgs CamEventArgs = new SettingsChangedEventArgs();
        TableLayoutPanel MainContainer = null;
        CheckBox chkStretchImage = null;
        CheckBox chkNegative = null;
        CheckBox chkCompress = null;
        Label lblRotate = null;
        ComboBox cmbRotate = null;
        ComboBox cmbQuality = null;
        Label lblPixelFormat = null;
        public ComboBox cmbPixelFormat = null;
        Label lblResolution = null;
        public ComboBox cmbResolution = null;
        CheckBox chkEdgeDetection = null;
        CheckBox chkAngleCheck = null;
        CheckBox chkInvertBackground = null;
        public ComboBox cmbAngleTolerance = null;
        Label LblAngleTolerance = null;
        TrackBar TbBackgroundFilter = null;
        public BindingList<string> Resolution = new BindingList<string>();
        public event SettingsChanged settingsChanged;
        Point FirstComponentLocation = new Point(10, 10);

        /// <summary>
        /// Initializes a new instance of settings form with controls on it
        /// </summary>
        public SettingsForm()
        {
            try
            {
                //form settings
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.Size = new Size(350, 300);
                this.Text = "Options";
                //components
                chkStretchImage = new CheckBox();
                chkNegative = new CheckBox();
                lblRotate = new Label();
                cmbRotate = new ComboBox();
                chkCompress = new CheckBox();
                cmbQuality = new ComboBox();
                lblPixelFormat = new Label();
                cmbPixelFormat = new ComboBox();
                lblResolution = new Label();
                cmbResolution = new ComboBox();
                chkEdgeDetection = new CheckBox();
                chkAngleCheck = new CheckBox();
                cmbAngleTolerance = new ComboBox();
                LblAngleTolerance = new Label();
                chkInvertBackground = new CheckBox();
                TbBackgroundFilter = new TrackBar();
                //Main container
                MainContainer = new TableLayoutPanel();
                MainContainer.Dock = DockStyle.Fill;
                MainContainer.ColumnCount = 3;
                MainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
                MainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
                MainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
                MainContainer.RowCount = 10;
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 9F));
                MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
                MainContainer.Controls.Add(chkStretchImage, 0, 0);
                MainContainer.Controls.Add(chkNegative, 0, 1);
                MainContainer.Controls.Add(lblRotate, 0, 2);
                MainContainer.Controls.Add(cmbRotate, 1, 2);
                MainContainer.Controls.Add(chkCompress, 0, 3);
                MainContainer.Controls.Add(cmbQuality, 1, 3);
                MainContainer.Controls.Add(lblPixelFormat, 0, 4);
                MainContainer.Controls.Add(cmbPixelFormat, 1, 4);
                MainContainer.Controls.Add(lblResolution, 0, 5);
                MainContainer.Controls.Add(cmbResolution, 1, 5);
                MainContainer.Controls.Add(chkEdgeDetection, 0, 6);
                MainContainer.Controls.Add(chkAngleCheck, 0, 7);
                MainContainer.Controls.Add(cmbAngleTolerance, 1, 7);
                MainContainer.Controls.Add(LblAngleTolerance, 2, 7);
                MainContainer.Controls.Add(chkInvertBackground, 0, 8);
                MainContainer.Controls.Add(TbBackgroundFilter, 0, 9);
                MainContainer.SetColumnSpan(TbBackgroundFilter, 3);
                MainContainer.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                //Main container
                //stretch image
                chkStretchImage.Checked = true;
                chkStretchImage.Text = "Stretch Image";
                chkStretchImage.AutoSize = true;
                chkStretchImage.Anchor = AnchorStyles.Left;
                chkStretchImage.Enabled = false;
                chkStretchImage.CheckedChanged += UpdateSettings;
                //stretch image
                //negative
                chkNegative.Checked = false;
                chkNegative.Text = "Negative";
                chkNegative.AutoSize = true;
                chkNegative.Anchor = AnchorStyles.Left;
                chkNegative.CheckedChanged += UpdateSettings;
                //negative
                //rotate - label
                lblRotate.Text = "Rotation";
                lblRotate.AutoSize = true;
                lblRotate.Anchor = AnchorStyles.Left;
                //rotate - label
                //rotate - combobox
                cmbRotate.DataSource = Enum.GetNames(typeof(RotateFlipType));
                cmbRotate.SelectedItem = RotateFlipType.RotateNoneFlipNone;
                cmbRotate.Anchor = AnchorStyles.Left;
                cmbRotate.Dock = DockStyle.Fill;
                cmbRotate.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbRotate.SelectedIndexChanged += UpdateSettings;
                //rotate - combobox
                //compress
                chkCompress.Checked = true;
                chkCompress.Text = "Compress";
                chkCompress.AutoSize = true;
                chkCompress.Anchor = AnchorStyles.Left;
                chkCompress.CheckedChanged += UpdateSettings;
                //compress
                //quality
                for (int i = 10; i <= 100; i += 10)
                {
                    cmbQuality.Items.Add(i);
                }
                cmbQuality.SelectedIndex = 9;
                cmbQuality.Anchor = AnchorStyles.Left;
                cmbQuality.Dock = DockStyle.Fill;
                cmbQuality.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbQuality.SelectedIndexChanged += UpdateSettings;
                //quality
                //pixel format - label
                lblPixelFormat.Text = "Pixel Format";
                lblPixelFormat.AutoSize = true;
                lblPixelFormat.Anchor = AnchorStyles.Left;
                //pixel format - label
                //pixel format - combobox
                cmbPixelFormat.DataSource = Enum.GetNames(typeof(PixelFormat));
                cmbPixelFormat.SelectedItem = PixelFormat.Format16bppRgb555;
                cmbPixelFormat.Anchor = AnchorStyles.Left;
                cmbPixelFormat.Dock = DockStyle.Fill;
                cmbPixelFormat.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbPixelFormat.SelectedIndexChanged += UpdateSettings;
                //pixel format - combobox
                //resolution - label
                lblResolution.Text = "Resolution";
                lblResolution.AutoSize = true;
                lblResolution.Anchor = AnchorStyles.Left;
                //resolution - label
                //resolution - combobox
                cmbResolution.Anchor = AnchorStyles.Left;
                cmbResolution.Dock = DockStyle.Fill;
                cmbResolution.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbResolution.SelectedIndexChanged += UpdateSettings;
                //resolution - combobox
                //edge detection
                chkEdgeDetection.Checked = false;
                chkEdgeDetection.Text = "Edge Detection";
                chkEdgeDetection.AutoSize = true;
                chkEdgeDetection.Anchor = AnchorStyles.Left;
                chkEdgeDetection.CheckedChanged += UpdateSettings;
                //edge detection
                //angle check - checkbox
                chkAngleCheck.Checked = false;
                chkAngleCheck.Text = "Angle Check (alpha)";
                chkAngleCheck.AutoSize = true;
                chkAngleCheck.Anchor = AnchorStyles.Left;
                chkAngleCheck.CheckedChanged += UpdateSettings;
                //angle check - checkbox
                //angle tolerance - combobox
                cmbAngleTolerance.Anchor = AnchorStyles.Left;
                cmbAngleTolerance.Dock = DockStyle.Fill;
                cmbAngleTolerance.DropDownStyle = ComboBoxStyle.DropDownList;
                for (int i = 0; i <= 10; i++)
                {
                    cmbAngleTolerance.Items.Add(i);
                }
                cmbAngleTolerance.SelectedIndexChanged += UpdateSettings;
                //angle tolerance - combobox
                //angle tolerance - label
                LblAngleTolerance.Text = "deg.";
                LblAngleTolerance.AutoSize = true;
                LblAngleTolerance.Anchor = AnchorStyles.Left;
                //angle tolerance - label
                //invert background
                chkInvertBackground.Checked = false;
                chkInvertBackground.Text = "Invert Background";
                chkInvertBackground.AutoSize = true;
                chkInvertBackground.Anchor = AnchorStyles.Left;
                chkInvertBackground.CheckedChanged += UpdateSettings;
                //invert background
                //backgroun filter
                TbBackgroundFilter.Minimum = 100;
                TbBackgroundFilter.Maximum = 200;
                TbBackgroundFilter.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                TbBackgroundFilter.Value = 128;
                TbBackgroundFilter.Enabled = false;
                TbBackgroundFilter.ValueChanged += UpdateSettings;
                //backgroun filter
                this.Controls.Add(MainContainer);
            }
            catch
            {
                throw new Exception("Settings_Constructor - Cannot create settings form");
            }
        }

        /// <summary>
        /// This is for disabling/enabling resolution combobox. We don't want to change the resolution of cropped image
        /// </summary>
        /// <param name="Status"></param>
        public void SettingsForm_imageCropped(bool Status)
        {
            cmbResolution.Enabled = !Status;
        }

        /// <summary>
        /// settings form controls' change events are hooked to this event procedure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateSettings(object sender, EventArgs e)
        {
            try
            {
                if (chkNegative.Checked)
                {
                    cmbPixelFormat.Enabled = false;
                    cmbPixelFormat.SelectedIndex = 0;
                }
                else
                {
                    cmbPixelFormat.Enabled = true;
                }
                PixelFormat selectedPixelFormat;
                RotateFlipType selectedRotate;
                Enum.TryParse<PixelFormat>(cmbPixelFormat.SelectedValue.ToString(), out selectedPixelFormat);
                Enum.TryParse<RotateFlipType>(cmbRotate.SelectedValue.ToString(), out selectedRotate);
                CamEventArgs.StretchImage = chkStretchImage.Checked;
                CamEventArgs.Rotate = selectedRotate;
                CamEventArgs.Negative = chkNegative.Checked;
                CamEventArgs.Compress = chkCompress.Checked;
                CamEventArgs.Quality = Convert.ToInt64(cmbQuality.Text);
                CamEventArgs.pixelFormat = selectedPixelFormat;
                CamEventArgs.Resolution = cmbResolution.Text;
                CamEventArgs.EdgeDetection = chkEdgeDetection.Checked;
                CamEventArgs.AngleCheck = chkAngleCheck.Checked;
                CamEventArgs.AngleTolerence = Convert.ToInt32(cmbAngleTolerance.Text);
                CamEventArgs.InvertBackground = chkInvertBackground.Checked;
                CamEventArgs.BackgroundFilter = TbBackgroundFilter.Value;
                TbBackgroundFilter.Enabled = chkAngleCheck.Checked;
                settingsChanged(CamEventArgs);
            }
            catch { }
        }
    }

    /// <summary>
    /// class for sending the settings to Camera class when settingsChanged event triggered
    /// </summary>
    public class SettingsChangedEventArgs
    {
        internal bool StretchImage;
        internal bool Negative;
        internal RotateFlipType Rotate;
        internal bool Compress;
        internal long Quality;
        internal PixelFormat pixelFormat;
        internal string Resolution;
        internal bool EdgeDetection;
        internal bool AngleCheck;
        internal int AngleTolerence;
        internal bool InvertBackground;
        internal int BackgroundFilter;
    }
}