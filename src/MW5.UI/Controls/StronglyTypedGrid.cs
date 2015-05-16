﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MW5.Plugins.Services;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Grid.Grouping;

namespace MW5.UI.Controls
{
    public abstract class StronglyTypedGrid<T> : CustomGridControl
            where T : class
    {
        public GridAdapter<T> Adapter { get; protected set; }

        protected StronglyTypedGrid()
        {
            InitStyle();

            InitGroupOptions();

            InitRowSelection();

            Adapter = new GridAdapter<T>(this);

            TableControl.CurrentCellValidating += TableControl_CurrentCellValidating;

            //TableControl.CurrentCell.ShowErrorIcon = false;
            //TableControl.CurrentCell.ShowErrorMessageBox = false;
        }


        /// <summary>
        /// Checks if the values typed in double and integer columns are numbers.
        /// </summary>
        /// <remarks>http ://www.syncfusion.com/kb/619/where-can-i-validate-changes-made-to-a-grid-cell</remarks>
        private void TableControl_CurrentCellValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var type = GetCurrentColumnFieldType();
            if (type != null )
            {
                string text = TableControl.CurrentCell.Renderer.ControlText;
                
                
                if (type == typeof (double))
                {
                    double val;
                    if (!Double.TryParse(text, out val))
                    {
                        e.Cancel = true;
                        TableControl.CurrentCell.CancelEdit();
                        MessageService.Current.Warn("Invalid double value.");
                    }
                }
                else if (type == typeof (int))
                {
                    int val;
                    if (!Int32.TryParse(text, out val))
                    {
                        e.Cancel = true;
                        TableControl.CurrentCell.CancelEdit();
                        MessageService.Current.Warn("Invalid integer value.");
                    }
                }
            }
        }

        private Type GetCurrentColumnFieldType()
        {
            int index = TableDescriptor.ColIndexToField(TableControl.CurrentCell.ColIndex);
            var cmn = TableDescriptor.VisibleColumns[index];
            if (cmn != null)
            {
                var fld = TableDescriptor.Fields[cmn.Name];
                if (fld != null)
                {
                    return fld.FieldPropertyType;
                }
            }

            return null;
        }

        public Color SelectionBackColor
        {
            get { return TableOptions.SelectionBackColor; }
            set { TableOptions.SelectionBackColor = value; }
        }

        private void InitStyle()
        {
            Appearance.AnyCell.VerticalAlignment = GridVerticalAlignment.Middle;
            Appearance.AnyCell.Borders.All = new GridBorder(GridBorderStyle.None, Color.White);
            GridLineColor = Color.White;
            BrowseOnly = false;
            ShowRowHeaders = false;
            ShowColumnHeaders = true;
        }

        private void InitGroupOptions()
        {
            ShowGroupDropArea = false;
            TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
            TopLevelGroupOptions.ShowCaption = false;
        }

        private void InitRowSelection()
        {
            TableOptions.ListBoxSelectionMode = SelectionMode.One;
            SelectionBackColor = Color.FromArgb(64, 51, 153, 255);
            TableOptions.SelectionTextColor = Color.Black;
            TableOptions.ListBoxSelectionColorOptions = GridListBoxSelectionColorOptions.ApplySelectionColor;

            // any option other than None will disable SelectedRecordsChanged event
            // http ://www.syncfusion.com/forums/46745/grid-grouping-control-selection-color
            TableOptions.AllowSelection = GridSelectionFlags.None;
        }
    }
}
