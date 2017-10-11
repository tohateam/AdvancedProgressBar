using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ProgressBarLibrary {
    [Description("Colored Progress Bar")]
    public partial class ColorProgresBar : UserControl {
        public ColorProgresBar() {
            InitializeComponent();

            //base.Size = new Size(150, 20);
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer,
                true
                );
        }

        #region Variables *************************************************************************
        private int _Value = 0;
        // Значение нижнего уровня
        private int _ValueLow = 0;
        // Значение высокого уровня
        private int _ValueGood = 0;

        private int _Minimum = 0;
        private int _Maximum = 100;
        private int _Step = 10;
        // Цветт полосы прогресс бара
        private Color _BarColor = Color.FromArgb(255, 255, 255);
        // Цвет полосы при значенияхм _Value <= _ValueLow
        private Color _BarColorLow = Color.FromArgb(255, 128, 128);
        // Цвет полосы при значенияхм _ValueLow >_Value < _ValueGood
        private Color _BarColorAverage = Color.FromArgb(255, 128, 128);
        // Цвет полосы при значенияхм _Value => _ValueGood
        private Color _BarColorGood = Color.FromArgb(255, 128, 128);
        //Цвет границы
        private Color _BorderColor = Color.Black;
        //Цвет фона
        private Color _BackColor = Color.White;
        //Тип полосы прогресс бара
        private FillStyles _FillStyle;
        public enum FillStyles {
            Solid,
            Dashed
        }
        //Label
        private bool _LabelProgress = true;
        private Color _LabelProgressColor = Color.FromArgb(0, 0, 0);
        private string _LabelChar = "%";

        #endregion

        #region Colors ****************************************************************************

        [Description("The border color of progress bar")]
        [Category("ColorProgressBar")]
        public Color BorderColor {
            get {
                return _BorderColor;
            }
            set {
                _BorderColor = value;
                this.Invalidate();
            }
        }

        [Description("Main color bar progress bar")]
        [Category("ColorProgressBar")]
        public Color BarColor {
            get {
                return _BarColor;
            }
            set {
                _BarColor = value;
                this.Invalidate();
            }
        }

        [Description("The color of the strip when the lower value is reached")]
        [Category("ColorProgressBar")]
        public Color BarColorLow {
            get {
                return _BarColorLow;
            }
            set {
                _BarColorLow = value;
                this.Invalidate();
            }
        }

        [Description("Band color for mean value")]
        [Category("ColorProgressBar")]
        public Color BarColorAverage {
            get {
                return _BarColorAverage;
            }
            set {
                _BarColorAverage = value;
                this.Invalidate();
            }
        }

        [Description("Band color when the upper value is reached")]
        [Category("ColorProgressBar")]
        public Color BarColorGood {
            get {
                return _BarColorGood;
            }
            set {
                _BarColorGood = value;
                this.Invalidate();
            }
        }

        [Description("Band color when the upper value is reached")]
        [Category("ColorProgressBar")]
        public Color LabelColor {
            get {
                label1.ForeColor = _LabelProgressColor;
                return _LabelProgressColor;
            }
            set {
                _LabelProgressColor = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Values ****************************************************************************

        [Description("The lower value at which the bar progress bar color changes.")]
        [Category("ColorProgressBar")]
        // the rest of the Properties windows must be updated when this peroperty is changed.
        [RefreshProperties(RefreshProperties.All)]
        public int ValueLow {
            get {
                return _ValueLow;
            }
            set {
                if (value < _Minimum) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                if (value > _Maximum) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                if (_ValueGood != 0 && value >= _ValueGood) {
                    throw new ArgumentException("A value of '" + value + "'  is invalid! Enter a number less than ValueGood.");
                }

                _ValueLow = value;
                this.Invalidate();
            }
        }

        [Description("The upper value at which the bar progress bar color changes.")]
        [Category("ColorProgressBar")]
        // the rest of the Properties windows must be updated when this peroperty is changed.
        [RefreshProperties(RefreshProperties.All)]
        public int ValueGood {
            get {
                return _ValueGood;
            }
            set {
                if (value < _Minimum) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                if (value > _Maximum) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                if (_ValueLow != 0 && value <= _ValueLow) {
                    throw new ArgumentException("A value of '" + value + "'  is invalid!  Enter a number greater than ValueLow.");
                }

                _ValueGood = value;
                this.Invalidate();
            }
        }

        [Description("The current value of the progress bar")]
        [Category("ColorProgressBar")]
        // the rest of the Properties windows must be updated when this peroperty is changed.
        [RefreshProperties(RefreshProperties.All)]
        public int Value {
            get {
                return _Value;
            }
            set {
                if (value < _Minimum) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                if (value > _Maximum) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                _Value = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Settigs ***************************************************************************

        [Description("Fill style bars progress bar")]
        [Category("ColorProgressBar")]
        public FillStyles FillStyle {
            get {
                return _FillStyle;
            }
            set {
                _FillStyle = value;
                this.Invalidate();
            }
        }

        [Description("The lower bound of the range this ColorProgressbar is working with.")]
        [Category("ColorProgressBar")]
        [RefreshProperties(RefreshProperties.All)]
        public int Minimum {
            get {
                return _Minimum;
            }
            set {
                _Minimum = value;

                if (_Minimum > _Maximum)
                    _Maximum = _Minimum;
                if (_Minimum > _Value)
                    _Value = _Minimum;

                this.Invalidate();
            }
        }

        [Description("The uppper bound of the range this ColorProgressbar is working with.")]
        [Category("ColorProgressBar")]
        [RefreshProperties(RefreshProperties.All)]
        public int Maximum {
            get {
                return _Maximum;
            }
            set {
                _Maximum = value;

                if (_Maximum < _Value)
                    _Value = _Maximum;
                if (_Maximum < _Minimum)
                    _Minimum = _Maximum;

                this.Invalidate();
            }
        }

        [Description("The amount to jump the current value of the control by when the Step() method is called.")]
        [Category("ColorProgressBar")]
        public int Step {
            get {
                return _Step;
            }
            set {
                _Step = value;
                this.Invalidate();
            }
        }

        [Description("The amount to jump the current value of the control by when the Step() method is called.")]
        [Category("ColorProgressBar")]
        public bool ShowLabel {
            get {
                return _LabelProgress;
            }
            set {
                label1.Visible = value;
                _LabelProgress = value;
                this.Invalidate();
            }
        }

        [Description("The symbol displayed next to the number(no more than 3 characters")]
        [Category("ColorProgressBar")]
        public string LabelChar {
            get {
                return _LabelChar;
            }
            set {
                if (value.Length > 3) {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'LabelChar'.\n" +
                        "The value must be no longer than 3 characters.");
                }

                _LabelChar = value;
                label1.Text = "" + _Value + " " + _LabelChar;
                this.Invalidate();
            }
        }

        #endregion

        #region Drawing ***************************************************************************
        protected override void OnPaint(PaintEventArgs e) {
            //
            // Calculate matching colors
            //
            Color darkColor; // = ControlPaint.Dark(_BarColor);
            Color lightColor; // = ControlPaint.Dark(_BarColor);
            Color bgColor = ControlPaint.Light(BackColor);

            if (_Value <= _ValueLow && _ValueLow != 0) {
                darkColor = ControlPaint.Dark(_BarColorLow);
                lightColor = ControlPaint.Dark(_BarColorLow);
            } else if ((_Value > _ValueLow && _ValueLow !=0) && (_Value < _ValueGood && _ValueGood !=0)) {
                darkColor = ControlPaint.Dark(_BarColorAverage);
                lightColor = ControlPaint.Dark(_BarColorAverage);
            } else if (_Value >= _ValueGood && _ValueGood != 0) {
                darkColor = ControlPaint.Dark(_BarColorGood);
                lightColor = ControlPaint.Dark(_BarColorGood);
            } else {
                darkColor = ControlPaint.Dark(_BarColor);
                lightColor = ControlPaint.Dark(_BarColor);
            }
            //
            // Fill background
            //
            var bgBrush = new SolidBrush(bgColor);
            e.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            bgBrush.Dispose();

            //
            // Check for value
            //
            if (_Maximum == _Minimum || _Value == 0) {
                // Draw border only and exit;
                DrawBorder(e.Graphics);
                return;
            }

            //
            // The following is the width of the bar. This will vary with each value.
            //
            int fillWidth = (this.Width * _Value) / (_Maximum - _Minimum);

            //
            // GDI+ doesn't like rectangles 0px wide or high
            //
            if (fillWidth == 0) {
                // Draw border only and exti;
                DrawBorder(e.Graphics);
                return;
            }

            //
            // Rectangles for upper and lower half of bar
            //
            var topRect = new Rectangle(0, 0, fillWidth, this.Height / 2);
            var buttomRect = new Rectangle(0, this.Height / 2, fillWidth, this.Height / 2);

            //
            // The gradient brush
            //
            LinearGradientBrush brush;

            //
            // Paint upper half
            //
            brush = new LinearGradientBrush(new Point(0, 0), new Point(0, this.Height / 2), darkColor, _BarColor);
            e.Graphics.FillRectangle(brush, topRect);
            brush.Dispose();

            //
            // Paint lower half
            // (this.Height/2 - 1 because there would be a dark line in the middle of the bar)
            //
            brush = new LinearGradientBrush(new Point(0, this.Height / 2 - 1), new Point(0, this.Height), _BarColor, darkColor);
            e.Graphics.FillRectangle(brush, buttomRect);
            brush.Dispose();

            //
            // Calculate separator's setting
            //
            int sepWidth = (int)(this.Height * .67);
            int sepCount = (int)(fillWidth / sepWidth);
            Color sepColor = ControlPaint.LightLight(_BarColor);

            //
            // Paint separators
            //
            switch (_FillStyle) {
                case FillStyles.Dashed:
                    // Draw each separator line
                    for (int i = 1; i <= sepCount; i++) {
                        e.Graphics.DrawLine(new Pen(sepColor, 1), sepWidth * i, 0, sepWidth * i, this.Height);
                    }
                    break;

                case FillStyles.Solid:
                    // Draw nothing
                    break;

                default:
                    break;
            }

            //
            // Draw border and exit
            //
            DrawBorder(e.Graphics);

            if(_LabelProgress) {
                label1.Text = "" + _Value + " "+_LabelChar;
            }
        }

        //
        // Draw border
        //
        protected void DrawBorder(Graphics g) {
            var borderRect = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            g.DrawRectangle(new Pen(_BorderColor, 1), borderRect);
        }
        #endregion
    }
}
