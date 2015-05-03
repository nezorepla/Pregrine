using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class _Default : System.Web.UI.Page
{
    const string USER = "A25318";
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

        DataTable dt0 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_AKTIVE] '" + USER + "'", "dev");
        string rv = dt0.Rows[0][0].ToString();
        string logout = dt0.Rows[0][1].ToString();
        lblWarning.Text = "logout zamanı" + logout;
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
            Session["IM"] = null; txtMemo.Text = null;
            lblWarning.Text = null;
        }
        else
        {
            //AKSIYON ALMA ALANI GELIYOR...
            tblGet.Visible = false;
            tblPREGRINE.Visible = true;
            FillDDL();
            Session["IM"] = rv;
            //ILK ONCE TEMEL BILGILER 
            DataTable dt1 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_PREGRINE] '" + rv + "'", "dev");
            if (dt1.Rows.Count > 0)
            {

                DataTable dt0 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_AKTIVE] '" + USER + "'", "dev");
                string logout = dt0.Rows[0][1].ToString();
                lblWarning.Text = "logout zamanı" + logout;

                DataTable dt3 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_HISTORY] '" + rv + "'", "dev");

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

        DataTable dt2 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_IM] '" + USER + "'", "dev");
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
        DataTable dtOPType = PCL.MsSQL_DBOperations.GetData("SELECT * FROM PTS_PRG_DEF_ACTLIST", "dev");
        ListItem li = new ListItem("Seçiniz", "0");
        ddlAksLst.Items.Add(li);
        foreach (DataRow drState in dtOPType.Rows)
        {
            li = new ListItem(drState["ACT"].ToString(), drState["INTCODE"].ToString());
            ddlAksLst.Items.Add(li);
        }
        ddlAksLst.Enabled = true;
        ddlAksLst.SelectedIndex = 0;

    }
    protected void btnSendAks_Click(object sender, EventArgs e)
    {
        if (Session["IM"] != null)
        {
            PCL.MsSQL_DBOperations.ExecuteSQLStr("EXEC PTS_PRG_SP_ACT '" + USER + "','" + Session["IM"].ToString() + "','" + txtMemo.Text.ToString() + "'," + ddlAksLst.SelectedValue.Trim(), "dev");
            CheckActive();
        }
        else
        {

            lblWarning.Text = "logout olmuş, kaydedilemedi.";
        }
    }
    protected void btnExit_Click(object sender, EventArgs e)
    {
        string query = "UPDATE PTS_PRG_LOGS SET LOGOUT=DATEADD(MINUTE,-20,GETDATE()) "
         + " WHERE UID= '" + USER + "'"
         + " AND LOGOUT>GETDATE() "
         + "  AND IM='" + Session["IM"].ToString() + "' ;";
        PCL.MsSQL_DBOperations.ExecuteSQLStr(query, "dev");
        CheckActive();
    }
    protected void btnIMSearch_Click(object sender, EventArgs e)
    {
        lblWarning.Text = null;
        DataTable dt2 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_IMSEARCH] '" + txtIMSearch.Text.ToString() + "', '" + USER + "'", "dev");
        string rv = dt2.Rows[0][0].ToString();
        txtIMSearch.Text = null;
        if (rv.Substring(0, 1) == "A")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowRetval", "alert('Bu IM şu anda " + rv + " tarafından kullanılıyor.');", true);
        }
        else
        {

            InitArea(rv);
        }
    }
    public void RprOnline()
    {

        DataTable dt4 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_ONLINE]", "dev");

        lblRapor.Text = HTMLTableString(dt4, "PRG_oNL", "PRG_oNL");

    }
    protected void btnRapor_Click(object sender, EventArgs e)
    {
        string path = string.Concat(@"C:\TempFiles\", "vvv.csv");
        DataTable dt5 = PCL.MsSQL_DBOperations.GetData("EXEC [PTS_PRG_SP_ONLINE]", "dev");
        CreateCSVFile(dt5, path);
    }



    public void CreateCSVFile(DataTable dt, string strFilePath)
    {
        try
        {
            // Create the CSV file to which grid data will be exported.
            StreamWriter sw = new StreamWriter(strFilePath, false);
            // First we will write the headers.
            //DataTable dt = m_dsProducts.Tables[0];
            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < iColCount - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);

            // Now write all the rows.

            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < iColCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        sw.Write(dr[i].ToString());
                    }
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }

                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }










}