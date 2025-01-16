using BilletApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BilletTracking
{
    public partial class istatistikler : Form
    {
        
        string connectionSQL = ("Data Source=localhost;Initial Catalog=kangal_had;Integrated Security=True");
        
        public istatistikler()
        {
            this.Size = new System.Drawing.Size(2100, 1300);
            InitializeComponent();
        }




        private DataTable LoadTemperatureData()
        {
          ; // Veritabanı bağlantı dizesi
            string query = "SELECT FÇS FROM TBLSICAKLIK"; // SQL sorgusu
         
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSQL))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
        private void LoadDataToChart()
        {
            DataTable temperatureData = LoadTemperatureData();

            // Chart alanını temizle
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // Yeni ChartArea oluştur
            ChartArea chartArea = new ChartArea();
            chart1.ChartAreas.Add(chartArea);

            chartArea.AxisY.Minimum = 800;
        chartArea.AxisY.Maximum = 1200;

         chartArea.AxisY.Interval = 50; // Y ekseni aralıkları
            // Yeni Series (Seri) oluştur ve türünü çizgi olarak ayarla
            Series series = new Series
            {
                
                Color = System.Drawing.Color.Red,
                ChartType = SeriesChartType.Line,
                YValueMembers = "FÇS" // Y ekseni için
            };

            chart1.Series.Add(series);
            chart1.Series["Series1"].Name = ""; // Seri adını boş bırakır


            // Verileri bağla
            chart1.DataSource = temperatureData;
            chart1.DataBind();
        }

        private void istatistikler_Load(object sender, EventArgs e)
        {
            // Formun tam ekran olarak açılmasını sağla
            this.WindowState = FormWindowState.Maximized;

            // Verileri yükle
            LoadDataToChart();
            LoadSKSToChart();
            LoadCV4ToChart();
            LoadRMToChart();
            LoadIMHToChart();
            LoadFMToChart();

            // Grafikleri güncelle
            chart1.Invalidate();
            chart2.Invalidate();
            chart3.Invalidate();
            chart6.Invalidate();
            chart5.Invalidate();
            chart4.Invalidate();

        }

        private void LoadSKSToChart()
        {
            DataTable sermekafa= sksdata();

            // Chart alanını temizle
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();

            // Yeni ChartArea oluştur
            ChartArea chartArea = new ChartArea();
            chart2.ChartAreas.Add(chartArea);

            chartArea.AxisY.Minimum = 700;
            chartArea.AxisY.Maximum = 900;

            chartArea.AxisY.Interval = 30
                ; // Y ekseni aralıkları
            // Yeni Series (Seri) oluştur ve türünü çizgi olarak ayarla
            Series series = new Series
            {
            
                Color = System.Drawing.Color.Orange,
                ChartType = SeriesChartType.Line,
                YValueMembers = "SermeKafa" // Y ekseni için
            };

            chart2.Series.Add(series);
            chart2.Series["Series1"].Name = ""; // Seri adını boş bırakır

            // Verileri bağla
            chart2.DataSource = sermekafa;
            chart2.DataBind();
        }
        private DataTable sksdata()
        {
             // Veritabanı bağlantı dizesi
            string query1 = "SELECT SermeKafa FROM TBLSICAKLIK"; // SQL sorgusu

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSQL))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query1, connection);
                dataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
        private DataTable cv4data()
        {
            string query2 = "SELECT CV4 FROM TBLSICAKLIK"; // SQL sorgusu

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSQL))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query2, connection);
                dataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
        private void LoadCV4ToChart()
        {
            DataTable CV4= cv4data();

            // Chart alanını temizle
            chart3.Series.Clear();
            chart3.ChartAreas.Clear();

            // Yeni ChartArea oluştur
            ChartArea chartArea = new ChartArea();
            chart3.ChartAreas.Add(chartArea);

            chartArea.AxisY.Minimum =200;
            chartArea.AxisY.Maximum = 500;

            chartArea.AxisY.Interval = 30
                ; // Y ekseni aralıkları
            // Yeni Series (Seri) oluştur ve türünü çizgi olarak ayarla
            Series series = new Series
            {
        
                Color = System.Drawing.Color.Brown,
                ChartType = SeriesChartType.Line,
                YValueMembers = "CV4" // Y ekseni için
            };

            chart3.Series.Add(series);
            chart3.Series["Series1"].Name = ""; // Seri adını boş bırakır

            // Verileri bağla
            chart3.DataSource = CV4;
            chart3.DataBind();
        }
        private void LoadRMToChart()
        {
            DataTable RM = RMdata();

            // Chart alanını temizle
            chart6.Series.Clear();
            chart6.ChartAreas.Clear();

            // Yeni ChartArea oluştur
            ChartArea chartArea = new ChartArea();
            chart6.ChartAreas.Add(chartArea);

            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = 0.2;

            chartArea.AxisY.Interval = 0.05

                ; // Y ekseni aralıkları
            // Yeni Series (Seri) oluştur ve türünü çizgi olarak ayarla
            Series series = new Series
            {
         
                Color = System.Drawing.Color.Green,
                ChartType = SeriesChartType.Line,
                YValueMembers = "RM" // Y ekseni için
            };

            chart6.Series.Add(series);
            chart6.Series["Series1"].Name = ""; // Seri adını boş bırakır

            // Verileri bağla
            chart6.DataSource = RM;
            chart6.DataBind();
        }
        private DataTable RMdata()
        {
            string query4 = "SELECT RM FROM TBLMALZEMEHIZI"; // SQL sorgusu

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSQL))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query4, connection);
                dataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
        private DataTable IMHdata()
        {
            string query5 = "SELECT IMH FROM TBLMALZEMEHIZI"; // SQL sorgusu

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSQL))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query5, connection);
                dataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
        private void LoadIMHToChart()
        {
            DataTable IMHspeed = IMHdata();

            chart5.Series.Clear();
            chart5.ChartAreas.Clear();

            ChartArea chartArea = new ChartArea();
            chart5.ChartAreas.Add(chartArea);
            chartArea.AxisY.Minimum = 3;
            chartArea.AxisY.Maximum = 5;
            chartArea.AxisY.Interval = 0.3;

            Series series = new Series
            {

                Color = System.Drawing.Color.Blue,
                ChartType = SeriesChartType.Line,
                YValueMembers = "IMH"
            };

            chart5.Series.Add(series);
            chart5.Series["Series1"].Name = ""; // Seri adını boş bırakır
            chart5.DataSource = IMHspeed;
            chart5.DataBind();
        }
        private void LoadFMToChart()
        {
            DataTable FMspeed = FMdata();

            chart4.Series.Clear();
            chart4.ChartAreas.Clear();

            ChartArea chartArea = new ChartArea();
            chart4.ChartAreas.Add(chartArea);
            chartArea.AxisY.Minimum = 30;
            chartArea.AxisY.Maximum = 90;
            chartArea.AxisY.Interval = 5;

            Series series = new Series
            {
               
                Color = System.Drawing.Color.Purple,
                ChartType = SeriesChartType.Line,
                YValueMembers = "FM"
            };

            chart4.Series.Add(series);
            chart4.Series["Series1"].Name = ""; // Seri adını boş bırakır
            chart4.DataSource = FMspeed;
            chart4.DataBind();
        }
        private DataTable FMdata()
        {
            string query6 = "SELECT FM FROM TBLMALZEMEHIZI"; // SQL sorgusu

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSQL))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query6, connection);
                dataAdapter.Fill(dataTable);
            }

            return dataTable;
        }




    }
}
