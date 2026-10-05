namespace WindowsFormsAppFruteira
{
    partial class EstoqueForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EstoqueForm));
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpPesquisar = new System.Windows.Forms.GroupBox();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.btnPesquisar = new System.Windows.Forms.Button();
            this.lstEstoque = new System.Windows.Forms.ListView();
            this.colFruta = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colQuantidade = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValidade = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnVoltar = new System.Windows.Forms.Button();
            this.grpPesquisar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(226, 29);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Estoque de Frutas";
            // 
            // grpPesquisar
            // 
            this.grpPesquisar.Controls.Add(this.lblNome);
            this.grpPesquisar.Controls.Add(this.lblCategoria);
            this.grpPesquisar.Controls.Add(this.txtNome);
            this.grpPesquisar.Controls.Add(this.cmbCategoria);
            this.grpPesquisar.Location = new System.Drawing.Point(24, 54);
            this.grpPesquisar.Name = "grpPesquisar";
            this.grpPesquisar.Size = new System.Drawing.Size(484, 97);
            this.grpPesquisar.TabIndex = 1;
            this.grpPesquisar.TabStop = false;
            this.grpPesquisar.Text = "Pesquisar";
            // 
            // lblCategoria
            // 
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Location = new System.Drawing.Point(6, 20);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new System.Drawing.Size(78, 20);
            this.lblCategoria.TabIndex = 0;
            this.lblCategoria.Text = "Categoria";
            // 
            // cmbCategoria
            // 
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.FormattingEnabled = true;
            this.cmbCategoria.Location = new System.Drawing.Point(10, 43);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(203, 28);
            this.cmbCategoria.TabIndex = 1;
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Location = new System.Drawing.Point(252, 20);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(51, 20);
            this.lblNome.TabIndex = 2;
            this.lblNome.Text = "Nome";
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(256, 43);
            this.txtNome.MaxLength = 60;
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(203, 26);
            this.txtNome.TabIndex = 3;
            // 
            // btnPesquisar
            // 
            this.btnPesquisar.Location = new System.Drawing.Point(602, 93);
            this.btnPesquisar.Name = "btnPesquisar";
            this.btnPesquisar.Size = new System.Drawing.Size(177, 43);
            this.btnPesquisar.TabIndex = 2;
            this.btnPesquisar.Text = "Pesquisar";
            this.btnPesquisar.UseVisualStyleBackColor = true;
            this.btnPesquisar.Click += new System.EventHandler(this.btnPesquisar_Click);
            // 
            // lstEstoque
            // 
            this.lstEstoque.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFruta,
            this.colQuantidade,
            this.colValidade,
            this.colValor,
            this.colTipo});
            this.lstEstoque.FullRowSelect = true;
            this.lstEstoque.GridLines = true;
            this.lstEstoque.HideSelection = false;
            this.lstEstoque.Location = new System.Drawing.Point(17, 157);
            this.lstEstoque.Name = "lstEstoque";
            this.lstEstoque.Size = new System.Drawing.Size(762, 223);
            this.lstEstoque.TabIndex = 3;
            this.lstEstoque.UseCompatibleStateImageBehavior = false;
            this.lstEstoque.View = System.Windows.Forms.View.Details;
            // 
            // colFruta
            // 
            this.colFruta.Text = "Fruta";
            this.colFruta.Width = 220;
            // 
            // colQuantidade
            // 
            this.colQuantidade.Text = "Quantidade";
            this.colQuantidade.Width = 110;
            // 
            // colValidade
            // 
            this.colValidade.Text = "Validade";
            this.colValidade.Width = 120;
            // 
            // colValor
            // 
            this.colValor.Text = "Valor";
            this.colValor.Width = 110;
            // 
            // colTipo
            // 
            this.colTipo.Text = "Tipo";
            this.colTipo.Width = 180;
            // 
            // btnVoltar
            // 
            this.btnVoltar.BackColor = System.Drawing.Color.Blue;
            this.btnVoltar.ForeColor = System.Drawing.Color.White;
            this.btnVoltar.Location = new System.Drawing.Point(602, 386);
            this.btnVoltar.Name = "btnVoltar";
            this.btnVoltar.Size = new System.Drawing.Size(177, 43);
            this.btnVoltar.TabIndex = 4;
            this.btnVoltar.Text = "Voltar";
            this.btnVoltar.UseVisualStyleBackColor = false;
            this.btnVoltar.Click += new System.EventHandler(this.btnVoltar_Click);
            // 
            // EstoqueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGreen;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnVoltar);
            this.Controls.Add(this.lstEstoque);
            this.Controls.Add(this.btnPesquisar);
            this.Controls.Add(this.grpPesquisar);
            this.Controls.Add(this.lblTitulo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EstoqueForm";
            this.Text = "Estoque";
            this.Load += new System.EventHandler(this.EstoqueForm_Load);
            this.grpPesquisar.ResumeLayout(false);
            this.grpPesquisar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpPesquisar;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Button btnPesquisar;
        private System.Windows.Forms.ListView lstEstoque;
        private System.Windows.Forms.ColumnHeader colFruta;
        private System.Windows.Forms.ColumnHeader colQuantidade;
        private System.Windows.Forms.ColumnHeader colValidade;
        private System.Windows.Forms.ColumnHeader colValor;
        private System.Windows.Forms.ColumnHeader colTipo;
        private System.Windows.Forms.Button btnVoltar;
    }
}
