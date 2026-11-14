using System.Globalization;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Vendinha.WinForms.Data;
using Vendinha.WinForms.Models;
using Vendinha.WinForms.Services;

namespace Vendinha.WinForms.Forms;

public sealed class MainForm : Form
{
    private static readonly Color Navy = Color.FromArgb(24, 37, 58);
    private static readonly Color Blue = Color.FromArgb(48, 111, 219);
    private static readonly Color Background = Color.FromArgb(244, 247, 251);
    private static readonly Color TextColor = Color.FromArgb(37, 48, 65);
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    private readonly TextBox _searchClient = new();
    private readonly TextBox _name = new();
    private readonly TextBox _cpf = new();
    private readonly TextBox _email = new();
    private readonly DateTimePicker _birthDate = new();
    private readonly DataGridView _clientsGrid = new();
    private readonly ComboBox _debtClient = new();
    private readonly TextBox _debtValue = new();
    private readonly TextBox _paymentValue = new();
    private readonly DataGridView _debtsGrid = new();
    private readonly Label _clientCountValue = new();
    private readonly Label _openDebtCountValue = new();
    private readonly Label _openDebtTotalValue = new();

    private int? _selectedClientId;
    private int? _selectedDebtId;

    public MainForm()
    {
        Text = "Vendinha | Clientes e Dívidas";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1050, 700);
        Size = new Size(1280, 850);
        BackColor = Background;
        Font = new Font("Segoe UI", 9.5F);

