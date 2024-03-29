﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Scanner.Controls;
using EyE.iOS.Renderers;

[assembly: ExportRenderer(typeof(GradientLabel), typeof(GradientLabelRenderer))]
namespace EyE.iOS.Renderers
{
    /// <summary>
    /// https://trailheadtechnology.com/gradient-label-control-in-xamarin-forms/
    /// </summary>
    public class GradientLabelRenderer : LabelRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (Control != null)
            {
                SetTextColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            SetTextColor();
        }

        private void SetTextColor()
        {
            var image = GetGradientImage(Control.Frame.Size);
            if (image != null)
            {
                Control.TextColor = UIColor.FromPatternImage(image);
            }
        }

        private UIImage GetGradientImage(CGSize size)
        {
            var c1 = (Element as GradientLabel).TextColor1.ToCGColor();
            var c2 = (Element as GradientLabel).TextColor2.ToCGColor();

            UIGraphics.BeginImageContextWithOptions(size, false, 0);

            var context = UIGraphics.GetCurrentContext();

            if (context == null)
            {
                return null;
            }

            context.SetFillColor(UIColor.Blue.CGColor);
            context.FillRect(new RectangleF(new PointF(0, 0), new SizeF((float)size.Width, (float)size.Height)));

            var left = new CGPoint(0, 0);
            var right = new CGPoint(size.Width, 0);
            var colorspace = CGColorSpace.CreateDeviceRGB();

            var gradient = new CGGradient(colorspace, new CGColor[] { c1, c2 }, new nfloat[] { 0f, 1f });

            context.DrawLinearGradient(gradient, left, right, CGGradientDrawingOptions.DrawsAfterEndLocation);

            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return img;
        }
    }
}