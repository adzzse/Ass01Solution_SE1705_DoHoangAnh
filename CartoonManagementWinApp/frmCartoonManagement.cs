﻿using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CartoonManagementWinApp_DOHOANGANH
{
    public partial class frmCartoonManagement : Form
    {
        public Cartoon loginCartoon { get; set; }
        ICartoonRepository cartoonRepository = new CartoonRepository();

        BindingSource source;
        BindingSource countrySource;

        bool search = false;
        bool filter = false;
        IEnumerable<Cartoon> dataSource;
        IEnumerable<Cartoon> searchResult;
        IEnumerable<Cartoon> filterResult;

        IEnumerable<string> countryList;

        public frmCartoonManagement()
        {
            InitializeComponent();
        }

        private void frmCartoonManagement_Load(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;

            txtCartoonID.Enabled = false;
            txtCartoonName.Enabled = false;
            txtShortDescription.Enabled = false;
            txtCartoonType.Enabled = false;
            txtProducer.Enabled = false;
            txtDuration.Enabled = false;
            txtActors.Enabled = false;
            txtDirector.Enabled = false;
            txtCountry.Enabled = false;
            txtLaunchDate.Enabled = false;

            btnNew.Enabled = false;
            dgvCartoonList.Enabled = false;

            btnLoad.Enabled = true;

            grSearch.Enabled = false;
            grFilter.Enabled = false;
        }

        private Cartoon GetCartoonInfo()
        {
            Cartoon cartoon = null;
            try
            {
                cartoon = new Cartoon
                {
                    CartoonID = int.Parse(txtCartoonID.Text),
                    CartoonName = txtCartoonName.Text,
                    ShortDescription = txtShortDescription.Text,
                    CartoonType = txtCartoonType.Text,
                    Duration = int.Parse(txtDuration.Text),
                    Actors = txtActors.Text,
                    Producer = txtProducer.Text,
                    Director = txtDirector.Text,
                    Country = txtCountry.Text,
                    LaunchDate = txtLaunchDate.Text,
                    
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Get Cartoon Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return cartoon;
        }
        private void LoadCartoonList()
        {
            try
            {
                source = new BindingSource();
                source.DataSource = filter ? filterResult : dataSource;
                if (!filter)
                {
                    countryList = from cartoon in dataSource
                                  where !string.IsNullOrEmpty(cartoon.Country.Trim())
                                  orderby cartoon.Country ascending
                                  select cartoon.Country;
                    countryList = countryList.Distinct();

                    countryList = countryList.Prepend("All");

                    if (dataSource.Count() > 0)
                    {
                        countrySource = new BindingSource();
                        countrySource.DataSource = countryList;
                        cboCountry.DataSource = null;
                        cboCountry.DataSource = countrySource;

                    }
                }

                txtCartoonID.DataBindings.Clear();
                txtCartoonName.DataBindings.Clear();
                txtShortDescription.DataBindings.Clear();
                txtCartoonType.DataBindings.Clear();
                txtProducer.DataBindings.Clear();
                txtDuration.DataBindings.Clear();
                txtDirector.DataBindings.Clear();
                txtActors.DataBindings.Clear();
                txtCountry.DataBindings.Clear();
                txtLaunchDate.DataBindings.Clear();

                txtCartoonID.DataBindings.Add("Text", source, "CartoonID");
                txtCartoonName.DataBindings.Add("Text", source, "CartoonName");
                txtShortDescription.DataBindings.Add("Text", source, "ShortDescription");
                txtCartoonType.DataBindings.Add("Text", source, "CartoonType");
                txtProducer.DataBindings.Add("Text", source, "Producer");
                txtDuration.DataBindings.Add("Text", source, "Duration");
                txtDirector.DataBindings.Add("Text", source, "Director");
                txtActors.DataBindings.Add("Text", source, "Actors");
                txtCountry.DataBindings.Add("Text", source, "Country");
                txtLaunchDate.DataBindings.Add("Text", source, "LaunchDate");

                dgvCartoonList.DataSource = null;
                dgvCartoonList.DataSource = source;

                if (dataSource.Count() > 0)
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Cartoon List");
            }
        }

        private void LoadFullList()
        {
            search = false;
            filter = false;
            var cartoons = cartoonRepository.GetCartoonsList();
            var cartoonList = from cartoon in cartoons
                              orderby cartoon.CartoonName descending
                              select cartoon;
            dataSource = cartoonList;
            searchResult = cartoonList;
            filterResult = cartoonList;
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {

            btnNew.Enabled = true;
            dgvCartoonList.Enabled = true;
            btnLoad.Enabled = true;
            grSearch.Enabled = true;
            grFilter.Enabled = true;
            LoadFullList();
            LoadCartoonList();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frmCartoonDetails frmCartoonDetails = new frmCartoonDetails
            {
                cartoonRepository = this.cartoonRepository,
                InsertOrUpdate = true,
                Text = "Add new cartoon"
            };

            if (frmCartoonDetails.ShowDialog() == DialogResult.OK)
            {
                LoadFullList();
                LoadCartoonList();
                source.Position = source.Count - 1;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Cartoon cartoon = GetCartoonInfo();

            if (MessageBox.Show($"Do you really want to delete the cartoon: \n" +
            $"Cartoon ID: {cartoon.CartoonID}\n" +
            $"Cartoon Name: {cartoon.CartoonName}\n" +
            $"ShortDescription: {cartoon.ShortDescription}\n" +
            $"Producer: {cartoon.Producer}\n" +
            $"Country: {cartoon.Country}", "Delete cartoon", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cartoonRepository.DeleteCartoon(cartoon.CartoonID);
                search = false;
                LoadFullList();
                LoadCartoonList();
            }
        }

        private void dgvCartoonList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Cartoon cartoon = GetCartoonInfo();

            frmCartoonDetails frmCartoonDetails = new frmCartoonDetails
            {
                cartoonRepository = this.cartoonRepository,
                InsertOrUpdate = false,
                cartoonInfo = cartoon,
                Text = "Update cartoon info"
            };

            if (frmCartoonDetails.ShowDialog() == DialogResult.OK)
            {
                LoadFullList();

                LoadCartoonList();
                source.Position = source.Count - 1;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtSearchValue.Text;
                if (radioByProducer.Checked)
                {
                    string searchP = searchValue;
                    IEnumerable<Cartoon> searchResult = cartoonRepository.SearchCartoon(searchP);
                    if (searchResult.Any())
                    {
                        dataSource = searchResult;
                        this.searchResult = searchResult;
                        this.filterResult = searchResult;
                        filter = false;
                        search = true;
                        LoadCartoonList();
                    }
                    else
                    {
                        MessageBox.Show("No result found!", "Search cartoon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else if (radioByActor.Checked)
                {
                    string searchA = searchValue;
                    IEnumerable<Cartoon> searchResult = cartoonRepository.SearchCartoon(searchA);
                    if (searchResult.Any())
                    {
                        dataSource = searchResult;
                        this.searchResult = searchResult;
                        this.filterResult = searchResult;
                        filter = false;
                        search = true;
                        LoadCartoonList();
                    }
                    else
                    {
                        MessageBox.Show("No result found!", "Search cartoon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search cartoon", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCountry.DataSource != null)
                {
                    string country = cboCountry.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(country))
                    {
                        IEnumerable<Cartoon> searchResult;
                        if (search)
                        {
                            searchResult = cartoonRepository.SearchCartoonByCountry(country, this.searchResult);
                        }
                        else
                        {
                            searchResult = cartoonRepository.SearchCartoonByCountry(country, this.dataSource);
                        }

                        if (searchResult.Any())
                        {

                            filterResult = searchResult;
                            filter = true;
                            LoadCartoonList();
                        }
                        else
                        {
                            MessageBox.Show("No result found!", "Search cartoon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search cartoon", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCartoonID_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbCartoonID_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void grSearch_Enter(object sender, EventArgs e)
        {

        }
    }
}
