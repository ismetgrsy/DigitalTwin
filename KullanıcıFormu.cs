using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using BilletApp;

namespace BilletTracking
{
    public partial class KullanıcıFormu : Form
    {
        SqlConnection connectionSQL = new SqlConnection("Data Source=localhost;Initial Catalog=kangal_had;Integrated Security=True");
        
      

        public KullanıcıFormu()
        {
            InitializeComponent();
        }

        private void btngiriş_Click(object sender, EventArgs e)
        {
            try
            {
                connectionSQL.Open();

                string Sqladmin = "SELECT * FROM TBLUSERKANGAL WHERE KullanıcıAdı=@KullanıcıAdı AND Şifre=@Şifre";
                using (SqlCommand cmd = new SqlCommand(Sqladmin, connectionSQL))
                {
                    // Kullanıcı adı ve şifreyi parametre olarak ekliyoruz
                    cmd.Parameters.AddWithValue("@KullanıcıAdı", txtkullanıcı.Text);
                    cmd.Parameters.AddWithValue("@Şifre", txtşifre.Text);

                    // Veritabanı sorgusunu çalıştırıyoruz
                    using (SqlDataReader dr1 = cmd.ExecuteReader())
                    {
                        if (dr1.Read())
                        {
                            if (txtkullanıcı.Text == "ismetgursoy" && txtşifre.Text == "1905")
                            {
                                MessageBox.Show("Hoşgeldiniz , Mühendis girişi yapıldı.", "GİRİŞ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                BilletForm blform = new BilletForm();
                                blform.Show();
                            }
                            else
                            {
                                MessageBox.Show("Hoşgeldiniz , Operator girişi yapıldı.", "GİRİŞ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                BilletForm bl = new BilletForm();
                                bl.Show();
                                bl.Hide();
                                this.Hide();
                            }

                            
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            finally
            {
                if (connectionSQL.State == System.Data.ConnectionState.Open)
                {
                    connectionSQL.Close();
                }
            }
        }



        private void txtşifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
              
                btngiriş.PerformClick();
            }
        }

        private void btngizle_Click(object sender, EventArgs e)
        {
            {
                {
                    if (txtşifre.PasswordChar == '*')
                    {
                        txtşifre.PasswordChar = '\0'; // Şifreyi göster

                    }
                    else
                    {
                        txtşifre.PasswordChar = '*'; // Şifreyi gizle

                    }
                }
            }
        }
    }
}
