using DominoForm.View;

namespace DominoForm.Controller
{
    internal class ConnectPopUpController
    {
        private ConnectPopUp popup;

        public ConnectPopUpController()
        {
            popup = new ConnectPopUp();
            InitListeners();
            popup.Show();
        }

        private void InitListeners()
        {
            popup.ip_TB.KeyPress += Ip_TB_KeyPress;
            popup.SubmitButton.Click += SubmitButton_Click;
        }

        private void Ip_TB_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.Equals("{ENTER}"))
            {
                new Controller_AllTiles(false, "ws://" + popup.ip_TB.Text + ":8080/host");
                popup.Dispose();
            }
            else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void SubmitButton_Click(object? sender, EventArgs e)
        {
            //Create web socket here
            if(popup.ip_TB.Text != ""){
                new Controller_AllTiles(false, "ws://" +  popup.ip_TB.Text + ":8080/host");
                popup.Dispose();
            }
        }
    }
}
