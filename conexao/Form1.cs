using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EM.Domain;
using EM.Repository;
using FirebirdSql.Data.FirebirdClient;

namespace EM.WindowsForms
{
    public partial class Form1 : Form
    {

        private  readonly RepositorioAluno _repositorioAluno = new RepositorioAluno();
        DateTime dt;

        public Form1()
        {
            InitializeComponent();
            ConexaoFirebird.Conectar();
            CarregarGrid(_repositorioAluno.GetAll());
            comboBoxSexo.SelectedIndex = 0;
           
        }

        private void InicializaFormulario()
        {
            ConexaoFirebird.Conectar();
            CarregarGrid(_repositorioAluno.GetAll());
            ConexaoFirebird.Desconectar();
        }

        //ADICIONAR
        private void buttonNovo_Click(object sender, EventArgs e)
        {
            IEnumerable<Aluno> alunos = _repositorioAluno.GetAll();
            var matricula = alunos.OrderByDescending(x => x.Matricula).First().Matricula + 1;

            if (string.IsNullOrEmpty(textmatricula.Text))
            {


                MessageBox.Show("Informe a matricula " + matricula + " para realizar o cadastro!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textmatricula.Focus();
                    return;
            }

            if (string.IsNullOrEmpty(textnome.Text))
            {
                MessageBox.Show("Informe o nome!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textnome.Focus();
                return;
            }

            if (!DateTime.TryParseExact(maskedTextNasc.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out dt))
            {
                MessageBox.Show("ATENÇÃO: Data Inválida ou Campo Vazio!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextNasc.Focus();
                return;
            }

            if (Convert.ToDateTime(maskedTextNasc.Text) > DateTime.Now)
            {
                MessageBox.Show("Data deve ser menor ou igual o dia atual!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextNasc.Focus();
                return;
            }

            if (validaCPF() != true)
            {
                MessageBox.Show("CPF inválido", "Validador de CPF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textCPF.Focus();
                return;
            }

            Aluno aluno = new Aluno();
            aluno.Matricula = Convert.ToInt32(textmatricula.Text.Trim());
            aluno.Nome = textnome.Text.Trim();
            aluno.Nascimento = Convert.ToDateTime(maskedTextNasc.Text.Trim());
            aluno.Sexo = (EnumeradorSexo)Enum.Parse(typeof(EnumeradorSexo), comboBoxSexo.Text);
            aluno.CPF = textCPF.Text.Trim();

            if (buttonNovo.Text == "Modificar")
            {
                AtualizarDados(aluno);
                LimparDados();
                buttonLimpar.Text = "Limpar";
                buttonNovo.Text = "Adicionar";
                label1.Text = "Novo Aluno";
                return;
            }

            if (buttonLimpar.Text == "Cancelar")
            {
                LimparDados();
            }

            if (_repositorioAluno.GetAll().Contains(aluno))
            {
                MessageBox.Show("Codigio de matricula já Utlizado");
                return;
            }

            try
            {
                _repositorioAluno.Add(aluno);
                MessageBox.Show("Gravado com Sucesso!");
                InicializaFormulario();
                LimparDados();
            }
            catch (Exception)
            {
                MessageBox.Show("Conexão Falhou");
            }
        }

        private void AtualizaGrid(Aluno aluno)
        {
            if (aluno.Matricula > 0)
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = aluno;
                dataGridView1.DataSource = bs;
                ConexaoFirebird.Desconectar();
            }
            else
            {
                BindingSource bs = new BindingSource();
                dataGridView1.DataSource = bs;
                ConexaoFirebird.Desconectar();
            }
        }

        private void CarregarGrid(IEnumerable<Aluno> aluno)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = aluno;
            dataGridView1.DataSource = bs;
            ConexaoFirebird.Desconectar();
        }

        //BUSCA
        private void buttonPesquisa_Click(object sender, EventArgs e)
        {
            int numero = 0;
            if (int.TryParse(textBoxPesquisa.Text.Trim(), out numero))
            {
                AtualizaGrid(_repositorioAluno.GetByMatricula(numero));
                return;
            }
            else  if (!string.IsNullOrEmpty(textBoxPesquisa.Text) )
            {
                try
                {
                    //Aluno aluno = new Aluno();

                    //var dadosnome = aluno.Nome;

                    //var partnome = from p in dadosnome where p.ToLower().Contains(textBoxPesquisa.Text.Trim().ToLower()) select p;
                    //CarregarGrid(_repositorioAluno.GetByContendoNoNme(partnome));

                    string nome = textBoxPesquisa.Text.Trim().ToLower();
                    CarregarGrid(_repositorioAluno.GetByContendoNome(nome));
                    return;
                }
                catch (Exception)
                {
                    MessageBox.Show("Dados não encontrado!", "Confirmação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else
            {
                try
                {
                    CarregarGrid(_repositorioAluno.GetAll());
                }
                catch (Exception)
                {
                    MessageBox.Show("Conexão Falhou!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LimparDados()
        {
            textmatricula.Text = null;
            textnome.Text = null;
            textBoxPesquisa.Text = null;
            textCPF.Text = String.Empty;
            comboBoxSexo.SelectedIndex = 0;
            maskedTextNasc.Text = String.Empty;
            textmatricula.Enabled = true;
            buttonEditar.Enabled = true;
            label1.Text = "Novo Aluno";
        }

        private void buttonLimpar_Click(object sender, EventArgs e)
        {
            LimparDados();
            buttonLimpar.Text = "Limpar";
            buttonNovo.Text = "Adicionar";
        }

        private void AtualizarDados(Aluno aluno)
        {
            try
            {
                _repositorioAluno.Update(aluno);
                MessageBox.Show("Atualizado com Sucesso!", "Confirmação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                InicializaFormulario();
                LimparDados();
            }
            catch (Exception)
            {
                MessageBox.Show("Conexão Falhou!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //EDICAO
        private void buttonEditar_Click(object sender, EventArgs e)
        {

            if(string.IsNullOrEmpty(textmatricula.Text) || (!DateTime.TryParseExact(maskedTextNasc.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out dt))
                || validaCPF() != true)
            {
                MessageBox.Show("Click no item e depois no editar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            label1.Text = "Editando aluno";
            buttonLimpar.Text = "Cancelar";
            buttonNovo.Text = "Modificar";
            buttonEditar.Enabled = false;

        }

        //EXCLUSAO
        private void buttonExcluir_Click(object sender, EventArgs e)
        {

            Aluno aluno = new Aluno();
            aluno.Matricula = (int)dataGridView1.CurrentRow.Cells[0].Value;
            aluno.Nome = (String)dataGridView1.CurrentRow.Cells[1].Value;
            aluno.Sexo = (EnumeradorSexo)dataGridView1.CurrentRow.Cells[2].Value;
            aluno.Nascimento = (DateTime)dataGridView1.CurrentRow.Cells[3].Value;
            aluno.CPF = (String)dataGridView1.CurrentRow.Cells[4].Value;

            DialogResult resultado = MessageBox.Show("Confirma exclusão deste usuário ?", "Confirma Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                try
                {
                    _repositorioAluno.Remove(aluno);
                    InicializaFormulario();
                    LimparDados();
                    MessageBox.Show("Usuário excluido com sucesso.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir o usuário." + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void textmatricula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != 08))
            {
                e.Handled = true;
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textmatricula.Text = (dataGridView1.CurrentRow.Cells[0].Value.ToString());
            textnome.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            comboBoxSexo.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            maskedTextNasc.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textCPF.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textmatricula.Enabled = false;

            //DataGridViewRow linha = dataGridView1.Rows[e.RowIndex];
            //textmatricula.Text = linha.Cells[0].Value.ToString();
            //textnome.Text = linha.Cells[1].Value.ToString();
            //comboBoxSexo.Text = linha.Cells[2].Value.ToString();
            //maskedTextNasc.Text = linha.Cells[3].Value.ToString();
            //textCPF.Text = linha.Cells[4].Value.ToString();
            //textmatricula.Enabled = false;


        }

        bool validaCPF()
        {
            if (textCPF.Text.Length == 14)
            {
                int n1 = Convert.ToInt16(textCPF.Text.Substring(0, 1));
                int n2 = Convert.ToInt16(textCPF.Text.Substring(1, 1));
                int n3 = Convert.ToInt16(textCPF.Text.Substring(2, 1));
                int n4 = Convert.ToInt16(textCPF.Text.Substring(4, 1));
                int n5 = Convert.ToInt16(textCPF.Text.Substring(5, 1));
                int n6 = Convert.ToInt16(textCPF.Text.Substring(6, 1));
                int n7 = Convert.ToInt16(textCPF.Text.Substring(8, 1));
                int n8 = Convert.ToInt16(textCPF.Text.Substring(9, 1));
                int n9 = Convert.ToInt16(textCPF.Text.Substring(10, 1));
                int n10 = Convert.ToInt16(textCPF.Text.Substring(12, 1));
                int n11 = Convert.ToInt16(textCPF.Text.Substring(13, 1));

                if (n1 == n2 && n2 == n3 && n3 == n4 && n4 == n5 && n5 == n6 && n6 == n7 && n7 == n8 && n8 == n9)
                {
                    return false;
                }
                else
                {
                    int Soma1 = n1 * 10 + n2 * 9 + n3 * 8 + n4 * 7 + n5 * 6 + n6 * 5 + n7 * 4 + n8 * 3 + n9 * 2;

                    int digitoVerificador1 = Soma1 % 11;

                    if (digitoVerificador1 < 2)
                    {
                        digitoVerificador1 = 0;
                    }
                    else
                    {
                        digitoVerificador1 = 11 - digitoVerificador1;
                    }

                    int Soma2 = n1 * 11 + n2 * 10 + n3 * 9 + n4 * 8 + n5 * 7 + n6 * 6 + n7 * 5 + n8 * 4 + n9 * 3 + digitoVerificador1 * 2;

                    int digitoVerificador2 = Soma2 % 11;

                    if (digitoVerificador2 < 2)
                    {
                        digitoVerificador2 = 0;
                    }
                    else
                    {
                        digitoVerificador2 = 11 - digitoVerificador2;
                    }

                    if (digitoVerificador1 == n10 && digitoVerificador2 == n11)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