        BuildInterface();
        Load += (_, _) => RefreshEverything();
    }

    private void BuildInterface()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Background,
            Padding = new Padding(18)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var header = new Panel { Dock = DockStyle.Fill, BackColor = Navy, Padding = new Padding(20, 12, 20, 10) };
        var title = new Label
        {
            Text = "VENDINHA",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(18, 8)
        };
        var subtitle = new Label
        {
            Text = "Gerenciamento de clientes e dívidas",
            ForeColor = Color.FromArgb(211, 222, 238),
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(21, 48)
        };
        header.Controls.Add(title);
        header.Controls.Add(subtitle);
        root.Controls.Add(header, 0, 0);

        var stats = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            Padding = new Padding(0, 10, 0, 5),
            WrapContents = false
        };
        stats.Controls.Add(CreateStatCard("CLIENTES CADASTRADOS", _clientCountValue, "Pessoas registradas"));
        stats.Controls.Add(CreateStatCard("DÍVIDAS EM ABERTO", _openDebtCountValue, "Pendências atuais"));
        stats.Controls.Add(CreateStatCard("TOTAL A RECEBER", _openDebtTotalValue, "   Saldo em aberto"));
        root.Controls.Add(stats, 0, 1);

        var tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        var clientsTab = new TabPage("Clientes") { BackColor = Background, Padding = new Padding(10) };
        var debtsTab = new TabPage("Dívidas e pagamentos") { BackColor = Background, Padding = new Padding(10) };
        tabs.TabPages.Add(clientsTab);
        tabs.TabPages.Add(debtsTab);
        root.Controls.Add(tabs, 0, 2);

        BuildClientsTab(clientsTab);
        BuildDebtsTab(debtsTab);
    }

    private Control CreateStatCard(string heading, Label valueLabel, string caption)
    {
        var card = new Panel
        {
            Width = 300,
            Height = 82,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 14, 0),
            Padding = new Padding(14)
        };
        var headingLabel = new Label
        {
            Text = heading,
            ForeColor = Color.FromArgb(105, 119, 139),
            Font = new Font("Segoe UI", 8.2F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(14, 8)
        };
        valueLabel.Text = "0";
        valueLabel.ForeColor = Navy;
        valueLabel.Font = new Font("Segoe UI", 19, FontStyle.Bold);
        valueLabel.AutoSize = true;
        valueLabel.Location = new Point(14, 27);
        var captionLabel = new Label
        {
            Text = caption,
            ForeColor = Color.FromArgb(130, 142, 158),
            Font = new Font("Segoe UI", 8),
            AutoSize = true,
            Location = new Point(155, 39)
        };
        card.Controls.Add(headingLabel);
        card.Controls.Add(valueLabel);
        card.Controls.Add(captionLabel);
        return card;
    }

    private void BuildClientsTab(TabPage tab)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Background };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tab.Controls.Add(layout);

        var searchBar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 4, 0, 3) };
        searchBar.Controls.Add(new Label { Text = "Pesquisar cliente:", AutoSize = true, Margin = new Padding(0, 10, 8, 0), ForeColor = TextColor });
        _searchClient.Width = 330;
        _searchClient.Height = 30;
        _searchClient.PlaceholderText = "Nome ou CPF";
        _searchClient.Margin = new Padding(0, 2, 10, 0);
        _searchClient.TextChanged += (_, _) => LoadClients();
        searchBar.Controls.Add(_searchClient);
        searchBar.Controls.Add(CreateButton("Atualizar lista", ButtonKind.Secondary, (_, _) => LoadClients()));
        layout.Controls.Add(searchBar, 0, 0);

        var editor = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        var fields = new TableLayoutPanel { Dock = DockStyle.Top, Height = 72, ColumnCount = 4, RowCount = 1 };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        ConfigureTextBox(_name, "Nome completo");
        ConfigureTextBox(_cpf, "CPF (11 números)");
        ConfigureTextBox(_email, "E-mail (opcional)");
        _birthDate.Format = DateTimePickerFormat.Short;
        _birthDate.MaxDate = DateTime.Today;
        _birthDate.MinDate = new DateTime(1900, 1, 1);
        fields.Controls.Add(CreateField("Nome completo", _name), 0, 0);
        fields.Controls.Add(CreateField("CPF", _cpf), 1, 0);
        fields.Controls.Add(CreateField("Data de nascimento", _birthDate), 2, 0);
        fields.Controls.Add(CreateField("E-mail (opcional)", _email), 3, 0);
        editor.Controls.Add(fields);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
        buttons.Controls.Add(CreateButton("Salvar cliente", ButtonKind.Primary, SaveClient));
        buttons.Controls.Add(CreateButton("Limpar campos", ButtonKind.Secondary, (_, _) => ClearClientFields()));
        buttons.Controls.Add(CreateButton("Excluir selecionado", ButtonKind.Danger, DeleteClient));
        editor.Controls.Add(buttons);
        layout.Controls.Add(editor, 0, 1);

        ConfigureGrid(_clientsGrid);
        layout.Controls.Add(_clientsGrid, 0, 2);
        _clientsGrid.CellClick += ClientGrid_CellClick;
    }

    private void BuildDebtsTab(TabPage tab)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Background };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tab.Controls.Add(layout);

        var actions = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        var row1 = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 56, WrapContents = false };
        row1.Controls.Add(CreateInlineLabel("Cliente"));
        _debtClient.DropDownStyle = ComboBoxStyle.DropDownList;
        _debtClient.Width = 300;
        _debtClient.Margin = new Padding(0, 5, 15, 0);
        _debtClient.SelectedIndexChanged += (_, _) => LoadDebts();
        row1.Controls.Add(_debtClient);
        row1.Controls.Add(CreateInlineLabel("Valor da nova dívida (R$)"));
        ConfigureTextBox(_debtValue, "Ex.: 150,00");
        _debtValue.Width = 145;
        row1.Controls.Add(_debtValue);
        row1.Controls.Add(CreateButton("Cadastrar dívida", ButtonKind.Primary, AddDebt));
        actions.Controls.Add(row1);

        var row2 = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, WrapContents = false };
        row2.Controls.Add(CreateInlineLabel("Pagamento parcial (R$)"));
        ConfigureTextBox(_paymentValue, "Ex.: 30,00");
        _paymentValue.Width = 145;
        row2.Controls.Add(_paymentValue);
        row2.Controls.Add(CreateButton("Registrar pagamento", ButtonKind.Primary, RegisterPartialPayment));
        row2.Controls.Add(CreateButton("Quitar dívida selecionada", ButtonKind.Secondary, PayInFull));
        row2.Controls.Add(CreateButton("Atualizar", ButtonKind.Secondary, (_, _) => LoadDebts()));
        actions.Controls.Add(row2);
        layout.Controls.Add(actions, 0, 0);

        ConfigureGrid(_debtsGrid);
        _debtsGrid.CellClick += DebtGrid_CellClick;
        layout.Controls.Add(_debtsGrid, 0, 1);
    }

    private static void ConfigureTextBox(TextBox textBox, string placeholder)
    {
        textBox.PlaceholderText = placeholder;
        textBox.Dock = DockStyle.Fill;
        textBox.Margin = new Padding(3, 4, 8, 3);
        textBox.Font = new Font("Segoe UI", 9.5F);
    }

    private static Control CreateField(string label, Control control)
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(3, 0, 8, 0) };
        var title = new Label { Text = label, Dock = DockStyle.Top, Height = 23, ForeColor = Color.FromArgb(91, 105, 124), Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) };
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 1, 0, 0);
        panel.Controls.Add(control);
        panel.Controls.Add(title);
        return panel;
    }

    private static Label CreateInlineLabel(string text)
    {
        return new Label { Text = text, AutoSize = true, ForeColor = TextColor, Margin = new Padding(3, 12, 8, 0), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
    }

    private enum ButtonKind { Primary, Secondary, Danger }

    private static Button CreateButton(string text, ButtonKind kind, EventHandler click)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Height = 34,
            MinimumSize = new Size(105, 34),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
            ForeColor = kind == ButtonKind.Secondary ? Navy : Color.White,
            BackColor = kind switch
            {
                ButtonKind.Primary => Blue,
                ButtonKind.Danger => Color.FromArgb(190, 62, 72),
                _ => Color.FromArgb(231, 237, 245)
            },
            Margin = new Padding(0, 3, 9, 0),
            Padding = new Padding(10, 0, 10, 0),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += click;
        return button;
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Navy;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        grid.ColumnHeadersHeight = 38;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        grid.DefaultCellStyle.ForeColor = TextColor;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 233, 252);
        grid.DefaultCellStyle.SelectionForeColor = TextColor;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
        grid.RowTemplate.Height = 32;
    }

    private void RefreshEverything()
    {
        LoadClients();
        LoadDebtClients();
        RefreshSummary();
    }

    private void LoadClients()
    {
        try
        {
            using var db = new VendinhaDbContext();
            string search = _searchClient.Text.Trim();
            var clientsQuery = db.Clientes.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string digits = CpfValidator.OnlyDigits(search);
                clientsQuery = digits.Length > 0
                    ? clientsQuery.Where(c => c.NomeCompleto.Contains(search) || c.CPF.Contains(digits))
                    : clientsQuery.Where(c => c.NomeCompleto.Contains(search));
            }

            var clients = clientsQuery.OrderBy(c => c.NomeCompleto).ToList();
            // Calcula os totais em memória para manter compatibilidade com as limitações
            // de agregação decimal do provedor SQLite.
            var openDebts = db.Dividas.AsNoTracking()
                .Where(d => !d.Paga)
                .Select(d => new { d.ClienteId, d.Valor, d.ValorPago })
                .ToList();
            var totals = openDebts
                .GroupBy(d => d.ClienteId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Valor - d.ValorPago));

            _clientsGrid.DataSource = clients.Select(c => new ClienteGridItem
            {
                Id = c.Id,
                Nome = c.NomeCompleto,
                CPF = c.CPF,
                Nascimento = c.DataNascimento.ToString("dd/MM/yyyy"),
                Idade = c.Idade,
                Email = c.Email ?? "—",
                TotalEmAberto = totals.TryGetValue(c.Id, out decimal total) ? total.ToString("C", PtBr) : 0m.ToString("C", PtBr)
            }).ToList();
            if (_clientsGrid.Columns.Contains(nameof(ClienteGridItem.Id)))
                _clientsGrid.Columns[nameof(ClienteGridItem.Id)]!.Visible = false;
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível carregar os clientes.", ex);
        }
    }

    private void LoadDebtClients()
    {
        int? previousId = (_debtClient.SelectedItem as ClienteOption)?.Id;
        using var db = new VendinhaDbContext();
        var options = db.Clientes.AsNoTracking()
            .OrderBy(c => c.NomeCompleto)
            .Select(c => new ClienteOption { Id = c.Id, Nome = c.NomeCompleto, CPF = c.CPF })
            .ToList();
        _debtClient.DataSource = options;
        _debtClient.DisplayMember = nameof(ClienteOption.Display);
        if (previousId.HasValue)
            _debtClient.SelectedItem = options.FirstOrDefault(c => c.Id == previousId.Value);
        if (_debtClient.SelectedIndex < 0 && options.Count > 0)
            _debtClient.SelectedIndex = 0;
        LoadDebts();
    }

    private void LoadDebts()
    {
        if (_debtClient.SelectedItem is not ClienteOption selected)
        {
            _debtsGrid.DataSource = null;
            _selectedDebtId = null;
            return;
        }

        try
        {
            using var db = new VendinhaDbContext();
            var debts = db.Dividas.AsNoTracking()
                .Where(d => d.ClienteId == selected.Id)
                .OrderByDescending(d => d.DataCriacao)
                .ToList();

            _debtsGrid.DataSource = debts.Select(d => new DividaGridItem
            {
                Id = d.Id,
                Valor = d.Valor.ToString("C", PtBr),
                ValorPago = d.ValorPago.ToString("C", PtBr),
                ValorRestante = d.ValorRestante.ToString("C", PtBr),
                Situacao = d.Paga ? "Quitada" : "Em aberto",
                DataCriacao = d.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                DataPagamento = d.DataPagamento?.ToString("dd/MM/yyyy HH:mm") ?? "—"
            }).ToList();
            if (_debtsGrid.Columns.Contains(nameof(DividaGridItem.Id)))
                _debtsGrid.Columns[nameof(DividaGridItem.Id)]!.Visible = false;
            _selectedDebtId = null;
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível carregar as dívidas.", ex);
        }
    }

    private void RefreshSummary()
    {
        using var db = new VendinhaDbContext();
        int clientCount = db.Clientes.Count();
        var openDebts = db.Dividas.Where(d => !d.Paga).ToList();
        _clientCountValue.Text = clientCount.ToString();
        _openDebtCountValue.Text = openDebts.Count.ToString();
        _openDebtTotalValue.Text = openDebts.Sum(d => d.ValorRestante).ToString("C", PtBr);
    }

    private void SaveClient(object? sender, EventArgs e)
    {
        string name = _name.Text.Trim();
        string cpf = CpfValidator.OnlyDigits(_cpf.Text);
        string email = _email.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowWarning("Informe o nome completo do cliente.");
            _name.Focus();
            return;
        }
        if (!CpfValidator.IsValid(cpf))
        {
            ShowWarning("Informe um CPF válido com 11 dígitos.");
            _cpf.Focus();
            return;
        }
        if (_birthDate.Value.Date > DateTime.Today)
        {
            ShowWarning("A data de nascimento não pode estar no futuro.");
            return;
        }
        if (!string.IsNullOrWhiteSpace(email) && !MailAddress.TryCreate(email, out _))
        {
            ShowWarning("Informe um e-mail válido ou deixe o campo vazio.");
            _email.Focus();
            return;
        }

        try
        {
            using var db = new VendinhaDbContext();
            bool duplicateCpf = db.Clientes.Any(c => c.CPF == cpf && c.Id != (_selectedClientId ?? 0));
            if (duplicateCpf)
            {
                ShowWarning("Já existe um cliente cadastrado com este CPF.");
                return;
            }

            Cliente client;
            if (_selectedClientId.HasValue)
            {
                client = db.Clientes.FirstOrDefault(c => c.Id == _selectedClientId.Value)
                    ?? throw new InvalidOperationException("O cliente selecionado não foi encontrado.");
            }
            else
            {
                client = new Cliente();
                db.Clientes.Add(client);
            }

            client.NomeCompleto = name;
            client.CPF = cpf;
            client.DataNascimento = _birthDate.Value.Date;
            client.Email = string.IsNullOrWhiteSpace(email) ? null : email;
            db.SaveChanges();

            MessageBox.Show("Cliente salvo com sucesso!", "Vendinha", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearClientFields();
            LoadClients();
            LoadDebtClients();
            RefreshSummary();
        }
        catch (DbUpdateException ex)
        {
            ShowError("Não foi possível salvar o cliente. Verifique se o CPF já está cadastrado.", ex);
        }
        catch (Exception ex)
        {
            ShowError("Ocorreu um erro ao salvar o cliente.", ex);
        }
    }

    private void DeleteClient(object? sender, EventArgs e)
    {
        if (!_selectedClientId.HasValue)
        {
            ShowWarning("Selecione um cliente na tabela primeiro.");
            return;
        }

        try
        {
            using var db = new VendinhaDbContext();
            var client = db.Clientes.FirstOrDefault(c => c.Id == _selectedClientId.Value);
            if (client is null)
            {
                ShowWarning("Cliente não encontrado.");
                return;
            }

            if (db.Dividas.Any(d => d.ClienteId == client.Id))
            {
                ShowWarning("Este cliente possui dívidas registradas e não pode ser excluído.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Deseja realmente excluir {client.NomeCompleto}?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            db.Clientes.Remove(client);
            db.SaveChanges();
            ClearClientFields();
            LoadClients();
            LoadDebtClients();
            RefreshSummary();
            MessageBox.Show("Cliente excluído com sucesso.", "Vendinha", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível excluir o cliente.", ex);
        }
    }

    private void ClientGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || _clientsGrid.Rows[e.RowIndex].DataBoundItem is not ClienteGridItem item)
            return;

        try
        {
            using var db = new VendinhaDbContext();
            var client = db.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == item.Id);
            if (client is null) return;

            _selectedClientId = client.Id;
            _name.Text = client.NomeCompleto;
            _cpf.Text = client.CPF;
            _email.Text = client.Email ?? string.Empty;
            _birthDate.Value = client.DataNascimento < _birthDate.MinDate || client.DataNascimento > _birthDate.MaxDate
                ? DateTime.Today
                : client.DataNascimento;

            var option = _debtClient.Items.Cast<ClienteOption>().FirstOrDefault(c => c.Id == client.Id);
            if (option is not null) _debtClient.SelectedItem = option;
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível selecionar o cliente.", ex);
        }
    }

    private void ClearClientFields()
    {
        _selectedClientId = null;
        _name.Clear();
        _cpf.Clear();
        _email.Clear();
        _birthDate.Value = DateTime.Today;
        _clientsGrid.ClearSelection();
        _name.Focus();
    }

    private void AddDebt(object? sender, EventArgs e)
    {
        if (_debtClient.SelectedItem is not ClienteOption client)
        {
            ShowWarning("Cadastre ou selecione um cliente primeiro.");
            return;
        }
        if (!TryReadMoney(_debtValue.Text, out decimal value) || value <= 0)
        {
            ShowWarning("Informe um valor de dívida maior que zero. Exemplo: 150,00.");
            _debtValue.Focus();
            return;
        }

        try
        {
            using var db = new VendinhaDbContext();
            var exists = db.Clientes.Any(c => c.Id == client.Id);
            if (!exists)
            {
                ShowWarning("O cliente selecionado não existe mais.");
                LoadDebtClients();
                return;
            }

            bool hasOpenDebt = db.Dividas.Any(d => d.ClienteId == client.Id && !d.Paga);
            if (hasOpenDebt)
            {
                ShowWarning("Este cliente já possui uma dívida em aberto. Quite a dívida atual antes de cadastrar outra.");
                return;
            }

            db.Dividas.Add(new Divida
            {
                ClienteId = client.Id,
                Valor = decimal.Round(value, 2),
                ValorPago = 0m,
                Paga = false,
                DataCriacao = DateTime.Now
            });
            db.SaveChanges();
            _debtValue.Clear();
            LoadDebts();
            LoadClients();
            RefreshSummary();
            MessageBox.Show("Dívida cadastrada com sucesso!", "Vendinha", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível cadastrar a dívida.", ex);
        }
    }

    private void RegisterPartialPayment(object? sender, EventArgs e)
    {
        if (!_selectedDebtId.HasValue)
        {
            ShowWarning("Selecione uma dívida na tabela primeiro.");
            return;
        }
        if (!TryReadMoney(_paymentValue.Text, out decimal amount) || amount <= 0)
        {
            ShowWarning("Informe um valor de pagamento maior que zero. Exemplo: 30,00.");
            _paymentValue.Focus();
            return;
        }

        try
        {
            using var db = new VendinhaDbContext();
            var debt = db.Dividas.FirstOrDefault(d => d.Id == _selectedDebtId.Value);
            if (debt is null)
            {
                ShowWarning("Dívida não encontrada. Atualize a lista.");
                LoadDebts();
                return;
            }
            if (debt.Paga)
            {
                ShowWarning("Esta dívida já foi quitada.");
                return;
            }

            decimal remaining = debt.Valor - debt.ValorPago;
            if (amount > remaining)
            {
                ShowWarning($"O pagamento não pode ser maior que o saldo restante ({remaining.ToString("C", PtBr)}).");
                return;
            }

            debt.ValorPago += decimal.Round(amount, 2);
            if (debt.ValorPago >= debt.Valor)
            {
                debt.ValorPago = debt.Valor;
                debt.Paga = true;
                debt.DataPagamento = DateTime.Now;
            }
            db.SaveChanges();

            _paymentValue.Clear();
            LoadDebts();
            LoadClients();
            RefreshSummary();
            MessageBox.Show(
                debt.Paga ? "Pagamento registrado. A dívida foi quitada!" : "Pagamento parcial registrado com sucesso!",
                "Vendinha",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível registrar o pagamento.", ex);
        }
    }

    private void PayInFull(object? sender, EventArgs e)
    {
        if (!_selectedDebtId.HasValue)
        {
            ShowWarning("Selecione uma dívida na tabela primeiro.");
            return;
        }

        try
        {
            using var db = new VendinhaDbContext();
            var debt = db.Dividas.FirstOrDefault(d => d.Id == _selectedDebtId.Value);
            if (debt is null)
            {
                ShowWarning("Dívida não encontrada. Atualize a lista.");
                LoadDebts();
                return;
            }
            if (debt.Paga)
            {
                ShowWarning("Esta dívida já está quitada.");
                return;
            }

            decimal remaining = debt.Valor - debt.ValorPago;
            var confirm = MessageBox.Show(
                $"Confirmar quitação do saldo de {remaining.ToString("C", PtBr)}?",
                "Quitar dívida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            debt.ValorPago = debt.Valor;
            debt.Paga = true;
            debt.DataPagamento = DateTime.Now;
            db.SaveChanges();

            LoadDebts();
            LoadClients();
            RefreshSummary();
            MessageBox.Show("Dívida quitada com sucesso!", "Vendinha", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Não foi possível quitar a dívida.", ex);
        }
    }

    private void DebtGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || _debtsGrid.Rows[e.RowIndex].DataBoundItem is not DividaGridItem item)
            return;
        _selectedDebtId = item.Id;
    }

    private static bool TryReadMoney(string text, out decimal value)
    {
        return decimal.TryParse(text, NumberStyles.Currency | NumberStyles.Number, PtBr, out value)
            || decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static void ShowError(string message, Exception ex)
    {
        MessageBox.Show($"{message}\n\nDetalhes: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private sealed class ClienteOption
    {
        public int Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public string CPF { get; init; } = string.Empty;
        public string Display => $"{Nome} — {CPF}";
    }

    private sealed class ClienteGridItem
    {
        public int Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public string CPF { get; init; } = string.Empty;
        public string Nascimento { get; init; } = string.Empty;
        public int Idade { get; init; }
        public string Email { get; init; } = string.Empty;
        public string TotalEmAberto { get; init; } = string.Empty;
    }

    private sealed class DividaGridItem
    {
        public int Id { get; init; }
        public string Valor { get; init; } = string.Empty;
        public string ValorPago { get; init; } = string.Empty;
        public string ValorRestante { get; init; } = string.Empty;
        public string Situacao { get; init; } = string.Empty;
        public string DataCriacao { get; init; } = string.Empty;
        public string DataPagamento { get; init; } = string.Empty;
    }
}
