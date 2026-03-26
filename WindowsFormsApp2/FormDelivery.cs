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
    public partial class FormDelivery : Form
    {
        public FormDelivery()
        {
            InitializeComponent();
        }

        private void FormDelivery_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Заказы". При необходимости она может быть перемещена или удалена.
            this.заказыTableAdapter.Fill(this.labDataSet.Заказы);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Доставки". При необходимости она может быть перемещена или удалена.
            this.доставкиTableAdapter.Fill(this.labDataSet.Доставки);

            cmb_filter.DisplayMember = "Text";
            cmb_filter.ValueMember = "Value";

            cmb_filter.Items.Add(new { Text = "Номер заказа", Value = "OrderID" });
            cmb_filter.Items.Add(new { Text = "Адрес доставки", Value = "DeliveryAddress" });
            cmb_filter.Items.Add(new { Text = "Дата доставки", Value = "DeliveryDate" });
            cmb_filter.Items.Add(new { Text = "Статус доставки", Value = "DeliveryStatus" });

            cmb_filter.SelectedIndex = 0;
        }

        private void UpdateDelivery()
        {
            this.доставкиTableAdapter.Fill(this.labDataSet.Доставки);
        }

        private void ClearInputFields()
        {
            cmb_order.SelectedIndex = -1;
            txt_address.Clear();
            cmb_status.SelectedIndex = -1;
        }

        private bool ValidateInput()
        {
            if (cmb_order.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите номер заказа");
                cmb_order.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("Введите адрес доставки");
                txt_address.Focus();
                return false;
            }

            if (cmb_status.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите статус доставки");
                cmb_status.Focus();
                return false;
            }

            return true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            DateTime? deliveryDate = null;

            if (dateTimePicker1.Checked)
                deliveryDate = dateTimePicker1.Value;

            this.queriesTableAdapter1.AddDelivery((int)cmb_order.SelectedValue, this.txt_address.Text, deliveryDate, cmb_status.Text);
            this.доставкиTableAdapter.Fill(this.labDataSet.Доставки);

            MessageBox.Show("Доставка успешно добавлена");
            UpdateDelivery();
            ClearInputFields();
        }
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btn_add_Click(sender, e);
        }

        private void btn_upd_Click(object sender, EventArgs e)
        {
            if (доставкиBindingSource.Current == null)
            {
                MessageBox.Show("Не выбрана доставка");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.доставкиBindingSource.Current;
            int p = Convert.ToInt32(j["DeliveryID"]);

            if (!ValidateInput())
                return;

            DateTime? deliveryDate = null;
            if (dateTimePicker1.Checked)
                deliveryDate = dateTimePicker1.Value;

            this.queriesTableAdapter1.UpdateDelivery(
                p,
                Convert.ToInt32(cmb_order.SelectedValue),
                this.txt_address.Text,
                deliveryDate,
                cmb_status.Text
                );

            this.доставкиTableAdapter.Fill(this.labDataSet.Доставки);

            MessageBox.Show("Доставка успешно обновлена");

            UpdateDelivery();
            ClearInputFields();
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (доставкиBindingSource.Current == null)
            {
                MessageBox.Show("Не выбрана доставка");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.доставкиBindingSource.Current;
            int p = Convert.ToInt32(j["DeliveryID"]);

            this.queriesTableAdapter1.DeleteDelivery(p);
            this.доставкиTableAdapter.Fill(this.labDataSet.Доставки);

            MessageBox.Show("Доставка успешно удалена");

            UpdateDelivery();
            ClearInputFields();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btn_del_Click(sender, e);
        }

        private void btn_app_Click(object sender, EventArgs e)
        {
            if (cmb_filter.SelectedItem == null)
            {
                MessageBox.Show("Выберите поле для фильтрации");
                return;
            }

            string column = (string)cmb_filter.SelectedItem // получение имени столбца
                .GetType()
                .GetProperty("Value")
                .GetValue(cmb_filter.SelectedItem, null);

            string filterValue = txt_filter.Text;

            // фильтр
            if (column == "OrderID") 
            {
                if (string.IsNullOrWhiteSpace(txt_filter.Text))
                {
                    MessageBox.Show("Введите значение для поиска");
                    return;
                }

                доставкиBindingSource.Filter = $"{column} = {filterValue}"; // = числовой
            }
            else if (column == "DeliveryDate")
            {
                string dateValue = dtp_filterDate.Value.ToString("yyyy-MM-dd");
                доставкиBindingSource.Filter = $"{column} = #{dateValue}#";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txt_filter.Text))
                {
                    MessageBox.Show("Введите значение для поиска");
                    return;
                }

                доставкиBindingSource.Filter = $"{column} LIKE '%{filterValue}%'"; // Like - строковый. // % значение содержит
            }
        }

        private void btn_can_Click(object sender, EventArgs e)
        {
            доставкиBindingSource.RemoveFilter();
            txt_filter.Clear();
            cmb_filter.SelectedIndex = 0;
        }
    }
}
