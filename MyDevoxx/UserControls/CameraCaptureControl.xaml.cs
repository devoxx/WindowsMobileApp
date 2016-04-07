using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using ZXing;

namespace MyDevoxx.UserControls
{
    public sealed partial class CameraCaptureControl : UserControl
    {
        #region Private Fields

        private WriteableBitmap wrb;
        private bool isCameraFound = false;
        private string qrCodeContent;
        private static bool running = false;

        private MediaCapture _mediaCapture;

        #endregion

        #region Public Properties and Events
        /// <summary>
        /// Event to let the consumer of the User control know that the user id
        /// has been decoded from the QR code that is scanned
        /// </summary>
        public event EventHandler<CameraClickedEventArgs> UserIdDecoded = delegate { };

        public string QrCodeContent
        {
            get { return qrCodeContent; }
            set { qrCodeContent = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the user control
        /// </summary>
        public CameraCaptureControl()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to handle the Loaded event of the user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Initialze();
        }

        public void Initialze()
        {
            if (!running)
            {
                running = true;
                ScanQrCode();
            }
        }

        private async Task InitializeQrCode()
        {
            DeviceInformationCollection devices = null;
            devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (devices == null || devices.Count == 0)
            {
                isCameraFound = false;
                return;
            }

            // Initializing MediaCapture
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                VideoDeviceId = devices[0].Id,
                AudioDeviceId = "",
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview
            });
            if (_mediaCapture.VideoDeviceController.FlashControl.Supported)
            {
                _mediaCapture.VideoDeviceController.FlashControl.Enabled = false;
            }
            // Adjust camera rotation for Phone
            //_mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            _mediaCapture.SetRecordRotation(VideoRotation.Clockwise90Degrees);

            // Set the source of CaptureElement to MediaCapture
            capturePreview.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();

            isCameraFound = true;
        }

        private async void ScanQrCode()
        {
            await InitializeQrCode();

            if (!isCameraFound)
            {
                return;
            }

            var imgProp = ImageEncodingProperties.CreateJpeg();
            var bcReader = new BarcodeReader
            {
                Options = {
                            TryHarder = false,
                            PossibleFormats = new BarcodeFormat[] { BarcodeFormat.QR_CODE }
                        }
            };

            Result result = null;
            try
            {
                while (result == null)
                {
                    await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();
                    var stream = new InMemoryRandomAccessStream();
                    await _mediaCapture.CapturePhotoToStreamAsync(imgProp, stream);

                    stream.Seek(0);
                    wrb = new WriteableBitmap(1, 1);
                    wrb.SetSource(stream);
                    wrb = new WriteableBitmap(wrb.PixelWidth, wrb.PixelHeight);
                    stream.Seek(0);
                    wrb.SetSource(stream);

                    result = bcReader.Decode(wrb);

                    if (result != null)
                    {
                        CameraClickedEventArgs cameraArgs = null;
                        QrCodeContent = result.Text;
                        cameraArgs = new CameraClickedEventArgs { EncodedData = this.QrCodeContent };
                        if (this.UserIdDecoded != null)
                        {
                            UserIdDecoded(this, cameraArgs);
                        }
                    }
                }
            }
            catch (Exception)
            { }
            Unload();
            running = false;
        }

        public void Unload()
        {
            try
            {
                capturePreview.Source = null;
                if (_mediaCapture != null)
                {
                    _mediaCapture.Dispose();
                }
                wrb = null;

                GC.Collect();
                running = false;
            }
            catch (Exception)
            { }
        }

        #endregion
    }

    public class CameraClickedEventArgs : EventArgs
    {
        private string encodedData;
        public string EncodedData
        {
            get { return encodedData; }
            set { encodedData = value; }
        }
    }
}
