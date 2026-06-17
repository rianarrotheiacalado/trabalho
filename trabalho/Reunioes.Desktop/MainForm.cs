using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Reunioes.Core.Models;
using Reunioes.Core.Services;

namespace Reunioes.Desktop
{
    public partial class MainWindow : Form
    {
        private readonly SalaService _salaService;
        private readonly ReservaService _reservaService;

        private DataGridView dgvSalas;
        private TextBox txtFiltroSala;
        private TextBox txtNomeSala;
        private NumericUpDown nudAndar;
        private NumericUpDown nudAssentos;
        private Button btnCriarSala;
        private Button btnVerHorasLivres;
        private DataGridView dgvReservas;
        private ComboBox cbSalasDisponiveis;
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFim;
        private Button btnAgendar;
        private DateTimePicker dtpConsultaData;
        private Button btnConsultarReservas;

        public MainWindow()
        {
            InitializeComponent(); // Chama o método do Designer.cs

            _salaService = new SalaService();
            _reservaService = new ReservaService();

            ConfigurarComponentesManualmente();

            try
            {
                CarregarDadosIniciais();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de conexão: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarDadosIniciais()
        {
            List<Sala> salas = _salaService.ListarSalas(txtFiltroSala.Text, pagina: 1);
            dgvSalas.DataSource = null;
            dgvSalas.DataSource = salas;

            cbSalasDisponiveis.DataSource = null;
            cbSalasDisponiveis.DataSource = salas;
            cbSalasDisponiveis.DisplayMember = "Nome";
            cbSalasDisponiveis.ValueMember = "Id";

            dgvReservas.DataSource = null;
            dgvReservas.DataSource = _reservaService.ConsultarReservas(null, null);
        }

        private void btnCriarSala_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeSala.Text))
            {
                MessageBox.Show("O nome da sala é obrigatório.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var novaSala = new Sala
            {
                Nome = txtNomeSala.Text.Trim(),
                Andar = (int)nudAndar.Value,
                QuantidadeAssentos = (int)nudAssentos.Value
            };

            _salaService.Criar(novaSala);
            MessageBox.Show("Sala criada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtNomeSala.Clear();
            nudAndar.Value = 0;
            nudAssentos.Value = 1;

            CarregarDadosIniciais();
        }

        private void btnVerHorasLivres_Click(object sender, EventArgs e)
        {
            if (dgvSalas.CurrentRow?.DataBoundItem is Sala salaSelecionada)
            {
                DateTime dataSelecionada = dtpConsultaData.Value;
                double horasLivres = _salaService.CalcularHorasLivresNoDia(salaSelecionada.Id, dataSelecionada);

                MessageBox.Show($"A sala '{salaSelecionada.Nome}' possui {horasLivres:F1} horas livres no dia {dataSelecionada.ToShortDateString()}.",
                                "Disponibilidade", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Selecione uma sala na tabela da esquerda primeiro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtFiltroSala_TextChanged(object sender, EventArgs e)
        {
            dgvSalas.DataSource = _salaService.ListarSalas(txtFiltroSala.Text, pagina: 1);
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            if (cbSalasDisponiveis.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma sala para agendar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var novaReserva = new Reserva
            {
                SalaId = (int)cbSalasDisponiveis.SelectedValue,
                Inicio = DateTime.SpecifyKind(dtpInicio.Value, DateTimeKind.Unspecified),
                Fim = DateTime.SpecifyKind(dtpFim.Value, DateTimeKind.Unspecified)
            };

            string resultado = _reservaService.AgendarReserva(novaReserva);

            if (resultado == "Sucesso")
            {
                MessageBox.Show("Reserva realizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarDadosIniciais();
            }
            else
            {
                MessageBox.Show(resultado, "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConsultarReservas_Click(object sender, EventArgs e)
        {
            int? salaIdFiltro = null;
            if (cbSalasDisponiveis.SelectedValue != null)
            {
                salaIdFiltro = (int)cbSalasDisponiveis.SelectedValue;
            }

            DateTime dataFiltro = dtpConsultaData.Value;
            dgvReservas.DataSource = _reservaService.ConsultarReservas(salaIdFiltro, dataFiltro);
        }

        private void ConfigurarComponentesManualmente()
        {
            this.Text = "Gerenciador de Reuniões e Salas";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            GroupBox gbSalas = new GroupBox { Text = "Gestão de Salas", Location = new System.Drawing.Point(15, 15), Size = new System.Drawing.Size(420, 530) };
            Label lblNomeSala = new Label { Text = "Nome da Sala:", Location = new System.Drawing.Point(15, 25), Width = 90 };
            txtNomeSala = new TextBox { Location = new System.Drawing.Point(110, 22), Width = 150 };
            Label lblAndar = new Label { Text = "Andar:", Location = new System.Drawing.Point(15, 55), Width = 50 };
            nudAndar = new NumericUpDown { Location = new System.Drawing.Point(110, 52), Width = 60, Minimum = -2, Maximum = 50 };
            Label lblAssentos = new Label { Text = "Assentos:", Location = new System.Drawing.Point(15, 85), Width = 70 };
            nudAssentos = new NumericUpDown { Location = new System.Drawing.Point(110, 82), Width = 60, Minimum = 1, Maximum = 500, Value = 1 };
            btnCriarSala = new Button { Text = "Adicionar Sala", Location = new System.Drawing.Point(280, 21), Size = new System.Drawing.Size(120, 83) };
            btnCriarSala.Click += btnCriarSala_Click;
            Label lblLinha = new Label { BorderStyle = BorderStyle.Fixed3D, Location = new System.Drawing.Point(15, 120), Size = new System.Drawing.Size(390, 2) };
            Label lblFiltro = new Label { Text = "Buscar por Nome:", Location = new System.Drawing.Point(15, 135), Width = 110 };
            txtFiltroSala = new TextBox { Location = new System.Drawing.Point(130, 132), Width = 270 };
            txtFiltroSala.TextChanged += txtFiltroSala_TextChanged;
            dgvSalas = new DataGridView { Location = new System.Drawing.Point(15, 165), Size = new System.Drawing.Size(385, 310), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = true };
            btnVerHorasLivres = new Button { Text = "Ver Horas Livres no Dia Selecionado", Location = new System.Drawing.Point(15, 485), Width = 385, Height = 30 };
            btnVerHorasLivres.Click += btnVerHorasLivres_Click;

            gbSalas.Controls.AddRange(new Control[] {
                lblNomeSala, txtNomeSala, lblAndar, nudAndar, lblAssentos, nudAssentos, btnCriarSala,
                lblLinha, lblFiltro, txtFiltroSala, dgvSalas, btnVerHorasLivres
            });

            GroupBox gbReservas = new GroupBox { Text = "Agendamentos & Reservas", Location = new System.Drawing.Point(450, 15), Size = new System.Drawing.Size(420, 530) };
            Label lblSala = new Label { Text = "Selecionar Sala:", Location = new System.Drawing.Point(15, 25), Width = 100 };
            cbSalasDisponiveis = new ComboBox { Location = new System.Drawing.Point(120, 22), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblInicio = new Label { Text = "Data/Hora Início:", Location = new System.Drawing.Point(15, 55), Width = 100 };
            dtpInicio = new DateTimePicker { Location = new System.Drawing.Point(120, 52), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Width = 150 };
            Label lblFim = new Label { Text = "Data/Hora Fim:", Location = new System.Drawing.Point(15, 85), Width = 100 };
            dtpFim = new DateTimePicker { Location = new System.Drawing.Point(120, 82), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Width = 150 };
            btnAgendar = new Button { Text = "Confirmar Reserva", Location = new System.Drawing.Point(280, 51), Size = new System.Drawing.Size(120, 53) };
            btnAgendar.Click += btnAgendar_Click;
            Label lblLinha2 = new Label { BorderStyle = BorderStyle.Fixed3D, Location = new System.Drawing.Point(15, 120), Size = new System.Drawing.Size(390, 2) };
            Label lblConsulta = new Label { Text = "Filtrar por Data:", Location = new System.Drawing.Point(15, 135), Width = 90 };
            dtpConsultaData = new DateTimePicker { Location = new System.Drawing.Point(110, 132), Format = DateTimePickerFormat.Short, Width = 130 };
            btnConsultarReservas = new Button { Text = "Filtrar Reservas", Location = new System.Drawing.Point(250, 131), Width = 150 };
            btnConsultarReservas.Click += btnConsultarReservas_Click;
            dgvReservas = new DataGridView { Location = new System.Drawing.Point(15, 165), Size = new System.Drawing.Size(385, 350), ReadOnly = true, AutoGenerateColumns = true };

            gbReservas.Controls.AddRange(new Control[] {
                lblSala, cbSalasDisponiveis, lblInicio, dtpInicio, lblFim, dtpFim, btnAgendar,
                lblLinha2, lblConsulta, dtpConsultaData, btnConsultarReservas, dgvReservas
            });

            this.Controls.Add(gbSalas);
            this.Controls.Add(gbReservas);
        }
    }
}