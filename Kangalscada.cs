using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.Data.SqlTypes;
using System.Diagnostics;



namespace Kangalscada
{
    public partial class Kangalscada : Form
    {
        public TcpListener tcpListener;
        public Thread thread;
        DateTime time1, time2, time3, time00, time7, time6, time4, time5, timekh6, time01, time02, time04, time10, time11, time12, time13, time14, time15, time03, time9, time8;

    


    public string connectionSql = ("Data Source=localhost;Initial Catalog=kangal_had;Integrated Security=True");
      

        public logger log;
        public int stand;
        public int WZ;

        public Kangalscada()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(1385, 673
                );
        }


        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null )
            {
                string columnName = textBox.Name; // txt öneki olmadan doğrudan TextBox adını kullan
                Console.WriteLine($"Sunucuda TextBox değişti: {textBox.Name}, Yeni Değer: {textBox.Text}, ColumnName: {columnName}");

                if (double.TryParse(textBox.Text, out double value))
                {
                    if(value>=0 && value <=100)
                    {
                        Console.WriteLine($"Sunucuda TextBox {textBox.Name} güncellendi: {textBox.Text}");
                    }

                   
                }
                else
                {
                    Console.WriteLine($"0-100 arasında bir değer giriniz {columnName}");
                }
            }
        }

        private void StartServer()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            tcpListener = new TcpListener(localAddr, 6000);
            tcpListener.Start();
            Console.WriteLine("Sunucu başlatıldı ve 6000 portunda dinleniyor...");

            thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        Console.WriteLine("Yeni bir bağlantı alındı!");

                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[client.ReceiveBufferSize];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Gelen JSON verisi: {jsonData}");

                            // Verileri JSON'dan ayrıştır
                            var receivedData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);

                            if (receivedData != null)
                            {
                                if (receivedData.ContainsKey("message") && receivedData["message"].ToString() == "startProgressBar")
                                {
                                    Console.WriteLine("Tetik mesajı alındı: startProgressBar");

                                    // ProgressBar'ı başlat
                                    this.BeginInvoke(new Action(async () =>
                                    {
                                      
                                        
                                        progressBar1.Value += 1;

                                        if (progressBar1.Value == progressBar1.Maximum)
                                        {
                                            progressBar1.Value = 0;
                                        }
                                    }));
                                }
                                else if (receivedData.ContainsKey("message1") && receivedData["message1"].ToString() == "stopProgressbar")
                                {
                                    Console.WriteLine("Mesaj alındı: stopProgressbar");

                                    // ProgressBar'ı sıfırla
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        progressBar1.Value = 0;
                                        Console.WriteLine("ProgressBar sıfırlandı.");
                                    }));
                                }
                                else if (receivedData.ContainsKey("mesaj") && receivedData["mesaj"].ToString() == "updateTextBox")
                                {
                                    Console.WriteLine("Güncelleme mesajı alındı: updateTextBox");

                                    // TextBox'u güncelle
                                    string columnName = receivedData["columnName"].ToString();
                                    object value = receivedData["value"];

                                    Console.WriteLine($"Güncelleme için: ColumnName = {columnName}, Value = {value}");

                                    this.BeginInvoke(new Action(() =>
                                    {
                                        UpdateTextBox(columnName, value);
                                    }));
                                }
                                else if (receivedData != null && receivedData.ContainsKey("command") && receivedData["command"].ToString() == "finish")
                                {
                                    Console.WriteLine("Kapanma mesajı alındı: finish");

                                    // Sunucuyu doğrudan kapat
                                    Console.WriteLine("Sunucu kapanıyor...");
                                    Application.Exit();
                                }

                                else if (receivedData.ContainsKey("clear") && receivedData["clear"].ToString() == "cleartextbox")
                                {
                                    Console.WriteLine("Mesaj alındı: cleartextbox");

                                    // ProgressBar'ı sıfırla
                                    this.BeginInvoke(new Action(async () =>
                                    {
                                        await Task.Delay(1000);
                                        clearall();
                                        Console.WriteLine("Textboxlar  sıfırlandı.");
                                    }));
                                }
                                else
                                {
                                    // Diğer veriler için işlemi gerçekleştir
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        PopulateTextBoxes(receivedData);
                                    }));
                                  
                                }
                            }
                            else
                            {
                                Console.WriteLine("Gelen veri JSON olarak ayrıştırılamadı.");
                            }
                        }
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Hata: {ex.Message}");
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }



        private void UpdateTextBox(string columnName, object value)
        {
            try
            {
                Console.WriteLine($"UpdateTextBox metodu çağrıldı: ColumnName = {columnName}, Value = {value}");

                // İlk TextBox'ı güncelle
                Control[] controls = this.Controls.Find(columnName, true);
                if (controls.Length > 0 && controls[0] is TextBox textBox)
                {
                    textBox.TextChanged -= TextBox_TextChanged; // Olay bağlantısını geçici olarak kaldır
                    textBox.Text = value.ToString();
                    textBox.TextChanged += TextBox_TextChanged; // Olayı yeniden bağla

                    Console.WriteLine($"TextBox {textBox.Name} sunucuda güncellendi: {textBox.Text}");
                }
                else
                {
                    Console.WriteLine($"TextBox bulunamadı: {columnName}");
                }

                // İlgili eşleşen TextBox'ları güncelle
                string pairedColumnName = null;
                if (columnName == "H2" || columnName == "H3")
                {
                    pairedColumnName = (columnName == "H2") ? "H3" : "H2";
                }
                else if (columnName == "H4" || columnName == "H5")
                {
                    pairedColumnName = (columnName == "H4") ? "H5" : "H4";
                }
                else if (columnName == "H6" || columnName == "H7")
                {
                    pairedColumnName = (columnName == "H6") ? "H7" : "H6";
                }

                if (pairedColumnName != null)
                {
                    Control[] pairedControls = this.Controls.Find(pairedColumnName, true);
                    if (pairedControls.Length > 0 && pairedControls[0] is TextBox pairedTextBox)
                    {
                        pairedTextBox.TextChanged -= TextBox_TextChanged; // Olay bağlantısını geçici olarak kaldır
                        pairedTextBox.Text = value.ToString();
                        pairedTextBox.TextChanged += TextBox_TextChanged; // Olayı yeniden bağla

                        Console.WriteLine($"TextBox {pairedTextBox.Name} sunucuda güncellendi: {pairedTextBox.Text}");
                    }
                    else
                    {
                        Console.WriteLine($"TextBox bulunamadı: {pairedColumnName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTextBox hatası: {ex.Message}");
            }
        }



        private void SendUpdatedData(string key, string value)
        {
            if (decimal.TryParse(value, out decimal decimalValue))
            {
                if (decimalValue >= 0 && decimalValue <= 100)
                {
                    var updatedData = new Dictionary<string, string>
            {
                { key, value }
            };

                    // H2'deki değeri H3'e ve FAN'a da atıyoruz
                    if (key == "H2")
                    {
                        updatedData["H3"] = value;
                        H3.Text = value; // H3 TextBox'ını güncelle
                    }
                    if (key == "H4")
                    {
                        updatedData["H5"] = value;
                        H5.Text = value; // H3 TextBox'ını güncelle
                    }
                    if (key == "H6")
                    {
                        updatedData["H7"] = value;
                        H7.Text = value; // H3 TextBox'ını güncelle
                    }
                    if (key == "H7")
                    {
                        updatedData["H6"] = value;
                        H6.Text = value; // H3 TextBox'ını güncelle
                    }
                    if (key == "H5")
                    {
                        updatedData["H4"] = value;
                        H4.Text = value; // H3 TextBox'ını güncelle
                    }
                    if (key == "H3")
                    {
                        updatedData["H2"] = value;
                        H2.Text = value; // H3 TextBox'ını güncelle
                    }
                    // FAN TextBox'ının verisini güncelle
                    if (key == "FAN")
                    {
                        updatedData["FAN"] = value;
                        FAN.Text = value; // FAN TextBox'ını güncelle
                    }

                    string jsonData = JsonSerializer.Serialize(updatedData, new JsonSerializerOptions { WriteIndented = true });

                    // TCP Client bağlantısı
                    using (var tcpClient = new TcpClient("127.0.0.1", 4545))
                    {
                        using (NetworkStream stream = tcpClient.GetStream())
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Geçerli bir değer giriniz (0-100): {key}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Geçerli bir sayı giriniz: {key}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    if (double.TryParse(textBox.Text, out double value))
                    {
                        // Değerin 0 ile 100 arasında olup olmadığını kontrol et (0 ve 100 dahil)
                        if (value >= 0 && value <= 100)
                        {
                            SendUpdatedData(textBox.Name, textBox.Text); // Yalnızca geçerli veriyi gönder
                            Console.WriteLine($"TextBox güncellendi ve veri gönderildi: {textBox.Name}, Değer: {textBox.Text}");
                        }
                        else
                        {
                            textBox.Clear(); // Hatalı değerde TextBox'u temizle
                            MessageBox.Show($"Lütfen 0 ile 100 arasında bir değer giriniz: {textBox.Name}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        textBox.Clear(); // Geçersiz değer girildiğinde TextBox'u temizle
                        MessageBox.Show($"Geçerli bir değer giriniz: {textBox.Name}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                e.Handled = true; // Enter tuşunun varsayılan işlemini engelle
            }
        }



        private void clearall()
        {
            FÇS.Text = "";
            KH1.Text = "";
            H1.Text = "";
            Y1.Text = "";
            RM.Text = "";
            KH2.Text= "";
            KH3.Text = "";
            H2.Text = "";
            Y2.Text = "";
            
            DESC.Text ="";
            H3.Text = "";
            KH4.Text = "";
            H4.Text = "";
            Y4.Text = "";
            IM.Text = "";
            KH6.Text = "";
            KH7.Text = "";
            KH5.Text = "";
            H5.Text = "";
            Y7.Text = "";
            LOOPFBL.Text = "";
            KH8.Text = "";
            H6.Text = "";
            H7.Text = "";
            Y6.Text = "";
            CVB4.Text = "";
            CV2.Text = "";
            CVV1.Text = "";
            sermekafa.Text = "";
            FAN.Text = ""
                ;
            KAPAK.Text = "";
            WZTemp.Text = "";
            WZ1.Text = "";
            WZ2.Text = "";
            WZ3.Text = "";
             WZ4.Text = "";
            LOOPFBL.Text = "";
            LOOPH14.Text = "";
            LOOPH15.Text = "";
            C15.Text = "";
            S15.Text = "";
            S9.Text = "";
            FBL.Text = "";
            H14.Text = "";
            KH14.Text = "";
            H13.Text = "";
            H12.Text = "";
            H11.Text = "";
            H10.Text = "";
            H9.Text = "";
            H8.Text = "";
            KH9.Text = "";
            KH10.Text = ""; 
            KH11.Text = "";
            KH12.Text = "";
            KH13.Text = "";
            KH14.Text = "";
            KH15.Text = "";
            IMH.Text = "";
            FM.Text = "";
            H15HADD.Text = "";
            Y8.Text = "";
            Y9.Text = "";
            Y10.Text = "";
            Y11.Text = "";
            Y12.Text = "";
            Y14.Text = "";
            Y3.Text = "";
            H15.Text = "";
            Y5.Text = "";
            H8TEMP.Text = "";
            FÇS.Text = "";
            FBLB.Text = "";
            Y13.Text = "";
            Y15.Text = "";
        }
      



        private void PopulateTextBoxes(Dictionary<string, object> data)
        {
            //StartIncrementProgressBar(8);
            //StartIncrementProgressBar(12);
            string logFilePath = ("log.txt"); // Uygulama klasöründe log.txt oluşturur
            logger log = new logger(logFilePath);
            foreach (var kvp in data)
            {
                string key = kvp.Key;
                object value = kvp.Value;

                Console.WriteLine($"Updating TextBox for {key} with value {value}.");

                switch (key)
                {
                    case "FÇS":
                        { 
                                FÇS.Text = value.ToString() + " " + "°C";
                                Console.WriteLine("FÇS updated"); 
                        }
                        break;
                    case "KH1":
                        {
                            { 
                                stand = 1;
                                time1 = DateTime.Now;
                                string zaman1 = time1.ToString("dd.MM.yyyy HH:mm:ss");
                                log.WriteLog($"Giriş zamanı: {time1}",  $"Stand1'e malzeme girdi. stand: {stand}");
                                KH1.Text = value.ToString();
                                Console.WriteLine("KH1 updated");
                               
                            }
                        }
                        break;
                    case "H1":
                        //string veri = Convert.ToString(value);
                        //H1.DecimalPlaces = 2;
                        //H1.Increment = 0.01M;
                        //decimal veri = decimal.Round(value, 3);
                        H1.Text = value.ToString();


                        //H1.Value = veri;

                        
                        Console.WriteLine("H1 updated");
                        break;
                    case "Y1":
                        Y1.Text = value.ToString();
                        Console.WriteLine("Y1 updated");
                        break;
                    case "RM":
                        RM.Text = value.ToString();
                        Console.WriteLine("RM updated");
                        break;

                    case "DS":
                        {
                           
                         DS.Text = value.ToString();
                        Console.WriteLine("DS updated");
                            
                        }
                        break;

                    case "KH2":
                        stand = 2;
                        time2 = DateTime.Now;
                        string zaman2=time2.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman2}",  $"Stand2'e malzeme girdi. stand: {stand}");

                        KH2.Text = value.ToString();
                        Console.WriteLine("KH2 updated");
                        break;
                    case "KH3":
                        stand = 3;
                        time3 = DateTime.Now;
                        string zaman3=time3.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman3}",  $"Stand3'e malzeme girdi. stand: {stand}");
                        KH3.Text = value.ToString();
                        Console.WriteLine("KH3 updated");
                        break;
                    case "H2":
                        H2.Text = value.ToString();
                        Console.WriteLine("H2 updated");
                        break;
                    case "H3":
                        H3.Text = value.ToString();
                        Console.WriteLine("H3 updated");
                        break;
                    case "Y2":
                        Y2.Text = value.ToString();
                        Console.WriteLine("Y2 updated");
                        break;
                    case "Y3":
                        Y3.Text = value.ToString();
                        Console.WriteLine("Y3 updated");
                        break;
                    case "KH4":
                        stand = 4;
                        time4 = DateTime.Now;
                        string zaman4 = time4.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman4}",  $"Stand4'e malzeme girdi. stand: {stand}");
                        KH4.Text = value.ToString();
                        Console.WriteLine("KH4 updated");
                        break;
                    case "KH5":
                        stand = 5;
                        time5 = DateTime.Now;
                        string zaman5 = time5.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman5}",  $"Stand5'e malzeme girdi. stand: {stand}");
                        KH5.Text = value.ToString();
                        Console.WriteLine("KH5 updated");
                        break;
                    case "H4":
                        H4.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H5":
                        H5.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y4":
                        Y4.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "Y5":
                        Y5.Text = value.ToString();
                        Console.WriteLine("Y5 updated");
                        break;
                    case "KH6":
                        stand = 6;
                        time6 = DateTime.Now;
                        string zaman6 = time6.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman6}",  $"Stand6'a malzeme girdi. stand: {stand}");
                        KH6.Text = value.ToString();
                        Console.WriteLine("KH4 updated");
                        break;
                    case "KH7":
                        stand = 7;
                        time7 = DateTime.Now;
                        string zaman7 = time7.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman7}",  $"Stand7'e malzeme girdi. stand: {stand}");
                        KH7.Text = value.ToString();
                        Console.WriteLine("KH5 updated");
                        break;
                    case "H6":
                        H6.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H7":
                        H7.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y6":
                        Y6.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "Y7":
                        Y7.Text = value.ToString();
                        Console.WriteLine("Y5 updated");
                        break;
                    case "KH8":
                        stand = 8;
                        time8 = DateTime.Now;
                        string zaman8 = time8.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman8}",  $"Stand8'e malzeme girdi. stand: {stand}");
                        KH8.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H8":
                        H8.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y8":
                        Y8.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "H8TEMP":
                        H8TEMP.Text = value.ToString() + " " + "°C";
                        Console.WriteLine("Y5 updated");
                        break;
                    case "KH9":
                        stand = 9;
                        time9 = DateTime.Now;
                        string zaman9 = time9.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman9}",  $"Stand9'a malzeme girdi. stand: {stand}");
                        KH9.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "IM":
                        IM.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;

                    case "H9":
                        H9.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y9":
                        Y9.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "S9":
                        S9.Text = value.ToString();
                        Console.WriteLine("Y5 updated");
                        break;
                    case "KH10":
                        stand = 10;
                        time10 = DateTime.Now;
                        string zaman10= time10.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman10}",  $"Stand10'a malzeme girdi. stand: {stand}");
                        KH10.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H10":
                        H10.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y10":
                        Y10.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "KH11":
                        stand = 11;
                        time11 = DateTime.Now;
                        string zaman11= time11.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman11}",  $"Stand11'e malzeme girdi. stand: {stand}");
                        KH11.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H11":
                        H11.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y11":
                        Y11.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "KH12":
                        stand = 12;
                        time12 = DateTime.Now;
                        string zaman12 = time12.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman12}",  $"Stand12'e malzeme girdi. stand: {stand}");
                        KH12.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H12":
                        H12.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y12":
                        Y12.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "KH13":
                        stand = 13;
                        time13 = DateTime.Now;
                        string zaman13 = time13.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman13}",  $"Stand13'e malzeme girdi. stand: {stand}");
                        KH13.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "IMH":
                        IMH.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H13":
                        H13.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y13":
                        Y13.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                        
                    case "KH14":
                        stand = 14;
                        time14 = DateTime.Now;
                        string zaman14 = time14.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman14}",  $"Stand14'e malzeme girdi. stand: {stand}");
                        KH14.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H14":
                        H14.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y14":
                        Y14.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "LOOPH14":
                        LOOPH14.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "FBL":
                        FBL.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;

                    case "KH15":
                        stand = 15;
                        time15 = DateTime.Now;
                        string zaman15 = time15.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Giriş zamanı: {zaman15}",  $"Stand15'e malzeme girdi. stand: {stand}");
                        KH15.Text = value.ToString(); 
                        H15HADD.Text = value.ToString();
                        Console.WriteLine("H4 updated");
                        break;
                    case "H15":
                        H15.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "Y15":
                        Y15.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "S15":
                        S15.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "C15":
                        C15.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "LOOPH15":
                        LOOPH15.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "FBLB":
                        FBLB.Text = value.ToString() + " " + "°C";
                        Console.WriteLine("Y4 updated");
                        break;
                    case "LOOPFBL":
                        LOOPFBL.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "WZ1":
                        WZ = 1;
                        time00 = DateTime.Now;
                        string zaman0 = time00.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Soğutma zamanı: {zaman0}",  $"Soğutma işlemi başlatıldı. WZ: {WZ}");
                        WZ1.Text = value.ToString();
                        Console.WriteLine("H5 updated");
                        break;
                    case "WZ2":
                        WZ = 2;
                        time01 = DateTime.Now;
                        string zaman01 = time01.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Soğutma zamanı: {zaman01}",  $"Soğutma işlemi başlatıldı. WZ: {WZ}");
                       
                        WZ2.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "WZ3":
                        WZ = 3;
                        time02 = DateTime.Now;
                        string zaman02 = time02.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Soğutma zamanı: {zaman02}",  $"Soğutma işlemi başlatıldı. WZ: {WZ}");
                        WZ3.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "WZ4":
                        WZ = 4;
                        time03 = DateTime.Now;
                        string zaman03 = time03.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"Soğutma zamanı: {zaman03}",  $"Soğutma işlemi başlatıldı. WZ: {WZ}");
                        WZ4.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "WZ":
                        WZTemp.Text = value.ToString() + " " + "°C";
                        Console.WriteLine("Y4 updated");
                        break;
                    case "KAPAK":
                        KAPAK.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "FAN":
                        WZ = 1;
                        time04 = DateTime.Now;
                        string zaman04 = time04.ToString("dd.MM.yyyy HH:mm:ss");
                        log.WriteLog($"FAN zamanı :{zaman04}",  $"FAN işlemi başlatıldı.FAN: {WZ}");
                        FAN.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "FM":
                        FM.Text = value.ToString();
                        Console.WriteLine("Y4 updated");
                        break;
                    case "SermeKafa":
                        sermekafa.Text = value.ToString() + " " + "°C";
                        CVV1.Text = value.ToString() + " " + "°C";
                        Console.WriteLine("Y4 updated");
                        break;
                    case "CV2":
                        CV2.Text = value.ToString() + " " + "°C";
                        Console.WriteLine("Y4 updated");
                        break;
                    case "CV4":
                        CVB4.Text = value.ToString() + " " + "°C";
                        break;
                }
            }
        }
        private void Kangalscada_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 15; i++)
            {
                Control[] controls = this.Controls.Find($"H{i}", true);
                if (controls.Length > 0 && controls[0] is TextBox textBox)
                {
                    textBox.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
                }
            }

            // FAN TextBox'ının KeyPress olayını bağlama
            Control[] fanControls = this.Controls.Find("FAN", true);
            if (fanControls.Length > 0 && fanControls[0] is TextBox fanTextBox)
            {
                fanTextBox.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            }
            StartServer();        
        }
    }
}




