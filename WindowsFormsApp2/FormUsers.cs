using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp2
{
    public partial class FormUsers : Form
    {
        public FormUsers()
        {
            InitializeComponent();
        }

        private void FormUsers_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Пользователи". При необходимости она может быть перемещена или удалена.
            this.пользователиTableAdapter.Fill(this.labDataSet.Пользователи);

            cmb_filter.DisplayMember = "Text";
            cmb_filter.ValueMember = "Value";

            cmb_filter.Items.Add(new { Text = "Фамилия", Value = "LastName" });
            cmb_filter.Items.Add(new { Text = "Имя", Value = "FirstName" });
            cmb_filter.Items.Add(new { Text = "Email", Value = "Email" });
            cmb_filter.Items.Add(new { Text = "Номер телефона", Value = "Phone" });

            cmb_filter.SelectedIndex = 0;
        }

        private void UpdateUsers()
        {
            this.пользователиTableAdapter.Fill(this.labDataSet.Пользователи);
        }

        private void ClearInputFields()
        {
            txt_lastname.Clear();
            txt_firstname.Clear();
            txt_email.Clear();
            txt_phone.Clear();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txt_lastname.Text))
            {
                MessageBox.Show("Введите фамилию");
                txt_lastname.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_firstname.Text))
            {
                MessageBox.Show("Введите имя");
                txt_firstname.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_email.Text))
            {
                MessageBox.Show("Введите Email");
                txt_email.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_phone.Text))
            {
                MessageBox.Show("Введите номер телефона");
                txt_phone.Focus();
                return false;
            }

            return true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            this.queriesTableAdapter1.AddUser(this.txt_lastname.Text, this.txt_firstname.Text,txt_email.Text,this.txt_phone.Text);
            this.пользователиTableAdapter.Fill(this.labDataSet.Пользователи);

            MessageBox.Show("Пользователь успешно добавлен");
            UpdateUsers();
            ClearInputFields();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btn_add_Click(sender, e);
        }

        private void txt_phone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_upd_Click(object sender, EventArgs e)
        {
            if (пользователиBindingSource.Current == null)
            {
                MessageBox.Show("Не выбран пользователь");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.пользователиBindingSource.Current;
            int p = Convert.ToInt32(j["UserID"]);

            if (!ValidateInput())
                return;

            this.queriesTableAdapter1.UpdateUser(
                p,
                this.txt_lastname.Text,
                this.txt_firstname.Text,
                this.txt_email.Text,
                this.txt_phone.Text
                );

            this.пользователиTableAdapter.Fill(this.labDataSet.Пользователи);

            MessageBox.Show("Пользователь успешно обновлен");

            UpdateUsers();
            ClearInputFields();
        }
        private void btn_del_Click(object sender, EventArgs e)
        {
            if (пользователиBindingSource.Current == null)
            {
                MessageBox.Show("Не выбран пользователь");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.пользователиBindingSource.Current;
            int p = Convert.ToInt32(j["UserID"]);

            this.queriesTableAdapter1.DeleteUser(p);
            this.пользователиTableAdapter.Fill(this.labDataSet.Пользователи);

            MessageBox.Show("Пользователь успешно удален");

            UpdateUsers();
            ClearInputFields();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btn_del_Click(sender, e);
        }
        private void btn_apply_Click(object sender, EventArgs e)
        {
            if (cmb_filter.SelectedItem == null)
            {
                MessageBox.Show("Выберите поле для фильтрации");
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_filter.Text))
            {
                MessageBox.Show("Введите значение для поиска");
                return;
            }

            string column = (string)cmb_filter.SelectedItem // получение имени столбца
                .GetType()
                .GetProperty("Value")
                .GetValue(cmb_filter.SelectedItem, null);

            string filter = txt_filter.Text;
            пользователиBindingSource.Filter = $"{column} LIKE '%{filter}%'";  // Like - строковый. // % значение содержит

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            пользователиBindingSource.RemoveFilter();
            txt_filter.Clear();
            cmb_filter.SelectedIndex = 0;
        }

        
    }
}
