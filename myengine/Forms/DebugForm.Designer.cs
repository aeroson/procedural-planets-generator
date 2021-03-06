﻿using System.Linq;
using System.Windows.Forms;

namespace MyEngine
{
	partial class DebugForm
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
			System.Windows.Forms.ColumnHeader columnName;
			System.Windows.Forms.ColumnHeader columnValue;
			System.Windows.Forms.ColumnHeader columnHeader1;
			System.Windows.Forms.ColumnHeader columnHeader2;
			System.Windows.Forms.ColumnHeader columnHeader3;
			this.listView1 = new System.Windows.Forms.ListView();
			this.listView2 = new System.Windows.Forms.ListView();
			columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// columnName
			// 
			columnName.Text = "Name";
			columnName.Width = 248;
			// 
			// columnValue
			// 
			columnValue.Text = "Value";
			columnValue.Width = 230;
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "Name";
			columnHeader1.Width = 390;
			// 
			// columnHeader2
			// 
			columnHeader2.Text = "Value";
			columnHeader2.Width = 41;
			// 
			// columnHeader3
			// 
			columnHeader3.Text = "Key";
			columnHeader3.Width = 31;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnName,
            columnValue});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(485, 334);
			this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// listView2
			// 
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
			this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.Location = new System.Drawing.Point(0, 334);
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(485, 477);
			this.listView2.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView2.TabIndex = 1;
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView2_MouseClick);
			// 
			// DebugForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 811);
			this.Controls.Add(this.listView2);
			this.Controls.Add(this.listView1);
			this.Name = "DebugForm";
			this.Text = "Debug form";
			this.ResumeLayout(false);

		}
		 
		#endregion
		private ListView listView2;
		private ListView listView1;
	}
}