using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSearch
{
	public partial class MultilineTextForm : Form
	{
		public MultilineTextForm()
		{
			InitializeComponent();
		}

		public string Value
		{
			get
			{
                return (new MultilineText(textBoxText.Lines)).Text;
			}

			set
			{
                textBoxText.Lines = (new MultilineText(value)).Lines;
			}
		}



	}
}
