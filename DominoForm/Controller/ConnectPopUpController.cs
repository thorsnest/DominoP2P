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
            this.popup.Show();
        }

        private void InitListeners()
        {
            this.popup.SubmitButton.Click += SubmitButton_Click;
        }

        private void SubmitButton_Click(object? sender, EventArgs e)
        {
            //Create web socket here
            throw new NotImplementedException();
        }
    }
}
