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
    public partial class FormPayment : Form
    {
        public FormPayment()
        {
            InitializeComponent();
        }

        private void FormPayment_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Заказы". При необходимости она может быть перемещена или удалена.
            this.заказыTableAdapter.Fill(this.labDataSet.Заказы);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Оплата". При необходимости она может быть перемещена или удалена.
            this.оплатаTableAdapter.Fill(this.labDataSet.Оплата);


            cmb_filter.DisplayMember = "Text";
            cmb_filter.ValueMember = "Value";

            cmb_filter.Items.Add(new { Text = "Номер заказа", Value = "OrderID" });
            cmb_filter.Items.Add(new { Text = "Дата оплаты", Value = "PaymentDate" });
            cmb_filter.Items.Add(new { Text = "Способ оплаты", Value = "PaymentMethod" });

            cmb_filter.SelectedIndex = 0;
        }

        private void UpdatePayment()
        {
            this.оплатаTableAdapter.Fill(this.labDataSet.Оплата);
        }

        private void ClearInputFields()
        {
            cmb_order.SelectedIndex = -1;
            cmb_method.SelectedIndex = -1;
        }

        private bool ValidateInput()
        {
            if (cmb_order.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите номер заказа");
                cmb_order.Focus();
                return false;
            }

            if (cmb_method.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите способ оплаты");
                cmb_method.Focus();
                return false;
            }

            return true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            DateTime? deliveryDate = null;

            if (dt_payment.Checked)
                deliveryDate = dt_payment.Value;

            this.queriesTableAdapter1.AddPayment((int)cmb_order.SelectedValue, deliveryDate, cmb_method.Text);
            this.оплатаTableAdapter.Fill(this.labDataSet.Оплата);

            MessageBox.Show("Оплата успешно добавлена");
            UpdatePayment();
            ClearInputFields();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btn_add_Click(sender, e);
        }

        private void btn_upd_Click(object sender, EventArgs e)
        {
            if (оплатаBindingSource.Current == null)
            {
                MessageBox.Show("Не выбрана оплата");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.оплатаBindingSource.Current;
            int p = Convert.ToInt32(j["PaymentID"]);

            if (!ValidateInput())
                return;

            DateTime? paymentDate = null;
            if (dt_payment.Checked)
                paymentDate = dt_payment.Value;

            this.queriesTableAdapter1.UpdatePayment(
                p,
                Convert.ToInt32(cmb_order.SelectedValue),
                paymentDate,
                cmb_method.Text
                );

            this.оплатаTableAdapter.Fill(this.labDataSet.Оплата);

            MessageBox.Show("Оплата успешно обновлена");

            UpdatePayment();
            ClearInputFields();
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (оплатаBindingSource.Current == null)
            {
                MessageBox.Show("Не выбрана оплата");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.оплатаBindingSource.Current;
            int p = Convert.ToInt32(j["PaymentID"]);

            this.queriesTableAdapter1.DeletePayment(p);
            this.оплатаTableAdapter.Fill(this.labDataSet.Оплата);

            MessageBox.Show("Оплата успешно удалена");

            UpdatePayment();
            ClearInputFields();
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

            // фильтр
            if (column == "OrderID")
            {
                if (string.IsNullOrWhiteSpace(txt_filter.Text))
                {
                    MessageBox.Show("Введите значение для поиска");
                    return;
                }

                оплатаBindingSource.Filter = $"{column} = {filter}"; // = числовой
            }
            else if (column == "PaymentDate")
            {
                string dateValue = dt_filter.Value.ToString("yyyy-MM-dd");
                оплатаBindingSource.Filter = $"{column} = #{dateValue}#";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txt_filter.Text))
                {
                    MessageBox.Show("Введите значение для поиска");
                    return;
                }

                оплатаBindingSource.Filter = $"{column} LIKE '%{filter}%'"; // Like - строковый. // % значение содержит
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            оплатаBindingSource.RemoveFilter();
            txt_filter.Clear();
            cmb_filter.SelectedIndex = 0;
        }
    }
}
