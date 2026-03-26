using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 product = new Form1();
            product.ShowDialog();
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormCategory category = new FormCategory();
            category.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormUsers users = new FormUsers();
            users.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormOrder order = new FormOrder();
            order.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormPayment payment = new FormPayment();
            payment.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormDelivery delivery = new FormDelivery();
            delivery.ShowDialog();
        }
    }
}
