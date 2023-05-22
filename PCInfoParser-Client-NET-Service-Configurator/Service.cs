using System;
using System.ServiceProcess;

namespace PCInfoParser_Client_NET_Service_Configurator
{
    internal class Service
    {
        private readonly string _serviceName;
        private readonly ServiceController _controller;

        public Service(string serviceName)
        {
            _serviceName = serviceName;

            this._controller = new();

        }

        public string GetStatus()
        {
            try
            {
                ServiceControllerStatus status = _controller.Status;
                return status switch
                {
                    ServiceControllerStatus.Running => "Запущена",
                    ServiceControllerStatus.Stopped => "Остановлена",
                    _ => "Неизвестный статус",
                };
            }
            catch (Exception)
            {
                return "Служба не установлена";
            }
        }
        public int GetStatusInCode()
        {
            try
            {
                ServiceControllerStatus status = _controller.Status;
                return status switch
                {
                    ServiceControllerStatus.Running => 0,
                    ServiceControllerStatus.Stopped => 1,
                    _ => 2,
                };
            }
            catch (Exception)
            {
                return 2;
            }
        }

        public void Start()
        {
            try
            {
                if (_controller.Status != ServiceControllerStatus.Running)
                {
                    _controller.Start();
                    _controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
            }
            catch
            {
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (_controller.Status != ServiceControllerStatus.Stopped)
                {
                    _controller.Stop();
                    _controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
            }
            catch
            {
                throw;
            }
        }

        private bool IsInstalled()
        {
            try
            {
                ServiceControllerStatus status = _controller.Status;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
