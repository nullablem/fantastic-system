using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace CustomZedGraphControlApp
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            customZedGraphControl.GraphPane.Title.Text = "";
            customZedGraphControl.GraphPane.YAxis.Title.Text = "";

            //customZedGraphControl.MasterPane.InnerPaneGap = 60;

            customZedGraphControl.PanButtons = MouseButtons.Left;
            customZedGraphControl.PanModifierKeys = Keys.None;

            customZedGraphControl.ZoomButtons = MouseButtons.None;
            customZedGraphControl.ZoomModifierKeys = (Keys.Control | Keys.Alt | Keys.None);

            customZedGraphControl.ZoomButtons2 = MouseButtons.Left;
            customZedGraphControl.ZoomModifierKeys2 = (Keys.Shift| Keys.None);
        }

        bool[] m_pane_flag = new bool[10];
        string xAxisLabel = "xAxisLabel";

        int last_index = 0;

        private void ArrangeGraph(int gp_no)
        {
            bool new_flag = false;
            
            // 순차적으로 확인을 하네..
            // 
            if (m_pane_flag[gp_no] == false)
            {
                m_pane_flag[gp_no] = true;
                new_flag = true;
            }
            ArrangeYAxis();

            // Find Reference Pane(Used in Case X-Axes Are Synchronous)
            //int num_pane = (int)m_graphMode + 1;    // 아마 1X1 1X2 이런거 일듯
            int num_pane = last_index + 1;
            int ref_idx = 0;

            for (int i = 0; i < num_pane; i++)
            {
                if (m_pane_flag[i] == true)
                {
                    ref_idx = i;
                    break;
                }
            }

            for (int i = 0; i < num_pane; i++)
            {
                // X-Axis Aligning (In Case X-Axes Are Synchronous)
                if (customZedGraphControl.IsSynchronizeXAxes == true && i != ref_idx)
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.Min = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.Min;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.Max = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.Max;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.MajorStep = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.MajorStep;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.MinorStep = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.MinorStep;
                }

                if (i == gp_no && new_flag == true)
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.MinAuto = true;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.MaxAuto = true;
                }
               // customZedGraphControl.MasterPane[i].XAxis.Scale.MinAuto = true;
                //customZedGraphControl.MasterPane[i].XAxis.Scale.MaxAuto = true;

                customZedGraphControl.MasterPane[i].Border.IsVisible = false;
                customZedGraphControl.MasterPane[i].YAxis.IsVisible = true;

                if (customZedGraphControl.IsSynchronizeXAxes == true)
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.IsVisible = false;
                    customZedGraphControl.MasterPane[i].XAxis.MajorTic.IsOutside = false;
                    customZedGraphControl.MasterPane[i].XAxis.MinorTic.IsOutside = false;
                }
                else
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.IsVisible = true;
                    customZedGraphControl.MasterPane[i].XAxis.MajorTic.IsOutside = true;
                    customZedGraphControl.MasterPane[i].XAxis.MinorTic.IsOutside = true;
                }

                customZedGraphControl.MasterPane[i].XAxis.Title.IsVisible = false;
                customZedGraphControl.MasterPane[i].XAxis.MajorGrid.IsVisible = true;
                customZedGraphControl.MasterPane[i].XAxis.MinorGrid.IsVisible = true;
                customZedGraphControl.MasterPane[i].IsFontsScaled = false;
                customZedGraphControl.MasterPane[i].BaseDimension = 6F;
                customZedGraphControl.MasterPane[i].Legend.Position = LegendPos.Top;
                customZedGraphControl.MasterPane[i].Legend.FontSpec.Size = 10F;
                customZedGraphControl.MasterPane[i].YAxis.Scale.Align = AlignP.Inside;
                //customZedGraphControl.MasterPane[i].YAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(YScaleFormatEvent);
                customZedGraphControl.MasterPane[i].YAxis.Scale.FormatAuto = false;
                customZedGraphControl.MasterPane[i].XAxis.Scale.IsVisible = false;

                //customZedGraphControl.MasterPane[i].YAxis.MinSpace = 90;

            }

            
            if (customZedGraphControl.IsSynchronizeXAxes == true)
            {

                // 이놈이 x 길이 안맞게 하는 원흉
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.IsVisible = true;
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.IsSkipFirstLabel = true;
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.IsSkipLastLabel =  true;


                customZedGraphControl.MasterPane[num_pane - 1].XAxis.MajorTic.IsOutside = true;
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.MinorTic.IsOutside = true;
            }

            customZedGraphControl.MasterPane[num_pane - 1].XAxis.Title.IsVisible = true;
            customZedGraphControl.MasterPane[num_pane - 1].XAxis.Title.Text = xAxisLabel;

            //customZedGraphControl.MasterPane[num_pane - 1].XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XScaleFormatEvent);
            customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.FormatAuto = false;

            using (Graphics g = CreateGraphics())
            {
                MasterPane myMaster = customZedGraphControl.MasterPane;
                if (num_pane == 1)
                {
                    myMaster.SetLayout(g, true, new int[] { 1 }, new float[] { 1f });
                }
                else if (num_pane == 2)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1 }, new float[] { 1f, 1.13f });
                }
                else if (num_pane == 3)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1 }, new float[] { 1f, 1f, 1.20f });
                }
                else if (num_pane == 4)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1.27f });
                }
                else if (num_pane == 5)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1.34f });
                }
                else if (num_pane == 6)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1.41f });
                }
                else if (num_pane == 7)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1.48f });
                }
                else if (num_pane == 8)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.55f });
                }
                else if (num_pane == 9)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.62f });
                }
                else if (num_pane == 10)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.69f });
                }
            }
            customZedGraphControl.Invalidate();
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            if (last_index < 9)
            {
                customZedGraphControl.MasterPane.Add(new GraphPane());
                ArrangeGraph(++last_index);
            }
            
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            if (last_index > 0)
            {
                ArrangeGraph(--last_index);
            }
        }

        // 0 이면 아무것도 X
        // 1이면 sin만
        // 2면 cos만
        // 3이면 둘다
        int[] graph_flag = new int[5];
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (SettingView settingView = new SettingView(graph_flag))
            {
                if (settingView.ShowDialog() == DialogResult.OK)
                {
                    for (int i=0; i<5; i++)
                    {
                        graph_flag[i] = settingView.setting[i];
                    }

                    // 다시 그린다.
                    DrawGraph();
                }
            }
        }

        private string XScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            if (DateTime.MinValue.Ticks <= val && val <= DateTime.MaxValue.Ticks)
            {
                DateTime d = new DateTime((long)val);
                return d.ToString("HH:mm:ss");
            }
            else
            {
                return val.ToString();
            }
        }

        private string YScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            return val.ToString();
        }


        const int YAXIS_MIN_GRACE = 10;
        private void ArrangeYAxis()
        {
            //int num_pane = (int)m_graphMode + 1;
            int num = customZedGraphControl.MasterPane.PaneList.Count;
            if (num > 1)
            {
                customZedGraphControl.AxisChange();
            }

            float y_axis_min_space = YAXIS_MIN_GRACE;
            for (int i=0; i<num; i++)
            {
                float gap = customZedGraphControl.MasterPane[i].Chart.Rect.X - customZedGraphControl.MasterPane[i].Rect.X - 10;
                if (gap > y_axis_min_space)
                {
                    y_axis_min_space = gap;
                }
            }

            for (int i=0; i<num; i++)
            {
                customZedGraphControl.MasterPane[i].YAxis.MinSpace = y_axis_min_space;
                customZedGraphControl.MasterPane[i].Y2Axis.MinSpace = YAXIS_MIN_GRACE;
            }
            customZedGraphControl.AxisChange();
        }


        private void DrawGraph()
        {
            customZedGraphControl.MasterPane.PaneList.Clear();

            List<GraphPane> paneList = new List<GraphPane>();
            for (int i=0; i<5; i++)
            {
                if (graph_flag[i] != 0)
                {
                    customZedGraphControl.AddGraphPane(i);
                }
            }

            int num_pane = customZedGraphControl.MasterPane.PaneList.Count;

            int ref_idx = num_pane - 1;

            for (int i = 0; i < num_pane; i++)
            {
                // X-Axis Aligning (In Case X-Axes Are Synchronous)
                if (customZedGraphControl.IsSynchronizeXAxes == true && i != ref_idx)
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.Min = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.Min;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.Max = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.Max;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.MajorStep = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.MajorStep;
                    customZedGraphControl.MasterPane[i].XAxis.Scale.MinorStep = customZedGraphControl.MasterPane[ref_idx].XAxis.Scale.MinorStep;
                }

                customZedGraphControl.MasterPane[i].XAxis.Scale.MinAuto = true;
                customZedGraphControl.MasterPane[i].XAxis.Scale.MaxAuto = true;

                customZedGraphControl.MasterPane[i].Border.IsVisible = false;
                customZedGraphControl.MasterPane[i].YAxis.IsVisible = true;

                if (customZedGraphControl.IsSynchronizeXAxes == true)
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.IsVisible = false;
                    customZedGraphControl.MasterPane[i].XAxis.MajorTic.IsOutside = false;
                    customZedGraphControl.MasterPane[i].XAxis.MinorTic.IsOutside = false;
                }
                else
                {
                    customZedGraphControl.MasterPane[i].XAxis.Scale.IsVisible = true;
                    customZedGraphControl.MasterPane[i].XAxis.MajorTic.IsOutside = true;
                    customZedGraphControl.MasterPane[i].XAxis.MinorTic.IsOutside = true;
                }

                customZedGraphControl.MasterPane[i].XAxis.Title.IsVisible = false;
                customZedGraphControl.MasterPane[i].XAxis.MajorGrid.IsVisible = true;
                customZedGraphControl.MasterPane[i].XAxis.MinorGrid.IsVisible = true;
                customZedGraphControl.MasterPane[i].IsFontsScaled = false;
                customZedGraphControl.MasterPane[i].BaseDimension = 6F;
                customZedGraphControl.MasterPane[i].Legend.Position = LegendPos.Top;
                customZedGraphControl.MasterPane[i].Legend.FontSpec.Size = 10F;
                customZedGraphControl.MasterPane[i].YAxis.Scale.Align = AlignP.Inside;
                //customZedGraphControl.MasterPane[i].YAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(YScaleFormatEvent);
                customZedGraphControl.MasterPane[i].YAxis.Scale.FormatAuto = false;
                customZedGraphControl.MasterPane[i].XAxis.Scale.IsVisible = false;

                //customZedGraphControl.MasterPane[i].YAxis.MinSpace = 90;
            }


            if (customZedGraphControl.IsSynchronizeXAxes == true)
            {

                // 이놈이 x 길이 안맞게 하는 원흉
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.IsVisible = true;
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.IsSkipFirstLabel = true;
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.IsSkipLastLabel = true;


                customZedGraphControl.MasterPane[num_pane - 1].XAxis.MajorTic.IsOutside = true;
                customZedGraphControl.MasterPane[num_pane - 1].XAxis.MinorTic.IsOutside = true;
            }

            customZedGraphControl.MasterPane[num_pane - 1].XAxis.Title.IsVisible = true;
            customZedGraphControl.MasterPane[num_pane - 1].XAxis.Title.Text = xAxisLabel;

            //customZedGraphControl.MasterPane[num_pane - 1].XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XScaleFormatEvent);
            customZedGraphControl.MasterPane[num_pane - 1].XAxis.Scale.FormatAuto = false;
            
            using (Graphics g = CreateGraphics())
            {
                MasterPane myMaster = customZedGraphControl.MasterPane;
                if (num_pane == 1)
                {
                    myMaster.SetLayout(g, true, new int[] { 1 }, new float[] { 1f });
                }
                else if (num_pane == 2)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1 }, new float[] { 1f, 1.13f });
                }
                else if (num_pane == 3)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1 }, new float[] { 1f, 1f, 1.20f });
                }
                else if (num_pane == 4)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1.27f });
                }
                else if (num_pane == 5)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1.34f });
                }
                else if (num_pane == 6)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1.41f });
                }
                else if (num_pane == 7)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1.48f });
                }
                else if (num_pane == 8)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.55f });
                }
                else if (num_pane == 9)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.62f });
                }
                else if (num_pane == 10)
                {
                    myMaster.SetLayout(g, true, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1.69f });
                }
            }
            customZedGraphControl.Invalidate();
        }
    }
}
