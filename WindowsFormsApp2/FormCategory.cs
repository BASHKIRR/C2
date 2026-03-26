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
    public partial class FormCategory : Form
    {
        public FormCategory()
        {
            InitializeComponent();
        }

        private void FormCategory_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "labDataSet.Категории". При необходимости она может быть перемещена или удалена.
            this.категорииTableAdapter.Fill(this.labDataSet.Категории);

            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";

            comboBox1.Items.Add(new { Text = "Наименование категории", Value = "CategoryName" });
            comboBox1.SelectedIndex = 0;
        }

        private void UpdateCategory()
        {
            this.категорииTableAdapter.Fill(this.labDataSet.Категории);
        }

        private void ClearInputFields()
        {
            textBox1.Clear();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(textBox1.Text)) 
            {
                MessageBox.Show("Введите наименование категории");
                textBox1.Focus();
                return false;
            }

            return true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            this.queriesTableAdapter1.AddCategory(this.textBox1.Text);
            this.категорииTableAdapter.Fill(this.labDataSet.Категории);

            MessageBox.Show("Новая категория добавлена");
            UpdateCategory();
            ClearInputFields();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btn_add_Click(sender, e);
        }

        private void btn_upd_Click(object sender, EventArgs e)
        {
            if (категорииBindingSource.Current == null)
            {
                MessageBox.Show("Не выбрана категория");
                return;
            }

            DataRowView dr;
            dr = (DataRowView)this.категорииBindingSource.Current;
            int p = Convert.ToInt32(dr["CategoryID"]);

            if (!ValidateInput())
                return;

            this.queriesTableAdapter1.UpdateCategory(
                p, this.textBox1.Text);

            this.категорииTableAdapter.Fill(this.labDataSet.Категории);

            MessageBox.Show("Категория успешно обновлена");

            UpdateCategory();
            ClearInputFields();
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (категорииBindingSource.Current == null)
            {
                MessageBox.Show("Не выбрана категория");
                return;
            }

            DataRowView dr;
            dr = (DataRowView)this.категорииBindingSource.Current;
            int p = Convert.ToInt32(dr["CategoryID"]);

            this.queriesTableAdapter1.DeleteCategory(p);
            this.категорииTableAdapter.Fill(this.labDataSet.Категории);

            MessageBox.Show("Категория успешно удалена");

            UpdateCategory();
            ClearInputFields();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btn_del_Click(sender, e);
        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите поле для фильтра");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text)) 
            {
                MessageBox.Show("Введите значение для поиска");
                return;
            }

            string column = (string)comboBox1.SelectedItem // получение имени столбца
                .GetType()
                .GetProperty("Value")
                .GetValue(comboBox1.SelectedItem, null);

            string filter = textBox2.Text;
            категорииBindingSource.Filter = $"{column} LIKE '%{filter}%'"; // Like - строковый. // % значение содержит
        }

        private void btn_can_Click(object sender, EventArgs e)
        {
            категорииBindingSource.RemoveFilter();
            textBox2.Clear();
            comboBox1.SelectedIndex = 0;
        }
    }
}
