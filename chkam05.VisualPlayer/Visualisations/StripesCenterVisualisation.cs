﻿using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Visualisations
{
    public class StripesCenterVisualisation : StripesVisualisation
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> StripesCenterVisualisation class constructor. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public StripesCenterVisualisation(SpectrumProvider spectrumProvider) : base(spectrumProvider)
        {
            //
        }

        #endregion CLASS METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        public override BitmapDrawer Draw()
        {
            if (_spectrumProvider == null)
                return null;

            var bitmapDrawer = new BitmapDrawer(DrawingAreaSize);

            Brush borderBrush = null;
            Brush fillBrush = null;

            Brush borderBrush2 = null;
            Brush fillBrush2 = null;

            UpdateRunTime();

            switch (ColorType)
            {
                case VisualisationColorType.RAINBOW_HORIZONTAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }
                    break;

                case VisualisationColorType.RAINBOW_VERTICAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }
                    break;

                case VisualisationColorType.CUSTOM:
                case VisualisationColorType.SYSTEM:
                default:
                    borderBrush = new SolidColorBrush(BorderColor) { Opacity = _opacity };
                    fillBrush = new SolidColorBrush(FillColor) { Opacity = _opacity };

                    borderBrush2 = new SolidColorBrush(BorderColor) { Opacity = _opacity };
                    fillBrush2 = new SolidColorBrush(FillColor) { Opacity = _opacity };
                    break;
            }

            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                var level = _spectrumData[iX];
                var pX = _firstX + (_stripeWidth * iX + _peakSpaceX * iX);
                var pY = Margin.Top + (DrawingAreaSize.Height - Margin.Top - Margin.Bottom) / 2;

                var point = new Point(pX, pY);
                var size = new Size(_stripeWidth, level.Value / 2);

                var point2 = new Point(pX, pY - level.Value / 2);
                var size2 = new Size(_stripeWidth, level.Value);

                Pen pen = null;
                Pen pen2 = null;

                switch (ColorType)
                {
                    case VisualisationColorType.RAINBOW_HORIZONTAL:
                        var hFColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + (RainbowX / SpectrumSize) * iX);
                        fillBrush = new SolidColorBrush(hFColor.ToColor()) { Opacity = _opacity };

                        if (BorderEnabled)
                        {
                            var hBColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + (RainbowX / SpectrumSize) * iX);
                            borderBrush = new SolidColorBrush(hBColor.ToColor()) { Opacity = _opacity };
                            borderBrush2 = new SolidColorBrush(hBColor.ToColor()) { Opacity = _opacity };
                        }

                        pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;
                        pen2 = BorderEnabled ? new Pen(borderBrush2, 1.0) : null;
                        break;

                    case VisualisationColorType.RAINBOW_VERTICAL:
                        var vFColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + (RainbowX / SpectrumSize) * iX);
                        fillBrush = RainbowColorGenerator.GetRainbowGradient(vFColor, RainbowY, (int)level.Value, _spectrumHeight);
                        fillBrush.Opacity = _opacity;

                        fillBrush2 = RainbowColorGenerator.GetRainbowGradient(vFColor, RainbowY, (int)level.Value, _spectrumHeight, true);
                        fillBrush2.Opacity = _opacity;

                        if (BorderEnabled)
                        {
                            var vBColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + (RainbowX / SpectrumSize) * iX);
                            borderBrush = RainbowColorGenerator.GetRainbowGradient(vBColor, RainbowY, (int)level.Value, _spectrumHeight);
                            borderBrush.Opacity = _opacity;

                            borderBrush2 = RainbowColorGenerator.GetRainbowGradient(vBColor, RainbowY, (int)level.Value, _spectrumHeight, true);
                            borderBrush2.Opacity = _opacity;
                        }

                        pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;
                        pen2 = BorderEnabled ? new Pen(borderBrush2, 1.0) : null;
                        break;

                    default:
                        pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;
                        pen2 = BorderEnabled ? new Pen(borderBrush2, 1.0) : null;
                        break;
                }

                bitmapDrawer.DrawRectangle(fillBrush, pen, point, size);
                bitmapDrawer.DrawRectangle(fillBrush2, pen2, point2, size2);
            }

            return bitmapDrawer;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update graphics configuration. </summary>
        public override void UpdateGraphics()
        {
            double width = DrawingAreaSize.Width - (Margin.Left + Margin.Right + (_peakSpaceX * 2));
            double spacesSize = _peakSpaceX * (SpectrumSize - 1);

            _firstX = Margin.Left + _peakSpaceX;
            _stripeWidth = (width - spacesSize) / SpectrumSize;
        }

        #endregion DRAWING METHODS

    }
}
