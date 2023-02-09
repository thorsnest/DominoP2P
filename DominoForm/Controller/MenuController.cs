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
            this.menu = new Menu();
            LoadImage();
            InitListeners();
            Application.Run(this.menu);
        }

        private void InitListeners()
        {
            this.menu.ConnectButton.Click += ConnectButton_Click;
            this.menu.HostButton.Click += HostButton_Click;
            this.menu.OptionsButton.Click += OptionsButton_Click;
        }

        private void OptionsButton_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HostButton_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConnectButton_Click(object? sender, EventArgs e)
        {
            new ConnectPopUpController();
        }

        private void LoadImage()
        {
            this.menu.LogoImage.ImageLocation = "C:\\Users\\prova\\Downloads\\Domino_pizza_logo.png";
            this.menu.LogoImage.SizeMode = PictureBoxSizeMode.CenterImage;
        }
    }
}
