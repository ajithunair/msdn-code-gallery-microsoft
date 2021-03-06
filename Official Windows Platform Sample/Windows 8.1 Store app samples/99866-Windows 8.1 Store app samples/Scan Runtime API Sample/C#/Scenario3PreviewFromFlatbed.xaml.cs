//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using System.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Devices.Scanners;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Threading;

using ScanRuntimeAPI.Sample_Utils;
using SDKTemplate;

namespace ScanRuntimeAPI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3PreviewFromFlatbed : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3PreviewFromFlatbed()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        /// <summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!ModelDataContext.ScannerDataContext.WatcherStarted)
            {
                ModelDataContext.ScannerDataContext.StartScannerWatcher();
            }
            
        }

        /// <summary>
        /// Invoked when user nagivates away from this page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ModelDataContext.UnLoad();
        }
        
        /// <summary>
        /// Even Handler for click on Start Scenario button. Starts the scenario of getting the preview from Faltbed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartScenario_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                DisplayImage.Source = null;
                rootPage.NotifyUser("Starting scenario of preview scanning from Flatbed.", NotifyType.StatusMessage);		

                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                ScanPreview(ModelDataContext.ScannerDataContext.CurrentScannerDeviceId, stream);
            }
        }

        /// <summary>
        /// Previews the image from the scanner with given device id
        /// The preview is allowed only if the selected scanner is equipped with a Flatbed and supports preview.
        /// </summary>
        /// <param name="deviceId">scanner device id</param>
        /// <param name="stream">RandomAccessStream in which preview given the by the scan runtime API is kept</param>
        public async void ScanPreview(string deviceId, IRandomAccessStream stream)
        {
            try
            {
                // Get the scanner object for this device id
                ImageScanner myScanner = await ImageScanner.FromIdAsync(deviceId);

                if (myScanner.IsScanSourceSupported(ImageScannerScanSource.Flatbed))
                {
                    if (myScanner.IsPreviewSupported(ImageScannerScanSource.Flatbed))
                    {
                        rootPage.NotifyUser("Scanning", NotifyType.StatusMessage);
                        // Scan API call to get preview from the flatbed
                        var result = await myScanner.ScanPreviewToStreamAsync(ImageScannerScanSource.Flatbed, stream);
                        if (result.Succeeded)
                        {
                            Utils.SetImageSourceFromStream(stream, DisplayImage);
                            rootPage.NotifyUser("Preview scanning is complete. Below is the preview image.", NotifyType.StatusMessage);
                        }
                        else
                        {
                            rootPage.NotifyUser("Failed to preview from Flatbed.", NotifyType.ErrorMessage);
                        }
                    }
                    else
                    {
                        rootPage.NotifyUser("The selected scanner does not support preview from Flatbed.", NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    rootPage.NotifyUser("The selected scanner does not report to be equipped with a Flatbed.", NotifyType.ErrorMessage);
                }
            }            
            catch (Exception ex)
            {
                Utils.DisplayExceptionErrorMessage(ex);
            }
        }


        /// <summary>
        /// Cancels the current scenario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelScenario_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("You clicked the " + b.Name + " button", NotifyType.StatusMessage);

            }
        }

    }
}
