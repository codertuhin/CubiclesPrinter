using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Cubicles.API;
using Cubicles.Utility;
using Newtonsoft.Json.Linq;

namespace CubiclesAPITest.View
{
    /// <summary>
    /// 
    /// </summary>
    public partial class APITestForm : Form
    {
        /// Private Variables
        #region Private Variables

        /// <summary>
        /// 
        /// </summary>
        private MethodInfo[] methods;

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public APITestForm()
        {
            InitializeComponent();

            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            //API.LoadConfig(ConfigData.Path_InnerConfig);

            methods = GetMethods();

            try
            {
                comboBoxMethods.Items.AddRange(methods.Select(x => x.Name).ToArray());
                comboBoxMethods.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        private void ExecuteMethod(MethodInfo method)
        {
            List<string> datas = new List<string>();
            foreach (Control c in flowLayoutPanelParameters.Controls)
            {
                if (c as TextBox != null)
                    datas.Add(c.Text);
            }

            List<object> data = new List<object>();

            int i = 0;
            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType == typeof (bool))
                {
                    bool b = false;
                    bool.TryParse(datas[i], out b);
                    data.Add(b);
                }
                else if (parameter.ParameterType == typeof (int))
                {
                    int num = 0;
                    int.TryParse(datas[i], out num);
                    data.Add(num);
                }
                else if (parameter.ParameterType == typeof (string))
                    data.Add(datas[i]);

                i++;
            }

            try
            {
                var res = method.Invoke(null, data.ToArray());
                
                if (res != null)
                {
                    string result = res.ToString();//.Replace("\\", "");
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        textBoxResults.Text = "Null result X";
                        return;
                    }

                    textBoxResults.Text = result;

                    try
                    {
                        JArray json = JArray.Parse(result);
                        if (json == null)
                        {
                            //textBoxResults.Text = result.ToString();
                        }
                        else
                        {
                            //textBoxResults.Text = JsonConvert.SerializeObject(json);
                        }
                    }
                    catch (Exception exx)
                    {
                        WPFNotifier.DebugError(exx);
                    }


                    /*
                    try
                    {
                        JObject json = JObject.Parse(result);
                        if (json == null)
                        {
                            //textBoxResults.Text = result.ToString();
                        }
                        else
                        {
                            //textBoxResults.Text = JsonConvert.SerializeObject(json);
                        }
                    }
                    catch (Exception exx)
                    {
                        WPFNotifier.DebugError(exx);
                    }
                    */
                    /*
                    try
                    {
                        Printers xxxx = JsonConvert.DeserializeObject<Printers>(result);
                    }
                    catch (Exception exx)
                    {
                        WPFNotifier.DebugError(exx);
                    }*/
                }
                else
                {
                    textBoxResults.Text = "Null result";
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        private void CreateUI(MethodInfo method)
        {
            if (method == null)
                return;

            textBoxResults.Text = "";

            flowLayoutPanelParameters.Controls.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append(method.ReturnType.Name + " ");
            sb.Append(method.Name);
            sb.Append("(");

            foreach (var parameter in method.GetParameters())
            {
                Label lb = new Label();
                lb.Name = "lb" + parameter.Name;
                lb.Text = string.Format("{0} ( {1} )", parameter.Name, parameter.ParameterType.ToString());
                lb.Size = new Size(220, 15);
                lb.Margin = new Padding(0, 5, 0, 0);
                lb.Padding = Padding.Empty;
                flowLayoutPanelParameters.Controls.Add(lb);
                TextBox tb = new TextBox();
                tb.Tag = parameter.ParameterType.ToString();
                tb.Name = "tb" + parameter.Name;
                tb.Size = new Size(220, 20);
                tb.Margin = Padding.Empty;
                tb.Padding = Padding.Empty;

                if (parameter.ParameterType == typeof(bool))
                {
                    tb.Text = bool.FalseString;
                    sb.Append("bool " + parameter.Name);
                }
                else if (parameter.ParameterType == typeof(int))
                {
                    tb.Text = "0";
                    sb.Append("int " + parameter.Name);
                }
                else if (parameter.ParameterType == typeof (string))
                {
                    sb.Append("string " + parameter.Name);
                }

                sb.Append(", ");
                flowLayoutPanelParameters.Controls.Add(tb);
            }

            if (sb[sb.Length - 2] == ',')
            {
                sb[sb.Length - 2] = ')';
                sb[sb.Length - 1] = ' ';
            }
            else
                sb.Append(")");
            labelMethod.Text = string.Format("{0}", sb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private MethodInfo[] GetMethods()
        {
            try
            {
                return (typeof(API)).GetMethods(BindingFlags.Public | BindingFlags.Static);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }
        
        #endregion

        /// Events
        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateUI(methods[comboBoxMethods.SelectedIndex]);
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            ExecuteMethod(methods[comboBoxMethods.SelectedIndex]);

            textBoxRequest.Text = Connection.LastCall;
        }

        #endregion

        private void bExecuteRaw_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxRequest.Text))
                return;

            try
            {
                var res = Connection.Get(textBoxRequest.Text);
                if (res == null)
                {
                    textBoxResults.Text = "Null result";
                }
                else
                {
                    textBoxResults.Text = res.ToString();
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
            
        }
    }
}
