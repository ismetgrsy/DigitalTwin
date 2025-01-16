using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BilletTracking
{
    public partial class frmyönlendirme : Form
    {
        public frmyönlendirme()
        {
            InitializeComponent();
        }

        private void btnkullanıcı_Click(object sender, EventArgs e)
        {
            KullanıcıFormu kullanıcıFormu = new KullanıcıFormu();
            kullanıcıFormu.Show(); // Formu göster
        }

        private void btnistatistik_Click(object sender, EventArgs e)
        {
            istatistikler istatistikler = new istatistikler();
            istatistikler.Show();   
        }
    }
}
