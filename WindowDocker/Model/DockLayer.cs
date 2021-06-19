using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WindowDocker.Model
{
    class DockLayer : ModelBase
    {
        #region Properties

        private int _layer;
        public int Layer
        {
            get => _layer;
            set
            {
                if (_layer != value)
                {
                    _layer = value;
                    RaisePropertyChanged("Layer");
                }
            }
        }

        private LayerOrientation _orientation;
        public LayerOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    RaisePropertyChanged("Orientation");
                }
            }
        }

        private LayerPosition _position;
        public LayerPosition Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }

        private UIElement _content;
        public UIElement Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region Enums

        public enum LayerOrientation
        {
            Row,
            Column
        }

        public enum LayerPosition
        {
            Left,
            Right
        }

        #endregion
    }
}
