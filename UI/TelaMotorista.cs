using Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Onibus
{
    public partial class TelaMotorista : Form
    {
        public TelaMotorista()
        {
            InitializeComponent();
        }
        Motorista mot;

        private void TelaMotorista_Load(object sender, EventArgs e)
        {
            new Motorista().CriarTabela();
            Carrega_DataGrid();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            mot = new Motorista();
            if(txtId.Text != "")
            {
                mot.ID = int.Parse(txtId.Text);
            }
            mot.Nome = txtNome.Text;    
            mot.Cpf = txtCpf.Text;
            mot.RG = txtRg.Text;
            mot.Cnh = txtCnh.Text;  
            mot.Celular = txtCelular.Text;  
            mot.Endereco = txtEndereco.Text;
            mot.Salvar();
            Carrega_DataGrid();
            Limpar();
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            mot = new Motorista();
            mot.ID = Convert.ToInt32(txtId.Text);
            foreach(Motorista m in mot.Buscar())
            {
                txtId.Text = m.ID.ToString();
                txtNome.Text = m.Nome;
                txtCpf.Text = m.Cpf;
                txtRg.Text = m.RG;
                txtCnh.Text = m.Cnh;
                txtCelular.Text = m.Celular;
                txtEndereco.Text = m.Endereco;
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            var retorno = MessageBox.Show("Tem certeza que deseja excluir?", "Excluir", MessageBoxButtons.YesNo);
            if(retorno == DialogResult.Yes)
            {
                mot = new Motorista();
                mot.ID = Convert.ToInt32(txtId.Text);
                mot.Excluir();
            }
            Carrega_DataGrid();
            Limpar();
        }
        private void Carrega_DataGrid()
        {
            mot = new Motorista();
            dgvDados.AutoGenerateColumns = false;
            dgvDados.DataSource = mot.Todos();
        }
        private void Limpar()
        {
            txtId.Text = null;
            txtNome.Text = null;
            txtCpf.Text = null;
            txtEndereco.Text = null;
            txtRg.Text = null;
            txtCnh.Text = null;
            txtCelular.Text = null;
        }
    }
}
