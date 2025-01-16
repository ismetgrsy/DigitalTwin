using BilletApp;
using LoggingLibrary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace BilletTracking

{
    public partial class BilletForm : Form
    { 
        public string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        Stand H1, H2, H3, H4, H5, H6, H7, H8, H9, H10, H11, H12, H13, H14, H15;
        public Billet billet;
        public string connectionSql = ("Data Source=localhost;Initial Catalog=kangal_had;Integrated Security=True");
        public string randomquality;
        Random rnd = new Random();
        public int randomIndex;
        public string[] qualities = { "5.2580_01", "1.1006_00", "6.3006_00", "6.1360_03", "1.1008_00", "1.1070_00" };
        logger log;
        public string ID;
        public double s9 = 100;
        public double c15 = 100;
        public double s15 = 100;
        //private void Datagridview_stand_CellClick(object sender, DataGridViewCellEventArgs e)
        //{

        //   txtID.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[0].Value.ToString();
        //    txtH1.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[1].Value.ToString();
        //    txtH2.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[2].Value.ToString();
        //    txtH2.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[3].Value.ToString();
        //    txtH4.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[4].Value.ToString();
        //    txtH4.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[5].Value.ToString();
        //    txtH6.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[6].Value.ToString();
        //    txtH6.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[7].Value.ToString();
        //    txtH8.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[8].Value.ToString();
        //    txtH9.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[9].Value.ToString();
        //    txtH10.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[10].Value.ToString();  // 11. hücre, index 10 olmalı
        //    txtH11.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[11].Value.ToString();  // 12. hücre, index 11 olmalı
        //    txtH12.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[12].Value.ToString();  // 13. hücre, index 12 olmalı
        //    txtH13.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[13].Value.ToString();  // 14. hücre, index 13 olmalı
        //    txtH14.Text =   dataGridView_standhızı.Rows[e.RowIndex].Cells[14].Value.ToString();  // 15. hücre, index 14 olmalı
        //    txtH15.Text = dataGridView_standhızı.Rows[e.RowIndex].Cells[15].Value.ToString();  // 16. hücre, index 15 olmalı
        //}

        private void StandVerilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView_standhızı.Visible = true;
            dataGridViewKutuk.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_su.Visible = false;
        }
        public BilletForm()
        {
            StartServer();
            InitializeComponent();    
            Random rnd = new Random();
         
            H1 = new Stand();
            H2 = new Stand();
            H3 = new Stand();
            H4 = new Stand();
            H5 = new Stand();
            H6 = new Stand();
            H7 = new Stand();
            H8 = new Stand();
            H9 = new Stand();
            H10 = new Stand();
            H11 = new Stand();
            H12 = new Stand();
            H13 = new Stand();
            H14 = new Stand();
            H15 = new Stand();
            billet = new Billet();
            this.StartPosition = FormStartPosition.CenterScreen; // Merkezde başlat
            this.Size = new System.Drawing.Size(2100, 1300); // Genişlik: 800, Yükseklik: 600
            billet = new Billet();
        }

        private void BilletForm_Load(object sender, EventArgs e)
        {

            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.KeyDown += TextBox_KeyDown;
                }
            }

            Process.Start(@"C:\Users\is\source\repos\Kangalscada\bin\Debug\Kangalscada.exe");
            this.Show();
            timer1.Start();
        
            DateTime starttime = DateTime.Now;
            string time1 = starttime.ToString("dd.MM.yyyy HH:mm:ss"); // Formatı düzelttim

            ID = starttime.ToString("yyMMddHHmmssff");
            log = new logger(logFilePath);
            log.WriteLog($"Başlama zamanı: {time1}",  $"Üretim başladı. ID: {ID}");



            Randomq();
            ProcessMaterial();
            Hesaplama();
            Alldata();
        SendSelectedData();

            dataGridViewKutuk.Visible = true;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;

        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    string columnName = textBox.Name.Substring(3); // Örneğin, txtH2 -> H2
                    Console.WriteLine($"TextBox değişti: {textBox.Name}, Yeni Değer: {textBox.Text}, ColumnName: {columnName}");

                    // TextBox boş ise işlem yapma
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Clear(); // Boş olduğunda TextBox'u temizle
                        return;
                    }

                    if (double.TryParse(textBox.Text, out double value))
                    {
                        // Değerin 0 ile 100 arasında olup olmadığını kontrol et (0 ve 100 dahil)
                        if (value >= 0 && value <= 100)
                        {
                            try
                            {
                                // Eşleşen diğer TextBox'ı güncelle
                                UpdatePairedTextBox(textBox.Name, value);

                                // Geçerli değeri sunucuya ve veritabanına gönder
                                UpdateTBLSTAND(ID, columnName, value); // SQL TBLSTAND tablosunu güncelle
                                UpdateDataGridView(columnName, value); // DataGridView'ı güncelle

                                // Sunucuya güncelleme mesajı gönder
                                var dataToSend = new Dictionary<string, object>
                        {
                            { "mesaj", "updateTextBox" },
                            { "columnName", columnName },
                            { "value", value }
                        };

                                Console.WriteLine("Sunucuya veri gönderiliyor...");
                                SendDataToServer(dataToSend);
                                Console.WriteLine("Veri sunucuya gönderildi.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Veri gönderme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            textBox.Clear();
                            MessageBox.Show($"Lütfen 0 ile 100 arasında bir değer giriniz: {columnName}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                  
                    e.SuppressKeyPress = true; // Enter tuşunun başka etkileşimlere neden olmasını engelle
                }
            }
        }

        private void UpdatePairedTextBox(string textBoxName, double value)
        {
            string pairedTextBoxName = null;

            // Eşleşen TextBox'ları belirle
            if (textBoxName == "txtH2") pairedTextBoxName = "txtH3";
            else if (textBoxName == "txtH3") pairedTextBoxName = "txtH2";
            else if (textBoxName == "txtH4") pairedTextBoxName = "txtH5";
            else if (textBoxName == "txtH5") pairedTextBoxName = "txtH4";
            else if (textBoxName == "txtH6") pairedTextBoxName = "txtH7";
            else if (textBoxName == "txtH7") pairedTextBoxName = "txtH6";

            if (pairedTextBoxName != null)
            {
                Control[] pairedControls = this.Controls.Find(pairedTextBoxName, true);
                if (pairedControls.Length > 0 && pairedControls[0] is TextBox pairedTextBox)
                {
                    pairedTextBox.TextChanged -= TextBox_TextChanged; // Olay bağlantısını geçici olarak kaldır
                    pairedTextBox.Text = value.ToString();
                    pairedTextBox.TextChanged += TextBox_TextChanged; // Olayı yeniden bağla

                    Console.WriteLine($"TextBox {pairedTextBox.Name} istemcide güncellendi: {pairedTextBox.Text}");
                }
                else
                {
                    Console.WriteLine($"TextBox bulunamadı: {pairedTextBoxName}");
                }
            }
        }
        private void InitializeDataGridView(DataGridView dataGridView, string[] columns)
        {
            dataGridView.Columns.Clear();
            foreach (var column in columns)
            {
                dataGridView.Columns.Add(column, column);
            }
        }
        private void AddDataToGridView(DataGridView dataGridView, Dictionary<string, object> data)
        {
            // Yeni satır ekle
            int rowIndex = dataGridView.Rows.Add();

            // Her bir veriyi yeni satıra ekle
            foreach (var key in data.Keys)
            {
                int columnIndex = dataGridView.Columns[key]?.Index ?? -1;
                if (columnIndex >= 0)
                {
                    dataGridView.Rows[rowIndex].Cells[columnIndex].Value = data[key];
                }
            }
        }
        private Dictionary<string, string> currentData = new Dictionary<string, string>();

        private void StartServer()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener tcpListener = new TcpListener(localAddr, 4545);
            tcpListener.Start();
            Console.WriteLine("Sunucu başlatıldı ve 5050 portunda dinleniyor...");

            Thread thread = new Thread(() =>
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

                            // Veriyi JSON'dan ayrıştır
                            var receivedData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);

                            if (receivedData != null)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    RefreshTextBoxes(receivedData); // Gelen verileri TextBox'lara güncelle
                                }));
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
            })
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string columnName = textBox.Name.Substring(3); // Örn: txtH1 -> H1
                Console.WriteLine($"TextBox değişti: {textBox.Name}, Yeni Değer: {textBox.Text}, ColumnName: {columnName}");

                if (double.TryParse(textBox.Text, out double value))
                {
                    UpdateTBLSTAND(ID, columnName, value);
                    UpdateDataGridView(columnName, value);
                }
               
            }
        }
        private void UpdateTBLSTAND(string ID, string columnName, double value)
        {
            using (SqlConnection connection = new SqlConnection(connectionSql))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Veritabanına bağlanıldı.");

                    // SQL güncelleme sorgusu
                    string updateQuery = $"UPDATE TBLSTAND SET {columnName} = @Value WHERE ID = @ID";
                    Console.WriteLine($"Güncelleme Sorgusu: {updateQuery}");

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Value", value);
                        command.Parameters.AddWithValue("@ID", ID);
                        Console.WriteLine($"Parametreler: @Value = {value}, @ID = {ID}");

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Güncellenen Satır Sayısı: {rowsAffected}");

                        if (rowsAffected == 0)
                        {
                            Console.WriteLine("Güncelleme yapılmadı. ID ve ColumnName değerlerini kontrol edin.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Veritabanı güncelleme hatası: {ex.Message}");
                    MessageBox.Show($"Veritabanı güncelleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Veritabanı bağlantısı kapatıldı.");
                }
            }
        }
        private void UpdateDataGridView(string columnName, double value)
        {
            // Değerin 0-100 aralığında olup olmadığını kontrol et
            if (value < 0 || value > 100)
            {
                Console.WriteLine($"Değer {value} 0-100 aralığında olmadığı için güncellenmedi.");
                return;
            }

            foreach (DataGridViewRow row in dataGridView_standhızı.Rows)
            {
                if (row.Cells["ID"].Value.ToString() == ID)
                {
                    row.Cells[columnName].Value = value;
                    Console.WriteLine($"DataGridView güncellendi: {columnName} = {value}");

                    // Eşleşen diğer sütunu da güncelle
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
                        row.Cells[pairedColumnName].Value = value;
                        Console.WriteLine($"DataGridView güncellendi: {pairedColumnName} = {value}");
                    }

                    break;
                }
            }
        }
        private void RefreshTextBoxes(Dictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                // Mevcut verileri güncelle
                currentData[kvp.Key] = kvp.Value?.ToString();

                // TextBox'ları güncelle
                Control[] controls = this.Controls.Find("txt" + kvp.Key, true);
                if (controls.Length > 0 && controls[0] is TextBox textBox)
                {
                    textBox.TextChanged -= TextBox_TextChanged; // Olay bağlantısını geçici olarak kaldır
                    textBox.Text = kvp.Value?.ToString();
                    textBox.TextChanged += TextBox_TextChanged; // Olayı yeniden bağla

                    Console.WriteLine($"TextBox {textBox.Name} güncellendi: {textBox.Text}");
                }

                // DataGridView'ı güncelle
                if (kvp.Value != null && double.TryParse(kvp.Value.ToString(), out double value))
                {
                    UpdateDataGridView(kvp.Key, value);
                    UpdateTBLSTAND(ID, kvp.Key, value); // SQL TBLSTAND tablosunu güncelle
                }
            }
        }
        private void Alldata()
        {
            try
            {
                InitializeDataGridView(dataGridView_malzemehızı, new string[]
                {
            "ID", "RM", "KH1", "KH2", "KH3", "KH4", "KH5", "KH6", "KH7", "KH8", "KH9", "IM", "KH10", "KH11", "KH12", "KH13", "IMH", "KH14", "KH15", "FM"
                });
                InitializeDataGridView(dataGridView_yük, new string[]
                {
            "ID", "Y1", "Y2", "Y3", "Y4", "Y5", "Y6", "Y7", "Y8", "Y9", "Y10", "Y11", "Y12", "Y13", "Y14", "Y15", "FBL"
                });
                InitializeDataGridView(dataGridView_su, new string[]
                {
            "ID", "WZ1", "WZ2", "WZ3", "WZ4", "FAN", "KAPAK", "LOOPH14", "LOOPH15", "LOOPFBL",  "DS"
                });
                InitializeDataGridView(dataGridView_standhızı, new string[]
                {
            "ID", "H1", "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9", "S9", "H10", "H11", "H12", "H13", "H14", "H15", "S15", "C15", "FBL"
                });
                InitializeDataGridView(dataGridView_sıcaklık, new string[]
                {
            "ID", "FÇS", "H8TEMP", "FBLB", "WZ", "SermeKafa", "CV2", "CV4"
                });

                var tempData = new Dictionary<string, object>();
                switch (randomIndex)
                {
                    case 0:
                        billet.Temperature = rnd.Next(950, 1001);
                        billet.H8Temp = rnd.Next(950, 961);
                        billet.FBLB = rnd.Next(960, 991);
                        billet.CV2 = rnd.Next(300, 401);
                        billet.CV4 = rnd.Next(300, 401);
                        billet.WZ = rnd.Next(1150, 1181);
                        billet.SermeKafaSıcaklığı = rnd.Next(800, 851);
                        tempData = new Dictionary<string, object>
                {
                    { "ID", ID},

                    { "FÇS",billet.Temperature },
                    { "H8TEMP",billet.H8Temp },
                    { "FBLB", billet.FBLB},
                    { "WZ", billet.WZ },
                    { "SermeKafa",billet.SermeKafaSıcaklığı },
                    { "CV2", billet.CV2},
                    { "CV4",  billet.CV4 }
                };
                        break;
                    case 1:
                        billet.Temperature = rnd.Next(1000, 1101);
                        billet.H8Temp = rnd.Next(955, 961);
                        billet.FBLB = rnd.Next(970, 991);
                        billet.CV2 = rnd.Next(350, 401);
                        billet.CV4 = rnd.Next(320, 401);
                        billet.WZ = rnd.Next(1160, 1181);
                        billet.SermeKafaSıcaklığı = rnd.Next(850, 881);
                        tempData = new Dictionary<string, object>
                {
                    { "ID", ID},

                    { "FÇS",billet.Temperature },
                    { "H8TEMP",billet.H8Temp },
                    { "FBLB", billet.FBLB},
                    { "WZ", billet.WZ },
                    { "SermeKafa",billet.SermeKafaSıcaklığı },
                    { "CV2", billet.CV2},
                    { "CV4",  billet.CV4 }
                };
                        break;
                    case 2:
                        billet.Temperature = rnd.Next(1050, 1101);
                        billet.H8Temp = rnd.Next(960, 966);
                        billet.FBLB = rnd.Next(980, 991);
                        billet.CV2 = rnd.Next(400, 451);
                        billet.CV4 = rnd.Next(330, 401);
                        billet.WZ = rnd.Next(1170, 1181);
                        billet.SermeKafaSıcaklığı = rnd.Next(870, 901);
                        tempData = new Dictionary<string, object>
                {
                    { "ID", ID},

                    { "FÇS",billet.Temperature },
                    { "H8TEMP",billet.H8Temp },
                    { "FBLB", billet.FBLB},
                    { "WZ", billet.WZ },
                    { "SermeKafa",billet.SermeKafaSıcaklığı },
                    { "CV2", billet.CV2},
                    { "CV4",  billet.CV4 }
                };
                        break;
                    case 3:
                        billet.Temperature = rnd.Next(1100, 1201);
                        billet.H8Temp = rnd.Next(960, 971);
                        billet.FBLB = rnd.Next(980, 1011);
                        billet.CV2 = rnd.Next(370, 401);
                        billet.CV4 = rnd.Next(310, 401);
                        billet.WZ = rnd.Next(1165, 1181);
                        billet.SermeKafaSıcaklığı = rnd.Next(880, 901);
                        tempData = new Dictionary<string, object>
                {
                   { "ID", ID},

                    { "FÇS",billet.Temperature },
                    { "H8TEMP",billet.H8Temp },
                    { "FBLB", billet.FBLB},
                    { "WZ", billet.WZ },
                    { "SermeKafa",billet.SermeKafaSıcaklığı },
                    { "CV2", billet.CV2},
                    { "CV4",  billet.CV4 }
                };
                        break;
                    case 4:
                        billet.Temperature = rnd.Next(950, 1101);
                        billet.H8Temp = rnd.Next(950, 956);
                        billet.FBLB = rnd.Next(970, 991);
                        billet.CV2 = rnd.Next(290, 401);
                        billet.CV4 = rnd.Next(300, 401);
                        billet.WZ = rnd.Next(1150, 1181);
                        billet.SermeKafaSıcaklığı = rnd.Next(820, 861);
                        tempData = new Dictionary<string, object>
                {
                   { "ID", ID},

                    { "FÇS",billet.Temperature },
                    { "H8TEMP",billet.H8Temp },
                    { "FBLB", billet.FBLB},
                    { "WZ", billet.WZ },
                    { "SermeKafa",billet.SermeKafaSıcaklığı },
                    { "CV2", billet.CV2},
                    { "CV4",  billet.CV4 }
                };
                        break;
                    case 5:
                        billet.Temperature = rnd.Next(1000, 1201);
                        billet.H8Temp = rnd.Next(960, 971);
                        billet.FBLB = rnd.Next(980, 1001);
                        billet.CV2 = rnd.Next(310, 401);
                        billet.CV4 = rnd.Next(320, 401);
                        billet.WZ = rnd.Next(1160, 1181);
                        billet.SermeKafaSıcaklığı = rnd.Next(830, 881);
                        tempData = new Dictionary<string, object>
                {
                    { "ID", ID},

                    { "FÇS",billet.Temperature },
                    { "H8TEMP",billet.H8Temp },
                    { "FBLB", billet.FBLB},
                    { "WZ", billet.WZ },
                    { "SermeKafa",billet.SermeKafaSıcaklığı },
                    { "CV2", billet.CV2},
                    { "CV4",  billet.CV4 }
                };
                        break;
                    default:
                        break;
                }



                // Veri dictionary'leri
                var billetSpeedData = new Dictionary<string, object>
{
    {"ID", ID},
    {"RM",  Math.Round(billet.RMspeed, 2)},
    {"KH1", Math.Round(H1.billetSpeed, 2)},
    {"KH2", Math.Round(H2.billetSpeed, 2)},
    {"KH3", Math.Round(H3.billetSpeed, 2)},
    {"KH4", Math.Round(H4.billetSpeed, 2)},
    {"KH5", Math.Round(H5.billetSpeed, 2)},
    {"KH6", Math.Round(H6.billetSpeed, 2)},
    {"KH7", Math.Round(H7.billetSpeed, 2)},
    {"KH8", Math.Round(H8.billetSpeed, 2)},
    {"KH9", Math.Round(H9.billetSpeed, 2)},
    {"IM", Math.Round(H9.billetSpeed, 2)},
    {"KH10", Math.Round(H10.billetSpeed, 2)},
    {"KH11", Math.Round(H11.billetSpeed, 2)},
    {"KH12", Math.Round(H12.billetSpeed, 2)},
    {"KH13", Math.Round(H13.billetSpeed, 2)},
    {"IMH", Math.Round(H13.billetSpeed, 2)},
    {"KH14", Math.Round(H14.billetSpeed, 2)},
    {"KH15", Math.Round(H15.billetSpeed, 2) },
    {"FM", Math.Round(billet.FMspeed, 2)    }
};

                var standSpeedData = new Dictionary<string, object>
{
    { "ID", ID },
    { "H1", Math.Round(H1.Speed, 2) },
    { "H2", Math.Round(H2.Speed, 2) },
    { "H3", Math.Round(H3.Speed, 2) },
    { "H4", Math.Round(H4.Speed, 2) },
    { "H5", Math.Round(H5.Speed, 2)  },
    { "H6", Math.Round(H6.Speed, 2) },
    { "H7", Math.Round(H7.Speed, 2) },
    { "H8", Math.Round(H8.Speed, 2) },
    { "H9", Math.Round(H9.Speed, 2) },
    { "S9", Math.Round(s9, 2) },
    { "H10", Math.Round(H10.Speed, 2) },
    { "H11", Math.Round(H11.Speed, 2) },
    { "H12", Math.Round(H12.Speed, 2) },
    { "H13", Math.Round(H13.Speed, 2) },
    { "H14", Math.Round(H14.Speed, 2) },
    { "H15", Math.Round(H15.Speed, 2) },
    { "S15", Math.Round(s15, 2) },
    { "C15", Math.Round(c15, 2) },
    { "FBL", Math.Round(H1.FBLSpeed, 2) }
};





                var waterData = new Dictionary<string, object>
{
    {"ID", ID},
    {"WZ1", Math.Round(H1.WZ1,2)},
    {"WZ2", Math.Round(H1.WZ2, 2)},
    {"WZ3", Math.Round(H1.WZ3, 2)},
    {"WZ4", Math.Round(H1.WZ4, 2)},
    {"FAN", H1.Fan},
    {"KAPAK", H1.kapak},
    {"LOOPH14", Math.Round(H1.looph14, 2)},
    {"LOOPH15", Math.Round(H1.looph15, 2)},
    {"LOOPFBL", Math.Round(H1.loopFBL, 2)},
    {"SB", Math.Round(H1.SB, 2)},
    {"DS", Math.Round(H1.DESC, 2)}
};


                ;
                var loadData = new Dictionary<string, object>
        {
                     {"ID",ID },

            { "Y1", H1.Yuk },
            { "Y2", H2.Yuk },
            { "Y3", H3.Yuk },
            { "Y4", H4.Yuk },
            { "Y5", H5.Yuk },
            { "Y6", H6.Yuk },
            { "Y7", H7.Yuk },
            { "Y8", H8.Yuk },
            { "Y9", H9.Yuk },
            { "Y10", H10.Yuk },
            { "Y11", H11.Yuk },
            { "Y12", H12.Yuk },
            { "Y13", H13.Yuk },
            { "Y14", H14.Yuk },
            { "Y15", H15.Yuk },
            { "FBL", H1.loadfbl }
        };

                txtsermekafagosterım.Text = billet.SermeKafaSıcaklığı.ToString() + " " + "°C";
                txtFÇSgosterım.Text = billet.Temperature.ToString() + " " + "°C";
                txtKAPAK.Text = H1.kapak.ToString();
      
                // Verileri birleştirme

                //SendDataToServer(combinedData);
                AddDataToGridView(dataGridView_malzemehızı, billetSpeedData);
                AddDataToGridView(dataGridView_su, waterData);
                AddDataToGridView(dataGridView_yük, loadData);
                AddDataToGridView(dataGridView_standhızı, standSpeedData);
                AddDataToGridView(dataGridView_sıcaklık, tempData);


                LoadDataFromDataGridViewToTextBoxes(dataGridViewKutuk);
                LoadDataFromDataGridViewToTextBoxes(dataGridView_malzemehızı);
                LoadDataFromDataGridViewToTextBoxes(dataGridView_standhızı);
                LoadDataFromDataGridViewToTextBoxes(dataGridView_su);
                LoadDataFromDataGridViewToTextBoxes(dataGridView_yük);
                LoadDataFromDataGridViewToTextBoxes(dataGridView_sıcaklık);

                InsertData("TBLMALZEMEHIZI", billetSpeedData);
                InsertData("TBLSTAND", standSpeedData);
                InsertData("TBLSICAKLIK", tempData);
                InsertData("TBLSOGUTMA", waterData);
                InsertData("TBLYUK", loadData);

            }
            catch (Exception )
            {

            }

        }
        private void InsertData(string tableName, Dictionary<string, object> data)
        {
            using (SqlConnection connection = new SqlConnection(connectionSql))
            {
                connection.Open();

                // ID sütununu ekle
                data["ID"] = ID;

                string columns = string.Join(", ", data.Keys);
                string parameters = string.Join(", ", data.Keys.Select(key => "@" + key));

                string query = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach (var kvp in data)
                    {
                        command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                    command.ExecuteNonQuery();
                }


            }
        }
        private void LoadDataFromDataGridViewToTextBoxes(DataGridView dataGridView)
        {
            // Eğer DataGridView'de satır varsa, ilk satırdaki veriyi al
            if (dataGridView.Rows.Count > 0)
            {
                // İlk satırdaki verileri al
                var firstRow = dataGridView.Rows[0];

                // TextBox'lara veri yazma
                foreach (DataGridViewCell cell in firstRow.Cells)
                {
                    string columnName = dataGridView.Columns[cell.ColumnIndex].Name;

                    // TextBox ismini oluştur (örneğin: "txtFirstName" gibi)
                    string textBoxName = $"txt{columnName}";

                    // Formda bu isme sahip bir TextBox bul
                    Control[] controls = this.Controls.Find(textBoxName, true);

                    if (controls.Length > 0 && controls[0] is TextBox textBox)
                    {
                        // TextBox varsa, veriyi aktar
                        textBox.Text = cell.Value?.ToString() ?? string.Empty;
                    }
                    else
                    {
                        // Hata ayıklama için log veya hata mesajı
                        Console.WriteLine($"Kontrol bulunamadı: {textBoxName}");
                    }
                }
            }
            else
            {
                Console.WriteLine("DataGridView'de veri bulunamadı.");
            }
        }
        private void SendDataToServer(Dictionary<string, object> data)
        {
            string jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            // TCP Client bağlantısı
            using (var tcpClient = new TcpClient("127.0.0.1", 6000))
            {
                // SendBufferSize ve ReceiveBufferSize ayarlarını arttırma
                tcpClient.SendBufferSize = 65536;  // 64KB veri gönderim limiti
                tcpClient.ReceiveBufferSize = 65536;  // 64KB veri alma limiti

                using (NetworkStream stream = tcpClient.GetStream())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(jsonData);

                    // Büyük veri gönderimi
                    stream.Write(buffer, 0, buffer.Length);
                }
            }


        }
        private void KutukIzlemeVerileriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = true;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;

        }

        private async void Resetsystem()
        {
            if (progressBarStand1.Value == progressBarStand1.Maximum)
            {
                progressBarStand1.Value = 0;
                timer1.Stop();
                var stop = new Dictionary<string, object>
{
    { "message1", "stopProgressbar" },

};
                SendDataToServer(stop);
            }



            logger log = new logger(logFilePath);
            DateTime endtime = DateTime.Now;
            string time2= endtime.ToString("dd.MM.yyyy HH:mm:ss");
            log.WriteLog($"Bitiş zamanı: {time2}",  $"Üretim sona erdi. ID: {ID}");
       

            var cleartextbox =new Dictionary<string, object>
                {
    { "clear", "cleartextbox" },

};
            SendDataToServer(cleartextbox);
            await Task.Delay(1000);

            timer1.Start();

            DateTime starttime2 = DateTime.Now;
            string tim3 = starttime2.ToString("dd.MM.yyyy HH:mm:ss");
            ID = starttime2.ToString("yyMMddHHmmssff");
            log = new logger(logFilePath);
            log.WriteLog($"Başlama zamanı: {tim3}",  $"Üretim başladı. ID: {ID}");


            await Task.Run(() =>
            {
                Randomq(); // Yeni veri oluşturuluyor
            });


            ProcessMaterial();
            Hesaplama();

            Alldata();
            SendSelectedData();
        }

        private void STANDHIZIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = true;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;
        }

        private void ZAMANToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_izleme.Visible = true;
            dataGridView_standhızı.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;
        }
        private void YUKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_yük.Visible = true;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;
        }



        private void STANDHIZIToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = true;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;
        }

        private void BilletProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Sunucuya kapanma mesajı gönder
            var dataToSend = new Dictionary<string, object>
    {
        { "command", "finish" }
    };
            Console.WriteLine("Sunucuya kapanma mesajı gönderiliyor...");
            SendDataToServer(dataToSend);
            Console.WriteLine("Kapanma mesajı sunucuya gönderildi.");
            //frmyönlendirme.Close();
        }

        private void DataGridViewKutuk_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dataGridViewKutuk.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtKalite.Text = dataGridViewKutuk.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtAcıklama.Text = dataGridViewKutuk.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
       private void MALZEMEHIZIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_malzemehızı.Visible = true;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;
        }
        private void ProcessMaterial()
        {
            // Veriyi oluştur ve doldur
            Dictionary<string, object> billet = new Dictionary<string, object>
    {
        { "ID", ID }  // Her seferinde benzersiz bir ID oluşturur
    };

            switch (randomIndex)
            {
                case 0:
                    billet["Kalite"] = "5.2580_01";
                    billet["Acıklama"] = "CIVATALIK - SOMUNLUK";
                    break;
                case 1:
                    billet["Kalite"] = "1.1006_00";
                    billet["Acıklama"] = "İNCE TELLİK - HASIRLIK";
                    break;
                case 2:
                    billet["Kalite"] = "6.3006_00";
                    billet["Acıklama"] = "İNCE TELLİK - HASIRLIK";
                    break;
                case 3:
                    billet["Kalite"] = "6.1360_03";
                    billet["Acıklama"] = "SG2 - SG3";
                    break;
                case 4:
                    billet["Kalite"] = "1.1008_00";
                    billet["Acıklama"] = "İNCE TELLİK - HASIRLIK";
                    break;
                case 5:
                    billet["Kalite"] = "1.1070_00";
                    billet["Acıklama"] = "YÜKSEK KARBON - YAYLIK";
                    break;
            }

            AddBilletToDataGridView(billet);
            InsertData("TBLMALZEME", billet);  // Tablo adını buraya girin

        }
        private void AddBilletToDataGridView(Dictionary<string, object> billet)
        {

            if (dataGridViewKutuk.Columns.Count == 0)
            {
                foreach (var key in billet.Keys)
                {
                    dataGridViewKutuk.Columns.Add(key, key);
                }
            }
        

            DataGridViewRow row = new DataGridViewRow();
            foreach (var value in billet.Values)
            {
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = value });
            }

    dataGridViewKutuk.Rows.Add(row);
        }
