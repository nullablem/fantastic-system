using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZedGraph;
using System.Drawing.Drawing2D;

namespace CustomZedGraphControlApp
{
    public partial class CustomZedGraphControl : ZedGraph.ZedGraphControl
    {
        private void MakeTempData()
        {
            // 임시로 데이터 추가

            PointPairList pointPairs = new PointPairList();
            double x = 0;
            for (int k = 0; k < 100000; k++)
            {
                pointPairs.Add(new PointPair(x, 10000 * Math.Sin(x)));
                x += 0.1;
            }

            this.GraphPane.AddCurve("test_data", pointPairs, ColorSymbolRotator.StaticNextColor, SymbolType.None);

            this.GraphPane.XAxis.ResetAutoScale(this.GraphPane, CreateGraphics());

            /*
            //this.GraphPane.XAxis.Scale.Min = -1000;
            //this.GraphPane.XAxis.Scale.Max = 1000;

            CrossHairFontSpec = new FontSpec
            {
                FontColor = Color.Black,
                Size = 9,
                Border = { IsVisible = true },
                Fill = { Color = Color.Beige, Brush = new SolidBrush(Color.Beige) },
                //TextBrush = new SolidBrush(Color.Black)
            };
            */
        }

        public void AddGraphPane(int index)
        {
            this.MasterPane.Add(graphPanes[index]);
        }


        #region MasterPane에 등록된 GraphPanes 관련
        
        #endregion

        #region graphPanes 관련
        const int PANE_COUNT_MAX = 5;

        // 실제로 보이지 않더라도 10개를 다 생성한다.
        GraphPane[] graphPanes = new GraphPane[PANE_COUNT_MAX];

        private void CreateGraphPanes()
        {
            for (int i = 0; i < PANE_COUNT_MAX; i++)
            {
                GraphPane graphPane = new GraphPane();

                // XAxis
                graphPane.XAxis.Scale.FontSpec.Size *= 0.88f;
                graphPane.XAxis.MajorGrid.DashOff /= 2f;  // Grid선 점과 점 사이의 길이
                graphPane.XAxis.MajorGrid.DashOn *= 10f;  // Grid선 점의 길이
                graphPane.XAxis.MajorGrid.Color = Color.Gray;
                graphPane.XAxis.Scale.LabelGap /= 2f;

                graphPane.XAxis.Scale.MinGrace = 0;
                graphPane.XAxis.Scale.MaxGrace = 0;

                // YAxis
                graphPane.YAxis.Scale.FontSpec.Size *= 0.88f;
                graphPane.YAxis.MajorGrid.DashOff /= 2f;
                graphPane.YAxis.MajorGrid.DashOn /= 2f;
                graphPane.YAxis.MajorGrid.Color = Color.Gray;
                graphPane.YAxis.Scale.LabelGap /= 2f;
                //graphPane.YAxis.MajorGrid.IsVisible = true;

                // Margin
                graphPane.Margin.Top /= 1.8f;
                graphPane.Margin.Bottom /= 2f;

                // Legend
                graphPane.Legend.Gap /= 1.1f;
                graphPane.Legend.IsVisible = true;

                PointPairList pointPairs = new PointPairList();
                for (int k = 0; k < 100; k++)
                {
                    pointPairs.Add(new PointPair(k, i));
                }

                graphPane.AddCurve(i.ToString(), pointPairs, ColorSymbolRotator.StaticNextColor, SymbolType.None);

                graphPanes[i] = graphPane;
            }
        }

        private void ClearGraphPanes()
        {
            for (int i = 0; i < PANE_COUNT_MAX; i++)
            {
                graphPanes[i].CurveList.Clear();
                graphPanes[i].GraphObjList.Clear();

                graphPanes[i] = null;
            }
        }
        #endregion

        #region GuideLine 관련
        LineObj removedGuide;
        LineObj selectedGuide;
        List<LineObj> LineObjList = new List<LineObj>();

        public void SetGuideLine()
        {
            int paneCount = this.MasterPane.PaneList.Count;
            for (int i = 0; i < paneCount; i++)
            {
                foreach (var lineObj in LineObjList)
                {
                    this.MasterPane[i].GraphObjList.Add(lineObj);
                }
            }
        }
        #endregion


