﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;
using SDP_Project.Entity;

namespace SDP_Project.User_Interface
{
    public partial class FormCustomerSalesManagement : Form
    {
        private String SQL = "";
        private MySqlCommand cmd;
        private MySqlDataReader myData;
        FormShoppingCart frmShoppingCart;
        List<SalesItem> shoppingCart;//List<> is similiar with Linkedlist
        public FormCustomerSalesManagement()
        {
            InitializeComponent();
            shoppingCart = new List<SalesItem>();
        }
        private void FormCustomerSalesManagement_Load(object sender, EventArgs e)
        {
            intializeDataGridView();
        }
        public void intializeDataGridView()
        {
            dgvProductList.Rows.Clear();
            try
            {
                SQL = "select * from product where productAmount >=0;";
                cmd = new MySqlCommand(SQL, FormContainer.conn);
                myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    while (myData.Read())
                    {
                        dgvProductList.Rows.Add(myData["productid"], myData["showcaseid"], myData["productName"], myData["productamount"], myData["price"], myData["remark"]);
                    }
                }
                myData.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Error " + ex.Number + " : " + ex.Message);
            }
        }
        private void dgvProductList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = e.RowIndex;
            String colname = dgvProductList.Columns[e.ColumnIndex].Name;
            if (colname == "select")
            {
                try
                {
                    Boolean isInvalidAmountInput = true;
                    Boolean hasDiscount = false;
                    Boolean isInvalidDiscountInput = false;
                    //initialize salesitem information
                    int Amount;
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Input quantity", "Title", "");
                    if(int.TryParse(input, out Amount) == true)
                    {
                        isInvalidAmountInput = false;
                    }
                    int Discount;
                    if(dgvProductList.Rows[rowindex].Cells[remark.Index].Value.ToString() != "")
                    {
                        hasDiscount = true;
                    }
                    if (hasDiscount == true)
                    {
                        input = Microsoft.VisualBasic.Interaction.InputBox(dgvProductList.Rows[rowindex].Cells[remark.Index].Value.ToString() + "\nPlease input integer discount", "Title", "");
                        if(int.TryParse(input,out Discount) != true)
                        {
                            isInvalidDiscountInput = true;
                        }
                    }
                    else
                    {
                        Discount = 0;
                    }
                    String Productid = dgvProductList.Rows[rowindex].Cells[productid.Index].Value.ToString();
                    String ProductName = dgvProductList.Rows[rowindex].Cells[productname.Index].Value.ToString();
                    decimal Price = Convert.ToDecimal(dgvProductList.Rows[rowindex].Cells[price.Index].Value);

                    if (shoppingCart.Contains(new SalesItem { productid = Productid }))
                    {
                        MessageBox.Show("This product has been already added into shopping cart");
                    }else if (isInvalidAmountInput==true)
                    {
                        MessageBox.Show("Invalid amount input");
                    }else if (isInvalidDiscountInput==true)
                    {
                        MessageBox.Show("Invalid discount input");
                    }
                    else
                    {
                        SalesItem salesitem = new SalesItem { amount = Amount, productid=Productid, productname = ProductName, price = Price,discount = Discount };
                        shoppingCart.Add(salesitem);
                    }
                }
                catch (ArgumentNullException)
                {
                    //undo event
                }
            }
        }
        private void btnShoppingCart_Click(object sender, EventArgs e)
        {
            frmShoppingCart = new FormShoppingCart(this,shoppingCart);
            frmShoppingCart.ShowDialog();
        }

        public void OnPaymentSettled()
        {
            shoppingCart.Clear();
            intializeDataGridView();
        }
    }
}
