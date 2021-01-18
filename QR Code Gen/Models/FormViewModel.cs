using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace WpfApp2.Models
{
    class FormViewModel: INotifyPropertyChanged
    {
        
        public ObservableCollection<string> Types { get; set; }

        private string _currentItem;
        public string CurrentItem { get; set; }

        public string InputData { get; set; } = "";
        
        public string InputData2 { get; set; } = "";

        private bool _isPngEnabled = false;

        public bool IsPngEnabled
        {
            get => _isPngEnabled;
            set
            {
                _isPngEnabled = value;
                OnPropertyChanged("IsPngEnabled");
            }
        }

        public ImageSource qrCode;

        public ImageSource QRCode
        {
            get => qrCode;
            set
            {
                qrCode = value;
                OnPropertyChanged();
            }
        }

        private Bitmap _QRCode;
        public FormViewModel()
        {
            Types = new ObservableCollection<string> { "Wi-Fi", "Mail", "URL", "Text" };
            CurrentItem = Types[0];
        }

        private ImageSource Generate(string text, string text2)
        {
            if (text.Length == 0) return null;
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData;
            
            switch (CurrentItem)
            {
                case "Text":
                    qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                    break;
                case "Wi-Fi": 
                    PayloadGenerator.WiFi wifiPayload = new PayloadGenerator.WiFi(text, text2, PayloadGenerator.WiFi.Authentication.WPA);
                    qrCodeData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
                    break;
                case "Mail":
                    PayloadGenerator.Mail mail = new PayloadGenerator.Mail(text, text2);
                    qrCodeData = qrGenerator.CreateQrCode(mail.ToString(), QRCodeGenerator.ECCLevel.Q);
                    break;
                case "URL": 
                    PayloadGenerator.Url generator = new PayloadGenerator.Url(text);
                    qrCodeData = qrGenerator.CreateQrCode(generator.ToString(), QRCodeGenerator.ECCLevel.Q);
                    break;
                default:
                    qrCodeData=null;
                    break;
            }

            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20,  Color.DarkRed, Color.PaleGreen, true);
            _QRCode = qrCodeImage;
            var handle = qrCodeImage.GetHbitmap();

            IsPngEnabled = true;
            
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        private void SaveQRCode()
        {
            
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg "
            };
            
            if (dialog.ShowDialog() == null) return;
            
            _QRCode.Save(dialog.FileName, ImageFormat.Jpeg); 
        }



        public RelayCommand GenerateButtonClick =>
            new RelayCommand(obj => { QRCode = Generate(InputData, InputData2); });

        public RelayCommand SaveQRCodeBtn =>
            new RelayCommand(obj => { SaveQRCode(); });
        
        public RelayCommand ChangeItem => 
            new RelayCommand(o =>
            {
                MessageBox.Show("Hello");
            });

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
