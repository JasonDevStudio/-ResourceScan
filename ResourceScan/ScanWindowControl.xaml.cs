using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace ResourceScan
{
    /// <summary>
    /// Interaction logic for ScanWindowControl.
    /// </summary>
    public partial class ScanWindowControl : UserControl
    { 
        

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanWindowControl"/> class.
        /// </summary>
        public ScanWindowControl()
        {
            this.InitializeComponent(); 
        }        
    }
}