        public CustomZedGraphControl()
        {
            InitializeComponent();

            ContextMenuStrip.Opening += ContextMenuStrip_Opening;

            this.IsShowPointValues = true;
            this.IsSynchronizeXAxes = true;
            MakeTempData();
            //this.MasterPane.PaneList.Clear();
            CreateGraphPanes();

        }

        enum CrossHairType {
            MasterPane,
        }

        bool IsShowCrossHair = false;
        public Pen CrossHairPen { get; set; } = new Pen(Color.Silver) { DashStyle = DashStyle.Dash };

        CrossHairType crossHairType = CrossHairType.MasterPane;
        public FontSpec CrossHairFontSpec { get; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //e.Graphics.DrawString(paneName, this.Font, Brushes.Black, 10, 10);
            //e.Graphics.DrawString(objName, this.Font, Brushes.Black, 10, 20);
            //e.Graphics.DrawString(c_index.ToString(), this.Font, Brushes.Black, 10, 30);

            return;
            if (IsShowCrossHair && /*_mouseInBounds &&*/
            !_lastCrosshairPoint.IsEmpty)
            {
                var g = e.Graphics;
                if (crossHairType == CrossHairType.MasterPane)
                {
                    var sz = this.ClientSize;
                    g.DrawLine(CrossHairPen, _lastCrosshairPoint.X, 0, _lastCrosshairPoint.X, sz.Height);
                    g.DrawLine(CrossHairPen, 0, _lastCrosshairPoint.Y, sz.Width, _lastCrosshairPoint.Y);
                    return;
                }

                //var pane = _currentPane as GraphPane;
                var pane = this.GraphPane;
                if (pane != null && !pane.CurveList.Any(c => c.IsPie)
                                 && pane.Chart.Rect.Contains(_lastCrosshairPoint))
                {
                    // Draw cross-hair lines
                    var rect = pane.Chart.Rect;
                    g.SetClip(rect);
                    g.DrawLine(CrossHairPen, (int)rect.Left, _lastCrosshairPoint.Y, (int)rect.Right, _lastCrosshairPoint.Y);
                    g.DrawLine(CrossHairPen, _lastCrosshairPoint.X, (int)rect.Top, _lastCrosshairPoint.X, (int)rect.Bottom);
                    g.ResetClip();

                    //var xaxis = pane.XAxis.Scale.Valid ? (Axis)pane.XAxis : pane.X2Axis;
                    //var yaxis = pane.YAxis.Scale.Valid ? (Axis)pane.YAxis : pane.Y2Axis;

                    //CrossHairFontSpec.ScaleFactor = this.MasterPane.ScaleFactor;

                    // Draw crosshair values at each axis
                    //_lastCrosshairXlabelRect = (Axis)pane.XAxis.DrawXValueLabel(g, pane, _lastCrosshairPoint.X, CrossHairFontSpec);
                    //_lastCrosshairYlabelRect = (Axis)pane.YAxis.DrawYValueLabel(g, pane, _lastCrosshairPoint.Y, CrossHairFontSpec);
                }
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (removedGuide != null)
            {
                removedGuide = null;
                e.Cancel = true;
            }
        }

        

        private void CustomZedGraphControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.MasterPane.FindNearestPaneObject(e.Location, this.CreateGraphics(), out GraphPane clickedPane, out object nearestObj, out int index))
            {
                selectedGuide = nearestObj as LineObj;
                if (selectedGuide != null)
                {
                    // pass
                }
                if (nearestObj is Legend l)
                {

                }
            }
            else if (clickedPane != null)
            {
                clickedPane.ReverseTransform(e.Location, out double x, out double y);
                LineObj lineObj = new LineObj(Color.Red, x, 0, x, 1);
                lineObj.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
                lineObj.IsClippedToChartRect = true;
                lineObj.Tag = null;
                lineObj.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
                lineObj.Line.Width = 2.0f;

                clickedPane.GraphObjList.Add(lineObj);
                LineObjList.Add(lineObj);

                this.Invalidate();
            }
        }

        string paneName;
        string objName;
        int c_index;

