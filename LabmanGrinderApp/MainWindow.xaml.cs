using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO;

namespace LabmanGrinderApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RobotArm arm;
        Grinder grinder;
        Dispenser dispenser;
        InputVialRack inputRack;
        OutputVialRack outputRack1;
        OutputVialRack outputRack2;
        OutputVialRack outputRack3;
        BackgroundWorker run;
        BackgroundWorker dispense;
        BackgroundWorker dispose;
        BackgroundWorker grind;
        bool disposeComplete;
        int outputVialNo;

        public MainWindow()
        {
            InitializeComponent();
            Load();
        }
        public void Load()
        {
            arm = new RobotArm();
            grinder = new Grinder();
            dispenser = new Dispenser();
            inputRack = new InputVialRack();
            outputRack1 = new OutputVialRack();
            outputRack2 = new OutputVialRack();
            outputRack3 = new OutputVialRack();

            RobotArm.DataContext = arm;
            InputVialRack.DataContext = inputRack;
            OutputRack1.DataContext = outputRack1;
            OutputRack2.DataContext = outputRack2;
            OutputRack3.DataContext = outputRack3;
            DispenseStation.DataContext = dispenser;
            GrindingStation.DataContext = grinder;
            DispenseWeight.DataContext = dispenser;

            outputVialNo = 1;
            Stop.IsEnabled = false;
            Start.IsEnabled = true;
            Reset.IsEnabled = false;
            OutputResults.IsEnabled = false;
        }

        private void Run()
        {
            run = new BackgroundWorker();
            run.WorkerSupportsCancellation = true;
            //run.WorkerReportsProgress = true;
            run.DoWork += Run_DoWork;
            //run.ProgressChanged += Run_ProgressChanged;
            run.RunWorkerCompleted += Run_RunWorkerCompleted;
            run.RunWorkerAsync();
        }

        private void Run_DoWork(object sender, DoWorkEventArgs e)
        {
            dispose = new BackgroundWorker();
            dispose.DoWork += Dispose_DoWork;
            dispose.RunWorkerCompleted += Dispose_RunWorkerCompleted;

            grind = new BackgroundWorker();
            grind.DoWork += Grind_DoWork;
            grind.RunWorkerCompleted += Grind_RunWorkerCompleted;

            dispense = new BackgroundWorker();
            dispense.DoWork += Dispense_DoWork;
            //dispense.RunWorkerCompleted += Dispense_RunWorkerCompleted;

            while (inputRack.EmptyVialsInRack != inputRack.VialCapacity)
            {
                if (run.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (grinder.Status == "Idle")
                {
                    LoadGrinder();
                }

                if (run.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                for (int i = 0; i < 3; i++)
                {
                    LoadOutputVialToDispenser();

                    if (run.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    while (dispenser.OutputVialLoaded == false | dispenser.InputVialLoaded == false) { continue; }

                    dispenser.Dispense();

                    if (run.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    //dispense.RunWorkerAsync();
                    //if (grinder.Status == "Idle" && arm.Status == "Idle")
                    //{
                    //    LoadGrinder();
                    //}

                    while (dispenser.Status != "Complete" && arm.Status != "Idle") { continue; }

                    if (i == 2)
                    {
                        dispose.RunWorkerAsync();
                    }

                    if (run.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    UnloadOutputVialFromDispenser();
                }

                if (run.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                MoveArm("dispenser");

                if (run.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                while (disposeComplete == false) { continue; }

                arm.PickUpVial(dispenser.UnloadInputVial());

                if (run.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                DropInInputRack();

                if (run.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

        }

        private void Run_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputResults.IsEnabled = true;
            Stop.IsEnabled = false;
            Reset.IsEnabled = true;
        }

        private void Dispose_DoWork(object sender, DoWorkEventArgs e)
        {
            disposeComplete = false;
            dispenser.DisposeWaste();
        }
        private void Dispose_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            disposeComplete = true;
        }

        private void LoadGrinder()
        {
            MoveArm("inputRack");
            arm.PickUpVial(inputRack.GetFullVial());
            MoveArm("grinder");
            grinder.LoadVial(arm.DropVial());
            grind.RunWorkerAsync();
        }
        private void Grind_DoWork(object sender, DoWorkEventArgs e)
        {

            this.Dispatcher.Invoke((Action)delegate ()
            {
                bar.Visibility = Visibility.Visible;
            });

            grinder.Grind();

            this.Dispatcher.Invoke((Action)delegate ()
            {
                bar.Visibility = Visibility.Hidden;
            });
        }
        private void Grind_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //while (dispenser.InputVialLoaded == true && arm.Status!="Idle" && arm.HasVial == true) { continue; } //not waiting
            while (dispenser.InputVialLoaded == true && arm.Status != "Idle" && arm.HasVial==true) { continue; }
            MoveArm("grinder");
            arm.PickUpVial(grinder.UnloadVial());
            MoveArm("dispenser");
            dispenser.LoadInputVial(arm.DropVial());
        }

        private void Dispense_DoWork(object sender, DoWorkEventArgs e)
        {
            dispenser.Dispense();
        }
        private void LoadOutputVialToDispenser()
        {
            MoveArm("outputRack");
            arm.PickUpVial(ActiveRack().GetEmptyVial());
            MoveArm("dispenser");
            dispenser.LoadOutputVial(arm.DropVial());
        }
        private void UnloadOutputVialFromDispenser()
        {
            this.Dispatcher.Invoke((Action)delegate ()
            {
                OutputVialListLV.Items.Add(new OutputData { VialNo = outputVialNo, Weight = dispenser.OutputVial.Weight });
            });
            outputVialNo++;

            MoveArm("dispenser");
            arm.PickUpVial(dispenser.UnloadOutputVial());
            MoveArm("outputRack");
            ActiveRack().AddToRack(arm.DropVial());
        }

        private void CollectFromInputRack()
        {
            MoveArm("inputRack");
            arm.PickUpVial(inputRack.GetFullVial());
        }
        private void DropInInputRack()
        {
            MoveArm("inputRack");
            inputRack.AddToRack(arm.DropVial());
        }

        private void CollectFromOutputRack()
        {
            MoveArm("outputRack");
            arm.PickUpVial(outputRack1.GetEmptyVial());
        }

        private void MoveArm(string location)
        {  
            if (location == arm.Location)
            {
                return;
            }
            else
            {
                arm.Location = location;
            }

            double goTo = 0;
            switch (location)
            {
                case "inputRack":
                    goTo = 0;
                    break;
                case "grinder":
                    goTo = 235;
                    break;
                case "dispenser":
                    goTo = 500;
                    break;
                case "outputRack":
                    goTo = 800;
                    break;
            }

            arm.Status = "In Transit";
            MoveArmAnimate(goTo);
            Thread.Sleep(1000);
            arm.Status = "Idle";
        }
        private void MoveArmAnimate(double goTo)
        {
            this.Dispatcher.Invoke((Action)delegate ()
            {
                Point p = RobotArm.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
                TranslateTransform offsetTransform = new TranslateTransform();
                DoubleAnimation offsetXAnimation = new DoubleAnimation
                {
                    From = p.X - 89,
                    To = goTo,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };

                offsetXAnimation.Completed += new EventHandler(OffsetXAnimation_Completed);
                offsetTransform.BeginAnimation(TranslateTransform.XProperty, offsetXAnimation);
                RobotArm.RenderTransform = offsetTransform;
            });
        }
        private void OffsetXAnimation_Completed(object sender, EventArgs e)
        {

        }

        private OutputVialRack ActiveRack()
        {
            if (outputRack1.EmptyVialsInRack > 0 | outputRack1.VialsInRack < outputRack1.VialCapacity)
            {
                return outputRack1;
            }
            if (outputRack2.EmptyVialsInRack > 0 | outputRack2.VialsInRack < outputRack2.VialCapacity)
            {
                return outputRack2;
            }
            else
            {
                return outputRack3;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (dispenser.TargetOutputWeight > (inputRack.InputVialWeight / 3))
            {
                string msg = "********* WARNING **********" + Environment.NewLine + Environment.NewLine +
                             "Input vial weight content is " + inputRack.InputVialWeight.ToString() + Environment.NewLine + 
                             "The desired output weight you have set will exceed this resulting in some vials not reaching desired weight!" + Environment.NewLine + Environment.NewLine +
                             "RECOMMENDED RANGE = 1 - 33" + Environment.NewLine + Environment.NewLine +
                             "Continue anyway?";


                MessageBoxResult r = MessageBox.Show(msg, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.No)
                {
                    dispenser.TargetOutputWeight = 0;
                    DispenseWeight.Focus();
                    return;
                }
            }

            if (dispenser.TargetOutputWeight == 0)
            {
                MessageBox.Show("Please set desired output weight");
                return;
            }

            Stop.IsEnabled = true;
            Start.IsEnabled = false;
            Run();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            run.CancelAsync();
        }

        private void OutputResults_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    string outputTime = DateTime.Now.ToString("ddMMyyyyHHmmss");
                    string fileName = "Output" + outputTime + ".csv";

                    using (StreamWriter sw = File.AppendText(path + @"\" + fileName))
                    {
                        sw.WriteLine("RackNo,VialNo,Weight");
                        int i = 1;
                        foreach (Vial v in outputRack1.Vials)
                        {
                            sw.WriteLine("1," + i + "," + v.Weight);
                            i++;
                        }
                        i = 1;
                        foreach (Vial v in outputRack2.Vials)
                        {
                            sw.WriteLine("2," + i + "," + v.Weight);
                            i++;
                        }
                        i = 1;
                        foreach (Vial v in outputRack3.Vials)
                        {
                            sw.WriteLine("3," + i + "," + v.Weight);
                            i++;
                        }
                    }
                    MessageBox.Show("Export complete");
                }
                else
                {
                    MessageBox.Show("Cancelled");
                }
            }

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MoveArm("inputRack");
            OutputVialListLV.Items.Clear();
            Load();
        }
    }

    class OutputData
    {
        public int VialNo { get; set; }
        public int Weight { get; set; }
    }
}
