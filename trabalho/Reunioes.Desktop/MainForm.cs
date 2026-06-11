using System;
using System.Drawing;
using System.Windows.Forms;
using Reunioes.Core.Models;
using Reunioes.Core.Services;

namespace Reunioes.Desktop
{
    public partial class MainForm : Form
    {
        private readonly SalaService _salaService;
        private readonly ReservaService _reservaService;
        private int _paginaAtual = 1;

        // Componentes visuais
        private DataGridView dgvSalas;
        private TextBox txtBuscaNome;
        private TextBox txtNomeSala;
        private NumericUpDown numAndar;
        private NumericUpDown numAssentos;
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFim;
        private Label lblTotalReunioes;
        private Button btnSalvarSala;
        private Button btnReservar;
        private Button btnProximo;
        private Button btnAnterior;
        private Label lblBusca, lblNome, lblAndar, lblAssentos, lblInicio, lblFim, lblTituloSala, lblTituloReserva;

        public MainForm()
        {
            // Executa a montagem dos componentes diretamente aqui
            CriarComponentesManualmente();

            _salaService = new SalaService();
            _reservaService = new ReservaService();
            CarregarDados();
        }

        private void CriarComponentesManualmente()
        {
            this.dgvSalas = new DataGridView();
            this.txtBuscaNome = new TextBox();
            this.txtNomeSala = new TextBox();
            this.numAndar = new NumericUpDown();
            this.numAssentos = new NumericUpDown();
            this.dtpInicio = new DateTimePicker();
            this.dtpFim = new DateTimePicker();
            this.lblTotalReunioes = new Label();
            this.btnSalvarSala = new Button();
            this.btnReservar = new Button();
            this.btnProximo = new Button();
            this.btnAnterior = new Button();

            this.lblBusca = new Label();
            this.lblNome = new Label();
            this.lblAndar = new Label();
            this.lblAssentos = new Label();
            this.lblInicio = new Label();
            this.lblFim = new Label();
            this.lblTituloSala = new Label();
            this.lblTituloReserva = new Label();

            this.SuspendLayout();

            // Configurações da Janela Principal
            this.ClientSize = new Size(920, 520);
            this.Text = "SAV - Sistema de Gerenciamento de Reuniões";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Painel de Busca e Grid
            this.lblBusca.Text = "Buscar Sala por Nome:";
            this.lblBusca.Location = new Point(20, 20);
            this.lblBusca.Size = new Size(150, 20);

            this.txtBuscaNome.Location = new Point(20, 40);
            this.txtBuscaNome.Size = new Size(400, 23);
            this.txtBuscaNome.TextChanged += new EventHandler(this.txtBuscaNome_TextChanged);

            this.dgvSalas.Location = new Point(20, 75);
            this.dgvSalas.Size = new Size(440, 340);
            this.dgvSalas.AllowUserToAddRows = false;
            this.dgvSalas.AllowUserToDeleteRows = false;
            this.dgvSalas.ReadOnly = true;
            this.dgvSalas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSalas.MultiSelect = false;
            this.dgvSalas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Botões de Paginação
            this.btnAnterior.Text = "◀ Anterior";
            this.btnAnterior.Location = new Point(20, 425);
            this.btnAnterior.Size = new Size(100, 30);
            this.btnAnterior.Click += new EventHandler(this.btnAnterior_Click);

            this.btnProximo.Text = "Próximo ▶";
            this.btnProximo.Location = new Point(360, 425);
            this.btnProximo.Size = new Size(100, 30);
            this.btnProximo.Click += new EventHandler(this.btnProximo_Click);

            // Cadastro de Sala
            this.lblTituloSala.Text = "CADASTRAR NOVA SALA";
            this.lblTituloSala.Font = new Font(this.Font, FontStyle.Bold);
            this.lblTituloSala.Location = new Point(500, 20);
            this.lblTituloSala.Size = new Size(200, 20);

            this.lblNome.Text = "Nome da Sala *";
            this.lblNome.Location = new Point(500, 45);
            this.lblNome.Size = new Size(100, 15);

            this.txtNomeSala.Location = new Point(500, 65);
            this.txtNomeSala.Size = new Size(380, 23);

            this.lblAndar.Text = "Andar *";
            this.lblAndar.Location = new Point(500, 95);
            this.lblAndar.Size = new Size(80, 15);

            this.numAndar.Location = new Point(500, 115);
            this.numAndar.Size = new Size(170, 23);

            this.lblAssentos.Text = "Assentos *";
            this.lblAssentos.Location = new Point(710, 95);
            this.lblAssentos.Size = new Size(100, 15);

            this.numAssentos.Location = new Point(710, 115);
            this.numAssentos.Size = new Size(170, 23);
            this.numAssentos.Minimum = 1;

            this.btnSalvarSala.Text = "Salvar Sala";
            this.btnSalvarSala.Location = new Point(500, 150);
            this.btnSalvarSala.Size = new Size(380, 35);
            this.btnSalvarSala.Click += new EventHandler(this.btnSalvarSala_Click);

            // Reserva de Horários
            this.lblTituloReserva.Text = "RESERVAR SALA SELECIONADA";
            this.lblTituloReserva.Font = new Font(this.Font, FontStyle.Bold);
            this.lblTituloReserva.Location = new Point(500, 215);
            this.lblTituloReserva.Size = new Size(250, 20);

            this.lblInicio.Text = "Data/Hora de Início *";
            this.lblInicio.Location = new Point(500, 240);
            this.lblInicio.Size = new Size(150, 15);

            this.dtpInicio.Location = new Point(500, 260);
            this.dtpInicio.Size = new Size(380, 23);
            this.dtpInicio.Format = DateTimePickerFormat.Custom;
            this.dtpInicio.CustomFormat = "dd/MM/yyyy HH:mm";

            this.lblFim.Text = "Data/Hora de Fim *";
            this.lblFim.Location = new Point(500, 295);
            this.lblFim.Size = new Size(150, 15);

            this.dtpFim.Location = new Point(500, 315);
            this.dtpFim.Size = new Size(380, 23);
            this.dtpFim.Format = DateTimePickerFormat.Custom;
            this.dtpFim.CustomFormat = "dd/MM/yyyy HH:mm";

            this.btnReservar.Text = "Confirmar Reserva";
            this.btnReservar.Location = new Point(500, 355);
            this.btnReservar.Size = new Size(380, 40);
            this.btnReservar.BackColor = Color.LightGreen;
            this.btnReservar.Click += new EventHandler(this.btnReservar_Click);

            this.lblTotalReunioes.Text = "Reuniões nos últimos 7 dias: 0";
            this.lblTotalReunioes.Location = new Point(500, 432);
            this.lblTotalReunioes.Size = new Size(380, 20);

            // Injeta os controles no formulário
            this.Controls.Add(this.lblBusca);
            this.Controls.Add(this.txtBuscaNome);
            this.Controls.Add(this.dgvSalas);
            this.Controls.Add(this.btnAnterior);
            this.Controls.Add(this.btnProximo);
            this.Controls.Add(this.lblTituloSala);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.txtNomeSala);
            this.Controls.Add(this.lblAndar);
            this.Controls.Add(this.numAndar);
            this.Controls.Add(this.lblAssentos);
            this.Controls.Add(this.numAssentos);
            this.Controls.Add(this.btnSalvarSala);
            this.Controls.Add(this.lblTituloReserva);
            this.Controls.Add(this.lblInicio);
            this.Controls.Add(this.dtpInicio);
            this.Controls.Add(this.lblFim);
            this.Controls.Add(this.dtpFim);
            this.Controls.Add(this.btnReservar);
            this.Controls.Add(this.lblTotalReunioes);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CarregarDados()
        {
            dgvSalas.DataSource = _salaService.ListarSalas(txtBuscaNome.Text, _paginaAtual);
            lblTotalReunioes.Text = $"Reuniões agendadas nos últimos 7 dias: {_salaService.ObterTotalReunioesUltimos7Dias()}";
        }

