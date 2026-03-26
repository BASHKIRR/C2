using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class FormOrder : Form
    {
        public FormOrder()
        {
            InitializeComponent();
        }

        private void FormOrder_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Товары". При необходимости она может быть перемещена или удалена.
            this.товарыTableAdapter.Fill(this.labDataSet.Товары);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Пользователи". При необходимости она может быть перемещена или удалена.
            this.пользователиTableAdapter.Fill(this.labDataSet.Пользователи);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Заказы". При необходимости она может быть перемещена или удалена.
            this.заказыTableAdapter.Fill(this.labDataSet.Заказы);

            cmb_filter.DisplayMember = "Text";
            cmb_filter.ValueMember = "Value";

            cmb_filter.Items.Add(new { Text = "Номер заказа", Value = "OrderID" });
            cmb_filter.Items.Add(new { Text = "Дата оформления заказа", Value = "OrderDate" });
            cmb_filter.Items.Add(new { Text = "Статус заказа", Value = "Status" });
            cmb_filter.Items.Add(new { Text = "Фамилия клиента", Value = "LastName" });
            cmb_filter.Items.Add(new { Text = "Имя клиента", Value = "FirstName" });
            cmb_filter.Items.Add(new { Text = "Товар клиента", Value = "ProductName" });
            cmb_filter.Items.Add(new { Text = "Цена за 1 ед.", Value = "Price" });
            cmb_filter.Items.Add(new { Text = "Сумма заказа", Value = "TotalAmount" });

            cmb_filter.SelectedIndex = 0;
        }

        private void UpdateOrder()
        {
            this.заказыTableAdapter.Fill(this.labDataSet.Заказы);
        }

        private void ClearInputFields()
        {
            cmb_status.SelectedIndex = -1;
            cmb_user.SelectedIndex = -1;
            cmb_product.SelectedIndex = -1;
            txt_quantity.Clear();
        }

        private bool ValidateInput()
        {
            if (cmb_status.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите статус заказа");
                cmb_status.Focus();
                return false;
            }

            if (cmb_user.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите покупателя");
                cmb_user.Focus();
                return false;
            }

            if (cmb_product.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар");
                cmb_product.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_quantity.Text))
            {
                MessageBox.Show("Укажите количество");
                txt_quantity.Focus();
                return false;
            }

            return true;
        }

        private void btn_order_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                DateTime? orderDate = null;

                if (dt_order.Checked)
                    orderDate = dt_order.Value;

                this.queriesTableAdapter1.AddOrder((int)cmb_product.SelectedValue, (int)cmb_user.SelectedValue,
                    int.Parse(txt_quantity.Text), orderDate, cmb_status.Text);
                this.заказыTableAdapter.Fill(this.labDataSet.Заказы);
                this.товарыTableAdapter.Fill(this.labDataSet.Товары);

                MessageBox.Show("Заказ успешно добавлен");
                UpdateOrder();
                ClearInputFields();

                lb_price.Text = string.Empty;
                lb_amount.Text = string.Empty;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmb_product_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_product.SelectedItem is DataRowView row)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                lb_price.Text = price.ToString("0.00");

                CalculateTotal();
            }
        }

        private void txt_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txt_quantity_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            if (!(cmb_product.SelectedItem is DataRowView row))
                return;

            if (!int.TryParse(txt_quantity.Text, out int quantity))
            {
                lb_amount.Text = "0.00";
                return;
            }

            decimal price = Convert.ToDecimal(row["Price"]);
            decimal total = price * quantity;

            lb_price.Text = price.ToString("0.00");
            lb_amount.Text = total.ToString("0.00");
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btn_order_Click(sender, e);
        }

        private void btn_upd_Click(object sender, EventArgs e)
        {
            if (заказыBindingSource.Current == null)
            {
                MessageBox.Show("Не выбран заказ");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.заказыBindingSource.Current;
            int p = Convert.ToInt32(j["OrderID"]);

            if (!ValidateInput())
                return;

            DateTime? orderDate = null;
            if (dt_order.Checked)
                orderDate = dt_order.Value;

            try
            {
                this.queriesTableAdapter1.UpdateOrder(
                    p,
                    Convert.ToInt32(cmb_product.SelectedValue),
                    Convert.ToInt32(cmb_user.SelectedValue),
                    int.Parse(txt_quantity.Text),
                    orderDate,
                    cmb_status.Text
                    );

                this.заказыTableAdapter.Fill(this.labDataSet.Заказы);
                this.товарыTableAdapter.Fill(this.labDataSet.Товары);

                MessageBox.Show("Заказ успешно обновлен");

                UpdateOrder();
                ClearInputFields();

                lb_price.Text = string.Empty;
                lb_amount.Text = string.Empty;
            }
            catch (Exception ex)
            {
                    MessageBox.Show(ex.Message, "Ошибка обновления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (заказыBindingSource.Current == null)
            {
                MessageBox.Show("Не выбран заказ");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.заказыBindingSource.Current;
            int p = Convert.ToInt32(j["OrderID"]);

            this.queriesTableAdapter1.DeleteOrder(p);
            this.заказыTableAdapter.Fill(this.labDataSet.Заказы);

            MessageBox.Show("Заказ успешно удален");

            UpdateOrder();
            ClearInputFields();

            lb_price.Text = string.Empty;
            lb_amount.Text = string.Empty;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btn_del_Click(sender, e);
        }

        private void btn_filter_Click(object sender, EventArgs e)
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

            string filter = txt_filter.Text;

            // Строковый или числовой фильтр
            if (column == "OrderID" || column == "Price" || column == "TotalAmount") 
            {
                if (string.IsNullOrWhiteSpace(txt_filter.Text))
                {
                    MessageBox.Show("Введите значение для поиска");
                    return;
                }

                заказыBindingSource.Filter = $"{column} = {filter}"; // = числовой
            }
            else if (column == "OrderDate")
            {
                string dateValue = dt_filter.Value.ToString("yyyy-MM-dd");
                заказыBindingSource.Filter = $"{column} = #{dateValue}#";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txt_filter.Text))
                {
                    MessageBox.Show("Введите значение для поиска");
                    return;
                }

                заказыBindingSource.Filter = $"{column} LIKE '%{filter}%'"; // Like - строковый. // % значение содержит
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            заказыBindingSource.RemoveFilter();
            txt_filter.Clear();
            cmb_filter.SelectedIndex = 0;
        }
    }
}
