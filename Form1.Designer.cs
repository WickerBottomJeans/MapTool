namespace MapTool {
    partial class mainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnCreateNewLayer = new System.Windows.Forms.Button();
            this.btnSaveLayer = new System.Windows.Forms.Button();
            this.btnLoadBG = new System.Windows.Forms.Button();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.clbLayers = new System.Windows.Forms.CheckedListBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBrush = new System.Windows.Forms.Button();
            this.btnEraser = new System.Windows.Forms.Button();
            this.btnDrawRect = new System.Windows.Forms.Button();
            this.btnDrawLine = new System.Windows.Forms.Button();
            this.btnBucket = new System.Windows.Forms.Button();
            this.btnRotate = new System.Windows.Forms.Button();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnReloadBitmap = new System.Windows.Forms.Button();
            this.btnXOR = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.nudBrushSize = new System.Windows.Forms.NumericUpDown();
            this.panelMap = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnLoadFolder = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.btnFLP.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrushSize)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.AntiqueWhite;
            this.flowLayoutPanel1.Controls.Add(this.btnLoadFile);
            this.flowLayoutPanel1.Controls.Add(this.btnLoadFolder);
            this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(305, 838);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.AutoSize = true;
            this.btnLoadFile.Location = new System.Drawing.Point(3, 3);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(299, 30);
            this.btnLoadFile.TabIndex = 0;
            this.btnLoadFile.Text = "Load file";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnFLP, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.clbLayers, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 75);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(299, 284);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btnFLP
            // 
            this.btnFLP.AutoSize = true;
            this.btnFLP.Controls.Add(this.btnUp);
            this.btnFLP.Controls.Add(this.btnDown);
            this.btnFLP.Controls.Add(this.btnCreateNewLayer);
            this.btnFLP.Controls.Add(this.btnSaveLayer);
            this.btnFLP.Controls.Add(this.btnLoadBG);
            this.btnFLP.Controls.Add(this.btnRemoveLayer);
            this.btnFLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFLP.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.btnFLP.Location = new System.Drawing.Point(169, 3);
            this.btnFLP.Name = "btnFLP";
            this.btnFLP.Size = new System.Drawing.Size(127, 278);
            this.btnFLP.TabIndex = 0;
            // 
            // btnUp
            // 
            this.btnUp.AutoSize = true;
            this.btnUp.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUp.Location = new System.Drawing.Point(3, 3);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(121, 30);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.AutoSize = true;
            this.btnDown.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDown.Location = new System.Drawing.Point(3, 39);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(121, 30);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnCreateNewLayer
            // 
            this.btnCreateNewLayer.AutoSize = true;
            this.btnCreateNewLayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCreateNewLayer.Location = new System.Drawing.Point(3, 75);
            this.btnCreateNewLayer.Name = "btnCreateNewLayer";
            this.btnCreateNewLayer.Size = new System.Drawing.Size(121, 30);
            this.btnCreateNewLayer.TabIndex = 2;
            this.btnCreateNewLayer.Text = "New layer";
            this.btnCreateNewLayer.UseVisualStyleBackColor = true;
            this.btnCreateNewLayer.Click += new System.EventHandler(this.btnCreateNewLayer_Click);
            // 
            // btnSaveLayer
            // 
            this.btnSaveLayer.AutoSize = true;
            this.btnSaveLayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSaveLayer.Location = new System.Drawing.Point(3, 111);
            this.btnSaveLayer.Name = "btnSaveLayer";
            this.btnSaveLayer.Size = new System.Drawing.Size(121, 30);
            this.btnSaveLayer.TabIndex = 1;
            this.btnSaveLayer.Text = "Save Layer";
            this.btnSaveLayer.UseVisualStyleBackColor = true;
            this.btnSaveLayer.Click += new System.EventHandler(this.btnSaveLayer_Click);
            // 
            // btnLoadBG
            // 
            this.btnLoadBG.AutoSize = true;
            this.btnLoadBG.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLoadBG.Location = new System.Drawing.Point(3, 147);
            this.btnLoadBG.Name = "btnLoadBG";
            this.btnLoadBG.Size = new System.Drawing.Size(121, 30);
            this.btnLoadBG.TabIndex = 4;
            this.btnLoadBG.Text = "Add BG";
            this.btnLoadBG.UseVisualStyleBackColor = true;
            this.btnLoadBG.Click += new System.EventHandler(this.btnLoadBG_Click);
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.AutoSize = true;
            this.btnRemoveLayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRemoveLayer.Location = new System.Drawing.Point(3, 183);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(121, 30);
            this.btnRemoveLayer.TabIndex = 5;
            this.btnRemoveLayer.Text = "Remove Layer";
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // clbLayers
            // 
            this.clbLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbLayers.FormattingEnabled = true;
            this.clbLayers.Location = new System.Drawing.Point(3, 3);
            this.clbLayers.Name = "clbLayers";
            this.clbLayers.Size = new System.Drawing.Size(160, 278);
            this.clbLayers.TabIndex = 1;
            this.clbLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbLayers_ItemCheck);
            this.clbLayers.SelectedIndexChanged += new System.EventHandler(this.clbLayers_SelectedIndexChanged);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.btnBrush);
            this.flowLayoutPanel2.Controls.Add(this.btnEraser);
            this.flowLayoutPanel2.Controls.Add(this.btnDrawRect);
            this.flowLayoutPanel2.Controls.Add(this.btnDrawLine);
            this.flowLayoutPanel2.Controls.Add(this.btnBucket);
            this.flowLayoutPanel2.Controls.Add(this.btnRotate);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel4);
            this.flowLayoutPanel2.Controls.Add(this.btnReloadBitmap);
            this.flowLayoutPanel2.Controls.Add(this.btnXOR);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel3);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel5);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 365);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(231, 318);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // btnBrush
            // 
            this.btnBrush.AutoSize = true;
            this.btnBrush.Image = ((System.Drawing.Image)(resources.GetObject("btnBrush.Image")));
            this.btnBrush.Location = new System.Drawing.Point(3, 3);
            this.btnBrush.Name = "btnBrush";
            this.btnBrush.Size = new System.Drawing.Size(69, 65);
            this.btnBrush.TabIndex = 0;
            this.btnBrush.Text = "Brush";
            this.btnBrush.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnBrush.UseVisualStyleBackColor = true;
            this.btnBrush.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // btnEraser
            // 
            this.btnEraser.AutoSize = true;
            this.btnEraser.Image = ((System.Drawing.Image)(resources.GetObject("btnEraser.Image")));
            this.btnEraser.Location = new System.Drawing.Point(78, 3);
            this.btnEraser.Name = "btnEraser";
            this.btnEraser.Size = new System.Drawing.Size(69, 65);
            this.btnEraser.TabIndex = 1;
            this.btnEraser.Text = "Eraser";
            this.btnEraser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEraser.UseVisualStyleBackColor = true;
            this.btnEraser.Click += new System.EventHandler(this.btnEraser_Click);
            // 
            // btnDrawRect
            // 
            this.btnDrawRect.AutoSize = true;
            this.btnDrawRect.BackColor = System.Drawing.Color.Transparent;
            this.btnDrawRect.Image = global::MapTool.Properties.Resources.DrawRectIco;
            this.btnDrawRect.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDrawRect.Location = new System.Drawing.Point(153, 3);
            this.btnDrawRect.Name = "btnDrawRect";
            this.btnDrawRect.Size = new System.Drawing.Size(69, 65);
            this.btnDrawRect.TabIndex = 2;
            this.btnDrawRect.Text = "Rect";
            this.btnDrawRect.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDrawRect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDrawRect.UseVisualStyleBackColor = false;
            this.btnDrawRect.Click += new System.EventHandler(this.btnDrawRect_Click);
            // 
            // btnDrawLine
            // 
            this.btnDrawLine.AutoSize = true;
            this.btnDrawLine.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawLine.Image")));
            this.btnDrawLine.Location = new System.Drawing.Point(3, 74);
            this.btnDrawLine.Name = "btnDrawLine";
            this.btnDrawLine.Size = new System.Drawing.Size(69, 65);
            this.btnDrawLine.TabIndex = 3;
            this.btnDrawLine.Text = "Line";
            this.btnDrawLine.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDrawLine.UseVisualStyleBackColor = true;
            this.btnDrawLine.Click += new System.EventHandler(this.btnDrawLine_Click);
            // 
            // btnBucket
            // 
            this.btnBucket.AutoSize = true;
            this.btnBucket.Image = ((System.Drawing.Image)(resources.GetObject("btnBucket.Image")));
            this.btnBucket.Location = new System.Drawing.Point(78, 74);
            this.btnBucket.Name = "btnBucket";
            this.btnBucket.Size = new System.Drawing.Size(69, 65);
            this.btnBucket.TabIndex = 4;
            this.btnBucket.Text = "Bucket";
            this.btnBucket.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnBucket.UseVisualStyleBackColor = true;
            this.btnBucket.Click += new System.EventHandler(this.btnBucket_Click);
            // 
            // btnRotate
            // 
            this.btnRotate.AutoSize = true;
            this.btnRotate.Image = ((System.Drawing.Image)(resources.GetObject("btnRotate.Image")));
            this.btnRotate.Location = new System.Drawing.Point(153, 74);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(69, 65);
            this.btnRotate.TabIndex = 8;
            this.btnRotate.Text = "Rotate";
            this.btnRotate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(228, 74);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel4.TabIndex = 5;
            // 
            // btnReloadBitmap
            // 
            this.btnReloadBitmap.AutoSize = true;
            this.btnReloadBitmap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReloadBitmap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnReloadBitmap.Image = global::MapTool.Properties.Resources.ReloadIco__1_;
            this.btnReloadBitmap.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReloadBitmap.Location = new System.Drawing.Point(3, 145);
            this.btnReloadBitmap.Name = "btnReloadBitmap";
            this.btnReloadBitmap.Size = new System.Drawing.Size(70, 70);
            this.btnReloadBitmap.TabIndex = 7;
            this.btnReloadBitmap.Text = "Reload";
            this.btnReloadBitmap.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReloadBitmap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnReloadBitmap.UseVisualStyleBackColor = true;
            this.btnReloadBitmap.Click += new System.EventHandler(this.btnReloadBitmap_Click);
            // 
            // btnXOR
            // 
            this.btnXOR.AutoSize = true;
            this.btnXOR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnXOR.Image = ((System.Drawing.Image)(resources.GetObject("btnXOR.Image")));
            this.btnXOR.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnXOR.Location = new System.Drawing.Point(79, 145);
            this.btnXOR.Name = "btnXOR";
            this.btnXOR.Size = new System.Drawing.Size(70, 70);
            this.btnXOR.TabIndex = 9;
            this.btnXOR.Text = "XOR";
            this.btnXOR.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnXOR.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnXOR.UseVisualStyleBackColor = true;
            this.btnXOR.Click += new System.EventHandler(this.btnXOR_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.label1);
            this.flowLayoutPanel3.Controls.Add(this.btnZoomIn);
            this.flowLayoutPanel3.Controls.Add(this.btnZoomOut);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 221);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(194, 46);
            this.flowLayoutPanel3.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zoom";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZoomIn.AutoSize = true;
            this.btnZoomIn.Location = new System.Drawing.Point(68, 8);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(56, 30);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZoomOut.AutoSize = true;
            this.btnZoomOut.Location = new System.Drawing.Point(130, 8);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(56, 30);
            this.btnZoomOut.TabIndex = 2;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.Controls.Add(this.label2);
            this.flowLayoutPanel5.Controls.Add(this.nudBrushSize);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 273);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel5.Size = new System.Drawing.Size(175, 42);
            this.flowLayoutPanel5.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Brush size";
            // 
            // nudBrushSize
            // 
            this.nudBrushSize.AutoSize = true;
            this.nudBrushSize.Location = new System.Drawing.Point(107, 8);
            this.nudBrushSize.Name = "nudBrushSize";
            this.nudBrushSize.Size = new System.Drawing.Size(60, 26);
            this.nudBrushSize.TabIndex = 2;
            this.nudBrushSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBrushSize.ValueChanged += new System.EventHandler(this.nudBrushSize_ValueChanged);
            // 
            // panelMap
            // 
            this.panelMap.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(314, 3);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(1026, 838);
            this.panelMap.TabIndex = 1;
            this.panelMap.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMap_Paint);
            this.panelMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseDown);
            this.panelMap.MouseLeave += new System.EventHandler(this.panelMap_MouseLeave);
            this.panelMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseMove);
            this.panelMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseUp);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Binary files (*.txt)|*.txt|All files (*.*)|*.*";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Select Map Binary Files";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panelMap, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1343, 844);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // btnLoadFolder
            // 
            this.btnLoadFolder.AutoSize = true;
            this.btnLoadFolder.Location = new System.Drawing.Point(3, 39);
            this.btnLoadFolder.Name = "btnLoadFolder";
            this.btnLoadFolder.Size = new System.Drawing.Size(299, 30);
            this.btnLoadFolder.TabIndex = 6;
            this.btnLoadFolder.Text = "Load folder";
            this.btnLoadFolder.UseVisualStyleBackColor = true;
            this.btnLoadFolder.Click += new System.EventHandler(this.btnLoadFolder_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1343, 844);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Map tool";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.btnFLP.ResumeLayout(false);
            this.btnFLP.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrushSize)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.Button btnSaveLayer;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnBrush;
        private System.Windows.Forms.Button btnEraser;
        private System.Windows.Forms.Button btnDrawRect;
        private System.Windows.Forms.Button btnDrawLine;
        private System.Windows.Forms.Button btnBucket;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel btnFLP;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnCreateNewLayer;
        private System.Windows.Forms.CheckedListBox clbLayers;
        private System.Windows.Forms.Button btnLoadBG;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnRemoveLayer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnReloadBitmap;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudBrushSize;
        private System.Windows.Forms.Button btnXOR;
        private System.Windows.Forms.Button btnLoadFolder;
    }
}

