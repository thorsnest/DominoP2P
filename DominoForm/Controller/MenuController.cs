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
            LoadImage();
            InitListeners();
            Application.Run(menu);
        }

        private void InitListeners()
        {
            menu.ConnectButton.Click += ConnectButton_Click;
            menu.HostButton.Click += HostButton_Click;
            menu.OptionsButton.Click += OptionsButton_Click;
        }

        private void OptionsButton_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HostButton_Click(object? sender, EventArgs e)
        {
            new Controller_AllTiles(true, null, null);
        }

        private void ConnectButton_Click(object? sender, EventArgs e)
        {
            new ConnectPopUpController();
        }

        private void LoadImage()
        {
            menu.LogoImage.ImageLocation = "C:\\Users\\prova\\Downloads\\Domino_pizza_logo.png";
            menu.LogoImage.SizeMode = PictureBoxSizeMode.CenterImage;
        }
    }
}
