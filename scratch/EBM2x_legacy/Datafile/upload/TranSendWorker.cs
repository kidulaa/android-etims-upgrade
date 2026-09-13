using EBM2x.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace EBM2x.Datafile.upload
{
    public class TranSendWorker
    {
        #region Member values
        private static TranSendWorker trxnSendWorker = null;
        //private BackgroundWorker m_worker = new BackgroundWorker();
        private static BackgroundWorker m_worker = null;
        private int m_interval = 3000;
        private PosModel posModel = null;
        #endregion

        #region Events
        public event ProgressChangedEventHandler ProgressChanged = null;
        public event RunWorkerCompletedEventHandler RunWorkerCompleted = null;
        #endregion

        #region Constructors
        private TranSendWorker(PosModel posmodel)
        {
            this.posModel = posmodel;
            m_worker = new BackgroundWorker();

            m_worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            m_worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            m_worker.WorkerReportsProgress = true;
            m_worker.WorkerSupportsCancellation = true;
        }

        public static TranSendWorker getInstance(PosModel posmodel)
        {
            if (trxnSendWorker == null)
            {
                trxnSendWorker = new TranSendWorker(posmodel);
            }
            return trxnSendWorker;
        }

        public static TranSendWorker getInstance()
        {
            return trxnSendWorker;
        }

        #endregion

        public void Start()
        {
            if (!IsBusy) m_worker.RunWorkerAsync(this.Interval);
        }

        public void Stop()
        {
            if (IsBusy)
            {
                m_worker.CancelAsync();
                while (m_worker.IsBusy)
                {
                    //Application.DoEvents();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return m_worker.IsBusy;
            }
        }

        public int Interval
        {
            get
            {
                return m_interval;
            }

            set
            {
                m_interval = value;
            }
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            int interval = (int)e.Argument;

            while (worker.CancellationPending == false)
            {
                if (TrlogUpload.upload(posModel, true))
                {
                    OperTotalUpload.upload(posModel);
                    RegiTotalUpload.upload(posModel);
                }
                else
                {
                }

                if (worker.CancellationPending == false)
                {
                    Thread.Sleep(interval);
                }
            }

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RunWorkerCompleted != null)
            {
                RunWorkerCompleted(sender, e);
            }
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(sender, e);
            }
        }
    }
}
