﻿namespace Wesley.Easycharts
{
    using System;
    using System.Linq;

    using SkiaSharp;

    /// <summary>
    /// 垂直条图表
    /// </summary>
    public class BarChart : PointChart
    {
        #region Constructors

        /// <summary>
        /// Ctor
        /// </summary>
        public BarChart()
        {
            this.PointSize = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 背景区域透明度
        /// </summary>
        public byte BarAreaAlpha { get; set; } = 32;

        #endregion

        #region Methods

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            if (this.Entries != null)
            {
                var labels = this.Entries.Select(x => x.Label).ToArray();
                var labelSizes = this.MeasureLabels(labels);
                var footerHeight = this.CalculateFooterHeaderHeight(labelSizes, this.LabelOrientation, labels);

                var valueLabels = this.Entries.Select(x => x.ValueLabel).ToArray();
                var valueLabelSizes = this.MeasureLabels(valueLabels);
                var headerHeight = this.CalculateFooterHeaderHeight(valueLabelSizes, this.ValueLabelOrientation, valueLabels);

                var itemSize = this.CalculateItemSize(width, height, footerHeight, headerHeight);
                var origin = this.CalculateYOrigin(itemSize.Height, headerHeight);
                var points = this.CalculatePoints(itemSize, origin, headerHeight);

                this.DrawBarAreas(canvas, points, itemSize, headerHeight);
                this.DrawBars(canvas, points, itemSize, origin, headerHeight);
                this.DrawPoints(canvas, points);
                this.DrawHeader(canvas, valueLabels, valueLabelSizes, points, itemSize, height, headerHeight);
                this.DrawFooter(canvas, labels, labelSizes, points, itemSize, height, footerHeight);
            }
        }

        /// <summary>
        /// 绘制条值
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="headerHeight">The Header height.</param>
        protected void DrawBars(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin, float headerHeight)
        {
            const float MinBarHeight = 4;
            if (points.Length > 0)
            {
                for (int i = 0; i < this.Entries.Count(); i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                    })
                    {
                        var x = point.X - (itemSize.Width / 2);
                        var y = Math.Min(origin, point.Y);
                        var height = Math.Max(MinBarHeight, Math.Abs(origin - point.Y));
                        if (height < MinBarHeight)
                        {
                            height = MinBarHeight;
                            if (y + height > this.Margin + itemSize.Height)
                            {
                                y = headerHeight + itemSize.Height - height;
                            }
                        }

                        var rect = SKRect.Create(x, y, itemSize.Width, height);
                        canvas.DrawRect(rect, paint);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制条区域
        /// </summary>
        /// <param name="canvas">The output canvas.</param>
        /// <param name="points">The entry points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="headerHeight">The header height.</param>
        protected void DrawBarAreas(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float headerHeight)
        {
            if (points.Length > 0 && this.PointAreaAlpha > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color.WithAlpha((byte)(this.BarAreaAlpha * this.AnimationProgress)),
                    })
                    {
                        var max = entry.Value > 0 ? headerHeight : headerHeight + itemSize.Height;
                        var height = Math.Abs(max - point.Y);
                        var y = Math.Min(max, point.Y);
                        canvas.DrawRect(SKRect.Create(point.X - (itemSize.Width / 2), y, itemSize.Width, height), paint);
                    }
                }
            }
        }

        #endregion
    }

    /*
    public class BarChart : PointChart
    {
        #region Constructors

        public BarChart()
        {
            this.PointSize = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bar background area alpha.
        /// </summary>
        /// <value>The bar area alpha.</value>
        public byte BarAreaAlpha { get; set; } = 32;

        #endregion

        #region Methods

        /// <summary>
        /// Draws the content of the chart onto the specified canvas.
        /// </summary>
        /// <param name="canvas">The output canvas.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            var valueLabelSizes = MeasureValueLabels();
            var footerHeight = CalculateFooterHeight(valueLabelSizes);
            var headerHeight = CalculateHeaderHeight(valueLabelSizes);
            var itemSize = CalculateItemSize(width, height, footerHeight, headerHeight);
            var origin = CalculateYOrigin(itemSize.Height, headerHeight);
            var points = this.CalculatePoints(itemSize, origin, headerHeight);

            this.DrawBarAreas(canvas, points, itemSize, headerHeight);
            this.DrawBars(canvas, points, itemSize, origin, headerHeight);
            this.DrawPoints(canvas, points);
            this.DrawFooter(canvas, points, itemSize, height, footerHeight);
            this.DrawValueLabel(canvas, points, itemSize, height, valueLabelSizes);
        }

        /// <summary>
        /// Draws the value bars.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="headerHeight">The Header height.</param>
        protected void DrawBars(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin, float headerHeight)
        {
            const float MinBarHeight = 4;
            if (points.Length > 0)
            {
                for (int i = 0; i < this.Entries.Count(); i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                    })
                    {
                        var x = point.X - (itemSize.Width / 2);
                        var y = Math.Min(origin, point.Y);
                        var height = Math.Max(MinBarHeight, Math.Abs(origin - point.Y));
                        if (height < MinBarHeight)
                        {
                            height = MinBarHeight;
                            if (y + height > this.Margin + itemSize.Height)
                            {
                                y = headerHeight + itemSize.Height - height;
                            }
                        }

                        var rect = SKRect.Create(x, y, itemSize.Width, height);
                        //绘制矩形
                        canvas.DrawRect(rect, paint);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the bar background areas.
        /// </summary>
        /// <param name="canvas">The output canvas.</param>
        /// <param name="points">The entry points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="headerHeight">The header height.</param>
        protected void DrawBarAreas(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float headerHeight)
        {
            if (points.Length > 0 && this.PointAreaAlpha > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color.WithAlpha(this.BarAreaAlpha),
                    })
                    {
                        var max = entry.Value > 0 ? headerHeight : headerHeight + itemSize.Height;
                        var height = Math.Abs(max - point.Y);
                        var y = Math.Min(max, point.Y);
                        canvas.DrawRect(SKRect.Create(point.X - (itemSize.Width / 2), y, itemSize.Width, height), paint);
                    }
                }
            }
        }

        #endregion
    }
    */
}