        private bool CustomZedGraphControl_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            /*
            if (e.Button == MouseButtons.Middle)
            {
                IsShowCrossHair = !IsShowCrossHair;
                return true;
            }

            if (this.MasterPane.FindNearestPaneObject(e.Location, this.CreateGraphics(), out GraphPane clickedPane, out object nearestObj, out int index))
            {
            }
            */
            /*
            int curveIndex = -1;
            if (!this.GraphPane.Legend.FindPoint(e.Location, this.GraphPane, this.GraphPane.CalcScaleFactor(), out curveIndex))
                return false;

            this.GraphPane.CurveList[curveIndex].IsVisible = !this.GraphPane.CurveList[curveIndex].IsVisible;
            this.Invalidate();
            */
            if (this.MasterPane.FindNearestPaneObject(e.Location, this.CreateGraphics(), out GraphPane clickedPane, out object nearestObj, out int index))
            {
                if (clickedPane != null)
                {
                    paneName = clickedPane.ToString();
                }
                else
                {
                    paneName = "null";
                }
                if (nearestObj != null)
                {
                    objName = nearestObj.ToString();
                }
                else
                {
                    objName = "null";
                }

                c_index = index;

                selectedGuide = nearestObj as LineObj;
                if (e.Button == MouseButtons.Right)
                {
                    removedGuide = selectedGuide;

                    clickedPane.GraphObjList.Remove(selectedGuide);
                    LineObjList.Remove(selectedGuide);

                    selectedGuide = null;
                }
            }
            else
            {
                paneName = "";
                objName = "";
                c_index = -1;
            }
            this.Invalidate();
            return true;
        }

        private bool CustomZedGraphControl_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            if (selectedGuide != null)
            {
                this.GraphPane.ReverseTransform(e.Location, out double x, out double y);
                selectedGuide.Location.X = x;

                this.Invalidate();
            }

            return true;
        }

        private bool CustomZedGraphControl_MouseUpEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            if (selectedGuide != null)
            {
                this.GraphPane.ReverseTransform(e.Location, out double x, out double y);

                selectedGuide.Location.X = x;
                selectedGuide = null;

                this.Invalidate();
            }

            return true;
        }

        private void CustomZedGraphControl_MouseMove(object sender, MouseEventArgs e)
        {
            // Do Somthing

            InvalidateCrossHair(e.Location);
        }

        internal Point _lastCrosshairPoint;
        void InvalidateCrossHair(Point mousePt)
        {
            // Display crosshair
            if (!IsShowCrossHair)
                return;

            if (!_lastCrosshairPoint.IsEmpty)
            {
                Invalidate(new Rectangle(_lastCrosshairPoint.X - 5, 0, 10, ClientSize.Height));
                Invalidate(new Rectangle(0, _lastCrosshairPoint.Y - 5, ClientSize.Width, 10));
                /*
                if (CrossHairType == CrossHairType.MasterPane)
                {
                    // Invalidate old cross-hair location
                    Invalidate(new Rectangle(_lastCrosshairPoint.X - 5, 0, 10, ClientSize.Height));
                    Invalidate(new Rectangle(0, _lastCrosshairPoint.Y - 5, ClientSize.Width, 10));
                }
                else if (_currentPane is GraphPane)
                {
                    // Invalidate old cross-hair location
                    var rect = ((GraphPane)_currentPane).Chart.Rect;
                    Invalidate(new Rectangle(_lastCrosshairPoint.X - 5, (int)rect.Top, 10, (int)rect.Height));
                    Invalidate(new Rectangle((int)rect.Left, _lastCrosshairPoint.Y - 5, (int)rect.Width, 10));
                    //Invalidate(_lastCrosshairXlabelRect);
                    //Invalidate(_lastCrosshairYlabelRect);
                }
                */
            }

            _lastCrosshairPoint = mousePt;
        }

        
        private void CustomZedGraphControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.ControlKey)
            {
                IsEnableHZoom = true;
                IsEnableVZoom = false;
            }
            else if (e.KeyValue == (int)Keys.Menu)
            {
                IsEnableHZoom = false;
                IsEnableVZoom = true;
            }
            else if (e.KeyValue == (int)Keys.ShiftKey)
            {
                IsEnableHZoom = true;
                IsEnableVZoom = true;
            }
        }

        private void CustomZedGraphControl_KeyUp(object sender, KeyEventArgs e)
        {
            IsEnableHZoom = true;
            IsEnableVZoom = true;
        }
    }
}
