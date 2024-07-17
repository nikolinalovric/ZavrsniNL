namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.btnObrisi = new System.Windows.Forms.Button();
            this.btnAstar = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnDijkstra = new System.Windows.Forms.Button();
            this.btnObrisiRutu = new System.Windows.Forms.Button();
            this.btnGBFS = new System.Windows.Forms.Button();
            this.btnBiDijkstra = new System.Windows.Forms.Button();
            this.btnAStarBezFibonacci = new System.Windows.Forms.Button();
            this.btnDijkstraBezFibonaccija = new System.Windows.Forms.Button();
            this.btnBiAStar = new System.Windows.Forms.Button();
            this.btnSimulacija = new System.Windows.Forms.Button();
            this.txtDonja = new System.Windows.Forms.TextBox();
            this.txtGornja = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(105, 12);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(695, 426);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDoubleClick);
            // 
            // btnObrisi
            // 
            this.btnObrisi.Location = new System.Drawing.Point(12, 279);
            this.btnObrisi.Name = "btnObrisi";
            this.btnObrisi.Size = new System.Drawing.Size(75, 46);
            this.btnObrisi.TabIndex = 1;
            this.btnObrisi.Text = "Obriši markere";
            this.btnObrisi.UseVisualStyleBackColor = true;
            this.btnObrisi.Click += new System.EventHandler(this.btnObrisi_Click);
            // 
            // btnAstar
            // 
            this.btnAstar.Location = new System.Drawing.Point(12, 12);
            this.btnAstar.Name = "btnAstar";
            this.btnAstar.Size = new System.Drawing.Size(75, 23);
            this.btnAstar.TabIndex = 3;
            this.btnAstar.Text = "A* ";
            this.btnAstar.UseVisualStyleBackColor = true;
            this.btnAstar.Click += new System.EventHandler(this.btnAstar_Click);
            // 
            // timer1
            // 
            // 
            // btnDijkstra
            // 
            this.btnDijkstra.Location = new System.Drawing.Point(12, 70);
            this.btnDijkstra.Name = "btnDijkstra";
            this.btnDijkstra.Size = new System.Drawing.Size(75, 23);
            this.btnDijkstra.TabIndex = 4;
            this.btnDijkstra.Text = "Dijkstra";
            this.btnDijkstra.UseVisualStyleBackColor = true;
            this.btnDijkstra.Click += new System.EventHandler(this.btnDijkstra_Click);
            // 
            // btnObrisiRutu
            // 
            this.btnObrisiRutu.Location = new System.Drawing.Point(13, 331);
            this.btnObrisiRutu.Name = "btnObrisiRutu";
            this.btnObrisiRutu.Size = new System.Drawing.Size(75, 23);
            this.btnObrisiRutu.TabIndex = 6;
            this.btnObrisiRutu.Text = "Obriši rutu";
            this.btnObrisiRutu.UseVisualStyleBackColor = true;
            this.btnObrisiRutu.Click += new System.EventHandler(this.btnObrisiRutu_Click);
            // 
            // btnGBFS
            // 
            this.btnGBFS.Location = new System.Drawing.Point(13, 129);
            this.btnGBFS.Name = "btnGBFS";
            this.btnGBFS.Size = new System.Drawing.Size(75, 48);
            this.btnGBFS.TabIndex = 7;
            this.btnGBFS.Text = "Greedy Best-First Search";
            this.btnGBFS.UseVisualStyleBackColor = true;
            this.btnGBFS.Click += new System.EventHandler(this.btnGBFS_Click);
            // 
            // btnBiDijkstra
            // 
            this.btnBiDijkstra.Location = new System.Drawing.Point(13, 183);
            this.btnBiDijkstra.Name = "btnBiDijkstra";
            this.btnBiDijkstra.Size = new System.Drawing.Size(75, 47);
            this.btnBiDijkstra.TabIndex = 8;
            this.btnBiDijkstra.Text = "Bidirectional Dijkstra";
            this.btnBiDijkstra.UseVisualStyleBackColor = true;
            this.btnBiDijkstra.Click += new System.EventHandler(this.btnBiDijkstra_Click);
            // 
            // btnAStarBezFibonacci
            // 
            this.btnAStarBezFibonacci.Location = new System.Drawing.Point(12, 41);
            this.btnAStarBezFibonacci.Name = "btnAStarBezFibonacci";
            this.btnAStarBezFibonacci.Size = new System.Drawing.Size(75, 23);
            this.btnAStarBezFibonacci.TabIndex = 9;
            this.btnAStarBezFibonacci.Text = "A* bez fibonaccija";
            this.btnAStarBezFibonacci.UseVisualStyleBackColor = true;
            this.btnAStarBezFibonacci.Click += new System.EventHandler(this.btnAStarBezFibonacci_Click);
            // 
            // btnDijkstraBezFibonaccija
            // 
            this.btnDijkstraBezFibonaccija.Location = new System.Drawing.Point(13, 100);
            this.btnDijkstraBezFibonaccija.Name = "btnDijkstraBezFibonaccija";
            this.btnDijkstraBezFibonaccija.Size = new System.Drawing.Size(75, 23);
            this.btnDijkstraBezFibonaccija.TabIndex = 10;
            this.btnDijkstraBezFibonaccija.Text = "Dijkstra bez fibonaccija";
            this.btnDijkstraBezFibonaccija.UseVisualStyleBackColor = true;
            this.btnDijkstraBezFibonaccija.Click += new System.EventHandler(this.btnDijkstraBezFibonaccija_Click);
            // 
            // btnBiAStar
            // 
            this.btnBiAStar.Location = new System.Drawing.Point(13, 236);
            this.btnBiAStar.Name = "btnBiAStar";
            this.btnBiAStar.Size = new System.Drawing.Size(75, 37);
            this.btnBiAStar.TabIndex = 11;
            this.btnBiAStar.Text = "Bidirectional A*";
            this.btnBiAStar.UseVisualStyleBackColor = true;
            this.btnBiAStar.Click += new System.EventHandler(this.btnBiAStar_Click);
            // 
            // btnSimulacija
            // 
            this.btnSimulacija.Location = new System.Drawing.Point(13, 360);
            this.btnSimulacija.Name = "btnSimulacija";
            this.btnSimulacija.Size = new System.Drawing.Size(75, 23);
            this.btnSimulacija.TabIndex = 12;
            this.btnSimulacija.Text = "Simulacija";
            this.btnSimulacija.UseVisualStyleBackColor = true;
            this.btnSimulacija.Click += new System.EventHandler(this.btnSimulacija_Click);
            // 
            // txtDonja
            // 
            this.txtDonja.Location = new System.Drawing.Point(13, 388);
            this.txtDonja.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDonja.Name = "txtDonja";
            this.txtDonja.Size = new System.Drawing.Size(73, 20);
            this.txtDonja.TabIndex = 13;
            // 
            // txtGornja
            // 
            this.txtGornja.Location = new System.Drawing.Point(13, 412);
            this.txtGornja.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtGornja.Name = "txtGornja";
            this.txtGornja.Size = new System.Drawing.Size(74, 20);
            this.txtGornja.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 449);
            this.Controls.Add(this.txtGornja);
            this.Controls.Add(this.txtDonja);
            this.Controls.Add(this.btnSimulacija);
            this.Controls.Add(this.btnBiAStar);
            this.Controls.Add(this.btnDijkstraBezFibonaccija);
            this.Controls.Add(this.btnAStarBezFibonacci);
            this.Controls.Add(this.btnBiDijkstra);
            this.Controls.Add(this.btnGBFS);
            this.Controls.Add(this.btnObrisiRutu);
            this.Controls.Add(this.btnDijkstra);
            this.Controls.Add(this.btnAstar);
            this.Controls.Add(this.btnObrisi);
            this.Controls.Add(this.gMapControl1);
            this.Name = "Form1";
            this.Text = "zavrsni";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Button btnObrisi;
        private System.Windows.Forms.Button btnAstar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnDijkstra;
        private System.Windows.Forms.Button btnObrisiRutu;
        private System.Windows.Forms.Button btnGBFS;
        private System.Windows.Forms.Button btnBiDijkstra;
        private System.Windows.Forms.Button btnAStarBezFibonacci;
        private System.Windows.Forms.Button btnDijkstraBezFibonaccija;
        private System.Windows.Forms.Button btnBiAStar;
        private System.Windows.Forms.Button btnSimulacija;
        private System.Windows.Forms.TextBox txtDonja;
        private System.Windows.Forms.TextBox txtGornja;
    }
}

