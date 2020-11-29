using SbsSW.SwiPlCs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proyecto_3_lenguajes_warner_hurtado
{
    public partial class Form1 : Form
    {
        private int y = 120;
        private int x = 220;
        private int _column = 0;
        private int _row = 0;
        private int btnName = 0;
        private int[,] matrix;
        private int size;
        private int cantGroup = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Environment.SetEnvironmentVariable("Path", @"C:\\Program Files (x86)\\swipl\\bin");
            string[] p = { "-q", "-f", @"operations.pl" };
            PlEngine.Initialize(p);
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            comboBox1.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void create_matrix()
        {
            try
            {
                size = Int16.Parse(textBox1.Text);
            }
            catch
            {
                DialogResult res = MessageBox.Show("Debe ingresar un valor entero", "Confirmar", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }

            if (size > 10)
            {
                DialogResult res = MessageBox.Show("El tamaño máximo permitido es de 10", "Confirmar", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }
            button1.Enabled = false;
            button5.Enabled = false;
            button2.Enabled = true;
            //button3.Enabled = true;
            //comboBox1.Enabled = true;
            matrix = new int[size, size];

            for (int i = 0; i < size; i++)
            {

                for (int n = 0; n < size; n++)
                {

                    Button btn = new Button();
                    btn.Name = _row.ToString() + "-" + _column.ToString();
                    btn.Height = 30;
                    btn.Width = 40;
                    btn.BackColor = Color.Red;
                    btn.Location = new Point(x, y);
                    btn.Text = btnName.ToString();
                    btn.ForeColor = Color.Red;
                    btn.Font = new Font("Georgia", 10);
                    btn.Click += new EventHandler(button_Click);
                    //btn.Cursor.Handle;
                    btn.Cursor = Cursors.Hand;

                    matrix[i, n] = btnName;

                    x += 30;
                    _column++;
                    btnName++;
                    Controls.Add(btn);

                }
                y += 20;
                x = 220;
                _column = 0;
                _row++;
            }
        }

        protected void fill_automatic()
        {

            for (int n = 0; n < btnName; n++)
            {
                var guid = Guid.NewGuid();
                var justNumbers = new String(guid.ToString().Where(Char.IsDigit).ToArray());
                var seed = int.Parse(justNumbers.Substring(0, 4));

                var random = new Random(seed);
                var value = random.Next(0, 5);

                if (value == 2)
                {
                    foreach (var ctrl in this.Controls.OfType<Button>())
                    {
                        if (n.ToString() == ctrl.Text)
                        {
                            ctrl.Text = "." + ctrl.Text;
                            ctrl.BackColor = Color.Lime;
                            ctrl.ForeColor = Color.Lime;
                            char row_buttom = ctrl.Name[0];
                            char column_buttom = ctrl.Name[2];
                            buttonTouched((int)Char.GetNumericValue(row_buttom), (int)Char.GetNumericValue(column_buttom));
                        }
                    }


                }
            }
        
        }

        protected void button_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = true;
            button5.Enabled = true;
            comboBox1.Enabled = true;

            delete_color_select();
            listBox1.Items.Clear();
            string s = (sender as Button).Text;
            string v = (sender as Button).Name;

            char row_buttom = v[0];
            char column_buttom = v[2];

            if ((sender as Button).Text[0] != '.')
            {
                ((Button)sender).BackColor = Color.Lime;
                ((Button)sender).ForeColor = Color.Lime;
               ((Button)sender).Text = "." + (sender as Button).Text;
            }
            buttonTouched((int)Char.GetNumericValue(row_buttom), (int)Char.GetNumericValue(column_buttom));
        }
        protected void buttonTouched(int row, int column)
        {
            bool p = true;
            int nValue = existG(matrix[row, column]);
            foreach (var ctrl in this.Controls.OfType<Button>())
            {
                if (nValue == 0 && p)
                {
                    try
                    {
                        string groupName = "";
                        listBox1.Items.Clear();
                        PlQuery carga = new PlQuery("cargar('operations.bd')");
                        carga.NextSolution();

                        PlQuery consul = new PlQuery("getConections(" + matrix[row, column].ToString() + ", X, Y)");
                        foreach (PlQueryVariables z in consul.SolutionVariables)
                        {
                            groupName = z["Y"].ToString();
                        }
                        int totalForGroups = 0;
                        foreach (PlQueryVariables z in consul.SolutionVariables)
                        {
                            totalForGroups++;
                        }
                        listBox1.Items.Add("Grupo: " + groupName + ", Total: " + totalForGroups);
                        foreach (PlQueryVariables z in consul.SolutionVariables)
                        {
                            //listBox1.Items.Add(z["X"].ToString());
                        }
                        consul.Dispose();
                        carga.Dispose();
                        chargeComboBox();
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }

                int r = row - 1;
                if (r != -1)
                {
                    if (ctrl.Text == "." + matrix[(r), column].ToString())
                    {
                        connection(ctrl.Text.Substring(1), matrix[row, column].ToString());
                        p = false;
                    }
                }


                int f = row + 1;
                if (f != size)
                {
                    if (ctrl.Text == "." + matrix[f, column].ToString())
                    {
                        connection(ctrl.Text.Substring(1), matrix[row, column].ToString());
                        p = false;
                    }
                }

                int c = column - 1;
                if (c != -1)
                {
                    if (ctrl.Text == "." + matrix[row, c].ToString())
                    {
                        connection(ctrl.Text.Substring(1), matrix[row, column].ToString());
                        p = false;
                    }

                }

                int cc = column + 1;
                if (cc != size)
                {
                    if (ctrl.Text == "." + matrix[row, cc].ToString())
                    {
                        connection(ctrl.Text.Substring(1), matrix[row, column].ToString());
                        p = false;
                    }
                }


            }
            if (p)
            {

                try
                {
                    listBox1.Items.Clear();
                    PlQuery cargar = new PlQuery("cargar('operations.bd')");
                    cargar.NextSolution();

                    PlQuery consulta = new PlQuery("newConnect(" + matrix[row, column].ToString() + ", " + cantGroup.ToString() + ")");
                    foreach (PlQueryVariables z in consulta.SolutionVariables)
                    { }
                    cantGroup++;
                    consulta.Dispose();
                    cargar.Dispose();
                }
                catch { }
            }
            chargeComboBox();
        }
        private void connection(string num1, string num2)
        {
            try
            {
                listBox1.Items.Clear();
                PlQuery cargar = new PlQuery("cargar('operations.bd')");
                cargar.NextSolution();

                PlQuery consulta = new PlQuery("addConnect(" + num1 + ", " + num2 + ", X)");
                foreach (PlQueryVariables z in consulta.SolutionVariables)
                {
                }
                consulta.Dispose();
                cargar.Dispose();
            }
            catch { }
        }

        private string getTotalForGroup()
        {
            int n = cantGroup;
            string p = "";

            try
            {
                while (!n.Equals(0))
                {
                    listBox1.Items.Clear();
                    PlQuery cargar = new PlQuery("cargar('operations.bd')");
                    cargar.NextSolution();

                    PlQuery consulta = new PlQuery("getTotalForGroup(" + n + ", X)");
                    foreach (PlQueryVariables z in consulta.SolutionVariables)
                    {
                        p += "Grupo: " + n + z["X"];
                    }
                    consulta.Dispose();
                    cargar.Dispose();
                }
            }
            catch { }

            return p;
        }
        private void chargeComboBox()
        {
            string list = "";
            try
            {

                PlQuery cargar = new PlQuery("cargar('operations.bd')");
                cargar.NextSolution();

                PlQuery consulta = new PlQuery("getListGroups(X)");
                foreach (PlQueryVariables z in consulta.SolutionVariables)
                {
                    list = z["X"].ToString();

                }
                consulta.Dispose();
                cargar.Dispose();

                string listTrimmed = list.Substring(1, list.Length - 2);
                string[] newList = listTrimmed.Split(',');
                comboBox1.Items.Clear();
                for (int n = 0; n < newList.Length; n++)
                {
                    comboBox1.Items.Add(newList[n]);
                }
            }
            catch { }
        }
        private int existG(int value)
        {
            listBox1.Items.Clear();
            PlQuery cargar = new PlQuery("cargar('operations.bd')");

            PlQuery consulta = new PlQuery("exist(" + value.ToString() + ", X)");
            try
            {
                cargar.NextSolution();
                foreach (PlQueryVariables z in consulta.SolutionVariables)
                {
                    listBox1.Items.Add("- " + z["X"]);
                    if (z["X"] == 0)
                    {
                        consulta.Dispose();
                        cargar.Dispose();
                        return 0;
                    }
                    else
                    {
                        consulta.Dispose();
                        cargar.Dispose();
                        return 1;
                    }
                }
            }
            catch
            {
            }
            consulta.Dispose();
            cargar.Dispose();
            return 1;
        }

        private void delete_color_select()
        {
            foreach (var ctrl in this.Controls.OfType<Button>())
            {
                if (ctrl.BackColor.Equals(Color.Navy))
                {
                    ctrl.BackColor = Color.Lime;
                    ctrl.ForeColor = Color.Lime;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fill_automatic();
            button2.Enabled = false;
            button3.Enabled = true;
            button5.Enabled = true;
            comboBox1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            create_matrix();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = false;
            cantGroup = 1;
            foreach (var ctrl in this.Controls.OfType<Button>())
            {
                if (ctrl.Text.Substring(0, 1).Equals("."))
                {
                    ctrl.BackColor = Color.Red;
                    ctrl.ForeColor = Color.Red;
                    ctrl.Text = ctrl.Text.Substring(1, (ctrl.Text.Length - 1));
                }
            }
            comboBox1.Items.Clear();
            try
            {
                listBox1.Items.Clear();
                PlQuery cargar = new PlQuery("cargar('operations.bd')");
                cargar.NextSolution();

                PlQuery consulta = new PlQuery("clean(X, Y)");
                listBox1.Items.Clear();
                foreach (PlQueryVariables z in consulta.SolutionVariables)
                {
                    listBox1.Items.Add(z["X"] + "Se limpió con éxito!" + z["Y"]);
                }
                consulta.Dispose();
                cargar.Dispose();
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                delete_color_select();
                string group = comboBox1.SelectedItem.ToString();
                listBox1.Items.Clear();
                PlQuery carga = new PlQuery("cargar('operations.bd')");
                carga.NextSolution();

                PlQuery consul = new PlQuery("connect(X, " + group + ")");

                foreach (PlQueryVariables z in consul.SolutionVariables)
                {
                    //listBox1.Items.Add(z["X"].ToString());

                    foreach (var ctrl in this.Controls.OfType<Button>())
                    {
                        if (("." + z["X"].ToString()).Equals(ctrl.Text))
                        {
                            ctrl.BackColor = Color.Navy;
                            ctrl.ForeColor = Color.Navy;
                        }
                    }

                }
                consul.Dispose();
                carga.Dispose();
                return;
            }
            catch
            {
                return;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string list = "";
            try
            {
                listBox1.Items.Clear();

                PlQuery cargar = new PlQuery("cargar('operations.bd')");
                cargar.NextSolution();

                PlQuery consulta1 = new PlQuery("getSizesGroups(X)");
                foreach (PlQueryVariables z in consulta1.SolutionVariables)
                {

                    list = z["X"].ToString();
                }
                consulta1.Dispose();
                cargar.Dispose();

                string listTrimmed = list.Substring(1, list.Length - 2);
                string[] newList = listTrimmed.Split(',');
                string[] newList2 = newList.Distinct().ToArray();

                for (int n = 0; n < newList2.Length; n++)
                {
                    int total_ = 0;
                    for (int p = 0; p < newList.Length; p++)
                    {
                        if (newList2[n].Equals(newList[p]))
                        {
                            total_++;
                        }
                    }
                    listBox1.Items.Add("Hay : " + total_ + " de" + newList2[n]);
                }
            }
            catch { }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
