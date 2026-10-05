namespace WindowsFormsAppFruteira
{
    partial class CadastroForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CadastroForm));
            this.lblTitulo = new System.Windows.Forms.Label();
            this.picPrevia = new System.Windows.Forms.PictureBox();
            this.btnEscolherImagem = new System.Windows.Forms.Button();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblQuantidade = new System.Windows.Forms.Label();
            this.txtQuantidade = new System.Windows.Forms.TextBox();
            this.lblTipo = new System.Windows.Forms.Label();
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.lblValor = new System.Windows.Forms.Label();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.lblValidade = new System.Windows.Forms.Label();
            this.dtpValidade = new System.Windows.Forms.DateTimePicker();
            this.btnCadastrar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnEstoque = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPrevia)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(235, 29);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cadastro de Frutas";
            // 
            // picPrevia
            // 
            this.picPrevia.BackColor = System.Drawing.SystemColors.Window;
            this.picPrevia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPrevia.Location = new System.Drawing.Point(25, 66);
            this.picPrevia.Name = "picPrevia";
            this.picPrevia.Size = new System.Drawing.Size(500, 300);
            this.picPrevia.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPrevia.TabIndex = 1;
            this.picPrevia.TabStop = false;
            // 
            // btnEscolherImagem
            // 
            this.btnEscolherImagem.Location = new System.Drawing.Point(25, 380);
            this.btnEscolherImagem.Name = "btnEscolherImagem";
            this.btnEscolherImagem.Size = new System.Drawing.Size(222, 40);
            this.btnEscolherImagem.TabIndex = 2;
            this.btnEscolherImagem.Text = "Escolher imagem";
            this.btnEscolherImagem.UseVisualStyleBackColor = true;
            this.btnEscolherImagem.Click += new System.EventHandler(this.btnEscolherImagem_Click);
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Location = new System.Drawing.Point(575, 66);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(51, 20);
            this.lblNome.TabIndex = 3;
            this.lblNome.Text = "Nome";
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(579, 88);
            this.txtNome.MaxLength = 60;
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(222, 26);
            this.txtNome.TabIndex = 4;
            // 
            // lblQuantidade
            // 
            this.lblQuantidade.AutoSize = true;
            this.lblQuantidade.Location = new System.Drawing.Point(575, 134);
            this.lblQuantidade.Name = "lblQuantidade";
            this.lblQuantidade.Size = new System.Drawing.Size(92, 20);
            this.lblQuantidade.TabIndex = 5;
            this.lblQuantidade.Text = "Quantidade";
            // 
            // txtQuantidade
            // 
            this.txtQuantidade.Location = new System.Drawing.Point(579, 156);
            this.txtQuantidade.MaxLength = 9;
            this.txtQuantidade.Name = "txtQuantidade";
            this.txtQuantidade.Size = new System.Drawing.Size(222, 26);
            this.txtQuantidade.TabIndex = 6;
            // 
            // lblTipo
            // 
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(575, 202);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(39, 20);
            this.lblTipo.TabIndex = 7;
            this.lblTipo.Text = "Tipo";
            // 
            // cmbTipo
            // 
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Location = new System.Drawing.Point(579, 224);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(222, 28);
            this.cmbTipo.TabIndex = 8;
            // 
            // lblValor
            // 
            this.lblValor.AutoSize = true;
            this.lblValor.Location = new System.Drawing.Point(575, 270);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(46, 20);
            this.lblValor.TabIndex = 9;
            this.lblValor.Text = "Valor";
            // 
            // txtValor
            // 
            this.txtValor.Location = new System.Drawing.Point(579, 292);
            this.txtValor.MaxLength = 12;
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(222, 26);
            this.txtValor.TabIndex = 10;
            // 
            // lblValidade
            // 
            this.lblValidade.AutoSize = true;
            this.lblValidade.Location = new System.Drawing.Point(575, 338);
            this.lblValidade.Name = "lblValidade";
            this.lblValidade.Size = new System.Drawing.Size(71, 20);
            this.lblValidade.TabIndex = 11;
            this.lblValidade.Text = "Validade";
            // 
            // dtpValidade
            // 
            this.dtpValidade.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpValidade.Location = new System.Drawing.Point(579, 360);
            this.dtpValidade.Name = "dtpValidade";
            this.dtpValidade.Size = new System.Drawing.Size(222, 26);
            this.dtpValidade.TabIndex = 12;
            // 
            // btnCadastrar
            // 
            this.btnCadastrar.Location = new System.Drawing.Point(579, 436);
            this.btnCadastrar.Name = "btnCadastrar";
            this.btnCadastrar.Size = new System.Drawing.Size(222, 46);
            this.btnCadastrar.TabIndex = 13;
            this.btnCadastrar.Text = "Cadastrar";
            this.btnCadastrar.UseVisualStyleBackColor = true;
            this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(351, 436);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(222, 46);
            this.btnCancelar.TabIndex = 14;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnEstoque
            // 
            this.btnEstoque.Location = new System.Drawing.Point(25, 436);
            this.btnEstoque.Name = "btnEstoque";
            this.btnEstoque.Size = new System.Drawing.Size(222, 46);
            this.btnEstoque.TabIndex = 15;
            this.btnEstoque.Text = "Estoque";
            this.btnEstoque.UseVisualStyleBackColor = true;
            this.btnEstoque.Click += new System.EventHandler(this.btnEstoque_Click);
            // 
            // CadastroForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.LightGreen;
            this.ClientSize = new System.Drawing.Size(831, 503);
            this.Controls.Add(this.btnEstoque);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCadastrar);
            this.Controls.Add(this.dtpValidade);
            this.Controls.Add(this.lblValidade);
            this.Controls.Add(this.txtValor);
            this.Controls.Add(this.lblValor);
            this.Controls.Add(this.cmbTipo);
            this.Controls.Add(this.lblTipo);
            this.Controls.Add(this.txtQuantidade);
            this.Controls.Add(this.lblQuantidade);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.btnEscolherImagem);
            this.Controls.Add(this.picPrevia);
            this.Controls.Add(this.lblTitulo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CadastroForm";
            this.Text = "Fruteira";
            this.Load += new System.EventHandler(this.CadastroForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picPrevia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.PictureBox picPrevia;
        private System.Windows.Forms.Button btnEscolherImagem;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label lblQuantidade;
        private System.Windows.Forms.TextBox txtQuantidade;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cmbTipo;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Label lblValidade;
        private System.Windows.Forms.DateTimePicker dtpValidade;
        private System.Windows.Forms.Button btnCadastrar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnEstoque;
    }
}
