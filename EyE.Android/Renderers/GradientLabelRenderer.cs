using Android.Content;
using Android.Graphics;
using EyE.Droid.Renderers;
using Scanner.Controls;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientLabel), typeof(GradientLabelRenderer))]
namespace EyE.Droid.Renderers
{
    /// <summary>
    /// https://trailheadtechnology.com/gradient-label-control-in-xamarin-forms/
    /// </summary>
    public class GradientLabelRenderer : LabelRenderer
    {
        public GradientLabelRenderer(Context context) : base(context)
        {
        }

        #region баг - не отрисовывается градиент(решено)
        /// <summary>
        /// Без этого метода при появление страницы с использованием уже созданной страницы и взятой из DI-контейнера
        /// (получается пользователь перешел на страницу, затем вернулся и попытался опять перейти на ту же страницу),
        /// не будет отрисовываться градиент для label, а будет взят только первый цвет.
        /// Несмотря на то, что OnElementChanged и OnElementPropertyChanged вызывались при появление страницы и у них вызывался метод SetColors().
        /// </summary>
        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);
            SetColors();
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        //{
        //    base.OnElementChanged(e);
        //    SetColors();
        //}

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            SetColors();
        }
        #endregion

        /// <summary>
        /// https://issue.life/questions/47650044
        /// </summary>
        private void SetColors()
        {
            var colorFrom = (Element as GradientLabel).TextColorFrom.ToAndroid();
            var colorTo = (Element as GradientLabel).TextColorTo.ToAndroid();

            var myShader = new LinearGradient(
                0, 0, 0, Control.MeasuredHeight,
                colorFrom, colorTo,
                Shader.TileMode.Clamp);

            Control.Paint.SetShader(myShader);
            Control.Invalidate();
        }
    }
}