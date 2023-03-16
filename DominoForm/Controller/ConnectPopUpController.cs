using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominoForm.View;

namespace DominoForm.Controller
{
    internal class ConnectPopUpController
    {
        private ConnectPopUp popup;

        public ConnectPopUpController()
        {
            this.popup = new ConnectPopUp();
            InitListeners();
            this.popup.Show();
        }

        private void InitListeners()
        {
            popup.ip_TB.KeyPress += Ip_TB_KeyPress;
            popup.port_TB.KeyPress += Port_TB_KeyPress;
            this.popup.SubmitButton.Click += SubmitButton_Click;
        }

        private void Port_TB_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Ip_TB_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void SubmitButton_Click(object? sender, EventArgs e)
        {
            //Create web socket here
            if(popup.ip_TB.Text != ""){
                new Controller_AllTiles(false, popup.ip_TB.Text);
                    }
        }
    }
}
