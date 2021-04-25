﻿using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

namespace Wesley.Client.Pages.Market
{

    public partial class DeliveriedView : ContentView
    {

        public DeliveriedView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

    }
}
