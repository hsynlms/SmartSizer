using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SmartSizer.Models;
using System;
using SmartCore.Models;
using SmartCore;

namespace SmartSizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<FileItem> myFileList = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbOS.Items.Clear();
            cbQuality.Items.Clear();

            foreach (var item in Types.includedOS)
            {
                cbOS.Items.Add(item);
            }

            if (cbOS.Items.Count > 0)
            {
                cbOS.SelectedIndex = (int)Types.supportedOS.Android;
            }

            foreach (var item in Types.includedQuality)
            {
                cbQuality.Items.Add(item);
            }

            if (cbQuality.Items.Count > 0)
            {
                cbQuality.SelectedIndex = (int)Methods.GetSetting("quality");
            }

            lblTo.Content = "to " + Types.includedAndroidScreenSize.First() + ".";

            ckSuffixIOS.IsChecked = (bool)Methods.GetSetting("ios_suffix");
            ckFolders.IsChecked = (bool)Methods.GetSetting("folders");
            ckSmartface.IsChecked = (bool)Methods.GetSetting("smartface");
            ckSearchSubFolders.IsChecked = (bool)Methods.GetSetting("sub_folders");
            ckRatio.IsChecked = (bool)Methods.GetSetting("ratio");

            if (!Directory.Exists("C:\\SmartResizer"))
            {
                Directory.CreateDirectory("C:\\SmartResizer");
            }
        }

        private void cbOS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbStartSize.Items.Clear();

            switch (cbOS.SelectedIndex)
            {
                case (int)Types.supportedOS.Android:
                    foreach (var item in Types.includedAndroidScreenSize)
                    {
                        cbStartSize.Items.Add(item);
                    }

                    if (cbStartSize.Items.Count > 0)
                    {
                        cbStartSize.SelectedIndex = (int)Types.supportedAndroidScreenSize.XXHDPI;
                    }

                    lblTo.Content = "to " + Types.includedAndroidScreenSize.First() + ".";

                    break;
                case (int)Types.supportedOS.iOS:
                    foreach (var item in Types.includediOSScreenSize)
                    {
                        cbStartSize.Items.Add(item);
                    }

                    if (cbStartSize.Items.Count > 0)
                    {
                        cbStartSize.SelectedIndex = (int)Types.supportediOSScreenSize.X3;
                    }

                    lblTo.Content = "to " + Types.includediOSScreenSize.First() + ".";

                    break;
                default:
                    break;
            }
        }

        private void cbStartSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbOS.SelectedIndex)
            {
                case (int)Types.supportedOS.Android:
                    switch (cbStartSize.SelectedIndex)
                    {
                        case (int)Types.supportedAndroidScreenSize.MDPI:
                            General.currentFromSize = Types.allScreenSizes.MDPI;
                            break;
                        case (int)Types.supportedAndroidScreenSize.HDPI:
                            General.currentFromSize = Types.allScreenSizes.HDPI;
                            break;
                        case (int)Types.supportedAndroidScreenSize.XHDPI:
                            General.currentFromSize = Types.allScreenSizes.XHDPI;
                            break;
                        case (int)Types.supportedAndroidScreenSize.XXHDPI:
                            General.currentFromSize = Types.allScreenSizes.XXHDPI;
                            break;
                        default:
                            break;
                    }

                    break;
                case (int)Types.supportedOS.iOS:
                    switch (cbStartSize.SelectedIndex)
                    {
                        case (int)Types.supportediOSScreenSize.X:
                            General.currentFromSize = Types.allScreenSizes.X;
                            break;
                        case (int)Types.supportediOSScreenSize.X2:
                            General.currentFromSize = Types.allScreenSizes.X2;
                            break;
                        case (int)Types.supportediOSScreenSize.X3:
                            General.currentFromSize = Types.allScreenSizes.X3;
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string[] strFiles;

                    if ((bool)ckSearchSubFolders.IsChecked)
                    {
                        strFiles = Directory.GetFiles(dialog.SelectedPath, "*.png", SearchOption.AllDirectories);
                    }
                    else
                    {
                        strFiles = Directory.GetFiles(dialog.SelectedPath);
                    }

                    myFileList = new ObservableCollection<FileItem>();

                    foreach (var file in strFiles)
                    {
                        FileInfo fi = new FileInfo(file);
                        myFileList.Add(new FileItem { FileName = fi.Name.Replace(fi.Extension, ""), FilePath = fi.FullName, FileSize = Methods.ConvertBytesToKilobytes(fi.Length).ToString() + " Kb", FileExtension = fi.Extension });
                    }

                    lblCount.Content = myFileList.Where(w => w.FileExtension == ".png").Count().ToString() + " file(s) listed.";
                    lvFiles.ItemsSource = myFileList.Where(w => w.FileExtension == ".png");
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.Items.Count > 0)
            {
                if (Directory.Exists("C:\\SmartResizer\\iOS"))
                {
                    Directory.Delete("C:\\SmartResizer\\iOS", true);
                }
                
                if (Directory.Exists("C:\\SmartResizer\\Android"))
                {
                    Directory.Delete("C:\\SmartResizer\\Android", true);
                }

                Directory.CreateDirectory("C:\\SmartResizer\\iOS");
                Directory.CreateDirectory("C:\\SmartResizer\\Android");

                var savePath = "C:\\SmartResizer\\";

                pbTotal.Maximum = (cbStartSize.SelectedIndex + 1) * lvFiles.Items.Count;

                //for android
                if (cbOS.SelectedIndex == 0)
                {
                    var folder = "";

                    for (int i = cbStartSize.SelectedIndex; i <= Types.includedAndroidScreenSize.Count() && i >= 0; i--)
                    {
                        foreach (FileItem image in lvFiles.Items)
                        {
                            switch (i)
                            {
                                case (int)Types.supportedAndroidScreenSize.MDPI:
                                    if ((bool)ckFolders.IsChecked)
                                    {
                                        if (!Directory.Exists("C:\\SmartResizer\\Android\\drawable-mdpi"))
                                        {
                                            Directory.CreateDirectory("C:\\SmartResizer\\Android\\drawable-mdpi");
                                        }

                                        folder = "drawable-mdpi\\";
                                    }

                                    SmartCore.Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "Android\\" + folder + image.FileName + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.MDPI,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                case (int)Types.supportedAndroidScreenSize.HDPI:
                                    if ((bool)ckFolders.IsChecked)
                                    {
                                        if (!Directory.Exists("C:\\SmartResizer\\Android\\drawable-hdpi"))
                                        {
                                            Directory.CreateDirectory("C:\\SmartResizer\\Android\\drawable-hdpi");
                                        }

                                        folder = "drawable-hdpi\\";
                                    }

                                    Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "Android\\" + folder + image.FileName + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.HDPI,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                case (int)Types.supportedAndroidScreenSize.XHDPI:
                                    if ((bool)ckFolders.IsChecked)
                                    {
                                        if (!Directory.Exists("C:\\SmartResizer\\Android\\drawable-xhdpi"))
                                        {
                                            Directory.CreateDirectory("C:\\SmartResizer\\Android\\drawable-xhdpi");
                                        }

                                        folder = "drawable-xhdpi\\";
                                    }

                                    Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "Android\\" + folder + image.FileName + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.XHDPI,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                case (int)Types.supportedAndroidScreenSize.XXHDPI:
                                    if ((bool)ckFolders.IsChecked)
                                    {
                                        if (!Directory.Exists("C:\\SmartResizer\\Android\\drawable-xxhdpi"))
                                        {
                                            Directory.CreateDirectory("C:\\SmartResizer\\Android\\drawable-xxhdpi");
                                        }

                                        folder = "drawable-xxhdpi\\";
                                    }

                                    Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "Android\\" + folder + image.FileName + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.XXHDPI,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                default:
                                    break;
                            }

                            pbTotal.Value += 1;
                        }

                        //progressbar
                    }
                }
                //for ios
                else if (cbOS.SelectedIndex == 1)
                {
                    var suffix = "";

                    for (int i = cbStartSize.SelectedIndex; i <= Types.includediOSScreenSize.Count() && i >= 0; i--)
                    {
                        foreach (FileItem image in lvFiles.Items)
                        {
                            switch (i)
                            {
                                case (int)Types.supportediOSScreenSize.X3:
                                    if ((bool)ckSuffixIOS.IsChecked)
                                    {
                                        suffix = "@3x";
                                    }

                                    Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "iOS\\" + image.FileName + suffix + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.X3,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                case (int)Types.supportediOSScreenSize.X2:
                                    if ((bool)ckSuffixIOS.IsChecked)
                                    {
                                        suffix = "@2x";
                                    }

                                    Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "iOS\\" + image.FileName + suffix + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.X2,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                case (int)Types.supportediOSScreenSize.X:
                                    Resizer.ResizeAndSaveImage(new SaveFile
                                    {
                                        FilePath = image.FilePath,
                                        SavePath = savePath + "iOS\\" + image.FileName + image.FileExtension
                                    }, new SmartCore.Models.Settings
                                    {
                                        currentFromSize = General.currentFromSize,
                                        currentToSize = Types.allScreenSizes.X,
                                        currentQuality = (Types.imageQuality)cbQuality.SelectedIndex,
                                        currentPartner = (Types.compatiblePartners)(((bool)ckSmartface.IsChecked) ? 1 : 0)
                                    });

                                    break;
                                default:
                                    break;
                            }

                            pbTotal.Value += 1;
                        }
                    }
                }
                
                MessageBox.Show("Process end.", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvFiles.SelectedItems.Count > 1)
                {
                    ObservableCollection<FileItem> fileList = new ObservableCollection<FileItem>();
                    foreach (FileItem file in lvFiles.SelectedItems)
                    {
                        fileList.Add(file);
                    }

                    foreach (FileItem file in fileList)
                    {
                        myFileList.Remove(file);
                    }
                }
                else
                {
                    myFileList.Remove(lvFiles.SelectedItems[0] as FileItem);
                }

                lvFiles.ItemsSource = myFileList;
            }
            catch (Exception ex)
            {
                //do nothing
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Methods.SaveSetting("ios_suffix", (bool)ckSuffixIOS.IsChecked);
            Methods.SaveSetting("folders", (bool)ckFolders.IsChecked);
            Methods.SaveSetting("ratio", (bool)ckRatio.IsChecked);
            Methods.SaveSetting("smartface", (bool)ckSmartface.IsChecked);
            Methods.SaveSetting("sub_folders", (bool)ckSearchSubFolders.IsChecked);
            Methods.SaveSetting("quality", cbQuality.SelectedIndex);
        }
    }
}