private void Hesaplama()
        {
            CalculateClass calc = new CalculateClass();

            if (randomIndex == 0)//5.2580_01
            {

                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                H1.billetSpeed = calc.Calculate( billet.RMspeed,0.1,0.05);
                H2.billetSpeed = calc.Calculate( H1.billetSpeed,0.1,0.05);
                H3.billetSpeed = calc.Calculate( H2.billetSpeed, 0.1, 0.05);
                H4.billetSpeed = calc.Calculate(H3.billetSpeed, 0.1, 0.05);
                H5.billetSpeed = calc.Calculate(H4.billetSpeed, 0.1, 0.05);
                H6.billetSpeed = calc.Calculate(H5.billetSpeed, 0.1, 0.05);
                H7.billetSpeed = calc.Calculate(H6.billetSpeed, 0.1, 0.05);
                H8.billetSpeed = calc.Calculate(H7.billetSpeed, 0.1, 0.05);
                H9.billetSpeed = calc.Calculate(H8.billetSpeed, 0.1, 0.05);
                H10.billetSpeed = calc.Calculate(H9.billetSpeed,0.75,0.1);
                H11.billetSpeed = calc.Calculate(H10.billetSpeed, 0.75, 0.1);
                H12.billetSpeed = calc.Calculate(H11.billetSpeed, 0.75, 0.1);
                H13.billetSpeed = calc.Calculate(H12.billetSpeed, 0.75, 0.1);
                H14.billetSpeed = calc.Calculate( H13.billetSpeed,1.3,0.1);
                H15.billetSpeed = calc.Calculate(H14.billetSpeed, 1.3, 0.1);
                billet.FMspeed = calc.Calculate2( H15.billetSpeed,11,0.1);
                //yük yüzdesi
                H1.Yuk = rnd.Next(60, 65);
                H2.Yuk = rnd.Next(45, 50);
                H3.Yuk = H2.Yuk;
                H4.Yuk = rnd.Next(40, 45);
                H5.Yuk = H4.Yuk;
                H6.Yuk = rnd.Next(60, 65);
                H7.Yuk = H6.Yuk;
                H8.Yuk = rnd.Next(60, 65);
                H9.Yuk = rnd.Next(40, 45);
                H10.Yuk = rnd.Next(30, 35);
                H11.Yuk = rnd.Next(55, 60);
                H12.Yuk = rnd.Next(50, 55);
                H13.Yuk = rnd.Next(65, 70);
                H14.Yuk = rnd.Next(100, 110);
                H15.Yuk = rnd.Next(68, 75);
                // stand hız yüzdesi
                H1.Speed = calc.Calculate1(15, 5);
                H2.Speed = calc.Calculate1(25, 5);
                H3.Speed = H2.Speed;
                H4.Speed = calc.Calculate1(30, 5);
                H5.Speed = H4.Speed;
                H6.Speed = calc.Calculate1(25, 5);
                H7.Speed = H6.Speed;
                H8.Speed = calc.Calculate1(24, 1);
                H9.Speed = calc.Calculate1(24, 1);
                H10.Speed = calc.Calculate1(24, 1);
                H11.Speed = calc.Calculate1(28, 2);
                H12.Speed = calc.Calculate1(28, 2);
                H13.Speed = calc.Calculate1(30, 5);
                H14.Speed = calc.Calculate1(20, 5);
                H15.Speed = calc.Calculate1(30, 5);
                H1.FBLSpeed = calc.Calculate1(75, 5);
                // su ile sogutma metrekupleri 
                H1.WZ1 = 24 + (rnd.NextDouble() * 2);
                H1.WZ2 = 24 + (rnd.NextDouble() * 1);
                H1.WZ3 = 24 + (rnd.NextDouble() * 2);
                H1.WZ4 = 31 + (rnd.NextDouble() * 3);
                H1.Fan = "KAPALI";
                H1.kapak = "KAPALI";
                H1.looph14 = calc.Calculate1(450, 5);
                H1.looph15 = calc.Calculate1(220, 5);
                H1.loopFBL = calc.Calculate1(210, 5);
                H1.SB = calc.Calculate1(5, 5);
                H1.DESC = calc.Calculate1(5, 5);
                H1.loadfbl = rnd.Next(60, 80);
                H1.loadfbl = rnd.Next(60, 80);


            }
            if (randomIndex == 1)//1.1006_00
            {
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);

                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                H1.billetSpeed = calc.Calculate(billet.RMspeed, 0.1, 0.05);
                H2.billetSpeed = calc.Calculate(H1.billetSpeed, 0.1, 0.05);
                H3.billetSpeed = calc.Calculate(H2.billetSpeed, 0.1, 0.05);
                H4.billetSpeed = calc.Calculate(H3.billetSpeed, 0.1, 0.05);
                H5.billetSpeed = calc.Calculate(H4.billetSpeed, 0.1, 0.05);
                H6.billetSpeed = calc.Calculate(H5.billetSpeed, 0.1, 0.05);
                H7.billetSpeed = calc.Calculate(H6.billetSpeed, 0.1, 0.05);
                H8.billetSpeed = calc.Calculate(H7.billetSpeed, 0.1, 0.05);
                H9.billetSpeed = calc.Calculate(H8.billetSpeed, 0.1, 0.05);
                H10.billetSpeed = calc.Calculate(H9.billetSpeed, 0.75, 0.1);
                H11.billetSpeed = calc.Calculate(H10.billetSpeed, 0.75, 0.1);
                H12.billetSpeed = calc.Calculate(H11.billetSpeed, 0.75, 0.1);
                H13.billetSpeed = calc.Calculate(H12.billetSpeed, 0.75, 0.1);
                H14.billetSpeed = calc.Calculate(H13.billetSpeed, 1.3, 0.1);
                H15.billetSpeed = calc.Calculate(H14.billetSpeed, 1.3, 0.1);
                billet.FMspeed = calc.Calculate2(H15.billetSpeed, 11, 0.1);
                // yük yüzdesi;
                H1.Yuk = rnd.Next(60, 65);
                H2.Yuk = rnd.Next(45, 50);
                H3.Yuk = H2.Yuk;
                H4.Yuk = rnd.Next(40, 45);
                H5.Yuk = H4.Yuk;
                H6.Yuk = rnd.Next(60, 65);
                H7.Yuk = H6.Yuk;
                H8.Yuk = rnd.Next(60, 65);
                H9.Yuk = rnd.Next(40, 45);
                H10.Yuk = rnd.Next(30, 35);
                H11.Yuk = rnd.Next(55, 60);
                H12.Yuk = rnd.Next(50, 55);
                H13.Yuk = rnd.Next(65, 70);
                H14.Yuk = rnd.Next(100, 110);
                H15.Yuk = rnd.Next(68, 75);
                // stand hızı
                H1.Speed = calc.Calculate1(15, 5);
                H2.Speed = calc.Calculate1(25, 5);
                H3.Speed = H2.Speed;
                H4.Speed = calc.Calculate1(30, 5);
                H5.Speed = H4.Speed;
                H6.Speed = calc.Calculate1(25, 5);
                H7.Speed = H6.Speed;
                H8.Speed = calc.Calculate1(24, 1);
                H9.Speed = calc.Calculate1(24, 1);
                H10.Speed = calc.Calculate1(24, 1);
                H11.Speed = calc.Calculate1(28, 2);
                H12.Speed = calc.Calculate1(28, 2);
                H13.Speed = calc.Calculate1(30, 5);
                H14.Speed = calc.Calculate1(20, 5);
                H15.Speed = calc.Calculate1(30, 5);
                H1.FBLSpeed = calc.Calculate1(75, 5);
                H1.WZ1 = 19 + (rnd.NextDouble() * 2);
                H1.WZ2 = 20 + (rnd.NextDouble() * 2);
                H1.WZ3 = 24 + (rnd.NextDouble() * 2);
                H1.WZ4 = 34 + (rnd.NextDouble() * 4);
                H1.Fan = "%" + " " + rnd.Next(60, 65).ToString();
                H1.kapak = "10-12 AÇIK";
                H1.looph14 = calc.Calculate1(450, 5);
                H1.looph15 = calc.Calculate1(220, 5);
                H1.loopFBL = calc.Calculate1(210, 5);
                H1.SB = calc.Calculate1(5, 5);
                H1.DESC = calc.Calculate1(5, 5);
                H1.loadfbl = rnd.Next(60, 80);
                H1.loadfbl = rnd.Next(60, 80);

            }
            if (randomIndex == 2)//6.3006_00
            {


                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                H1.billetSpeed = calc.Calculate(billet.RMspeed, 0.1, 0.05);
                H2.billetSpeed = calc.Calculate(H1.billetSpeed, 0.1, 0.05);
                H3.billetSpeed = calc.Calculate(H2.billetSpeed, 0.1, 0.05);
                H4.billetSpeed = calc.Calculate(H3.billetSpeed, 0.1, 0.05);
                H5.billetSpeed = calc.Calculate(H4.billetSpeed, 0.1, 0.05);
                H6.billetSpeed = calc.Calculate(H5.billetSpeed, 0.1, 0.05);
                H7.billetSpeed = calc.Calculate(H6.billetSpeed, 0.1, 0.05);
                H8.billetSpeed = calc.Calculate(H7.billetSpeed, 0.1, 0.05);
                H9.billetSpeed = calc.Calculate(H8.billetSpeed, 0.1, 0.05);
                H10.billetSpeed = calc.Calculate(H9.billetSpeed, 0.75, 0.1);
                H11.billetSpeed = calc.Calculate(H10.billetSpeed, 0.75, 0.1);
                H12.billetSpeed = calc.Calculate(H11.billetSpeed, 0.75, 0.1);
                H13.billetSpeed = calc.Calculate(H12.billetSpeed, 0.75, 0.1);
                H14.billetSpeed = calc.Calculate(H13.billetSpeed, 1.3, 0.1);
                H15.billetSpeed = calc.Calculate(H14.billetSpeed, 1.3, 0.1);
                billet.FMspeed = calc.Calculate2(H15.billetSpeed, 11, 0.1);
                H1.Yuk = rnd.Next(60, 65);
                H2.Yuk = rnd.Next(45, 50);
                H3.Yuk = H2.Yuk;
                H4.Yuk = rnd.Next(40, 45);
                H5.Yuk = H4.Yuk;
                H6.Yuk = rnd.Next(60, 65);
                H7.Yuk = H6.Yuk;
                H8.Yuk = rnd.Next(60, 65);
                H9.Yuk = rnd.Next(40, 45);
                H10.Yuk = rnd.Next(30, 35);
                H11.Yuk = rnd.Next(55, 60);
                H12.Yuk = rnd.Next(50, 55);
                H13.Yuk = rnd.Next(65, 70);
                H14.Yuk = rnd.Next(100, 110);
                H15.Yuk = rnd.Next(68, 75);
                // stand hızı, Pı fonksiyonundaki formulle hesaplanır;
                H1.Speed = calc.Calculate1(15, 5);
                H2.Speed = calc.Calculate1(25, 5);
                H3.Speed = H2.Speed;
                H4.Speed = calc.Calculate1(30, 5);
                H5.Speed = H4.Speed;
                H6.Speed = calc.Calculate1(25, 5);
                H7.Speed = H6.Speed;
                H8.Speed = calc.Calculate1(24, 1);
                H9.Speed = calc.Calculate1(24, 1);
                H10.Speed = calc.Calculate1(24, 1);
                H11.Speed = calc.Calculate1(28, 2);
                H12.Speed = calc.Calculate1(28, 2);
                H13.Speed = calc.Calculate1(30, 5);
                H14.Speed = calc.Calculate1(20, 5);
                H15.Speed = calc.Calculate1(30, 5);
                H1.FBLSpeed = calc.Calculate1(75, 5);
                H1.WZ1 = 12 + (rnd.NextDouble() * 8);
                H1.WZ2 = 14 + (rnd.NextDouble() * 6);
                H1.WZ3 = 19 + (rnd.NextDouble() * 2);
                H1.WZ4 = 24 + (rnd.NextDouble() * 6);
                H1.Fan = "%" + " " + rnd.Next(60, 65).ToString();
                H1.kapak = "10-12 AÇIK";
                H1.looph14 = calc.Calculate1(450, 5);
                H1.looph15 = calc.Calculate1(220, 5);
                H1.loopFBL = calc.Calculate1(210, 5);
                H1.SB = calc.Calculate1(5, 5);
                H1.DESC = calc.Calculate1(5, 5);
                H1.loadfbl = rnd.Next(60, 80);
                H1.loadfbl = rnd.Next(60, 80);

            }
            if (randomIndex == 3)//6.1360_03
            {
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                H1.billetSpeed = calc.Calculate(billet.RMspeed, 0.1, 0.05);
                H2.billetSpeed = calc.Calculate(H1.billetSpeed, 0.1, 0.05);
                H3.billetSpeed = calc.Calculate(H2.billetSpeed, 0.1, 0.05);
                H4.billetSpeed = calc.Calculate(H3.billetSpeed, 0.1, 0.05);
                H5.billetSpeed = calc.Calculate(H4.billetSpeed, 0.1, 0.05);
                H6.billetSpeed = calc.Calculate(H5.billetSpeed, 0.1, 0.05);
                H7.billetSpeed = calc.Calculate(H6.billetSpeed, 0.1, 0.05);
                H8.billetSpeed = calc.Calculate(H7.billetSpeed, 0.1, 0.05);
                H9.billetSpeed = calc.Calculate(H8.billetSpeed, 0.1, 0.05);
                H10.billetSpeed = calc.Calculate(H9.billetSpeed, 0.75, 0.1);
                H11.billetSpeed = calc.Calculate(H10.billetSpeed, 0.75, 0.1);
                H12.billetSpeed = calc.Calculate(H11.billetSpeed, 0.75, 0.1);
                H13.billetSpeed = calc.Calculate(H12.billetSpeed, 0.75, 0.1);
                H14.billetSpeed = calc.Calculate(H13.billetSpeed, 1.3, 0.1);
                H15.billetSpeed = calc.Calculate(H14.billetSpeed, 1.3, 0.1);
                billet.FMspeed = calc.Calculate2(H15.billetSpeed, 11, 0.1);
                H1.Yuk = rnd.Next(60, 65);
                H2.Yuk = rnd.Next(45, 50);
                H3.Yuk = H2.Yuk;
                H4.Yuk = rnd.Next(40, 45);
                H5.Yuk = H4.Yuk;
                H6.Yuk = rnd.Next(60, 65);
                H7.Yuk = H6.Yuk;
                H8.Yuk = rnd.Next(60, 65);
                H9.Yuk = rnd.Next(40, 45);
                H10.Yuk = rnd.Next(30, 35);
                H11.Yuk = rnd.Next(55, 60);
                H12.Yuk = rnd.Next(50, 55);
                H13.Yuk = rnd.Next(65, 70);
                H14.Yuk = rnd.Next(100, 110);
                H15.Yuk = rnd.Next(68, 75);
                // stand hızı, Pı fonksiyonundaki formulle hesaplanır;
                H1.Speed = calc.Calculate1(15, 5);
                H2.Speed = calc.Calculate1(25, 5);
                H3.Speed = H2.Speed;
                H4.Speed = calc.Calculate1(30, 5);
                H5.Speed = H4.Speed;
                H6.Speed = calc.Calculate1(25, 5);
                H7.Speed = H6.Speed;
                H8.Speed = calc.Calculate1(24, 1);
                H9.Speed = calc.Calculate1(24, 1);
                H10.Speed = calc.Calculate1(24, 1);
                H11.Speed = calc.Calculate1(28, 2);
                H12.Speed = calc.Calculate1(28, 2);
                H13.Speed = calc.Calculate1(30, 5);
                H14.Speed = calc.Calculate1(20, 5);
                H15.Speed = calc.Calculate1(30, 5);
                H1.FBLSpeed = calc.Calculate1(75, 5);
                H1.WZ1 = 17 + (rnd.NextDouble() * 2);
                H1.WZ2 = 17 + (rnd.NextDouble() * 2);
                H1.WZ3 = 15 + (rnd.NextDouble() * 3);
                H1.WZ4 = 14 + (rnd.NextDouble() * 4);
                H1.Fan = "KAPALI";
                H1.kapak = "KAPALI";
                H1.looph14 = calc.Calculate1(450, 5);
                H1.looph15 = calc.Calculate1(220, 5);
                H1.loopFBL = calc.Calculate1(210, 5);
                H1.SB = calc.Calculate1(5, 5);
                H1.DESC = calc.Calculate1(5, 5);
                H1.loadfbl = rnd.Next(60, 80);
                H1.loadfbl = rnd.Next(60, 80);


            }
            if (randomIndex == 4)//1.1008_00
            {
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);


                //Her bir H'tan geçen malzemenin hızı ;
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                H1.billetSpeed = calc.Calculate(billet.RMspeed, 0.1, 0.05);
                H2.billetSpeed = calc.Calculate(H1.billetSpeed, 0.1, 0.05);
                H3.billetSpeed = calc.Calculate(H2.billetSpeed, 0.1, 0.05);
                H4.billetSpeed = calc.Calculate(H3.billetSpeed, 0.1, 0.05);
                H5.billetSpeed = calc.Calculate(H4.billetSpeed, 0.1, 0.05);
                H6.billetSpeed = calc.Calculate(H5.billetSpeed, 0.1, 0.05);
                H7.billetSpeed = calc.Calculate(H6.billetSpeed, 0.1, 0.05);
                H8.billetSpeed = calc.Calculate(H7.billetSpeed, 0.1, 0.05);
                H9.billetSpeed = calc.Calculate(H8.billetSpeed, 0.1, 0.05);
                H10.billetSpeed = calc.Calculate(H9.billetSpeed, 0.75, 0.1);
                H11.billetSpeed = calc.Calculate(H10.billetSpeed, 0.75, 0.1);
                H12.billetSpeed = calc.Calculate(H11.billetSpeed, 0.75, 0.1);
                H13.billetSpeed = calc.Calculate(H12.billetSpeed, 0.75, 0.1);
                H14.billetSpeed = calc.Calculate(H13.billetSpeed, 1.3, 0.1);
                H15.billetSpeed = calc.Calculate(H14.billetSpeed, 1.3, 0.1);
                billet.FMspeed = calc.Calculate2(H15.billetSpeed, 11, 0.1);
                H1.Yuk = rnd.Next(60, 65);
                H2.Yuk = rnd.Next(45, 50);
                H3.Yuk = H2.Yuk;
                H4.Yuk = rnd.Next(40, 45);
                H5.Yuk = H4.Yuk;
                H6.Yuk = rnd.Next(60, 65);
                H7.Yuk = H6.Yuk;
                H8.Yuk = rnd.Next(60, 65);
                H9.Yuk = rnd.Next(40, 45);
                H10.Yuk = rnd.Next(30, 35);
                H11.Yuk = rnd.Next(55, 60);
                H12.Yuk = rnd.Next(50, 55);
                H13.Yuk = rnd.Next(65, 70);
                H14.Yuk = rnd.Next(100, 110);
                H15.Yuk = rnd.Next(68, 75);

                // stand hızı, Pı fonksiyonundaki formulle hesaplanır;
                H1.Speed = calc.Calculate1(15, 5);
                H2.Speed = calc.Calculate1(25, 5);
                H3.Speed = H2.Speed;
                H4.Speed = calc.Calculate1(30, 5);
                H5.Speed = H4.Speed;
                H6.Speed = calc.Calculate1(25, 5);
                H7.Speed = H6.Speed;
                H8.Speed = calc.Calculate1(24, 1);
                H9.Speed = calc.Calculate1(24, 1);
                H10.Speed = calc.Calculate1(24, 1);
                H11.Speed = calc.Calculate1(28, 2);
                H12.Speed = calc.Calculate1(28, 2);
                H13.Speed = calc.Calculate1(30, 5);
                H14.Speed = calc.Calculate1(20, 5);
                H15.Speed = calc.Calculate1(30, 5);
                H1.FBLSpeed = calc.Calculate1(75, 5);
                H1.WZ1 = 12 + (rnd.NextDouble() * 2);
                H1.WZ2 = 12 + (rnd.NextDouble() * 2);
                H1.WZ3 = 9 + (rnd.NextDouble() * 2);
                H1.WZ4 = 31 + (rnd.NextDouble() * 3);
                H1.Fan = "%" + " " + rnd.Next(60, 65).ToString();
                H1.kapak = "10-12 AÇIK";
                H1.looph14 = calc.Calculate1(450, 5);
                H1.looph15 = calc.Calculate1(220, 5);
                H1.loopFBL = calc.Calculate1(210, 5);
                H1.SB = calc.Calculate1(5, 5);
                H1.DESC = calc.Calculate1(5, 5);
                H1.loadfbl = rnd.Next(60, 80);
                H1.loadfbl = rnd.Next(60, 80);

            }
            if (randomIndex == 5)//1.1070_00
            {


                //Her bir H'tan geçen malzemenin hızı ;
                billet.RMspeed = 0.1 + (rnd.NextDouble() * 0.1);
                //Her bir H'tan geçen malzemenin hızı ;
                H1.billetSpeed = calc.Calculate(billet.RMspeed, 0.1, 0.05);
                H2.billetSpeed = calc.Calculate(H1.billetSpeed, 0.1, 0.05);
                H3.billetSpeed = calc.Calculate(H2.billetSpeed, 0.1, 0.05);
                H4.billetSpeed = calc.Calculate(H3.billetSpeed, 0.1, 0.05);
                H5.billetSpeed = calc.Calculate(H4.billetSpeed, 0.1, 0.05);
                H6.billetSpeed = calc.Calculate(H5.billetSpeed, 0.1, 0.05);
                H7.billetSpeed = calc.Calculate(H6.billetSpeed, 0.1, 0.05);
                H8.billetSpeed = calc.Calculate(H7.billetSpeed, 0.1, 0.05);
                H9.billetSpeed = calc.Calculate(H8.billetSpeed, 0.1, 0.05);
                H10.billetSpeed = calc.Calculate(H9.billetSpeed, 0.75, 0.1);
                H11.billetSpeed = calc.Calculate(H10.billetSpeed, 0.75, 0.1);
                H12.billetSpeed = calc.Calculate(H11.billetSpeed, 0.75, 0.1);
                H13.billetSpeed = calc.Calculate(H12.billetSpeed, 0.75, 0.1);
                H14.billetSpeed = calc.Calculate(H13.billetSpeed, 1.3, 0.1);
                H15.billetSpeed = calc.Calculate(H14.billetSpeed, 1.3, 0.1);
                billet.FMspeed = calc.Calculate2(H15.billetSpeed, 11, 0.1);
                H1.Yuk = rnd.Next(60, 65);
                H2.Yuk = rnd.Next(45, 50);
                H3.Yuk = H2.Yuk;
                H4.Yuk = rnd.Next(40, 45);
                H5.Yuk = H4.Yuk;
                H6.Yuk = rnd.Next(60, 65);
                H7.Yuk = H6.Yuk;
                H8.Yuk = rnd.Next(60, 65);
                H9.Yuk = rnd.Next(40, 45);
                H10.Yuk = rnd.Next(30, 35);
                H11.Yuk = rnd.Next(55, 60);
                H12.Yuk = rnd.Next(50, 55);
                H13.Yuk = rnd.Next(65, 70);
                H14.Yuk = rnd.Next(100, 110);
                H15.Yuk = rnd.Next(68, 75);

                // stand hızı, Pı fonksiyonundaki formulle hesaplanır;
                H1.Speed = calc.Calculate1(15, 5);
                H2.Speed = calc.Calculate1(25, 5);
                H3.Speed = H2.Speed;
                H4.Speed = calc.Calculate1(30,5);
                H5.Speed = H4.Speed;
                H6.Speed = calc.Calculate1(25, 5);
                H7.Speed = H6.Speed;
                H8.Speed = calc.Calculate1(24, 1);
                H9.Speed = calc.Calculate1(24, 1);
                H10.Speed = calc.Calculate1(24, 1);
                H11.Speed = calc.Calculate1(28, 2);
                H12.Speed = calc.Calculate1(28, 2);
                H13.Speed = calc.Calculate1(30, 5);
                H14.Speed = calc.Calculate1(20, 5);
                H15.Speed = calc.Calculate1(30, 5);
                H1.FBLSpeed = calc.Calculate1(75, 5);
                H1.WZ1 = calc.Calculate1(19, 2);
                H1.WZ2 = calc.Calculate1(20, 2);
                H1.WZ3 = calc.Calculate1(19, 2);
                H1.WZ4 = calc.Calculate1(16, 3);
                H1.Fan = "%" + " " + rnd.Next(78, 80).ToString();
                H1.kapak = "AÇIK";
                H1.looph14 = calc.Calculate1(450, 5);
                H1.looph15 = calc.Calculate1(220, 5);
                H1.loopFBL = calc.Calculate1(210, 5);
                H1.SB = calc.Calculate1(5, 5);
                H1.DESC = calc.Calculate1(5, 5);
                H1.loadfbl = rnd.Next(60, 80);
                H1.loadfbl = rnd.Next(60, 80);
            }
        }





        //string sql1 = "INSERT INTO tblStandsure(ID,Stand1Giriş, Stand2Giriş, Stand3Giriş, Stand4Giriş, Stand5Giriş, Stand1Çıkış, Stand2Çıkış, Stand3Çıkış, Stand4Çıkış, Stand5Çıkış) " +
        //            "VALUES ('"+ID+ "','"+startTime1+"','" + startTime2.ToString("HH.mm.ss.fff"))+"')";

        //dbrw.Write()


        private void DataGridViewMALZEMEHIZI_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtRM.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtKH1.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtKH2.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtKH3.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtKH4.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[5].Value.ToString();
            txtKH5.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[6].Value.ToString();
            txtKH6.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[7].Value.ToString();
            txtKH7.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[8].Value.ToString();
            txtKH8.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[9].Value.ToString();
            txtKH9.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[10].Value.ToString();
            txtIM.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[11].Value.ToString();
            txtKH10.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[12].Value.ToString();
            txtKH11.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[13].Value.ToString();
            txtKH12.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[14].Value.ToString();
            txtKH13.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[15].Value.ToString();
            txtIMH.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[16].Value.ToString();
            txtKH14.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[17].Value.ToString();
            txtKH15.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[18].Value.ToString();
            txtFM.Text = dataGridView_malzemehızı.Rows[e.RowIndex].Cells[19].Value.ToString();
        }

        private void DataGridViewSıcaklık_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtFÇS.Text = dataGridView_sıcaklık.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtH8temp.Text = dataGridView_sıcaklık.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtFBLB.Text = dataGridView_sıcaklık.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtWZ.Text = dataGridView_sıcaklık.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtsermekafa.Text = dataGridView_sıcaklık.Rows[e.RowIndex].Cells[5].Value.ToString();
        }

        private void MalzemeStripMenuItem1_Click(object sender, EventArgs e)
        {

            dataGridViewKutuk.Visible = true;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = false;
        }

        private void SUWZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_yük.Visible = false;
            dataGridView_sıcaklık.Visible = false;
            dataGridView_su.Visible = true;
        }

        private void DataGridViewWATER_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtWZ1.Text = dataGridView_su.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtWZ2.Text = dataGridView_su.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtWZ3.Text = dataGridView_su.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtWZ4.Text = dataGridView_su.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void DataGridViewYuk_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtY1.Text = dataGridView_yük.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtY2.Text = dataGridView_yük.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtY4.Text = dataGridView_yük.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtY6.Text = dataGridView_yük.Rows[e.RowIndex].Cells[6].Value.ToString();
            txtY8.Text = dataGridView_yük.Rows[e.RowIndex].Cells[8].Value.ToString();
            txtY9.Text = dataGridView_yük.Rows[e.RowIndex].Cells[9].Value.ToString();
            txtY10.Text = dataGridView_yük.Rows[e.RowIndex].Cells[10].Value.ToString();
            txtY11.Text = dataGridView_yük.Rows[e.RowIndex].Cells[11].Value.ToString();
            txtY12.Text = dataGridView_yük.Rows[e.RowIndex].Cells[12].Value.ToString();
            txtY13.Text = dataGridView_yük.Rows[e.RowIndex].Cells[13].Value.ToString();
            txtY14.Text = dataGridView_yük.Rows[e.RowIndex].Cells[14].Value.ToString();
            txtY15.Text = dataGridView_yük.Rows[e.RowIndex].Cells[15].Value.ToString();

        }

        private void SICAKLIKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewKutuk.Visible = false;
            dataGridView_standhızı.Visible = false;
            dataGridView_izleme.Visible = false;
            dataGridView_malzemehızı.Visible = false;
            dataGridView_sıcaklık.Visible = true;
            dataGridView_yük.Visible = false;
            dataGridView_su.Visible = false;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {


            var data = new Dictionary<string, object>
{
    { "message", "startProgressBar" },

};
           
            if (progressBarStand1.Value < progressBarStand1.Maximum || progressBarStand1.Value == 0)
            {
                SendDataToServer(data);
                progressBarStand1.Value += 1;

            }
        }
        private async void SendSelectedData()
        {


            var fcsData = new Dictionary<string, object>
{
    { "FÇS",billet.Temperature},
    };
            var descdata = new Dictionary<string, object>
{
      { "DS", H1.DESC }
    };
            var SBdata = new Dictionary<string, object>
{
       { "calculateSB", H1.SB },

    };
            var RMdata = new Dictionary<string, object>
{
       { "RM", billet.RMspeed },

    };

            // Belirli veriler gönderiliyor
            var Stand1Data = new Dictionary<string, object>
{
    { "KH1", H1.billetSpeed },
    { "H1", H1.Speed },
    { "Y1", H1.Yuk },

};
            var Stand2Data = new Dictionary<string, object>
{

    { "KH2", H2.billetSpeed },
    { "H2", H2.Speed },
    { "Y2", H2.Yuk },

};
            var Stand3Data = new Dictionary<string, object>
{
          { "KH3", H3.billetSpeed },
           { "H3", H2.Speed },
           { "Y3", H2.Yuk },
};
            var Stand4Data = new Dictionary<string, object>
{
    { "KH4", H4.billetSpeed },
    { "H4", H4.Speed },
    { "Y4", H4.Yuk },

};
            var Stand5Data = new Dictionary<string, object>
{
    { "KH5", H5.billetSpeed },
    { "H5", H5.Speed },
    { "Y5", H5.Yuk },
};

            var Stand6Data = new Dictionary<string, object>
{
    { "KH6", H6.billetSpeed },
    { "H6", H6.Speed },
    { "Y6", H6.Yuk },

};
            var Stand7Data = new Dictionary<string, object>
{
   { "KH7", H7.billetSpeed },
   { "H7", H7.Speed },
    { "Y7", H7.Yuk },
};

            var Stand8Data = new Dictionary<string, object>
{
    { "KH8", H8.billetSpeed },

    { "H8", H8.Speed },
   { "Y8", H8.Yuk },
};
            var h8tempData = new Dictionary<string, object>
    {
    { "H8TEMP", billet.H8Temp },
    };
            var Stand9Data = new Dictionary<string, object>
{
       { "KH9", H9.billetSpeed },
       { "IM", H9.billetSpeed },
       { "H9", H9.Speed },
       { "Y9", H9.Yuk },


};
            var Asenkrn9 = new Dictionary<string, object>
    {
       { "S9", s9},
       };
            var Stand10Data = new Dictionary<string, object>
{
    { "KH10", H10.billetSpeed },
    { "H10", H10.Speed },
    { "Y10", H10.Yuk },
};
            var Stand11Data = new Dictionary<string, object>
{
    { "KH11", H11.billetSpeed },
    { "H11", H11.Speed },
    { "Y11", H11.Yuk },
};
            var Stand12Data = new Dictionary<string, object>
{
    { "KH12", H12.billetSpeed },
    { "H12", H12.Speed },
    { "Y12", H12.Yuk },
};
            var Stand13Data = new Dictionary<string, object>
{
    { "KH13", H13.billetSpeed },
     { "IMH", H13.billetSpeed },
    { "H13", H13.Speed },
    { "Y13", H13.Yuk },
};
            var Stand14Data = new Dictionary<string, object>
{
    { "KH14", H14.billetSpeed },
     { "LOOPH14",H1.looph14},
    { "H14", H14.Speed },
    { "Y14", H14.Yuk },
};

            var S15a = new Dictionary<string, object>
{

      { "S15",s15 },


};
            var c15a = new Dictionary<string, object>
{

      { "C15",c15 },


};
            var Stand15Data = new Dictionary<string, object>
{
    { "KH15", H15.billetSpeed },

     { "LOOPH15", H1.looph15 },
    { "H15", H15.Speed },
    { "Y15", H15.Yuk },


};
            var FBLData = new Dictionary<string, object>
{
    { "FBLB", billet.FBLB },
     { "LOOPFBL",H1.loopFBL},

}; var wzdata = new Dictionary<string, object>
{
    { "WZ", billet.WZ},


};
            var fandata = new Dictionary<string, object>
{

      { "FAN",H1.Fan},

};
            var wz1data = new Dictionary<string, object>
{
    { "WZ1", H1.WZ1},
};
            var wz2data = new Dictionary<string, object>
{
    { "WZ2", H1.WZ2},
};
            var wz3data = new Dictionary<string, object>
{
    { "WZ3", H1.WZ3},
};
            var wz4data = new Dictionary<string, object>
{
    { "WZ4",H1.WZ4},
};
            var kapak = new Dictionary<string, object>
{
    { "KAPAK",H1.kapak},
};
            var finalspeed = new Dictionary<string, object>
{
    { "FM",billet.FMspeed},
};
            var SermeKafa = new Dictionary<string, object>
{
   { "SermeKafa",billet.SermeKafaSıcaklığı},
};
            var cv2 = new Dictionary<string, object>
{
   { "CV2",billet.CV2},
};
            var cv4 = new Dictionary<string, object>
{
    { "CV4",billet.CV4},
};
            await Task.Delay(4000);
            SendDataToServer(descdata);

            await Task.Delay(1000);
            SendDataToServer(RMdata);

            await Task.Delay(1000);
            SendDataToServer(fcsData);

            await Task.Delay(4000);
            SendDataToServer(Stand1Data);

            await Task.Delay(8000);
            SendDataToServer(Stand2Data);

            await Task.Delay(11000);
            SendDataToServer(Stand3Data);

            await Task.Delay(8000);
            SendDataToServer(Stand4Data);

            await Task.Delay(11000);
            SendDataToServer(Stand5Data);

            await Task.Delay(8000);
            SendDataToServer(Stand6Data);

            await Task.Delay(11000);
            SendDataToServer(Stand7Data);

            await Task.Delay(7000);
            SendDataToServer(Stand8Data);

            await Task.Delay(5000);
            SendDataToServer(h8tempData);
            await Task.Delay(4000);
            SendDataToServer(Stand9Data);
            await Task.Delay(6000);
            SendDataToServer(Asenkrn9);
            await Task.Delay(9000);
            SendDataToServer(Stand10Data);
            await Task.Delay(9000);
            SendDataToServer(Stand11Data);
            await Task.Delay(8000);
            SendDataToServer(Stand12Data);
            await Task.Delay(9000);
            SendDataToServer(Stand13Data);
            await Task.Delay(8000);
            SendDataToServer(Stand14Data);
            await Task.Delay(9000);
            SendDataToServer(Stand15Data);

            await Task.Delay(11000);
            SendDataToServer(S15a);


            await Task.Delay(3000);
            SendDataToServer(c15a);

            await Task.Delay(3000);

            SendDataToServer(FBLData);
            await Task.Delay(3000);

            SendDataToServer(fandata);

            await Task.Delay(3000);

            SendDataToServer(wzdata);

            await Task.Delay(3000);
            SendDataToServer(wz1data);

            await Task.Delay(2000);
            SendDataToServer(wz2data);
            await Task.Delay(2000);
            SendDataToServer(wz3data);
            await Task.Delay(2000);
            SendDataToServer(wz4data);
            await Task.Delay(2000);
            SendDataToServer(kapak);
            await Task.Delay(2000);
            SendDataToServer(finalspeed);
            await Task.Delay(2000);
            SendDataToServer(SermeKafa);
            await Task.Delay(2000);
            SendDataToServer(cv2);
            await Task.Delay(2000);
            SendDataToServer(cv4);
            Resetsystem();
        }
        private void Randomq()
        {
            randomIndex = rnd.Next(0, qualities.Length);
        }
    }
}