        private void btnSalvarSala_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeSala.Text))
            {
                MessageBox.Show("O campo Nome da sala é obrigatório.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sala = new Sala
            {
                Nome = txtNomeSala.Text,
                Andar = (int)numAndar.Value,
                QuantidadeAssentos = (int)numAssentos.Value
            };

            _salaService.Criar(sala);
            MessageBox.Show("Sala cadastrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtNomeSala.Clear();
            CarregarDados();
        }

        private void btnReservar_Click(object sender, EventArgs e)
        {
            if (dgvSalas.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma sala na tabela antes.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int salaId = (int)dgvSalas.CurrentRow.Cells["Id"].Value;

            var reserva = new Reserva
            {
                SalaId = salaId,
                Inicio = dtpInicio.Value,
                Fim = dtpFim.Value
            };

            string resultado = _reservaService.AgendarReserva(reserva);
            MessageBox.Show(resultado == "Sucesso" ? "Reserva cadastrada!" : resultado);
            CarregarDados();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            _paginaAtual++;
            CarregarDados();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_paginaAtual > 1)
            {
                _paginaAtual--;
                CarregarDados();
            }
        }

        private void txtBuscaNome_TextChanged(object sender, EventArgs e)
        {
            _paginaAtual = 1;
            CarregarDados();
        }
    }
}