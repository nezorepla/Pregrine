using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    public string USER = HttpContext.Current.User.Identity.Name.ToUpper().Replace("İ", "I").Substring(7, 6).ToString();
    public static string HTMLTableString(DataTable dt, string id, string css)
    {
        String RVl = "";
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table id=\"" + id + "\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\"  class=\"" + css + "\"  ><thead><tr>");

            foreach (DataColumn c in dt.Columns)
            {
                sb.AppendFormat("<th  scope=\"col\">{0}</th>", c.ColumnName);
            }
            sb.AppendLine("</tr></thead><tbody>");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>"); foreach (object o in dr.ItemArray)
                {
                    sb.AppendFormat("<td>{0}</td>", o.ToString());
                    //System.Web.HttpUtility.HtmlEncode());
                } sb.AppendLine("</tr>");
            } sb.AppendLine("</tbody></table>");
            RVl = sb.ToString();
        }
        catch (Exception ex)
        {
            RVl = "HATA @ConvertDataTable2HTMLString: " + ex;//  Page.ClientScript.RegisterStartupScript(typeof(Page), "bisey3", "alert('bunu Alper Özen e gönderiniz\n" + strM + "\n" + ex.ToString() + "');", true);
        }
        return RVl;
    }

    public static DataTable HTMLTransposedTable(DataTable inputTable)
    {
        DataTable outputTable = new DataTable();
        // Add columns by looping rows
        // Header row's first column is same as in inputTable

        outputTable.Columns.Add(inputTable.Columns[0].ColumnName.ToString());
        // Header row's second column onwards, 'inputTable's first column taken
        foreach (DataRow inRow in inputTable.Rows)
        {
            string newColName = inRow[0].ToString();
            outputTable.Columns.Add(newColName);
        }
        // Add rows by looping columns       
        for (int rCount = 1; rCount <= inputTable.Columns.Count - 1; rCount++)
        {
            DataRow newRow = outputTable.NewRow();
            // First column is inputTable's Header row's second column
            newRow[0] = inputTable.Columns[rCount].ColumnName.ToString();
            for (int cCount = 0; cCount <= inputTable.Rows.Count - 1; cCount++)
            {
                string colValue = inputTable.Rows[cCount][rCount].ToString();
                newRow[cCount + 1] = colValue;
            }
            outputTable.Rows.Add(newRow);
        }
        return outputTable;

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            RprOnline();
            CheckActive();
        }
    }
    public void CheckActive()
    {


        //  string user = "A25318";

        DataTable dt0 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_AKTIVE] '" + USER + "'", "sqlConn");
        string rv = dt0.Rows[0][0].ToString();
        string logout = dt0.Rows[0][1].ToString();
        lblWarning.Text = "Logout Zamanı: " + logout;
        InitArea(rv);
    }
    public void InitArea(string rv)
    {
        //SORGULADI GELDI. RV 0 ISE BUTONLAR GORUNEBILIR, DEGILSE AKSIYON ALMA ALANI AKTIFLESTIRILIR.
        if (rv == "0")
        {
            //AKTIF KAYIT YOK, YENI KAYIT CAGIRILSIN DIYE ALAN AKTIFLESTIRILECEK..
            tblGet.Visible = true;
            tblPREGRINE.Visible = false;
            Session["IM"] = null; Session["PID"] = null;
            txtMemo.Text = null;
            lblWarning.Text = null;
            txtCallBack.Text = null;
        }
        else
        {
            //AKSIYON ALMA ALANI GELIYOR...
            tblGet.Visible = false;
            tblPREGRINE.Visible = true;
            FillDDL();
            Session["IM"] = rv;
            Session["PID"] = rv;
            //ILK ONCE TEMEL BILGILER 
            DataTable dt1 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_PREGRINE] '" + rv + "'", "sqlConn");
            if (dt1.Rows.Count > 0)
            {

                DataTable dt0 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_AKTIVE] '" + USER + "'", "sqlConn");
                string logout = dt0.Rows[0][1].ToString();
                lblWarning.Text = "logout zamanı" + logout;

                DataTable dt3 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_HISTORY] '" + rv + "'", "sqlConn");

                lblPregrineArea.Text = HTMLTableString(HTMLTransposedTable(dt1), "PRG_Tbl", "Prg_Tbl");
                lblHistory.Text = HTMLTableString(dt3, "PRG_Hst", "PRG_Hst");
            }
            else
            {
                tblGet.Visible = true;
                tblPREGRINE.Visible = false;
                Session["IM"] = null; txtMemo.Text = null;
                lblWarning.Text = "<h3>Kayıt Bulunamadı</h3>";
            }
        }
    }
    protected void btnGetIM_Click(object sender, EventArgs e)
    {
        //BUTONA BASILDI TEPEDEN BIRISI GELICEK

        DataTable dt2 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_IM] '" + USER + "'", "sqlConn");
        string rv = dt2.Rows[0][0].ToString();
        InitArea(rv);
    }

    public void FillDDL()
    {
        /*
         CREATE TABLE [dbo].[PTS_PRG_DEF_ACTLIST](
	   [INTCODE] [int] IDENTITY(1,1) NOT NULL,
	   [ACT] [varchar](50) NOT NULL,
	   [ISRPC] [bit] NOT NULL
         ) ON [PRIMARY]
       */
        ddlAksLst.Items.Clear();
        DataTable dtOPType = PCL.MsSQL_DBOperations.GetData("EXEC  PTS_PRG_SP_ACTLIST", "sqlConn");
        ListItem li = new ListItem("Seçiniz", "0");
        ddlAksLst.Items.Add(li);
        foreach (DataRow drState in dtOPType.Rows)
        {
            li = new ListItem(drState["ACT"].ToString(), drState["INTCODE"].ToString());
            ddlAksLst.Items.Add(li);
        }
        ddlAksLst.Enabled = true;
        ddlAksLst.SelectedIndex = 0;



        ddlUlasma.Items.Clear();
        DataTable dtuType = PCL.MsSQL_DBOperations.GetData("SELECT * FROM PTS_PRG_DEF_ULSLST", "sqlConn");
        ListItem li2 = new ListItem("Seçiniz", "0");
        ddlUlasma.Items.Add(li2);
        foreach (DataRow dru in dtuType.Rows)
        {
            li2 = new ListItem(dru["ACT"].ToString(), dru["INTCODE"].ToString());
            ddlUlasma.Items.Add(li2);
        }
        ddlUlasma.Enabled = true;
        ddlUlasma.SelectedIndex = 0;


        
    }
    protected void btnSendAks_Click(object sender, EventArgs e)
    {
        string q = "EXEC PTS_PRG_SP_ACT '" + USER + "','" + Session["IM"].ToString() + "','" + txtMemo.Text.Replace("'"," ").ToString() + "'," + ddlAksLst.SelectedValue.Trim() + "," + ddlUlasma.SelectedValue.Trim() + ",'" + txtCallBack.Text.ToString().Trim() + "'";
        if (Session["IM"] != null)
        {
            PCL.MsSQL_DBOperations.ExecuteSQLStr(q, "sqlConn");
            CheckActive();
        }
        else
        {

            lblWarning.Text = "<h3>Logout olmuş, kaydedilemedi.</h3>";
        }
    }
    protected void btnExit_Click(object sender, EventArgs e)
    {
        string query = "UPDATE PTS_PRG_LOGS SET LOGOUT=DATEADD(MINUTE,-20,GETDATE()) "
         + " WHERE UID= '" + USER + "'"
         + " AND LOGOUT>GETDATE() "
         + "  AND IM='" + Session["IM"].ToString() + "' ;";
        PCL.MsSQL_DBOperations.ExecuteSQLStr(query, "sqlConn");
        CheckActive();
    }
    protected void btnIMSearch_Click(object sender, EventArgs e)
    {
        lblWarning.Text = null;
        DataTable dt2 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_IMSEARCH] '" + txtIMSearch.Text.ToString() + "', '" + USER + "'", "sqlConn");
        string rv = dt2.Rows[0][0].ToString();
        txtIMSearch.Text = null;
        if (dt2.Rows.Count > 0&&rv.Length >5)
        {

            if (rv.Substring(0, 1) == "A")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowRetvalll", "alert('Bu IM şu anda " + rv + " tarafından kullanılıyor.');", true);
            }
            else
            {

                InitArea(rv);
            }
        }
        else
        {
            tblGet.Visible = true;
            tblPREGRINE.Visible = false;
            Session["IM"] = null; txtMemo.Text = null; txtIMSearch.Text = null;
            lblWarning.Text = "<h3>Kayıt Bulunamadı</h3>";
        }
    }
    public void RprOnline()
    {

        DataTable dt4 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_ONLINE]", "sqlConn");

        lblRapor.Text = HTMLTableString(dt4, "PRG_oNL", "PRG_oNL");

    }
    protected void btnRapor_Click(object sender, EventArgs e)
    {
        DataTable dt5 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_GETRPR] '" + txtBasla.Text.ToString().Trim() + "','" + txtBitis.Text.ToString().Trim() + "'", "sqlConn");

        lblRapor.Text = HTMLTableString(dt5, "PRG_GETRPR", "PRG_GETRPR");
    }



    protected void btnONLINE_Click(object sender, EventArgs e)
    {
        RprOnline();
    }
}



