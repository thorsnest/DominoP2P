using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominoForm.View;
using DominoForm.Controller;

namespace DominoForm.Controller
{
    internal class MenuController
    {
        private Menu menu;

        public MenuController()
        {
            menu = new Menu();
            SetTransparency();
            InitListeners();
            Application.Run(menu);
        }

        private void SetTransparency()
        {
            foreach (Control c in menu.Controls)
            {
                c.BackColor = Color.Transparent;
            }
        }

        private void InitListeners()
        {
            menu.connect_B.Click += ConnectButton_Click;
            menu.host_B.Click += HostButton_Click;
            menu.exit_B.Click += ExitButton_Click;
        }

        private void ExitButton_Click(object? sender, EventArgs e)
        {
            menu.Dispose();
        }

        private void HostButton_Click(object? sender, EventArgs e)
        {
            new Controller_AllTiles(true, "ws://localhost:8080/host");
        }

        private void ConnectButton_Click(object? sender, EventArgs e)
        {
            new ConnectPopUpController();
        }
    }
}
