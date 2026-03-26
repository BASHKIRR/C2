using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            UpdateProducts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Категории". При необходимости она может быть перемещена или удалена.
            this.категорииTableAdapter.Fill(this.labDataSet.Категории);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Товары". При необходимости она может быть перемещена или удалена.
            this.товарыTableAdapter.Fill(this.labDataSet.Товары);

            cmbFilterField.DisplayMember = "Text";
            cmbFilterField.ValueMember = "Value";

            cmbFilterField.Items.Add(new { Text = "Название товара", Value = "ProductName" });
            cmbFilterField.Items.Add(new { Text = "Категория", Value = "CategoryName" });
            cmbFilterField.Items.Add(new { Text = "Цена", Value = "Price" });
            cmbFilterField.Items.Add(new { Text = "Количество на складе", Value = "StockQuantity" });

            cmbFilterField.SelectedIndex = 0;
        }

        private void UpdateProducts()
        {
            this.товарыTableAdapter.Fill(this.labDataSet.Товары);
        }

        private void ClearInputFields()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();

            cmbCategory.SelectedIndex = -1; // сброс выбора категории
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Введите наименование товара");
                txtProductName.Focus();
                return false;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите категорию");
                cmbCategory.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Введите цену");
                txtPrice.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Введите количество");
                txtStock.Focus();
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            this.queriesTableAdapter.AddProduct(this.txtProductName.Text, (int)cmbCategory.SelectedValue, decimal.Parse(txtPrice.Text), int.Parse(txtStock.Text));
            this.товарыTableAdapter.Fill(this.labDataSet.Товары);

            MessageBox.Show("Товар успешно добавлен");
            UpdateProducts();
            ClearInputFields();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            if (товарыBindingSource.Current == null)
            {
                MessageBox.Show("Не выбран товар");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.товарыBindingSource.Current;
            int p = Convert.ToInt32(j["ProductID"]);

            if (!ValidateInput())
                return;

            this.queriesTableAdapter.UpdateProduct(
                p, 
                this.txtProductName.Text,
                Convert.ToInt32(cmbCategory.SelectedValue),
                decimal.Parse(txtPrice.Text), 
                int.Parse(txtStock.Text)
                );

            this.товарыTableAdapter.Fill(this.labDataSet.Товары);

            MessageBox.Show("Товар успешно обновлены");

            UpdateProducts();
            ClearInputFields();
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (товарыBindingSource.Current == null)
            {
                MessageBox.Show("Не выбран товар");
                return;
            }

            DataRowView j;
            j = (DataRowView)this.товарыBindingSource.Current;
            int p = Convert.ToInt32(j["ProductID"]);

            this.queriesTableAdapter.DeleteProduct(p);
            this.товарыTableAdapter.Fill(this.labDataSet.Товары);

            MessageBox.Show("Товар успешно удален");

            UpdateProducts();
            ClearInputFields();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btn_del_Click(sender, e);
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (cmbFilterField.SelectedItem == null)
            {
                MessageBox.Show("Выберите поле для фильтрации");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text))
            {
                MessageBox.Show("Введите значение для поиска");
                return;
            }

            string column = (string)cmbFilterField.SelectedItem  // получение имени столбца
                .GetType()
                .GetProperty("Value")
                .GetValue(cmbFilterField.SelectedItem, null);

            string filter = txtFilterValue.Text;

            // Строковый или числовой фильтр
            if (column == "Price" || column == "StockQuantity")
            {
                товарыBindingSource.Filter = $"{column} = {filter}"; // = числовой
            }
            else
            {
                товарыBindingSource.Filter = $"{column} LIKE '%{filter}%'"; // Like - строковый. // % значение содержит
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            товарыBindingSource.RemoveFilter();
            txtFilterValue.Clear();
            cmbFilterField.SelectedIndex = 0;
        }
    }
}
