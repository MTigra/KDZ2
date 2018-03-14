using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using System.Reflection;

namespace ClassLib
{

    /// <summary>
    /// Represents a ListBox control used as a drop-down filter list
    /// in a DataGridView control.
    /// </summary>
    public class FilterListBox : ListBox
    {
        /// <summary>
        /// Initializes a new instance of the FilterListBox class.
        /// </summary>
        public FilterListBox()
        {
            Visible = true;
            IntegralHeight = true;
            BorderStyle = BorderStyle.FixedSingle;
            TabStop = false;
        }

        /// <summary>
        /// Indicates that the FilterListBox will handle (or ignore) all 
        /// keystrokes that are not handled by the operating system. 
        /// </summary>
        /// <param name="keyData">A Keys value that represents the keyboard input.</param>
        /// <returns>true in all cases.</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        /// <summary>
        /// Processes a keyboard message directly, preventing it from being
        /// intercepted by the parent DataGridView control.
        /// </summary>
        /// <param name="m">A Message, passed by reference, that 
        /// represents the window message to process.</param>
        /// <returns>true if the message was processed by the control;
        /// otherwise, false.</returns>
        protected override bool ProcessKeyMessage(ref Message m)
        {
            return ProcessKeyEventArgs(ref m);
        }

    }

}